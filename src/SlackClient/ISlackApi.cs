using System.Threading.Tasks;
using Refit;

namespace SlackPublicaties.SlackClient
{
    public interface ISlackApi {
        [Get("/channels.history?token={token}&channel={channel}&oldest={start}&latest={end}")]
        Task<SlackTypes.ChannelMessages> GetChannelHistory(string token, string channel, int start, int end);

        [Get("/users.info?token={token}&user={user}")]
        Task<SlackTypes.UserInfo> GetUserInfo(string token, string user);
    }
}