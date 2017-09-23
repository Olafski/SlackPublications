using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Refit;
using SlackPublications.SlackApi;
using SlackPublications.SlackApi.SlackTypes;
using SlackPublications.MessageParser;

namespace SlackPublications
{
    /// <summary>
    /// Publications Generator. Used to generate a list of links posted last week from a given channel in Slack.
    /// </summary>
    internal class PublicationsGenerator
    {
        private ISlackApi _slackClient;
        private SlackMessageParser _messageParser;
        private UsernameRepository _usernameRepository;

        /// <summary>
        /// Sets the generator up with its defaults. Pass in the token to get started.
        /// </summary>
        public PublicationsGenerator(ISlackApi slackClient)
        {
            this._slackClient = slackClient;
            this._usernameRepository = new UsernameRepository(_slackClient);
            this._messageParser = new SlackMessageParser(_slackClient, new HtmlUtility(), _usernameRepository);
        }

        /// <summary>
        /// Generates a list of publications and outputs it.
        /// </summary>
        public async Task<string> GeneratePublicaties()
        {
            var messages = await GetLastWeeksLinks();

            var content = new StringBuilder();
            content.Append($"Hieronder is de lijst met #publicaties van de afgelopen week ({Utilities.GetIso8601WeekOfYear(Utilities.StartOfLastWeek)}) te vinden. Veel leesplezier!");
            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            for(int i = 1; i <= messages.Count(); i++)
            {
                Message message = messages.ElementAt(i-1);
                string parsedMessage = await _messageParser.ParseMessage(message);
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
                string username = await _usernameRepository.GetUserName(message);

                if (!names.Contains(username))
                {
                    names.Add(username);
                }
            }

            return names;
        }

        /// <summary>
        /// Gets the links posted last week.
        /// </summary>
        private async Task<IEnumerable<Message>> GetLastWeeksLinks()
        {
            IEnumerable<Message> lastWeeksMessages = await GetLastWeeksMessages();

            var links = lastWeeksMessages.Where(m => _messageParser.IsLink(m.Text));

            return links;
        }

        /// <summary>
        /// Gets the messages posted last week.
        /// </summary>
        private async Task<IEnumerable<Message>> GetLastWeeksMessages()
        {
            int start = Utilities.GetUnixTimestamp(Utilities.StartOfLastWeek);
            int end = Utilities.GetUnixTimestamp(Utilities.EndOfLastWeek);

            ChannelMessages messageResult = await _slackClient.GetChannelHistory("C1UEZJNTS", start, end);

            if (!messageResult.Ok)
            {
                throw new InvalidOperationException("Could not retrieve messages: " + messageResult.Error);
            }

            Message[] messages = messageResult.Messages;

            return messages.OrderBy(m => m.PostedAt);
        }
    }
}