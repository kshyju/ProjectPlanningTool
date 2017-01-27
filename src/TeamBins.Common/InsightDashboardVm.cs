using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamBins.Common.ViewModels
{
    public class InsightDashboardVm
    {
        public IEnumerable<TeamDto> Teams { set; get; }
        public IEnumerable<UserDto> Users { get; set; }
        public IEnumerable<IssueVM> Issues { get; set; }

        public InsightDashboardVm()
        {
            this.Teams=new List<TeamDto>();
            this.Users=new List<UserDto>();
            
        }
    }
}