var TeamBins;
(function (TeamBins) {
    var DashboardController = (function () {
        function DashboardController(summaryService) {
            this.summaryService = summaryService;
            this.getActivityStream();
            this.getSummary();
        }
        DashboardController.prototype.getActivityStream = function () {
            var self = this;
            this.summaryService.getActivityStream().then(function (data) {
                self.activities = data;
            });
        };
        DashboardController.prototype.getSummary = function () {
            var self = this;
            this.summaryService.getSummary().then(function (data) {
                self.summary = data.IssueCountsByStatus;
            });
        };
        return DashboardController;
    })();
    TeamBins.DashboardController = DashboardController;
})(TeamBins || (TeamBins = {}));
