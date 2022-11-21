using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using SeleniumNetCore.Support.Reporting;
using SeleniumNetCore.Support.WebElement;

namespace SeleniumNetCore.Support.WebDriver
{
    internal class WebDriverActionListeners : EventFiringWebDriver
    {
        private IWebDriver _driver;

        private string elementDetails;

        private string inputElementDetails;

        public WebDriverActionListeners(IWebDriver driver) : base(driver)
        {
            _driver = driver;
        }

        protected override void OnException(WebDriverExceptionEventArgs e)
        {
            base.OnException(e);
            Reporter.WriteFailureEventLog(_driver, $"Exception Triggered - {e.ThrownException.Message}");
        }

        protected override void OnElementClicking(WebElementEventArgs e)
        {
            base.OnElementClicking(e);
            _driver.Sync();
            elementDetails = e.Element.GetReportName();
        }

        protected override void OnElementClicked(WebElementEventArgs e)
        {
            base.OnElementClicked(e);
            Reporter.WriteSuccessfulEventLog($"Successfully clicked on the element '{elementDetails}'");
            _driver.Sync();
        }


        protected override void OnElementValueChanging(WebElementValueEventArgs e)
        {
            base.OnElementValueChanging(e);
            inputElementDetails = e.Element.GetReportName();
        }

        protected override void OnElementValueChanged(WebElementValueEventArgs e)
        {
            base.OnElementValueChanged(e);
            _driver.Sync();
            Reporter.WriteSuccessfulEventLog($"Successfully updated the field '{inputElementDetails}' value ");
        }

        protected override void OnNavigated(WebDriverNavigationEventArgs e)
        {
            base.OnNavigated(e);
            _driver.Sync();
            Reporter.WriteSuccessfulEventLog("Navigated to the site successfully");
        }

        protected override void OnNavigatedBack(WebDriverNavigationEventArgs e)
        {
            base.OnNavigatedBack(e);
            _driver.Sync();
            Reporter.WriteSuccessfulEventLog("Navigated to back successfully.");
        }

        protected override void OnNavigatingForward(WebDriverNavigationEventArgs e)
        {
            base.OnNavigatingForward(e);
            _driver.Sync();
            Reporter.WriteSuccessfulEventLog("Navigated to forward successfully");
        }

        protected override void OnScriptExecuting(WebDriverScriptEventArgs e)
        {
            base.OnScriptExecuting(e);
            _driver.Sync();
            Reporter.WriteSuccessfulEventLog($"Script executed successfully.");
        }
    }
}
