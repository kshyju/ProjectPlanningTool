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
            this.getIssuesGroupedByPriority();
        }
        DashboardController.prototype.getActivityStream = function (teamId) {
            var self = this;
            this.summaryService.getActivityStream(teamId, 10).then(function (data) {
                self.activities = data;
            });
        };
        DashboardController.prototype.getIssuesGroupedByPriority = function () {
            var self = this;
            this.summaryService.getIssuesGroupedByPriority().then(function (data) {
                self.issuesGroupedByPriority = data;
                var pieChartData2 = [];
                angular.forEach(self.issuesGroupedByPriority, function (a, b) {
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData2.push(pieChartItem);
                });
                self.renderPie(pieChartData2, "issuesPriorityPieChart");
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
                self.renderPie(pieChartData, "myChart");
            });
        };
        DashboardController.prototype.renderPie = function (pieChartDataSet, elementId) {
            var ctx = document.getElementById(elementId).getContext("2d");
            new Chart(ctx).Pie(pieChartDataSet);
        };
        return DashboardController;
    }());
    TeamBins.DashboardController = DashboardController;
})(TeamBins || (TeamBins = {}));
