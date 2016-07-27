
/// <reference path="../../../teambins.common/scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../../TeamBins.Common/Scripts/typings/chartjs/chart.d.ts" />
module TeamBins {
    

     export class DashboardController {
       
        summaryItems: any[];
        activities: any[];


        constructor($scope: ng.IScope, private summaryService: any, private pageContext any) {

            
            this.getActivityStream(this.pageContext.TeamId);
            this.getSummary();

        }
        getActivityStream(teamId) {
            var self = this;
            this.summaryService.getActivityStream(teamId,10).then(function (data) {
                self.activities = data;
            });
        }

        getSummary() {
            var self = this;
            this.summaryService.getSummary().then(function (data) {
                self.summaryItems = data.issueCountsByStatus;
                var pieChartData = [];
                angular.forEach(self.summaryItems, function(a, b) {                  
                    var pieChartItem = { value: a.count, color: a.color, highlight: "#FF5A5E", label: a.itemName };
                    pieChartData.push(pieChartItem);
                });

                self.renderPie(pieChartData);

            });
        }
        renderPie(pieChartDataSet:any[]) {

            var ctx = document.getElementById("myChart").getContext("2d");
            new Chart(ctx).Pie(pieChartDataSet);
        }
    }
    
    

}
interface HTMLElement extends Element {
    getContext(name:string): CanvasRenderingContext2D;
}