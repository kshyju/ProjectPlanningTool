using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins.Services
{
    public interface IActivity
    {
        ActivityVM GetActivityVM(Activity activity);        
    }
}