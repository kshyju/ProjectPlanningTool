/// <reference path="../../../teambins.common/scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../../TeamBins.Common/Scripts/typings/chartjs/chart.d.ts" />
var TeamBins;
(function (TeamBins) {
    var DashboardController = (function () {
        function DashboardController($scope, summaryService, pageContext) {
            if (pageContext === void 0) { pageContext = any; }
            this.summaryService = summaryService;
            this.pageContext = pageContext;
            this.getActivityStream(this.pageContext.TeamId);
            this.getSummary();
        }
        DashboardController.prototype.getActivityStream = function (teamId) {
            var self = this;
            this.summaryService.getActivityStream(teamId, 10).then(function (data) {
                self.activities = data;
            });
        };
        DashboardController.prototype.getSummary = function () {
            var self = this;
            this.summaryService.getSummary().then(function (data) {
                self.summaryItems = data.issueCountsByStatus;
                var pieChartData = [];
                angular.forEach(self.summaryItems, function (a, b) {
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData.push(pieChartItem);
                });
                self.renderPie(pieChartData);
            });
        };
        DashboardController.prototype.renderPie = function (pieChartDataSet) {
            var ctx = document.getElementById("myChart").getContext("2d");
            new Chart(ctx).Pie(pieChartDataSet);
        };
        return DashboardController;
    }());
    TeamBins.DashboardController = DashboardController;
})(TeamBins || (TeamBins = {}));
