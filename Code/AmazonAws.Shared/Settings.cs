using System.Configuration;

namespace AmazonAws.Shared
{
    public static class Settings
    {
        public static string AccessKey
        {
            get { return ConfigurationManager.AppSettings["AccessKey"]; }
        }

        public static string Secret
        {
            get { return ConfigurationManager.AppSettings["Secret"]; }
        }

        public static string Region
        {
            get { return ConfigurationManager.AppSettings["AWSRegion"]; }
        }
    }
}
