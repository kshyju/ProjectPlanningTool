/// <reference path="../typings/angular.d.ts" />
/// <reference path="../typings/chart.d.ts" />


module TeamBins {
    

     export class DashboardController {

         issuesGroupedByProject: any[];
         issuesGroupedByPriority : any[];
        summaryItems: any[];
        activities: any[];


        constructor($scope: ng.IScope, private summaryService: any, private pageContext:any) {
            
            this.getActivityStream(this.pageContext.TeamId);
            this.getSummary(this.pageContext.TeamId);
            this.getIssuesGroupedByPriority(this.pageContext.TeamId);
            this.getIssuesGroupedByProject(this.pageContext.TeamId);

        }
        getActivityStream(teamId:any) {
            var self = this;
            this.summaryService.getActivityStream(teamId,10).then(function (data) {
                self.activities = data;
            });
        }

        getIssuesGroupedByProject(teamId: any) {
            var self = this;
            this.summaryService.getIssuesGroupedByProject(teamId, 10).then(function (data) {
                self.issuesGroupedByProject = data;
            });
        }

        getIssuesGroupedByPriority(teamId: any) {
            var self = this;
            this.summaryService.getIssuesGroupedByPriority(teamId).then(function (data) {
                self.issuesGroupedByPriority = data;
                var pieChartData2 = [];
                angular.forEach(self.issuesGroupedByPriority, function (a, b) {
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData2.push(pieChartItem);
                });

                self.renderPie(pieChartData2,'issuesPriorityPieChart');

            });
        }
        getSummary(teamId: any) {
            var self = this;
            this.summaryService.getSummary(teamId).then(function (data) {
                self.summaryItems = data.issueCountsByStatus;
                var pieChartData = [];
                angular.forEach(self.summaryItems, function(a, b) {                  
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData.push(pieChartItem);
                });

                self.renderPie(pieChartData, 'myChart');

            });
        }          
        renderPie(pieChartDataSet: any[], elementId: String) {
            var canvas: any = document.getElementById('' + elementId);
            var ctx = canvas.getContext('2d');
           new Chart(ctx).Pie(pieChartDataSet, null);
        }
    }
}
