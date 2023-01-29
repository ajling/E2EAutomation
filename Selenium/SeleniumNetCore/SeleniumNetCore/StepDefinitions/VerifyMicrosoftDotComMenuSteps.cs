using SeleniumNetCore.PageObjects;
using SeleniumNetCore.Support;
using SeleniumNetCore.Support.Enums;
using TechTalk.SpecFlow;

namespace SeleniumNetCore.StepDefinitions
{
    [Binding]
    public class VerifyMicrosoftDotComMenuSteps : TestBase
    {
        MicrosoftHomePage microsoftHomePage;

        [Given(@"I navigate to Microsoft\.com")]
        public void GivenINavigateToMicrosoft_Com()
        {
            OpenBrowser(BrowserType.Chrome, "Microsoft", "Verify Microsoft.com Menu");
            microsoftHomePage = NavigateTo<MicrosoftHomePage>("www.microsoft.com");
        }

        [When(@"I accept cookies")]
        public void WhenIAcceptCookies()
        {
            microsoftHomePage.ClickAcceptCookies();
        }

        [Then(@"I verify the menu item (.*)")]
        public void ThenIVerifyTheMenuItem(string menuItemText)
        {
            microsoftHomePage.VerifyMenuItemDisplayed(menuItemText);
        }

    }
}