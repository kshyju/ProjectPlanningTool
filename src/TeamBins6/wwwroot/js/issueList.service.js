var issueService = function ($http, appSettings) {

    console.log('service');
    console.log(appSettings);
    
    return {
        getActivityStream: getActivityStream,
        getIssues: getIssues,
        create : create
    }

    function create(newIssue) {
        
        return $http.post(appSettings.appSettings.baseUrl + 'issue/add', newIssue)
      .then(createIssueCompleted)
      .catch(createIssueFailed);

        function createIssueCompleted(response) {
            // console.log("r");
            //console.log(response.data);
            var s = response.headers(0);
            console.log(s);
            return response.data;
        }

        function createIssueFailed(error) {
            console.log("error");
            return error;
        }
    }

    function getIssues(count) {
        return $http.get(appSettings.appSettings.baseUrl + 'api/issue/' + count)
      .then(getIssuesCompleted)
      .catch(getIssuesFailed);

        function getIssuesCompleted(response) {
           // console.log("r");
            //console.log(response.data);
           return response.data;
        }

        function getIssuesFailed(error) {
            console.log("error");
            return error;
        }
    }


    function getActivityStream(count) {

        return $http.get('api/team/activitystream?count=' + count)
           .then(getActivityStreamCompleted)
           .catch(getActivityStreamFailed);

        function getActivityStreamCompleted(response) {
            return response.data;
        }

        function getActivityStreamFailed(error) {
            console.log(error);
            return error;
        }

    }
}
//console.log('appSettings');
//console.log(appSettings);
issueService.$inject = ['$http', 'appSettings'];

angular.module("teamBins").service("issueService", issueService);