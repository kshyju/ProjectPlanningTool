module TeamBins {
    export class DashboardService {
        
        http: any;
        constructor(private $http: any,private appSettings:any) {
            
        }

        getActivityStream() {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/ActivityStream")
                .then((response:any): any=> {
                    return response.data;
                });

        }

        getSummary() {
            return this.$http.get(this.appSettings.urls.baseUrl +"/api/team/summary")
                .then((response: any): any=> {
                    return response.data;
                });

        }

    }
}