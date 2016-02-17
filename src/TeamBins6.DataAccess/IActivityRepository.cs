using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins6.Common;

namespace TeamBins.DataAccess
{
    public interface IActivityRepository
    {
        IEnumerable<ActivityDto> GetActivityItems(int teamId, int count);
        int Save(ActivityDto activity);
        ActivityDto GetActivityItem(int id);
    }

    public class ActivityRepository : BaseRepo, IActivityRepository
    {
        public IEnumerable<ActivityDto> GetActivityItems(int teamId, int count)
        {

            var q = @"SELECT TOP 1000 A.[ID]
                    ,[ObjectID]
                    ,[ObjectType]
                    ,[ActivityDesc] as DeSCRIPTION
                    ,[ObjectTitle]
                    ,[OldState]
                    ,[NewState]
                    ,[TeamID]
                    ,A.[CreatedDate] as CreatedTime
                    ,U.ID
                    ,U.FirstName as Name
                    ,U.EmailAddress
                    FROM [Activity] A
                    INNER JOIN [User] U ON A.CreatedByID = U.ID
                    WHERE A.TeamID=@teamId";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<ActivityDto, UserDto, ActivityDto>(q, (a, u) =>
                {
                    a.Actor = u;
                    return a;
                }, new {@teamId = teamId}, null, true, "Id");
                return projects;
            }

        }

        public int Save(ActivityDto activity)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var q =
                    con.Query<int>(
                        @"INSERT INTO Activity(Title,Description,DueDate,CategoryId,StatusID,PriorityID,ProjectID,TeamID,Active,CreatedDate,CreatedByID) 
                        VALUES(@title,@description,@dueDate,@categoryId,@statusId,@priortiyId,@projectId,@teamId,1,@createdDate,@userId);SELECT CAST(SCOPE_IDENTITY() as int)",
                        new
                        {
                            @itemId = activity.TeamId,
                            @desc= activity.Description,
                            title = activity.CreatedTime,
                            @state = activity.ObjectTitle

                        });

                return q.First();

            }
        }

        public ActivityDto GetActivityItem(int id)
        {
            throw new NotImplementedException();
        }
    }
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
