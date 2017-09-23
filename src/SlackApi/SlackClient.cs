using System.Threading.Tasks;
using Refit;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications.SlackApi
{
    /// <summary>
    /// Slack client.
    /// </summary>
    public class SlackClient : ISlackApi
    {
        private ISlackApiBase _slackApiBase;
        private string _token;

        /// <summary>
        /// Initializes this Slack client.
        /// </summary>
        /// <param name="token">The Slack OAuth token to use for this client.</param>
        public SlackClient(string token)
        {
            this._token = token;
            this._slackApiBase = RestService.For<ISlackApiBase>("https://slack.com/api/");
        }

        /// <summary>
        /// Gets history from a channel.
        /// </summary>
        /// <param name="channel">The Slack channel to get messages from, in the format C1234567890.</param>
        /// <param name="start">The start timestamp.</param>
        /// <param name="end">The end timestamp.</param>        
        public Task<ChannelMessages> GetChannelHistory(string channel, int start, int end)
        {
            return _slackApiBase.GetChannelHistory(_token, channel, start, end);
        }

        /// <summary>
        /// Gets a user profile.
        /// </summary>
        /// <param name="user">The Slack user ID, in the format W1234567890.</param>
        public Task<UserInfo> GetUserInfo(string user)
        {
            return _slackApiBase.GetUserInfo(_token, user);
        }
    }
}