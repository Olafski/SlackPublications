using System;
using System.Net;
using System.Text.RegularExpressions;

namespace SlackPublications
{
    /// <summary>
    /// Utility for getting and parsing websites.
    /// </summary>
    public class HtmlUtility
    {
        /// <summary>
        /// Reusable WebClient reference.
        /// </summary>
        WebClient _client;

        /// <summary>
        /// Creates an instance and initializes the WebClient.
        /// </summary>
        public HtmlUtility()
        {
            _client = new WebClient();
        }

        /// </summary>
        /// Gets the contents of the title tag from a given URL.
        /// </summary>
        public string GetTitle(string url)
        {
            try 
            {
                string source = _client.DownloadString(url);

                var pattern = new Regex(@"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase);
                Match match = pattern.Match(source);

                if (match.Success)
                {
                    return match.Groups["Title"].Value;
                }
            }
            catch (Exception)
            { 
            }

            return null;
        }
    }    
}