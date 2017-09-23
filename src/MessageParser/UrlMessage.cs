using System;
using System.Text.RegularExpressions;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications.MessageParser
{
    /// <summary>
    /// Implementation of ParsedSlackMessage where just a URL was posted. Example:
    /// www.google.com
    /// </summary>
    public class UrlMessage : ParsedSlackMessage
    {
        private static Regex _regex = new Regex("<((?:http).*)>");
        private HtmlUtility _htmlUtility;

        /// <summary>
        /// Initializes this UrlMessage.
        /// </summary>
        public UrlMessage(Message message, UsernameRepository usernameRepository, HtmlUtility htmlUtility) : base(message, usernameRepository)
        {
            this._match = _regex.Match(_message);
            this._htmlUtility = htmlUtility;
        }

        /// <summary>
        /// Checks whether the given message matches the format for this implementation of ParsedSlackMessage.
        /// </summary>
        public static bool IsMatch(string messageText)
        {
            Match match = _regex.Match(messageText);
            return match.Success;
        }

        /// <summary>
        /// The URL in this message.
        /// </summary>
        public override string Url 
        { 
            get 
            {
                return Utilities.CleanUrl(_match.Groups[1].Value);
            }
        }

        /// <summary>
        /// The description for this message.
        ///
        /// We try to get this from the URL by retrieving the website and getting the title tag from it.
        /// </summary>
        public override string Description 
        { 
            get
            {
                string title = _htmlUtility.GetTitle(Url);

                return title;
            }
        }
    }
}