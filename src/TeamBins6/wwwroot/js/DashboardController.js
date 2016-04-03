/// <reference path="../../../teambins.common/scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../../TeamBins.Common/Scripts/typings/chartjs/chart.d.ts" />
var TeamBins;
(function (TeamBins) {
    var DashboardController = (function () {
        function DashboardController($scope, summaryService) {
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
                self.summaryItems = data.IssueCountsByStatus;
                var pieChartData = [];
                angular.forEach(self.summaryItems, function (a, b) {
                    var pieChartItem = { value: a.Count, color: a.Color, highlight: "#FF5A5E", label: a.ItemName };
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
    })();
    TeamBins.DashboardController = DashboardController;
})(TeamBins || (TeamBins = {}));
