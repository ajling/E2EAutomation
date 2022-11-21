using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using SeleniumNetCore.Support.Helpers;
using System;
using System.IO;

namespace SeleniumNetCore.Support.Reporting
{
    public class Reporter 
    {
        private static Log log = new Log();
        private static IConfiguration _configuration = null;
        //private static readonly object _lock = new object();

        public static void StartLog(string scenarioName)
        {
            //lock (_lock)
            //{
                log.StartScenarioLog(scenarioName);
            //}

            _configuration = General.GetConfiguration();
        }

        public static void CloseLog()
        {
            log.CloseLog();
        }

        public static void WriteSuccessfulVerification(string description)
        {
            //lock (_lock)
            //{
                try
                {
                    log.SummaryReportLog(Status.Pass, description);
                }
                catch (FileNotFoundException)
                {
                    // Do nothing - razor engine issue
                }
            //}
        }

        public static void WriteTestInfo(string description)
        {
            //lock (_lock)
            //{
                try
                {
                    log.EventReportLog(Status.Info, description);
                }
                catch (FormatException)
                {
                    // Do nothing - for base64 issue
                }
                catch (FileNotFoundException)
                {
                    // Do nothing - razor engine issue
                }
            //}
        }

        public static void WriteTestInfoAndTakeScreenshot(IWebDriver driver, string description)
        {
            log.EventReportLog(Status.Info, description);

            try
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var screenshotBytes = screenshot.AsByteArray;
                var newBase64String = Convert.ToBase64String(screenshotBytes, Base64FormattingOptions.None);
                MediaEntityModelProvider mediaModelMobile = null;

                if (newBase64String.Contains(','))
                {
                    newBase64String = newBase64String.Substring(newBase64String.IndexOf(",") + 1, newBase64String.Length - (newBase64String.IndexOf(",") + 1));
                }

                mediaModelMobile = MediaEntityBuilder.CreateScreenCaptureFromBase64String(newBase64String, string.Empty).Build();
                log.EventReportLog(Status.Info, $"Screenshot below: ", mediaModelMobile);
            }
            catch (FormatException)
            {
                log.EventReportLog(Status.Info, $"No screenshot captured");
            }
            catch (FileNotFoundException)
            {
                // Do nothing - razor engine issue
            }
        }

        public static void WriteSuccessfulEventLog(string description)
        {
            //lock (_lock)
            //{
                try
                {
                    log.EventReportLog(Status.Pass, description);
                }
                catch (FileNotFoundException)
                {
                    // Do nothing - razor engine issue
                }
            //}
        }

        public static void AddScenarioNames(string scenarioName)
        {
            log.AddScenarioNames(scenarioName);
        }

        public static void WriteFailureEventLog(IWebDriver driver, string description)
        {
            var scenarioName = log.GetScenarioName();
            log.EventReportLog(Status.Info, "Failing Test, taking screenshot, closing app, and closing log");

            //lock (_lock)
            //{
                log.EventReportLog(Status.Fail, $"Description: '{description}'");
            //}
            try
            {
                log.TakeScreenshot(driver, scenarioName);
            }
            catch (FormatException)
            {
                // Do nothing - for base64 issue
            }
            catch (FileNotFoundException)
            {
                // Do nothing - razor engine issue
            }
            catch (Exception ex)
            {
                WriteTestInfo($"Failed to take screenshot: '{ex.GetType()}'. Message: '{ex.Message}'");
            }

            var useBrowserStack = bool.Parse(_configuration.GetSection("DriverSettings")["UseBrowserStack"]);
            if (useBrowserStack)
            {
                try
                {
                    var script = "browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \"" + description + "\"}}";
                    ((IJavaScriptExecutor)driver).ExecuteScript(script);
                }
                catch { }
            }
            throw new Exception($"Test failed: {description}");
        }

        public static void WriteFailureEventLog(IWebDriver driver, Exception exception, string description)
        {
            var scenarioName = log.GetScenarioName();
            string exceptionError = string.Empty;

            if (exception?.InnerException == null && exception?.Message != null)
            {
                exceptionError = $"Scenario failed: '{scenarioName}'. Exception message: '{exception?.Message}'. Extra info: '{description}'";
            }
            else
            {
                exceptionError = $"Scenario failed: '{scenarioName}'. Exception type: '{exception?.GetType()}' Exception message: '{exception?.InnerException?.Message}'. Extra info: '{description}'";
            }

            log.EventReportLog(Status.Info, "Failing Test, taking screenshot, closing app, and closing log");
            //lock (_lock)
            //{
                log.EventReportLog(Status.Fail, exceptionError);
            //}
            try
            {
                log.TakeScreenshot(driver, scenarioName);
            }
            catch (FormatException)
            {
                // Do nothing - for base64 issue
            }
            catch (FileNotFoundException)
            {
                // Do nothing - razor engine issue
            }
            catch (Exception ex)
            {
                WriteTestInfo($"Failed to take screenshot: '{ex.GetType()}'. Message: '{ex.Message}'");
            }

            var useBrowserStack = bool.Parse(_configuration.GetSection("DriverSettings")["UseBrowserStack"]);
            if (useBrowserStack)
            {
                try
                {
                    var script = "browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \"" + exceptionError + "\"}}";
                    ((IJavaScriptExecutor)driver).ExecuteScript(script);
                }
                catch { };
            }
            throw new Exception($"Test failed: {exceptionError}");
        }
    }
}
