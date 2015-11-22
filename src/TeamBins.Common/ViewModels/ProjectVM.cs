namespace TeamBins.Common.ViewModels
{
    public class ProjectVM
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public bool IsProjectOwner { set; get; }

        public bool IsDefaultProject { get; set; }
    }
}