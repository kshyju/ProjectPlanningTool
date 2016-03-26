module TeamBins {
    export class DashboardService {
        
        http: any;
        constructor(private $http: any) {
            
        }

        getActivityStream() {
            return this.$http.post("projects/add", {})
                .then((response:any): any=> {
                    return response.data;
                });

        }

    }
}