using System.Threading.Tasks;
using TeamBins.Common;

namespace TeamBins.Services
{
    public interface ICommentEmailManager
    {
        Task SendNewCommentEmail(CommentVM comment, int teamId);
    }
}