using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface ICommentRepository
    {
        List<CommentVM> GetComments(int issueId);
        int Save(CommentVM comment);
        CommentVM GetComment(int commentId);
    }
    public class CommentRepository : ICommentRepository
    {
        public CommentVM GetComment(int commentId)
        {
            using (var db = new TeamEntitiesConn())
            {
                var comment = db.Comments.FirstOrDefault(s => s.ID == commentId);
                if (comment != null)
                {
                    return new CommentVM
                    {
                        CommentBody = comment.CommentText,
                        CreativeDate = comment.CreatedDate,
                        Author =  new UserDto { EmailAddress = comment.Author.EmailAddress, Name = comment.Author.FirstName },
                        Issue =  new IssueVM {  ID = comment.Issue.ID, Title = comment.Issue.Title}
                    };
                }
            }
            return null;
        }

        public List<CommentVM> GetComments(int issueId)
        {
            var db = new TeamEntitiesConn();
            return db.Comments.Where(s => s.IssueID == issueId)
                .Select(s => new CommentVM
                {
                    ID = s.ID,
                    CommentBody = s.CommentText,
                    CreativeDate = s.CreatedDate,
                    Author = new UserDto { EmailAddress = s.Author.EmailAddress, Name = s.Author.FirstName }
                }).ToList();
        }

        public int Save(CommentVM comment)
        {
            using (var db = new TeamEntitiesConn())
            {
                var commentEntity = new Comment();
                commentEntity.IssueID = comment.IssueId;
                commentEntity.CommentText = comment.CommentBody;
                commentEntity.CreatedDate = DateTime.Now;
                commentEntity.CreatedByID = comment.Author.Id;
                ;
                db.Comments.Add(commentEntity);
                db.SaveChanges();
                return commentEntity.ID;
            }
        }
    }
}