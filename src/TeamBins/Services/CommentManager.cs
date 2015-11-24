using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public interface ICommentManager
    {
        IEnumerable<CommentVM> GetComments(int issueId);
    }
    public class CommentManager : ICommentManager
    {
        ICommentRepository commentRepository;

        public CommentManager(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }
        public IEnumerable<CommentVM> GetComments(int issueId)
        {
            return commentRepository.GetComments(issueId);
        }
    }
}