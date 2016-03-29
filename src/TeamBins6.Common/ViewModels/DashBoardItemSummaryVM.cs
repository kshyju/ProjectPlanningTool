using System.Collections.Generic;

namespace TeamBins.Common.ViewModels
{
    public class DashBoardItemSummaryVM
    {
        public int CurrentItems { set; get; }
        public int ItemsInProgress { set; get; }
        public int NewItems { set; get; }
        public int BacklogItems { set; get; }
        public int CompletedItems { set; get; }

        public IEnumerable<ItemCount> IssueCountsByStatus { set; get; } 
    }
}