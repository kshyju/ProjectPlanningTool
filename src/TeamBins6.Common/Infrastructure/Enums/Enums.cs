using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamBins.Common.Infrastructure.Enums
{
    namespace TeamBins.Helpers.Enums
    {
        public enum IssueMemberRelationType
        {
            Assigned,
            Starred,
            Following
        }
       
        public enum ActivityObjectType
        {
            Issue,
            IssueComment
        }

        public enum NotificationTypeCode
        {
            NewComment,
            NewIssue
        }

    }
}
