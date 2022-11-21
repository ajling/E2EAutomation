using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace SeleniumNetCore.Support.Reporting
{
    internal class Log
    {
        private static Dictionary<long, string> _eventThreadToExtentTest = new Dictionary<long, string>();

        private static ConcurrentDictionary<long, string> _scenarioNames = new ConcurrentDictionary<long, string>();

        private static Dictionary<string, ExtentTest> _eventNameToTest = new Dictionary<string, ExtentTest>();

        private readonly string dateTime = DateTime.Now.ToString("ddMMyyyyHHmmss");

        private string resultFolderPath;

        private static ExtentReports _eventReports;

        //private static readonly object _lock = new object();


        public Log()
        {
            var executionFolder = $"Execution_{dateTime}";

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var resultFolderPath = string.Empty;
            var resultScreenshotFolderPath = string.Empty;
            var appRoot = string.Empty;

            if (isWindows)
            {
                appRoot = appPathMatcher.Match(exePath).Value;
                resultFolderPath = $"{appRoot}/Logs/{executionFolder}/";
            }
            else
            {
                appRoot = exePath.Split("/bin")[0];
                resultFolderPath = $"../../../Logs/{executionFolder}/";
            }

            Console.WriteLine($"REPORTS FOLDER PATH: {resultFolderPath}");
            try
            {
                if (!Directory.Exists(resultFolderPath))
                {
                    Directory.CreateDirectory(resultFolderPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - exception creating log folder '{resultFolderPath}' - {e.Message}");
            }

            _eventReports = CreateExtentReportInstance($@"{resultFolderPath}\EventReport_{dateTime}.html");
        }

        public void TakeScreenshot(IWebDriver driver, string scenarioName)
        {
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var screenshotBytes = screenshot.AsByteArray;
            var newBase64String = Convert.ToBase64String(screenshotBytes, Base64FormattingOptions.None);
            MediaEntityModelProvider mediaModelMobile = null;

            try
            {
                if (newBase64String.Contains(','))
                {
                    newBase64String = newBase64String.Substring(newBase64String.IndexOf(",") + 1, newBase64String.Length - (newBase64String.IndexOf(",") + 1));
                }

                mediaModelMobile = MediaEntityBuilder.CreateScreenCaptureFromBase64String(newBase64String, scenarioName).Build();
                GetEventTest().Log(Status.Info, $"Screenshot below: ", mediaModelMobile);
            }
            catch
            {
                GetEventTest().Log(Status.Info, $"No screenshot captured");
            }
        }

        public void StartScenarioLog(string scenarioName)
        {
            GetEventTest(scenarioName);
        }

        public void AddScenarioNames(string scenarioName)
        {
            int? threadId = Thread.CurrentThread?.ManagedThreadId;
            if (threadId.HasValue && !_scenarioNames.ContainsKey((long)threadId))
                _scenarioNames.TryAdd((long)(threadId), scenarioName);
        }

        public string GetScenarioName()
        {
            try
            {
                int? threadId = Thread.CurrentThread?.ManagedThreadId;
                if (threadId.HasValue)
                {
                    return _scenarioNames[(long)threadId];
                }
                else
                {
                    return "Failed to get Scenario name";
                }
            }
            catch
            {
                return "Failed to get Scenario name";
            }
        }

        public void CloseLog()
        {
            _eventReports.Flush();
        }

        public void SummaryReportLog(Status status, string description)
        {
            GetEventTest().Log(status, description);
        }

        public void EventReportLog(Status status, string description)
        {
            if (description.Equals(null))
            {
                description = "DESCRIPTION IS NULL";
            }
            var noHtmlDescription = Regex.Replace(description, "<.*?>", String.Empty);
            GetEventTest().Log(status, noHtmlDescription);
            if (status != Status.Fail)
            {
                return;
            }
        }

        public void EventReportLog(Status status, string description, MediaEntityModelProvider mediaEntityModelProvider)
        {
            if (description.Equals(null))
            {
                description = "DESCRIPTION IS NULL";
            }
            var noHtmlDescription = Regex.Replace(description, "<.*?>", String.Empty);
            GetEventTest().Log(status, noHtmlDescription, mediaEntityModelProvider);
            if (status != Status.Fail)
            {
                return;
            }
        }

        private ExtentReports CreateExtentReportInstance(string filePath)
        {
            var html = new ExtentHtmlReporter(filePath);
            html.Config.ReportName = "SeleniumE2E Testing";
            html.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;
            html.Config.DocumentTitle = "SeleniumE2E Automation Report";
            html.Config.CSS = "span.label.grey.badge.white-text.text-white { width: 150px; height: 150px; }";

            var extent = new ExtentReports();
            extent.AttachReporter(html);

            return extent;
        }

        private static ExtentTest GetEventTest(string scenarioName, string description = "")
        {
            //lock (_lock)
            //{
                if (!_eventNameToTest.ContainsKey(scenarioName))
                {
                    var threadId = Thread.CurrentThread.ManagedThreadId;
                    var eventTest = _eventReports.CreateTest(scenarioName, description);
                    _eventNameToTest.Add(scenarioName, eventTest);
                    if (_eventThreadToExtentTest.ContainsKey(threadId))
                    {
                        _eventThreadToExtentTest[threadId] = scenarioName;
                    }
                    else
                    {
                        _eventThreadToExtentTest.Add(threadId, scenarioName);
                    }
                }
                return _eventNameToTest[scenarioName];
            //}
        }

        private static ExtentTest GetEventTest(string scenarioName)
        {
            //lock (_lock)
            //{
                return GetEventTest(scenarioName, string.Empty);
            //}
        }

        private static ExtentTest GetEventTest()
        {
            //lock (_lock)
            //{
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (_eventThreadToExtentTest.ContainsKey(threadId))
                {
                    var scenarioName = _eventThreadToExtentTest[threadId];
                    return _eventNameToTest[scenarioName];
                }
                return null;
            //}
        }
    }
}
