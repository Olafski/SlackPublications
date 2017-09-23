// Slack types. Generated with quicktype.io, then modified a little. 
// Add extensions in the SlackTypesExtensions file.
namespace SlackPublicaties.SlackClient.SlackTypes
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class SlackResult
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("errror")]
        public string Error { get; set; }
    }

    public partial class ChannelMessages : SlackResult
    {
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

    }

    public partial class UserInfo : SlackResult
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("subscribed")]
        public bool? Subscribed { get; set; }

        [JsonProperty("parent_user_id")]
        public string ParentUserId { get; set; }

        [JsonProperty("edited")]
        public Edited Edited { get; set; }

        [JsonProperty("attachments")]
        public Attachments[] Attachments { get; set; }

        [JsonProperty("last_read")]
        public string LastRead { get; set; }

        [JsonProperty("replies")]
        public Edited[] Replies { get; set; }

        [JsonProperty("reactions")]
        public Reactions[] Reactions { get; set; }

        [JsonProperty("reply_count")]
        public long? ReplyCount { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("thread_ts")]
        public string ThreadTs { get; set; }

        [JsonProperty("unread_count")]
        public long? UnreadCount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }

    public partial class Edited
    {
        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }

    public partial class Attachments
    {
        [JsonProperty("image_height")]
        public long? ImageHeight { get; set; }

        [JsonProperty("thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonProperty("fields")]
        public Fields[] Fields { get; set; }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author_link")]
        public string AuthorLink { get; set; }

        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("from_url")]
        public string FromUrl { get; set; }

        [JsonProperty("image_bytes")]
        public long? ImageBytes { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("image_width")]
        public long? ImageWidth { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("service_icon")]
        public string ServiceIcon { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("service_url")]
        public string ServiceUrl { get; set; }

        [JsonProperty("thumb_height")]
        public long? ThumbHeight { get; set; }

        [JsonProperty("ts")]
        public long? Ts { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("thumb_width")]
        public long? ThumbWidth { get; set; }

        [JsonProperty("title_link")]
        public string TitleLink { get; set; }

        [JsonProperty("video_html_height")]
        public long? VideoHtmlHeight { get; set; }

        [JsonProperty("video_html")]
        public string VideoHtml { get; set; }

        [JsonProperty("video_html_width")]
        public long? VideoHtmlWidth { get; set; }
    }

    public partial class Fields
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("short")]
        public bool Short { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Reactions
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("users")]
        public string[] Users { get; set; }
    }

    public partial class User
    {
        [JsonProperty("is_owner")]
        public bool IsOwner { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("has_2fa")]
        public bool Has2fa { get; set; }

        [JsonProperty("is_app_user")]
        public bool IsAppUser { get; set; }

        [JsonProperty("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tz")]
        public string Tz { get; set; }

        [JsonProperty("is_restricted")]
        public bool IsRestricted { get; set; }

        [JsonProperty("is_primary_owner")]
        public bool IsPrimaryOwner { get; set; }

        [JsonProperty("is_ultra_restricted")]
        public bool IsUltraRestricted { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("profile")]
        public Profile Profile { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        [JsonProperty("tz_offset")]
        public long TzOffset { get; set; }

        [JsonProperty("tz_label")]
        public string TzLabel { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class Profile
    {
        [JsonProperty("image_48")]
        public string Image48 { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("avatar_hash")]
        public string AvatarHash { get; set; }

        [JsonProperty("display_name_normalized")]
        public string DisplayNameNormalized { get; set; }

        [JsonProperty("image_24")]
        public string Image24 { get; set; }

        [JsonProperty("image_192")]
        public string Image192 { get; set; }

        [JsonProperty("image_32")]
        public string Image32 { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("image_72")]
        public string Image72 { get; set; }

        [JsonProperty("image_512")]
        public string Image512 { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("real_name_normalized")]
        public string RealNameNormalized { get; set; }

        [JsonProperty("status_emoji")]
        public string StatusEmoji { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("skype")]
        public string Skype { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class GettingStarted
    {
        public static GettingStarted FromJson(string json) => JsonConvert.DeserializeObject<GettingStarted>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GettingStarted self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
