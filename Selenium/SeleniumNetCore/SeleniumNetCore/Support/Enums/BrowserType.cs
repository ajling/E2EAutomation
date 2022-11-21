namespace SeleniumNetCore.Support.Enums
{
    public enum BrowserType
    {
        Chrome = 0,
        Edge = 1,
        Firefox = 2,
        Safari = 3
    }

    public static class BrowserTypeName
    {
        public static string GetBrowserName(BrowserType browserType)
        {
            var browserName = string.Empty;
            switch (browserType)
            {
                case BrowserType.Edge:
                    browserName = "Edge";
                    break;
                case BrowserType.Firefox:
                    browserName = "FireFox";
                    break;
                case BrowserType.Safari:
                    browserName = "Safari";
                    break;
                case BrowserType.Chrome:
                default:
                    browserName = "Chrome";
                    break;
            }
            return browserName;
        }
    }
}
