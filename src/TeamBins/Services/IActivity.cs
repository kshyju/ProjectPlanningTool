using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins.Services
{
    public interface IActivitySavable
    {
        ActivityVM GetActivityVM(IActivity activity);
        //Activity SaveActivity(IActivity activity);
    }
}