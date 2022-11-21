using Microsoft.Extensions.Configuration;
using SeleniumNetCore.Support.Reporting;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SeleniumNetCore.Support.Helpers
{
    internal class General : TestBase
    {
        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = builder.Build();
            }
            return _configuration;
        }

        /// <summary>
        /// Function using regex to strip HTML and JavaScipt from text - 
        /// Generally this is used for Extent Reports so it doesn't corrupt 
        /// the report files
        /// </summary>
        /// <param name="textToStrip"></param>
        /// <returns></returns>
        public static string StripHtmlAndJavaScript(string textToStrip)
        {
            try
            {
                var patternHtml = "/<[^>] +>/ g";
                var regex = new Regex(patternHtml);
                var text = regex.Replace(textToStrip, " ");
                if (text.IndexOf("/script") > 0)
                {
                    text = text.Substring(0, text.IndexOf("/script"));
                }
                return text;
            }
            catch
            {
                if (textToStrip.IndexOf("/script") > 0)
                {
                    return textToStrip.Substring(0, textToStrip.IndexOf("/script"));
                }
                else
                {
                    return textToStrip;
                }
            }
        }

        /// <summary>
        /// Function to generate a random number from a minimum value, with
        /// a maximum value from an object i.e. a List or Array
        /// </summary>
        /// <param name="count"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int ReturnRandomIndex(int count, int min = 2)
        {
            int randomValue = 0;
            Random value = new Random();
            try
            {
                randomValue = value.Next(min, count);
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(GetDriver, e.Message);
            }
            Reporter.WriteSuccessfulEventLog($"Random index generated: '{randomValue}'");
            return randomValue;
        }

        /// <summary>
        /// Function to add specific number of days to the current date with a specified
        /// format.
        /// </summary>
        /// <param name="dateFormat"></param>
        /// <param name="noOfDays"></param>
        /// <returns></returns>
        public static string AddDaysToCurrentDate(string dateFormat, int noOfDays)
        {
            DateTime dateFormat2 = DateTime.Now.AddDays(noOfDays);
            return dateFormat2.ToString(dateFormat);
        }

        /// <summary>
        /// Function to return the day integer with 'st', 'nd', 'rd' or 'th'
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetDayOrdinalValue(int day)
        {
            if (day <= 0) return day.ToString();

            switch (day % 100)
            {
                case 11:
                case 12:
                case 13:
                    return day + "th";
            }

            switch (day % 10)
            {
                case 1:
                    return day + "st";
                case 2:
                    return day + "nd";
                case 3:
                    return day + "rd";
                default:
                    return day + "th";
            }
        }
    }
}
