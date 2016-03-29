module TeamBins {
    
    export class DashboardController {

        summary : any[];
        activities: any[];

        
        constructor(private summaryService: any) {


            this.getActivityStream();
            this.getSummary();
            
        }
        getActivityStream() {
            var self = this;
            this.summaryService.getActivityStream().then(function(data) {
                self.activities = data;
            });
        }

        getSummary() {
            var self = this;
            this.summaryService.getSummary().then(function (data) {
                self.summary = data.IssueCountsByStatus;
            });
        }
    }


}