
/// <reference path="../../../teambins.common/scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../../TeamBins.Common/Scripts/typings/chartjs/chart.d.ts" />
module TeamBins {
    

     export class DashboardController {
       
        summaryItems: any[];
        activities: any[];


        constructor($scope: ng.IScope,private summaryService: any) {


            this.getActivityStream();
            this.getSummary();

        }
        getActivityStream() {
            var self = this;
            this.summaryService.getActivityStream().then(function (data) {
                self.activities = data;
            });
        }

        getSummary() {
            var self = this;
            this.summaryService.getSummary().then(function (data) {
                self.summaryItems = data.IssueCountsByStatus;
                var pieChartData = [];
                angular.forEach(self.summaryItems, function(a, b) {                  
                    var pieChartItem = { value: a.Count, color: a.ChartColor, highlight: "#FF5A5E", label: a.ItemName };
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