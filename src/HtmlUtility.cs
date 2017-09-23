using System;
using System.Net;
using System.Text.RegularExpressions;

namespace SlackPublicaties
{
    public class HtmlUtility
    {
        WebClient _client;

        public HtmlUtility()
        {
            _client = new WebClient();
        }

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