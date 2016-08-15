var issueService = function ($http, appSettings) {

   
    
    return {
        getActivityStream: getActivityStream,
        getIssues: getIssues,
        create : create
    }

    function create(newIssue) {
        
        return $http.post(appSettings.urls.baseUrl + '/issues/add', newIssue)
      .then(createIssueCompleted)
      .catch(createIssueFailed);

        function createIssueCompleted(response) {
            return response.data;
        }

        function createIssueFailed(error) {
            console.log("error");
            return error;
        }
    }

    function getIssues(team,count) {
        return $http.get(appSettings.urls.baseUrl + '/api/issues/' + team.id +"/"+count)
      .then(getIssuesCompleted)
      .catch(getIssuesFailed);

        function getIssuesCompleted(response) {
           return response.data;
        }

        function getIssuesFailed(error) {
            console.log("error");
            return error;
        }
    }


    function getActivityStream(currentTeam,count) {

        return $http.get(appSettings.urls.baseUrl +'/api/team/activitystream/'+currentTeam.id+'?count=' +count)
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

issueService.$inject = ['$http', 'appSettings'];

angular.module("teamBins").service("issueService", issueService);