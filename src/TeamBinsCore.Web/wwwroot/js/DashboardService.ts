module TeamBins {
    export class DashboardService {
        
        http: any;
        constructor(private $http: any,private appSettings:any) {
            
        }

        getActivityStream(teamId,count) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/ActivityStream/" + teamId+"?count="+count)
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
        getIssuesGroupedByPriority() {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/IssuesGroupedByPrioroty")
                .then((response: any): any => {
                    return response.data;
                });

        }

    }
}