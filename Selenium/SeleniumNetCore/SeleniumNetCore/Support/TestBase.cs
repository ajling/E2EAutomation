using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SeleniumNetCore.Support.Enums;
using SeleniumNetCore.Support.Helpers;
using SeleniumNetCore.Support.Reporting;
using SeleniumNetCore.Support.WebDriver;
using SeleniumNetCore.Support.WebElement;
using System;
using System.Threading;

namespace SeleniumNetCore.Support
{
    public class TestBase
    {
        private static readonly ThreadLocal<IWebDriver> _threadDriver = new ThreadLocal<IWebDriver>();
        public IWebDriver driver
        {
            get
            {
                return _threadDriver.Value;
            }
        }
        public BrowserType browserType;
        public static IConfiguration _configuration;
        private static Status _testStatus = Status.Pass;

        public TestBase()
        {
            _configuration = General.GetConfiguration();
        }

        public static IWebDriver OpenBrowser(BrowserType browser, string testCategory, string testName, bool productionTest = false)
        {
            var browserName = BrowserTypeName.GetBrowserName(browser);

            _threadDriver.Value = WebDriver.WebDriver.InitializeDriver(browser, _configuration, testCategory, testName);

            var test = $"{testName}-{browserName}";
            Reporter.AddScenarioNames(test);
            Reporter.StartLog(test);

            Reporter.WriteSuccessfulVerification($"'{browserName}' opened for test execution.");
            return _threadDriver.Value;
        }

        public static IWebDriver GetDriver
        {
            get
            {
                return _threadDriver.Value;
            }
        }

        public static Status Status
        {
            get
            {
                return _testStatus;
            }
            set
            {
                _testStatus = value;
            }
        }

        public T NavigateTo<T>(string url)
        {
            GetDriver.Navigate().GoToUrl(url);
            Reporter.WriteSuccessfulEventLog($"Successfully navogated to url '{url}'");
            return (T)Activator.CreateInstance(typeof(T), new object[] { });
        }

        public void MarkTestAsPassed(IWebDriver driver)
        {
            if (bool.Parse(_configuration.GetSection("DriverSettings")["UseBrowserStack"]))
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \"Test passed\"}}");
            }
            Status = Status.Pass;
            Reporter.WriteSuccessfulEventLog("Test has passed");
        }

        protected void TestTearDown(IWebDriver driver)
        {
            // Ensures another exception isn't raised as it hides the original test failure
            if (driver == null)
            {
                Reporter.CloseLog();
                return;
            }

            try
            {
                Reporter.CloseLog();
                driver.Close();
                driver.Quit();
            }
            catch (Exception e)
            {
                Reporter.WriteTestInfo($"An error occurred closing the driver. {e.Message}");
                Reporter.CloseLog();
                driver?.Quit();
            }
        }

        public static void DoRefreshOnPage(IWebDriver driver)
        {
            driver.Navigate().Refresh();
            Reporter.WriteSuccessfulEventLog($"Page with URL '{driver.Url}' refreshed");
        }
        public static void DoRefreshOnPage(IWebDriver driver, int secondsToWait = 5)
        {
            Thread.Sleep(TimeSpan.FromSeconds(secondsToWait));
            DoRefreshOnPage(driver);
            Reporter.WriteSuccessfulEventLog($"Page with URL '{driver.Url}' refreshed");
        }

        public static void WaitForElementToBeDisplayed(IWebElement element, int seconds = 10)
        {
            int i = 0;
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                i++;
            } while (!element.Displayed && i < seconds);
        }

        public static void WaitForElementToBeEnabled(IWebElement element, int seconds = 10)
        {
            int i = 0;
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                i++;
                if (i > seconds)
                {
                    return;
                }
            } while (!element.Enabled);
        }

        public static void RefreshPageIfLoadingCircleOr502Error(IWebDriver driver)
        {
            var byLoadingCircle = By.CssSelector("[class^='p_sk-circle'],[data-testid='spinner-ball']");
            var by502Error = By.XPath("//*[contains(text(), '502 Bad Gateway')]");

            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(5000);
                if (driver.FindIfElementIsPresent(byLoadingCircle, 5))
                {
                    Reporter.WriteTestInfo($"The 'Loading Circle/Spinning Ball' was found on URL: {driver.Url}.");
                    DoRefreshOnPage(driver);
                }
                else if (driver.FindIfElementIsPresent(by502Error, 5))
                {
                    Reporter.WriteTestInfo($"A '502 Bad Gateway' error was found on URL: {driver.Url}.");
                    DoRefreshOnPage(driver);
                }
                else
                {
                    break;
                }
            }
        }

        public static void RefreshPageIfUberLoadingCircle(IWebDriver driver)
        {
            var byLoadMask = By.XPath("//div[@class='loadmask-msg maskOpaque']");

            for (int i = 0; i < 5; i++)
            {
                if (driver.FindIfElementIsPresent(byLoadMask, 5))
                {
                    Reporter.WriteTestInfo($"An 'old loading circle' was found on URL: {driver.Url}.");
                    DoRefreshOnPage(driver);
                    Thread.Sleep(5000);
                }
                else
                {
                    break;
                }
            }
        }

        protected void TapInstructFlowPreviousButton(bool upperCaseSelector = false)
        {
            if (upperCaseSelector)
            {
                driver.FindElement(By.XPath("//p[normalize-space()='PREVIOUS']")).ClickElement();
            }
            else
            {
                driver.FindElement(By.XPath("//p[normalize-space()='Previous']")).ClickElement();
            }
        }

        public void ActionsClick(IWebElement ele)
        {
            Actions action = new Actions(driver);

            action.Click(ele)
                .Perform();
        }
    }
}

