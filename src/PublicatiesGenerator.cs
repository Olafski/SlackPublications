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
    internal class PublicatiesGenerator
    {
        private string _token;
        private const DayOfWeek StartOfWeek = DayOfWeek.Monday;
        private ISlackApi _client;
        private HtmlUtility _htmlUtility;

        private Dictionary<string,string> _usernames;

        public PublicatiesGenerator(string token)
        {
            this._token = token;
            this._client  = RestService.For<ISlackApi>("https://slack.com/api/");
            this._htmlUtility = new HtmlUtility();
            this._usernames = new Dictionary<string,string>();
        }

        public async Task<string> GenereerPublicaties()
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

            IEnumerable<string> gebruikers = await GetUserNames(messages);
            string gebruikerNamen = string.Join(", @", gebruikers);
            content.Append($"De lijst is deze week mogelijk gemaakt door @{gebruikerNamen}. Bedankt!");

            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            content.Append("Kom je een interessante publicatie (foto/video/textueel) tegen? Deel deze dan in het #publicaties kanaal zodat iedereen er van kan leren :slightly_smiling_face:");

            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            content.Append("Nog een fijne dag!");

            return content.ToString();
        }

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

        private async Task<string> GetUserName(Message message)
        {
            return await GetUserName(message.User);
        }

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

        private async Task<IEnumerable<Message>> GetLastWeeksLinks()
        {
            IEnumerable<Message> lastWeeksMessages = await GetLastWeeksMessages();

            var links = lastWeeksMessages.Where(m => IsLink(m.Text));

            return links;
        }

        private bool IsLink(string messageText)
        {
            bool isUrl = messageText.Contains("http://") || messageText.Contains("https://") || messageText.Contains("www");

            return isUrl;
        }

        private async Task<IEnumerable<Message>> GetLastWeeksMessages()
        {
            int start = Utilities.GetUnixTimestamp(StartOfLastWeek);
            int end = Utilities.GetUnixTimestamp(EndOfLastWeek);

            var client = RestService.For<ISlackApi>("https://slack.com/api/");
            ChannelMessages messageResult = await client.GetChannelHistory("xoxp-61820345425-84445245876-242544919669-0af7e103674ad5e9a778c31207da6ca7", "C1UEZJNTS", start, end);

            if (!messageResult.Ok)
            {
                throw new InvalidOperationException("Could not retrieve messages: " + messageResult.Error);
            }

            Message[] messages = messageResult.Messages;

            return messages.OrderBy(m => m.PostedAt);
        }

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

        private DateTime EndOfLastWeek
        {
            get 
            {
                return StartOfLastWeek.AddDays(7);
            }
        }

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

        private string Normalize(string messageText) {
            return messageText.Replace(":", "");
        }

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