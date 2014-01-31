using System.Collections.Generic;
using System.Linq;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;
using System;

namespace TeamBins.Services
{
    public class IssueService : IDisposable
    {
        IRepositary repo;
        public string SiteBaseURL { set; get; }
      
        public IssueService(IRepositary repositary)
        {
            repo = repositary;
        }

        public List<ActivityVM> GetTeamActivityVMs(int teamId)
        {
            List<ActivityVM> activityVMList = new List<ActivityVM>();
            try
            {
                var activityList = repo.GetTeamActivity(teamId).OrderByDescending(s => s.CreatedDate).ToList();

                foreach (var item in activityList)
                {
                    var activityVM = GetActivityVM(item);
                    activityVMList.Add(activityVM);
                }
            }
            catch (Exception ex)
            {
                
            }
            return activityVMList;
        }

        public ActivityVM GetActivityVM(Activity item)
        {
            var activityVM = new ActivityVM() { Author = item.User.FirstName, CreatedDateRelative = item.CreatedDate.ToString() };
            if (item.ActivityDesc.ToUpper() == "CREATED")
            {
                activityVM.Activity = item.ActivityDesc;
                activityVM.ObjectTite = item.NewState;
                activityVM.ObjectURL = String.Format("{0}Issues/details/{1}", SiteBaseURL, item.ObjectID);
            }
            return activityVM;
        }

        public void Dispose()
        {
            if (repo != null)
            {
                repo.Dispose();
            }
        }
    }
}
