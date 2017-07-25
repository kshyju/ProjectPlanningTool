using System.Text;

namespace TeamBins.Infrastrucutre
{
    public class AppInsightSetting
    {
        public string InstrumentationKey { set; get; }
    }
    public class AppSettings
    {
        public AppInsightSetting ApplicationInsights { set; get; }
        public TeamBinsAppSettings TeamBins { set; get; }
    }
    public class TeamBinsAppSettings
    {
        /// <summary>
        /// Google analytics unique key
        /// </summary>
        public string GoogleAnalyticsKey { set; get; }
        public string LocalFileSystemStorageUriPrefix { set; get; }
        public string LocalFileSystemStoragePath { set; get; }
        public string AzureblobStorageConnectionString { set; get; }

        public DataSetting Data { set; get; }

        public EmailSetting Email { set; get; }

        public string SiteUrl { set; get; }

        /// <summary>
        /// Redis conn string to be used by StackExchange.Redis
        /// </summary>
        public string RedisConnectionString { set; get; }
}

    public class EmailSetting
    {
        public string SmtpServer { set; get; }
        public string BccEmailAddress { set; get; }
        public string FromEmailAddress { set; get; }
        public int Port { set; get; }
        public string UserName { get;  set; }
        public string Password { get;  set; }
    }
    public class DataSetting
    {
        public string MyTestSetting { set; get; }
    }
}