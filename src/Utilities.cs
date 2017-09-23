using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SlackPublications
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Turns a DateTime into a Week number.
        /// </summary>
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        } 

        /// <summary>
        /// Turns a DateTime into a Unix timestamp.
        /// </summary>
        public static int GetUnixTimestamp(DateTime dateTime)
        {
            return (Int32)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// Turns a Unix timestamp into a DateTime.
        /// </summary>
        public static DateTime GetDateTime(string timestamp)
        {
			if (timestamp.Contains("."))
			{
				timestamp = timestamp.Substring(0, timestamp.IndexOf("."));
			}
		
			long unixTimeStamp = long.Parse(timestamp);
		
			System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
		    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);

		    return dtDateTime;
        }

        /// <summary>
        /// Cleans a Slack url. Used because Slack sometimes turns yoursite.com into http://www.yoursite.com|yoursite.com in the messages.
        /// </summary>
        public static string CleanUrl(string url)
        {
            var pattern = new Regex(@"((.+)://(.+))\|(.+)");
            Match match = pattern.Match(url);

            if (match.Success && match.Groups[3].Value == match.Groups[4].Value)
            {
	            return match.Groups[1].Value;
            }

            return url;
        }
    }
}