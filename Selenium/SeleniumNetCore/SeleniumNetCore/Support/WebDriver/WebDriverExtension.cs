using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumNetCore.Support.Helpers;
using SeleniumNetCore.Support.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SeleniumNetCore.Support.WebDriver
{
    public static class WebDriverExtension
    {
        public static IConfiguration _configuration;

        public static void Sync(this IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
            wait.IgnoreExceptionTypes(typeof(InvalidOperationException), typeof(WebDriverTimeoutException));
            wait.PollingInterval = TimeSpan.FromMilliseconds(500);
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("driver", "Driver must support javascript execution");
            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript("return document.readyState").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public static object ExecuteJavaScript(this IWebDriver driver, string scriptToExecute, IWebElement webElement = null)
        {
            object resultObject = null;
            try
            {
                IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
                if (webElement == null)
                {
                    resultObject = javascript.ExecuteScript(scriptToExecute);
                }
                else
                {
                    resultObject = javascript.ExecuteScript(scriptToExecute, webElement);
                }
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(driver, $"WebDriverExtensions - ExecuteJavaScript: {e.Message}");
            }
            Reporter.WriteTestInfo($"Executed Javascript supplied{(webElement == null ? "." : " using the web element provided")}");
            return resultObject;
        }

        public static void WaitTillElementVisible(this IWebDriver driver, By by, int timeout = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(driver, e.Message);
            }
            Reporter.WriteTestInfo($"Waited and element is visible within {timeout} seconds.");
        }

        public static void WaitTillElementIsInVisible(this IWebDriver driver, By by, int timeout = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(driver, e.Message);
            }
            Reporter.WriteTestInfo($"Waited and element is now invisible within {timeout} seconds.");
        }

        public static void WaitTillElementClickable(this IWebDriver driver, By by, int timeout = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(driver, e.Message);
            }
            Reporter.WriteTestInfo($"Waited and element is clickable within {timeout} seconds.");
        }

        public static void WaitTillElementExists(this IWebDriver driver, By locator, int timeout = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
            }
            catch (Exception e)
            {

                Reporter.WriteFailureEventLog(driver, e.Message);
            }
            Reporter.WriteTestInfo($"Waited and element now exist within {timeout} seconds.");
        }

        public static string SwitchToNewWindowByTitle(this IWebDriver driver, string title)
        {
            IReadOnlyCollection<string> allWindowHandles = driver.WindowHandles;
            string MainWindow = allWindowHandles.First();
            if (!(allWindowHandles.Count >= 1))
            {
                Reporter.WriteFailureEventLog(driver, "I am unable to switch to new window contains title as it doesn't exist - " + title);
                return MainWindow;
            }

            bool newWindowFound = false;
            foreach (string handle in allWindowHandles)
            {
                if (driver.SwitchTo().Window(handle).Title.Contains(title))
                {
                    newWindowFound = true;
                    Reporter.WriteSuccessfulEventLog("I switched to new window containing title: " + title);
                    break;
                }
            }

            if (!newWindowFound)
            {
                Reporter.WriteFailureEventLog(driver, "I am unable to switch to new window containing title: " + title);
            }
            return MainWindow;
        }

        public static string SwitchToSecondWindowHandle(this IWebDriver driver)
        {
            string MainWindow = "";
            string PopUpWindow = "";

            IReadOnlyCollection<string> Windows = driver.WindowHandles;
            Reporter.WriteSuccessfulEventLog("I found: " + Windows.Count + " browser tabs while running this test.");
            int attemptCount = 0;
            int x = Windows.Count;
            while (x == 1)
            {
                Windows = driver.WindowHandles;
                x = Windows.Count;
                Reporter.WriteSuccessfulEventLog("Window handle size is still not 2 yet, will keep checking it.");
                attemptCount++;
                if (attemptCount >= 60)
                {
                    throw new Exception("Unable to open the second window as expected");
                }
            }

            if (Windows.Count > 1)
            {
                using (IEnumerator<string> enumerator = Windows.GetEnumerator())
                {
                    enumerator.MoveNext();
                    MainWindow = enumerator.Current;
                    while (enumerator.MoveNext() && MainWindow != PopUpWindow)
                    {
                        Reporter.WriteSuccessfulEventLog("Switching to the pop up window so that you can carry on with the test.");
                        PopUpWindow = enumerator.Current;
                        driver.SwitchTo().Window(PopUpWindow);
                        Reporter.WriteSuccessfulEventLog("I have succesfully switched to the next window.");
                    }
                }
            }
            return MainWindow;
        }

        public static void SwitchToMainWindow(this IWebDriver driver, string windowHandleToSwitchTo)
        {
            var Windows = driver.WindowHandles;
            var count = 0;
            while (Windows.Count > 1)
            {
                count++;
                Thread.Sleep(1000);
                if (count >= 120)
                {
                    Reporter.WriteFailureEventLog(driver, "Its taking over 2 minutes for the child window to close.");
                }
                Reporter.WriteSuccessfulEventLog($"I have more than 1 windows still opened so I need to wait for that to complete its processes and close.");
                Windows = driver.WindowHandles;
            }
            driver.SwitchTo().Window(windowHandleToSwitchTo);
            Reporter.WriteSuccessfulEventLog($"I have succesfully switched to the window '{windowHandleToSwitchTo}' after going through the loop waiting {count} time.");
        }

        public static void SwitchToFirstWindow(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.First());
            Reporter.WriteSuccessfulEventLog("I have succesfully switched to the first window.");
        }

        public static void SwitchToLastWindow(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            Reporter.WriteSuccessfulEventLog("I have succesfully switched to the last window.");
        }

        public static void JavaScriptClick(this IWebDriver driver, IWebElement element)
        {
            try
            {
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", element);
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"An exception occurred performing java script click. Error: {ex.Message} - {element}");
            }
            Reporter.WriteTestInfo($"Clicked element {element} using javascript click.");
        }

        public static void JavaScriptFocus(this IWebDriver driver, IWebElement element)
        {
            try
            {
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].focus();", element);
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"An exception occurred performing focus using java script. Error: {ex.Message} - {element}");
            }
            Reporter.WriteTestInfo($"Focused on element {element} using javascript.");
        }

        public static void SwitchToFrame(this IWebDriver driver, IWebElement element)
        {
            driver.SwitchTo().Frame(element);
        }

        public static void SwitchToDefaultFrame(this IWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
        }

        public static void ScrollToView(this IWebDriver driver, IWebElement element)
        {
            try
            {
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"Can't scroll this element - {element}; you requested for to view. {ex.Message}");
            }
            Reporter.WriteSuccessfulEventLog($"Scrolled element - {element} to view successfully.");
        }

        /// <summary>
        /// This will check if the retrieved url 'Ends' with passed in expected url and refresh the page if it matches. 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="expectedEdndOfUrl"></param>
        public static void CheckUrlMatchesAndRefresh(this IWebDriver driver, string expectedEndOfUrl)
        {
            var currentUrl = driver.Url.ToLower();
            var urlExpected = expectedEndOfUrl.ToLower();
            var count = 0;
            while (!currentUrl.EndsWith(urlExpected))
            {
                count++;
                Thread.Sleep(1000);
                if (count >= 120)
                {
                    Reporter.WriteFailureEventLog(driver, $"After looping 120 times the url still doesn't have the same ending. Expected {urlExpected} but found {currentUrl}");
                    break;
                }
                currentUrl = driver.Url.ToLower();
            }
            Reporter.WriteTestInfo($"Url retrieved : {currentUrl} is equal to expected url : {urlExpected}, I will refresh the page.");
            TestBase.DoRefreshOnPage(driver);
        }

        public static Boolean IsCurrentUrlEqualsExpectedUrl(this IWebDriver driver, string expectedUrl)
        {
            driver.Sync();
            var currentUrl = driver.Url.ToLower();
            expectedUrl = expectedUrl.ToLower();
            var count = 0;
            while (!currentUrl.Equals(expectedUrl))
            {
                Thread.Sleep(1000);
                count++;
                if (count >= 120)
                {
                    Reporter.WriteTestInfo($"After looping {count} times, current url is not equal to expected url - Current Url: {currentUrl} - Expected Url : {expectedUrl}");
                    return false;
                }
                currentUrl = driver.Url.ToLower();
            }
            Reporter.WriteTestInfo($"url :{currentUrl} is equals expected url :{expectedUrl}.");
            return true;
        }

        public static void CheckUrlContainsAndDoRefresh(this IWebDriver driver, string expectedTextForUrlToContain, string doRefresh = "yes")
        {
            var currentUrl = driver.Url.ToLower();
            var expectedTextToContain = expectedTextForUrlToContain.ToLower();
            var count = 0;
            while (!currentUrl.Contains(expectedTextToContain))
            {
                Thread.Sleep(2000);
                count++;
                if (count >= 120)
                {
                    Reporter.WriteFailureEventLog(driver, $"After looping {count} times, url doesn't contain expected text. Actual Url : {currentUrl} - Expected text : {expectedTextToContain}");
                    break;
                }
                currentUrl = driver.Url.ToLower();
            }
            Reporter.WriteTestInfo($"Url retrieved :{currentUrl} contains expected text :{expectedTextToContain}");
            if (doRefresh.Equals("yes"))
            {
                Reporter.WriteTestInfo($"I will refresh the page.");
                TestBase.DoRefreshOnPage(driver);
            }
        }

        public static void WaitTillOnlyOneWindowHandle(this IWebDriver driver, int secondsToWait)
        {
            var count = 0;
            do
            {
                Thread.Sleep(1000);
                count++;
            } while (driver.WindowHandles.Count > 1 && count <= secondsToWait);
        }

        public static void WaitTillWindowHandleCountEquals(this IWebDriver driver, int secondsToWait, int windowCount)
        {
            var count = 0;
            do
            {
                Thread.Sleep(1000);
                count++;
            } while (driver.WindowHandles.Count != windowCount && count <= secondsToWait);
        }

        public static string GetPropertyIdFromCurrentWindowUrl(this IWebDriver driver)
        {
            var currentUrl = driver.Url;
            var splittedUrl = currentUrl.Split('=');
            var propertyId = splittedUrl[1];
            return propertyId;
        }

        public static bool SearchTextInTableCell(this IWebDriver driver, string cssLocatorForCells, string searchText)
        {
            By byCssSelector = By.CssSelector(cssLocatorForCells);

            var totalItems = driver.FindElements(byCssSelector).Count();
            Reporter.WriteTestInfo($"Total elements in the list are {totalItems}");

            var cellElement = driver.FindElements(byCssSelector).FirstOrDefault(x => x.Text.ToLower().Trim() == searchText.ToLower().Trim());

            return cellElement != null;
        }

        public static string ReturnCurrentPageUrl(this IWebDriver driver)
        {
            var foundUrl = driver.Url;
            Reporter.WriteTestInfo($"Url found and returned - {foundUrl}.");
            return foundUrl;
        }

        public static void WaitTillLoadMaskIsInvisible(this IWebDriver driver, By element, int timeout = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(WebDriverTimeoutException));
            try
            {
                wait.Until(result => driver.FindElements(element).Count == 0);
            }
            catch (Exception)
            {
                Reporter.WriteTestInfo("Page is still loading");
            }
        }

        public static void ScrollToBottomOfPage(this IWebDriver driver)
        {
            try
            {
                var executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"Can't scroll to the bottom of the page. {ex.Message}");
            }
            Reporter.WriteSuccessfulEventLog($"Scrolled to the bottom of the page successfully.");
        }

        public static void ScrollToTopOfPage(this IWebDriver driver)
        {
            try
            {
                var executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("window.scrollTo(0, 0);");
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"Can't scroll to the top of the page. {ex.Message}");
            }
            Reporter.WriteSuccessfulEventLog($"Scrolled to the top of the page successfully.");
        }

        public static void ClickHoldAndMoveElementByOffsetPosition(this IWebDriver driver, IWebElement element, int offSetX, int offSetY)
        {
            try
            {
                Actions actions = new Actions(driver);
                var actionsToPerform = actions
                    .ClickAndHold(element)
                    .MoveByOffset(offSetX, offSetY)
                    .Release(element)
                    .Build();

                actionsToPerform.Perform();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"Unable to move Webelement by offset position beacuase of exception: {ex}");
            }
        }

        public static void MoveToElementWithKeysToSend(this IWebDriver driver, string address, IWebElement element)
        {
            try
            {
                Actions actions = new Actions(driver);
                var actionsToPerform = actions
                    .MoveToElement(element)
                    .Click(element)
                    .SendKeys(address)
                    .Build();

                actionsToPerform.Perform();
                Reporter.WriteSuccessfulEventLog($"I have moved to the element with address '{address}'");
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(driver, $"Unable to move to the Webelement by address and locator beacuase of exception: {ex}");
            }
        }

        public static void CloseAlert(this IWebDriver driver)
        {
            Thread.Sleep(4000);
            IAlert alert = SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent().Invoke(driver);
            if (alert != null)
            {
                driver.SwitchTo().Alert();
                alert.Accept();
                Reporter.WriteSuccessfulEventLog("Successfully closed Alert");
            }
        }

        public static void ClearLocalStorageAndRefreshPage(this IWebDriver driver)
        {
            driver.Sync();
            Thread.Sleep(5000);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(String.Format("window.localStorage.clear();"));
            driver.Manage().Cookies.DeleteAllCookies();
            Reporter.WriteSuccessfulEventLog("Cleared all local storage and browser cookies.");

            Actions actions = new Actions(driver);
            var actionsToPerform = actions
                .KeyDown(Keys.Control)
                .SendKeys(Keys.F5)
                .Build();

            actionsToPerform.Perform();

            TestBase.DoRefreshOnPage(driver);
            Thread.Sleep(5000);

            Reporter.WriteSuccessfulEventLog("Successfully hard refreshed the page.");
        }

        /// <summary>
        /// Navigates to and hovers over the specified webelement (Mouse Over)
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        public static void HoverOverWebElement(this IWebDriver driver, IWebElement element)
        {
            driver.Sync();
            Actions actions = new Actions(driver);
            var actionsToPerform = actions
                .MoveToElement(element)
                .Build();

            actionsToPerform.Perform();

            Thread.Sleep(2000);
            Reporter.WriteSuccessfulEventLog($"Successfully performed mouse-over (Hover) webelement:{element}");
        }

        public static IReadOnlyCollection<IWebElement> FindElementsWithTimeout(this IWebDriver driver, By by, int timeoutInSeconds = 20)
        {
            _configuration = General.GetConfiguration();
            var defaultElementWaitTime = double.Parse(_configuration.GetSection("DriverSettings")["ElementLoadWaitTime"]);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutInSeconds);
            IReadOnlyCollection<IWebElement> result;
            try
            {
                result = driver.FindElements(by);
                Reporter.WriteSuccessfulEventLog($"Successfully returned '{result.Count}' elements within '{timeoutInSeconds}' seconds");
            }
            catch (Exception)
            {
                Reporter.WriteTestInfo($"No elements returned within '{timeoutInSeconds}' seconds due to exception");
                result = new List<IWebElement>();
            }

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(defaultElementWaitTime);
            return result;
        }

        public static bool FindIfElementIsPresent(this IWebDriver driver, By by, int timeout = 20)
        {
            var elementPresent = false;
            if (driver.FindElementsWithTimeout(by, timeout).Count > 0)
            {
                elementPresent = true;
            }
            return elementPresent;
        }

        /**
         * Use this, if finding element by tag. Tag can be present on previous 
         * pages and therefore cause stale elements, as test will look
         * for element with same tag on previous page
         */
        public static bool FindIfElementIsPresentByText(this IWebDriver driver, By by, string text, int timeout = 20)
        {
            var elementPresent = false;
            var attempts = 0;

            var tagElements = driver.FindElementsWithTimeout(by, timeout);

            if (tagElements.Count > 0)
            {
                while (attempts < 3)
                {
                    try
                    {
                        driver.FindElementsWithTimeout(by, timeout).FirstOrDefault(x => x.Text.Equals(text));
                        elementPresent = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Reporter.WriteTestInfo($"Exception caught finding element with By '{by}' and text value of '{text}'. Exception type: '{ex.GetType()}'. Exception message: '{ex.Message}'");
                    }
                    attempts++;
                }
            }
            return elementPresent;
        }

        public static void ScrollPageBasedOnLocation(this IWebDriver driver, int X, int Y)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("javascript:window.scrollBy(" + X + "," + Y + ")");
            driver.Sync();
        }
    }
}

