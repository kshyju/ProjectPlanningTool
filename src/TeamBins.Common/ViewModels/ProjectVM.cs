using System.ComponentModel.DataAnnotations;

namespace TeamBins.Common.ViewModels
{
    public class ProjectVM
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public bool IsProjectOwner { set; get; }

        public bool IsDefaultProject { get; set; }
    }
}