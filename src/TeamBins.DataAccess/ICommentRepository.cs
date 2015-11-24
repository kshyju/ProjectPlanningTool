using System.Collections.Generic;
using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface ICommentRepository
    {
        List<CommentVM> GetComments(int issueId);
    }
    public class CommentRepository : ICommentRepository
    {
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
    }
}