using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using TeamBins.DataAccessCore;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface ICommentRepository
    {
        int Save(CommentVM comment);
        CommentVM GetComment(int id);

        IEnumerable<CommentVM> GetComments(int issueId);
        void Delete(int id);
    }
    public class CommentRepository : BaseRepo, ICommentRepository
    {
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public int Save(CommentVM comment)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var p = con.Query<int>("INSERT INTO Comment(CommentText,IssueID,CreatedDate,CreatedByID) VALUES (@cmnt,@issueId,@dt,@createdById);SELECT CAST(SCOPE_IDENTITY() as int)",
                                        new { cmnt = comment.CommentText, @issueId = comment.IssueId, @dt = DateTime.Now, @createdById = comment.Author.Id });
                return p.First();

            }
        }

        public IEnumerable<CommentVM> GetComments(int issueId)
        {
            var q = @"SELECT C.*,U.Id,U.FIRSTNAME AS NAME,U.EmailAddress FROM COMMENT C WITH (NOLOCK) 
                    INNER JOIN [USER] U WITH (NOLOCK)  ON C.CREATEDBYID=U.Id
                    WHERE C.IssueId=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = con.Query<CommentVM, UserDto, CommentVM>(q, (c, a) => { c.Author = a; return c; }, new { @id = issueId }, null, false).ToList();
                return com;
            }

        }
        public void Delete(int id)
        {
            var q = @"DELETE FROM COMMENT WHERE Id=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Query<int>(q, new {id });

            }
        }

        public CommentVM GetComment(int id)
        {
            var q = @"SELECT C.*,U.Id,
                    U.FIRSTNAME AS Name,U.EmailAddress,
                    I.ID, I.Title
                    FROM COMMENT C WITH (NOLOCK) 
                    INNER JOIN [USER] U WITH (NOLOCK)  ON C.CREATEDBYID=U.Id
                    INNER JOIN Issue I WITH (NOLOCK) ON I.ID=C.IssueID
                    WHERE C.Id=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = con.Query<CommentVM, UserDto, IssueVM, CommentVM>(q, (c, a,i) => { c.Author = a;
                    c.Issue = i; return c; }, new {id }, null, false).ToList();
                return com.FirstOrDefault();
            }
        }
    }
}