using Microsoft.Extensions.Configuration;

namespace AmazonAws.Shared
{
    public static class ConfigManager
    {
        private static IConfiguration Configuration { get; set; }
        public static Settings ConfigSettings { get; private set; }

        static ConfigManager()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ConfigSettings = Configuration.GetRequiredSection("Settings").Get<Settings>();
        }
    }
}
