using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamBins.Helpers.Enums
{
    public enum IssueMemberRelationType
    {
        Member,
        Star,
        Following
    }
    public enum LocationType
    {
        SPRNT,
        ARCHV, 
        BKLOG
    }
    public enum ActivityObjectType
    {
        Issue,
        IssueComment
    }
}