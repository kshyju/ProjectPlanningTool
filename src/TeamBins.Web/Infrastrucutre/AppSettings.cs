namespace TeamBins.Infrastrucutre
{
    public class AppSettings
    {
        public string LocalFileSystemStorageUriPrefix { set; get; }
        public string LocalFileSystemStoragePath { set; get; }
        public string AzureblobStorageConnectionString { set; get; }

        public DataSetting Data { set; get; }
    }

    public class DataSetting
    {
        public string MyTestSetting { set; get; }
    }
}