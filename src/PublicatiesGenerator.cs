using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Refit;
using SlackPublicaties.SlackClient;
using SlackPublicaties.SlackClient.SlackTypes;

namespace SlackPublicaties
{
    /// <summary>
    /// Publications Generator. Used to generate a list of links posted last week from a given channel in Slack.
    /// </summary>
    internal class PublicationsGenerator
    {
        /// <summary>
        /// The Slack OAuth token to use.
        /// </summary>
        private string _token;

        /// <summary>
        /// Start of the week. This is used to choose which days to post links from.
        /// </summary>
        private const DayOfWeek StartOfWeek = DayOfWeek.Monday;

        /// <summary>
        /// Reusable Slack API client reference.
        /// </summary>
        private ISlackApi _client;

        /// <summary>
        /// Reusable HTML Utility reference.
        /// </summary>
        private HtmlUtility _htmlUtility;

        /// <summary>
        /// Cache of user IDs to Usernames. Slack messages contain @userID references, so this saves us some lookups in the API.
        /// </summary>
        private Dictionary<string,string> _usernames;

        /// <summary>
        /// Sets the generator up with its defaults. Pass in the token to get started.
        /// </summary>
        public PublicationsGenerator(string token)
        {
            this._token = token;
            this._client  = RestService.For<ISlackApi>("https://slack.com/api/");
            this._htmlUtility = new HtmlUtility();
            this._usernames = new Dictionary<string,string>();
        }

        /// <summary>
        /// Generates a list of publications and outputs it.
        /// </summary>
        public async Task<string> GeneratePublicaties()
        {
            var messages = await GetLastWeeksLinks();

            var content = new StringBuilder();
            content.Append($"Hieronder is de lijst met #publicaties van de afgelopen week ({Utilities.GetIso8601WeekOfYear(StartOfLastWeek)}) te vinden. Veel leesplezier!");
            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            for(int i = 1; i <= messages.Count(); i++)
            {
                string message = messages.ElementAt(i-1).Text;
                string parsedMessage = await ParseMessage(message);
                content.Append($"{i}. {parsedMessage}");
                content.Append(Environment.NewLine);
            }

            content.Append(Environment.NewLine);

            IEnumerable<string> users = await GetUserNames(messages);
            string userNames = string.Join(", @", users);
            content.Append($"De lijst is deze week mogelijk gemaakt door @{userNames}. Bedankt!");

            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            content.Append("Kom je een interessante publicatie (foto/video/textueel) tegen? Deel deze dan in het #publicaties kanaal zodat iedereen er van kan leren :slightly_smiling_face:");

            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            content.Append("Nog een fijne dag!");

            return content.ToString();
        }

        /// <summary>
        /// Gets a list of unique usernames from Slack messages.
        /// </summary>
        private async Task<IEnumerable<string>> GetUserNames(IEnumerable<Message> messages)
        {
            var names = new List<string>();

            foreach (Message message in messages)
            {
                string username = await GetUserName(message);

                if (!names.Contains(username))
                {
                    names.Add(username);
                }
            }

            return names;
        }

        /// <summary>
        /// Gets the username from a Slack message.
        /// </summary>
        private async Task<string> GetUserName(Message message)
        {
            return await GetUserName(message.User);
        }

        /// <summary>
        /// Gets the username from a Slack user ID.
        /// </summary>
        private async Task<string> GetUserName(string userId)
        {
            if (_usernames.ContainsKey(userId))
            {
                return _usernames[userId];
            }

            UserInfo user = await _client.GetUserInfo(_token, userId);
            string name = user.User.Name;
            _usernames[userId] = name;
            
            return name;
        }

        /// <summary>
        /// Gets the links posted last week.
        /// </summary>
        private async Task<IEnumerable<Message>> GetLastWeeksLinks()
        {
            IEnumerable<Message> lastWeeksMessages = await GetLastWeeksMessages();

            var links = lastWeeksMessages.Where(m => IsLink(m.Text));

            return links;
        }

        /// <summary>
        /// Determines whether a message contains a link.
        /// </summary>
        private bool IsLink(string messageText)
        {
            bool isUrl = messageText.Contains("http://") || messageText.Contains("https://") || messageText.Contains("www");

            return isUrl;
        }

        /// <summary>
        /// Gets the messages posted last week.
        /// </summary>
        private async Task<IEnumerable<Message>> GetLastWeeksMessages()
        {
            int start = Utilities.GetUnixTimestamp(StartOfLastWeek);
            int end = Utilities.GetUnixTimestamp(EndOfLastWeek);

            ChannelMessages messageResult = await _client.GetChannelHistory(_token, "C1UEZJNTS", start, end);

            if (!messageResult.Ok)
            {
                throw new InvalidOperationException("Could not retrieve messages: " + messageResult.Error);
            }

            Message[] messages = messageResult.Messages;

            return messages.OrderBy(m => m.PostedAt);
        }

        /// <summary>
        /// Gets the start of last week.
        /// </summary>
        private DateTime StartOfLastWeek
        {
            get 
            {
                DateTime now = DateTime.Now;
                int diff = now.DayOfWeek - StartOfWeek;

                if (diff < 0)
                {
                    diff += 7;
                }

                // last week
                diff += 7;

                return now.AddDays(-1 * diff).Date;
            }
        }

        /// <summary>
        /// Gets the end of last week.
        /// </summary>
        private DateTime EndOfLastWeek
        {
            get 
            {
                return StartOfLastWeek.AddDays(7);
            }
        }

        /// <summary>
        /// Parses a message and tries to return it in the format [URL] - [Description]. If the message is just a URL, that URL is returned.
        /// </summary>
        private async Task<string> ParseMessage(string messageText)
        {
            string modifiedMessage = Regex.Replace(messageText, "[\r\n]", "");

            var pattern1 = new Regex("<((?:http).*)> +(.*)");
            Match match1 = pattern1.Match(modifiedMessage);

            var pattern2 = new Regex("(.*) +<((?:http).*)>");
            Match match2 = pattern2.Match(modifiedMessage);

            var pattern3 = new Regex("<((?:http).*)>");
            Match match3 = pattern3.Match(modifiedMessage);

            string url = null;
            string title = null;
            if (match1.Success)
            {
                url = Utilities.CleanUrl(match1.Groups[1].Value);
                title = Normalize(match1.Groups[2].Value);
            }
            else if (match2.Success)
            {
                url = Utilities.CleanUrl(match2.Groups[2].Value);
                title = Normalize(match2.Groups[1].Value);
            }
            else if (match3.Success)
            {
                url = Utilities.CleanUrl(match3.Groups[1].Value);
                title = _htmlUtility.GetTitle(url);
            }

            if (url == null)
            {
                return modifiedMessage;
            }

            if (title == null)
            {
                return url;
            }

            title = await ReplaceUsernames(title);
            return $"{url} - {title}";
        }

        /// <summary>
        /// Normalizes a URL.
        /// </summary>
        private string Normalize(string messageText) {
            return messageText.Replace(":", "");
        }

        /// <summary>
        /// Replaces the user IDs in a Slack message with user names.
        /// </summary>
        private async Task<string> ReplaceUsernames(string messageText)
        {
            var pattern = new Regex(@"(<@(U[A-Z0-9]*)>)");
            MatchCollection matches = pattern.Matches(messageText);

            if (matches.Count > 0)
            {
				foreach (Match match in matches)
				{
                    string username = await GetUserName(match.Groups[2].Value);
					messageText = messageText.Replace(match.Groups[1].Value, "@" + username);
				}
            }

            return messageText;
        }
    }
}