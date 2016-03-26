module TeamBins {
    
    export class DashboardController {

        activityStream : any[];
        activities:any[];
        
        constructor(private summaryService: any) {


            this.getActivityStream();

            
        }
        getActivityStream() {
            var self = this;
            this.summaryService.getActivityStream().then(function(data) {
                self.activities = data;
            });
        }


    }


}