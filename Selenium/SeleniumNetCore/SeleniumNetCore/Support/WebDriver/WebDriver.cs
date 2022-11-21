using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using SeleniumNetCore.Support.Enums;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace SeleniumNetCore.Support.WebDriver
{
    public static class WebDriver
    {
        public static WebDriverWait wait;
        public static IConfiguration _configuration;

        internal static WebDriverActionListeners InitializeDriver(BrowserType browserType, IConfiguration configuration, string testCategory, string testName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IWebDriver _driver = null;
            _configuration = configuration;

            var useBrowserStack = bool.Parse(_configuration.GetSection("DriverSettings")["UseBrowserStack"]);
            var bsUserName = _configuration.GetSection("BrowserStackSettings")["BrowserStackUser"];
            var bsKey = _configuration.GetSection("BrowserStackSettings")["BrowserStackKey"];
            var useLocalGrid = bool.Parse(_configuration.GetSection("GridSettings")["UseLocalGrid"]);
            var useMobileView = bool.Parse(_configuration.GetSection("DriverSettings")["UseMobileView"]);
            var mobileDeviceName = _configuration.GetSection("DriverSettings")["MobileDeviceName"];
            var hubIpAddress = _configuration.GetSection("GridSettings")["HubIpAddress"];

            string pathToChrome = 
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
                _configuration.GetSection("BrowserSettings")["PathToChrome"] : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 
                _configuration.GetSection("BrowserSettings")["PathToChromeLinux"] : _configuration.GetSection("BrowserSettings")["PathToChromeMacOS"];

            var pathToEdge = _configuration.GetSection("BrowserSettings")["PathToEdge"];

            var elementLoadWaitTime = double.Parse(_configuration.GetSection("DriverSettings")["ElementLoadWaitTime"]);
            var pageLoadWaitTime = double.Parse(_configuration.GetSection("DriverSettings")["PageLoadWaitTime"]);
            var remoteWebDriverWaitTime = double.Parse(_configuration.GetSection("DriverSettings")["RemoteWebDriverWaitTime"]);

            if (useLocalGrid)
            {
                _driver = InitializeLocalGridWebDriver(browserType, useMobileView, useBrowserStack, hubIpAddress, remoteWebDriverWaitTime, testCategory, testName, bsUserName, bsKey, browserType == BrowserType.Chrome ? pathToChrome : pathToEdge, mobileDeviceName);
            }
            else if (browserType == BrowserType.Chrome)
            {
                _driver = InitializeChromeWebDriver(useBrowserStack, useMobileView, testCategory, testName, bsUserName, bsKey, pathToChrome, hubIpAddress, remoteWebDriverWaitTime, mobileDeviceName);
            }
            else if (browserType == BrowserType.Edge)
            {
                _driver = InitializeEdgeWebDriver(useBrowserStack, useMobileView, testCategory, testName, bsUserName, bsKey, pathToEdge, hubIpAddress, remoteWebDriverWaitTime, mobileDeviceName);
            }
            else if (browserType == BrowserType.Safari)
            {
                _driver = InitializeSafariWebDriver(useBrowserStack, useMobileView, testCategory, testName, bsUserName, bsKey, hubIpAddress, remoteWebDriverWaitTime);
            }
            else
            {
                _driver = InitializeFirefoxWebDriver(useBrowserStack, useMobileView, testCategory, testName, bsUserName, bsKey, hubIpAddress, remoteWebDriverWaitTime);
            }

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(elementLoadWaitTime);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadWaitTime);
            _driver.Manage().Cookies.DeleteAllCookies();
            if (browserType != BrowserType.Chrome && !useMobileView)
            {
                _driver.Manage().Window.Maximize();
            }

            var allowsDetection = (IAllowsFileDetection)_driver;
            allowsDetection.FileDetector = new LocalFileDetector();
            return new WebDriverActionListeners(_driver);
        }

        
        private static IWebDriver InitializeChromeWebDriver(bool useBrowserStack, bool useMobileView, string testCategory, string testName, string bsUserName, string bsKey, string pathToChrome, string hubIpAddress, double remoteWebDriverWaitTime, string mobileDeviceName)
        {
            if (useBrowserStack)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetChromeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, pathToChrome, mobileDeviceName).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else
            {
                return new ChromeDriver(GetChromeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, pathToChrome, mobileDeviceName));
            }
        }

        private static IWebDriver InitializeEdgeWebDriver(bool useBrowserStack, bool useMobileView, string testCategory, string testName, string bsUserName, string bsKey, string pathToEdge, string hubIpAddress, double remoteWebDriverWaitTime, string mobileDeviceName)
        {
            if (useBrowserStack)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetEdgeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, pathToEdge, mobileDeviceName).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else
            {
                return new EdgeDriver(GetEdgeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, pathToEdge, mobileDeviceName));
            }
        }

        private static IWebDriver InitializeSafariWebDriver(bool useBrowserStack, bool useMobileView, string testCategory, string testName, string bsUserName, string bsKey, string hubIpAddress, double remoteWebDriverWaitTime)
        {
            if (useBrowserStack)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetSafariOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else
            {
                return new SafariDriver(GetSafariOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey));
            }
        }

        private static IWebDriver InitializeFirefoxWebDriver(bool useBrowserStack, bool useMobileView, string testCategory, string testName, string bsUserName, string bsKey, string hubIpAddress, double remoteWebDriverWaitTime)
        {
            if (useBrowserStack)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetFirefoxOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else
            {
                /*
                 * Below commented section was occurring in v29 of gecko driver, doesn't seem to be an issue in v30 of gecko driver
                // the below three lines of code are required for firefox to run quickly - see below links.
                // i.e. with this code a test can take 1 min, without the code, the same test can take 8 mins
                // 1) https://stackoverflow.com/questions/53629542/selenium-geckodriver-executes-findelement-10-times-slower-than-chromedriver-ne
                // 2) https://github.com/SeleniumHQ/selenium/issues/6597
                //string geckoDriverDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); //"Path of geckodriver.exe"
                //FirefoxDriverService geckoService = FirefoxDriverService.CreateDefaultService(geckoDriverDirectory);
                //geckoService.Host = "::1";
                //var driver = new FirefoxDriver(geckoService, GetFirefoxOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, enableLogging));
                */
                var driver = new FirefoxDriver(GetFirefoxOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey));
                if (useMobileView)
                {
                    driver.Manage().Window.Size = new System.Drawing.Size(812, 375);
                }
                else
                {
                    driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
                }
                return driver;
            }
        }

        private static IWebDriver InitializeLocalGridWebDriver(BrowserType browserType, bool useMobileView, bool useBrowserStack, string hubIpAddress, double remoteWebDriverWaitTime, string testCategory, string testName, string bsUserName, string bsKey, string path, string mobileDeviceName)
        {
            if (browserType == BrowserType.Chrome)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetChromeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, path, mobileDeviceName).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else if (browserType == BrowserType.Edge)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetEdgeOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey, path, mobileDeviceName).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else if (browserType == BrowserType.Safari)
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetSafariOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
            else
            {
                return new RemoteWebDriver(
                    new Uri($"{hubIpAddress}"),
                    GetFirefoxOptions(useMobileView, useBrowserStack, testCategory, testName, bsUserName, bsKey).ToCapabilities(),
                    TimeSpan.FromSeconds(remoteWebDriverWaitTime)
                );
            }
        }


        private static FirefoxOptions GetFirefoxOptions(bool useMobileView, bool useBrowserStack, string testCategory, string testName, string bsUserName, string bsKey)
        {
            var fireFoxOptions = new FirefoxOptions();
            fireFoxOptions.AcceptInsecureCertificates = true;


            if (useMobileView)
            {
                var user_agent = "Mozilla/5.0 (iPhone X; U; CPU iPhone OS 3_0 like Mac OS X; en-us) AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16";
                var fireFoxProfile = fireFoxOptions.Profile;
                fireFoxProfile.SetPreference("general.useragent.override", user_agent);
            }

            if (useBrowserStack)
            {
                fireFoxOptions.AddAdditionalFirefoxOption("os", "Windows");
                fireFoxOptions.AddAdditionalFirefoxOption("os_version", "10");
                fireFoxOptions.AddAdditionalFirefoxOption("browser", "Firefox");
                fireFoxOptions.AddAdditionalFirefoxOption("browser_version", "latest");
                fireFoxOptions.AddAdditionalFirefoxOption("project", testCategory);
                fireFoxOptions.AddAdditionalFirefoxOption("name", testName);
                fireFoxOptions.AddAdditionalFirefoxOption("resolution", "1920x1080");
                fireFoxOptions.AddAdditionalFirefoxOption("browserstack.local", "false");
                fireFoxOptions.AddAdditionalFirefoxOption("browserstack.video", "false");
                fireFoxOptions.AddAdditionalFirefoxOption("browserstack.selenium_version", "4.0.1");
                fireFoxOptions.AddAdditionalFirefoxOption("browserstack.user", bsUserName);
                fireFoxOptions.AddAdditionalFirefoxOption("browserstack.key", bsKey);

                return fireFoxOptions;
            }
            else
            {
                return fireFoxOptions;
            }
        }

        private static EdgeOptions GetEdgeOptions(bool useMobileView, bool useBrowserStack, string testCategory, string testName, string bsUserName, string bsKey, string pathToEdge, string mobileDeviceName)
        {
            var edgeOptions = new EdgeOptions();

            if (useMobileView)
            {
                edgeOptions.EnableMobileEmulation(mobileDeviceName);
            }

            if (useBrowserStack)
            {
                edgeOptions.AddAdditionalEdgeOption("os", "Windows");
                edgeOptions.AddAdditionalEdgeOption("os_version", "10");
                edgeOptions.AddAdditionalEdgeOption("browser", "Edge");
                edgeOptions.AddAdditionalEdgeOption("browser_version", "latest");
                edgeOptions.AddAdditionalEdgeOption("project", testCategory);
                edgeOptions.AddAdditionalEdgeOption("name", testName);
                edgeOptions.AddAdditionalEdgeOption("resolution", "1920x1080");
                edgeOptions.AddAdditionalEdgeOption("browserstack.local", "false");
                edgeOptions.AddAdditionalEdgeOption("browserstack.video", "false");
                edgeOptions.AddAdditionalEdgeOption("browserstack.selenium_version", "3.141.0");
                edgeOptions.AddAdditionalEdgeOption("browserstack.user", bsUserName);
                edgeOptions.AddAdditionalEdgeOption("browserstack.key", bsKey);

                return edgeOptions;
            }
            else
            {
                edgeOptions.AddArgument("--window-size=1920,1080");
                edgeOptions.AddArgument("--start-maximized");

                if (!string.IsNullOrEmpty(pathToEdge))
                {
                    edgeOptions.BinaryLocation = pathToEdge;
                }

                return edgeOptions;
            }
        }

        private static ChromeOptions GetChromeOptions(bool useMobileView, bool useBrowserStack, string testCategory, string testName, string bsUserName, string bsKey, string path, string mobileDeviceName)
        {
            var chromeOptions = new ChromeOptions();

            if (useMobileView)
            {
                chromeOptions.EnableMobileEmulation(mobileDeviceName);
            }

            if (useBrowserStack)
            {
                chromeOptions.AddAdditionalChromeOption("os", "Windows");
                chromeOptions.AddAdditionalChromeOption("os_version", "10");
                chromeOptions.AddAdditionalChromeOption("browser", "Chrome");
                chromeOptions.AddAdditionalChromeOption("browser_version", "latest");
                chromeOptions.AddAdditionalChromeOption("project", testCategory);
                chromeOptions.AddAdditionalChromeOption("name", testName);
                chromeOptions.AddAdditionalChromeOption("resolution", "1920x1080");
                chromeOptions.AddAdditionalChromeOption("browserstack.local", "false");
                chromeOptions.AddAdditionalChromeOption("browserstack.selenium_version", "4.0.1");
                chromeOptions.AddAdditionalChromeOption("browserstack.user", bsUserName);
                chromeOptions.AddAdditionalChromeOption("browserstack.key", bsKey);
                chromeOptions.AddArgument("--start-maximized");

                return chromeOptions;
            }
            else
            {
                chromeOptions.AddArgument("--window-size=1920,1080");
                chromeOptions.AddArgument("--start-maximized");
                // below arguments are for when running on the build server
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--disable-extensions");
                    chromeOptions.AddArgument("disable-infobars");
                    Console.WriteLine("You are running on Linux");
                }

                if (!string.IsNullOrEmpty(path))
                {
                    chromeOptions.BinaryLocation = path;
                }

                return chromeOptions;
            }
        }

        private static SafariOptions GetSafariOptions(bool useMobileView, bool useBrowserStack, string testCategory, string testName, string bsUserName, string bsKey)
        {
            var safariOptions = new SafariOptions();

            if (useMobileView)
            {
                //safariOptions..EnableMobileEmulation(mobileDeviceName);
            }

            if (useBrowserStack)
            {
                safariOptions.AddAdditionalOption("os", "OS X");
                safariOptions.AddAdditionalOption("osVersion", "Catalina");
                safariOptions.AddAdditionalOption("browserVersion", "15.0");
                safariOptions.AddAdditionalOption("project", testCategory);
                safariOptions.AddAdditionalOption("name", testName);
                safariOptions.AddAdditionalOption("resolution", "1920x1080");
                safariOptions.AddAdditionalOption("local", "false");
                safariOptions.AddAdditionalOption("seleniumVersion", "4.0.0-alpha-6");
                safariOptions.AddAdditionalOption("userName", bsUserName);
                safariOptions.AddAdditionalOption("accessKey", bsKey);
                safariOptions.AddAdditionalOption("enablePopups", true);
                safariOptions.AddAdditionalOption("allowAllCookies", true);

                return safariOptions;
            }
            else
            {
                safariOptions.AddAdditionalOption("resolution", "1920x1080");
                safariOptions.AddAdditionalOption("enablePopups", true);
                safariOptions.AddAdditionalOption("allowAllCookies", true);
                return safariOptions;
            }

        }
    }
}