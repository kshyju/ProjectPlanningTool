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
            this.getSummary(this.pageContext.TeamId);
            this.getIssuesGroupedByPriority(this.pageContext.TeamId);
            this.getIssuesGroupedByProject(this.pageContext.TeamId);
        }
        DashboardController.prototype.getActivityStream = function (teamId) {
            var self = this;
            this.summaryService.getActivityStream(teamId, 10).then(function (data) {
                self.activities = data;
            });
        };
        DashboardController.prototype.getIssuesGroupedByProject = function (teamId) {
            var self = this;
            this.summaryService.getIssuesGroupedByProject(teamId, 10).then(function (data) {
                self.issuesGroupedByProject = data;
            });
        };
        DashboardController.prototype.getIssuesGroupedByPriority = function (teamId) {
            var self = this;
            this.summaryService.getIssuesGroupedByPriority(teamId).then(function (data) {
                self.issuesGroupedByPriority = data;
                var pieChartData2 = [];
                angular.forEach(self.issuesGroupedByPriority, function (a, b) {
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData2.push(pieChartItem);
                });
                self.renderPie(pieChartData2, "issuesPriorityPieChart");
            });
        };
        DashboardController.prototype.getSummary = function (teamId) {
            var self = this;
            this.summaryService.getSummary(teamId).then(function (data) {
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
