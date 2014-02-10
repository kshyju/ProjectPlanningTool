using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamBins.DataAccess
{
    public interface IActivity
    {
        int ObjectID { get; set; }
        string ObjectType { get; set; }
        string ActivityDesc { get; set; }
        string OldState { get; set; }
        string NewState { get; set; }
        int CreatedByID { get; set; }
        System.DateTime CreatedDate { get; set; }
        int TeamID { get; set; }
        User User { set; get; }
    }
    public partial class Activity : IActivity
    {

    }
}
