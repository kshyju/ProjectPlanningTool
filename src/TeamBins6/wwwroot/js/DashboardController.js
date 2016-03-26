var TeamBins;
(function (TeamBins) {
    var DashboardController = (function () {
        function DashboardController(summaryService) {
            this.summaryService = summaryService;
            this.getActivityStream();
        }
        DashboardController.prototype.getActivityStream = function () {
            var self = this;
            this.summaryService.getActivityStream().then(function (data) {
                self.activities = data;
            });
        };
        return DashboardController;
    })();
    TeamBins.DashboardController = DashboardController;
})(TeamBins || (TeamBins = {}));
