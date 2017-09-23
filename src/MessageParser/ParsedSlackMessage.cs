using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications.MessageParser
{
    /// <summary>
    /// Parsed version of a Slack message.
    /// </summary>
    public abstract class ParsedSlackMessage
    {
        /// <summary>
        /// Cached Regex match.
        /// </summary>
        protected Match _match;

        /// <summary>
        /// The URL in this message.
        /// </summary>
        public abstract string Url { get; }

        /// <summary>
        /// The description for this message.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The text for this message.
        /// </summary>
        protected string _message;

        /// <summary>
        /// Reusable UsernameRepository reference.
        /// </summary>
        protected UsernameRepository UsernameRepository { get; }

        /// <summary>
        /// Initializes common features in ParsedSlackMessage instances.
        /// </summary>
        public ParsedSlackMessage(Message message, UsernameRepository usernameRepository)
        {
            string messageText = message.Text;
            messageText = Regex.Replace(messageText, "[\r\n]", "");
            this._message = messageText;
            this.UsernameRepository = usernameRepository;
        }

        /// <summary>
        /// Normalizes a URL.
        /// </summary>
        protected string Normalize(string url) {
            return url.Replace(":", "");
        }

        /// <summary>
        /// Gets the text to put into the Publications post for this message.
        /// </summary>
        public async virtual Task<string> GetPublicationText()
        {
            string description = Description;

            if (string.IsNullOrWhiteSpace(description))
            {
                return Url;
            }

            description = await ReplaceUsernames(description);

            return $"{Url} - {description}";
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
                    string username = await UsernameRepository.GetUserName(match.Groups[2].Value);
					messageText = messageText.Replace(match.Groups[1].Value, "@" + username);
				}
            }

            return messageText;
        }
    }
}