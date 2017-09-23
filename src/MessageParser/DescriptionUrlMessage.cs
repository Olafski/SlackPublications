using System;
using System.Text.RegularExpressions;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications.MessageParser
{
    /// <summary>
    /// Implementation of ParsedSlackMessage where the Description precedes the Url. Example:
    /// This is a really cool website guys! www.google.com
    /// </summary>
    public class DescriptionUrlMessage : ParsedSlackMessage
    {
        private static Regex _regex = new Regex("(.*) +<((?:http).*)>");

        /// <summary>
        /// Initializes this DescriptionUrlMessage.
        /// </summary>
        public DescriptionUrlMessage(Message message, UsernameRepository usernameRepository) : base(message, usernameRepository)
        {
            this._match = _regex.Match(_message);
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
                return Utilities.CleanUrl(_match.Groups[2].Value);
            }
        }

        /// <summary>
        /// The description for this message.
        /// </summary>
        public override string Description 
        { 
            get
            {
                return Normalize(_match.Groups[1].Value);
            }
        }
    }
}