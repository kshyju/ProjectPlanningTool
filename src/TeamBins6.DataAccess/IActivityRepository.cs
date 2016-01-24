using System;
using System.Collections.Generic;
using System.Linq;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface IActivityRepository
    {
        List<ActivityDto> GetActivityItems(int count);
        int Save(ActivityDto activity);
        ActivityDto GetActivityItem(int id);
    }
    //public class ActivityRepository : IActivityRepository
    //{
    //    public ActivityDto GetActivityItem(int id)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var x = db.Activities.FirstOrDefault(s=>s.ID==id);


    //            return new ActivityDto
    //            {
    //                ObjectId = x.ObjectID,
    //                CreatedTime = x.CreatedDate,
    //                ObjectTite = x.ObjectTitle,
    //                Actor = new UserDto {Id = x.User.ID, Name = x.User.FirstName, EmailAddress = x.User.EmailAddress},
    //                NewState = x.NewState,
    //                ObjectType = x.ObjectType,
    //                Description = x.ActivityDesc
    //            };

    //        }
    //    }

    //    public List<ActivityDto> GetActivityItems(int count)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //           return db.Activities
    //                .OrderByDescending(g=>g.CreatedDate)
    //                .Take(count)
    //                .Select(x => new ActivityDto
    //            {
    //               ObjectId =  x.ObjectID,
    //               CreatedTime = x.CreatedDate,
    //               ObjectTite = x.ObjectTitle,
    //                    Actor = new UserDto {  Id = x.User.ID, Name = x.User.FirstName, EmailAddress = x.User.EmailAddress},
    //               NewState = x.NewState,
    //               ObjectType = x.ObjectType,
    //               Description = x.ActivityDesc
    //            }).ToList();

    //        }
    //    }

    //    public int Save(ActivityDto activity)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var a = new Activity
    //            {
    //                OldState = activity.OldState,
    //                NewState = activity.NewState,
    //                ObjectID = activity.ObjectId,
    //                ObjectType = activity.ObjectType,
    //                ActivityDesc = activity.Description,
    //                ObjectTitle = activity.ObjectTite,
    //                CreatedByID = activity.Actor.Id,
    //                TeamID = activity.TeamId,
    //                CreatedDate = DateTime.Now
    //            };

    //            db.Activities.Add(a);
    //            db.SaveChanges();
    //            return a.ID;
    //        }
    //    }
    //}
}