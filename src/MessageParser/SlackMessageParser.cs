using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackPublications.SlackApi;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications.MessageParser
{
    /// <summary>
    /// Parser for Slack Messages, to get relevant data for the publications generator.
    /// </summary>
    public class SlackMessageParser
    {
        private HtmlUtility _htmlUtility;
        private ISlackApi _slackClient;
        private UsernameRepository _usernameRepository;

        /// <summary>
        /// Initializes the SlackMessageParser.
        /// </summary>
        public SlackMessageParser(ISlackApi slackClient, HtmlUtility htmlUtility, UsernameRepository usernameRepository)
        {
            this._slackClient = slackClient;
            this._htmlUtility = htmlUtility;
            this._usernameRepository = usernameRepository;
        }

        /// <summary>
        /// Determines whether a message contains a link.
        /// </summary>
        public bool IsLink(string messageText)
        {
            bool isUrl = messageText.Contains("http://") || messageText.Contains("https://") || messageText.Contains("www");

            return isUrl;
        }

        /// <summary>
        /// Parses the given Slack message. Returns a parsed version of the message.
        /// </summary>
        public async Task<string> ParseMessage(Message slackMessage)
        {
            ParsedSlackMessage parsedSlackMessage = null;

            if (UrlDescriptionMessage.IsMatch(slackMessage.Text))
            {
                parsedSlackMessage = new UrlDescriptionMessage(slackMessage, _usernameRepository);
            }
            else if (DescriptionUrlMessage.IsMatch(slackMessage.Text))
            {
                parsedSlackMessage = new DescriptionUrlMessage(slackMessage, _usernameRepository);
            }
            else if (UrlMessage.IsMatch(slackMessage.Text))
            {
                parsedSlackMessage = new UrlMessage(slackMessage, _usernameRepository, _htmlUtility);
            }

            if (parsedSlackMessage == null)
            {
                return null;
            }

            return await parsedSlackMessage.GetPublicationText();
        }
    }
}