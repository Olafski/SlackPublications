using System.Collections.Generic;
using System.Threading.Tasks;
using SlackPublications.SlackApi;
using SlackPublications.SlackApi.SlackTypes;

namespace SlackPublications
{
    /// <summary>
    /// Repository for Slack usernames.
    /// </summary>
    public class UsernameRepository
    {
        private Dictionary<string,string> _usernames;
        private ISlackApi _slackClient;

        /// <summary>
        /// Initializes a Username repository.
        /// </summary>
        public UsernameRepository(ISlackApi slackClient)
        {
            this._slackClient = slackClient;
            this._usernames = new Dictionary<string,string>();
        }
        
        /// <summary>
        /// Gets the username from a Slack user ID.
        /// </summary>
        public async Task<string> GetUserName(string userId)
        {
            if (_usernames.ContainsKey(userId))
            {
                return _usernames[userId];
            }

            UserInfo user = await _slackClient.GetUserInfo(userId);
            string name = user.User.Name;
            _usernames[userId] = name;
            
            return name;
        }

        /// <summary>
        /// Gets the username from a Slack message.
        /// </summary>
        public async Task<string> GetUserName(Message message)
        {
            return await GetUserName(message.User);
        }
    }
}