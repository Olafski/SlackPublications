using System;

namespace SlackPublications.SlackClient.SlackTypes
{
    /// <summary>
    /// Manual additions to the Message type.
    /// </summary>
    public partial class Message
    {
        /// <summary>
        /// The DateTime for when this message was posted.
        /// </summary>
        public DateTime PostedAt
        {
            get 
            {
                return Utilities.GetDateTime(this.Ts);
            }
        }
    }
}