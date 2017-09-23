using System;

namespace SlackPublicaties.SlackClient.SlackTypes
{
    public partial class Message
    {
        public DateTime PostedAt
        {
            get 
            {
                return Utilities.GetDateTime(this.Ts);
            }
        }
    }
}