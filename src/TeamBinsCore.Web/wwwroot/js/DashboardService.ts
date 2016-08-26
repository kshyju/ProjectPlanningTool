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

        getSummary(teamId) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/summary/" + teamId)
                .then((response: any): any=> {
                    return response.data;
                });

        }
        getIssuesGroupedByPriority(teamId) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/IssuesGroupedByPrioroty/" + teamId)
                .then((response: any): any => {
                    return response.data;
                });

        }

    }
}