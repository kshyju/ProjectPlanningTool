module TeamBins {
    export class DashboardService {
        
        http: any;
        constructor(private $http: any) {
            
        }

        getActivityStream() {
            return this.$http.get("api/team/ActivityStream")
                .then((response:any): any=> {
                    return response.data;
                });

        }

        getSummary() {
            return this.$http.get("api/team/summary")
                .then((response: any): any=> {
                    return response.data;
                });

        }

    }
}