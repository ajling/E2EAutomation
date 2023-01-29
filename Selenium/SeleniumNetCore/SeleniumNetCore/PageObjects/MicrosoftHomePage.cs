using OpenQA.Selenium;
using SeleniumNetCore.Support;
using SeleniumNetCore.Support.Reporting;
using SeleniumNetCore.Support.WebDriver;
using SeleniumNetCore.Support.WebElement;

namespace SeleniumNetCore.PageObjects
{
    public class MicrosoftHomePage : TestBase
    {
        private IWebDriver _driver;

        private IWebElement AcceptCookiesButton => driver.FindElement(By.XPath("//button[contains(text(), 'Accept')]"));
        private IWebElement Microsoft365MenuItem => driver.FindElement(By.Id("shellmenu_0"));

        public MicrosoftHomePage()
        {
            _driver = GetDriver;
            _driver.Sync();
            if (!Microsoft365MenuItem.IsDisplayed())
            {
                Reporter.WriteFailureEventLog(driver, "Microsoft home page is not loaded");
            }
            Reporter.WriteSuccessfulEventLog("Microsoft home page loaded successfully");
        }

        public MicrosoftHomePage ClickAcceptCookies()
        {
            _driver.Sync();
            if (AcceptCookiesButton.IsDisplayed())
            {
                AcceptCookiesButton.ClickElement();
            }
            return this;
        }

        public MicrosoftHomePage VerifyMenuItemDisplayed(string menuItem) 
        {
            var elements = _driver.FindElements(By.XPath($"//a[contains(text(), '{menuItem}')]"));
            if (elements.Count == 0)
            {
                Reporter.WriteFailureEventLog(_driver, $"The menu item '{menuItem}' is not displayed.");
            }
            Reporter.WriteSuccessfulVerification($"Successfully verified the menu item '{menuItem}' is displayed");
            return this;
        }
    }
}
