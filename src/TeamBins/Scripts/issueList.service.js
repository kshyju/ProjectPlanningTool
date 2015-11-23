var issueService = function ($http) {

    return {
        getActivityStream: getActivityStream,
        getIssues: getIssues
    }


    function getIssues(count) {
        return $http.get('issues?size=' + count)
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

issueService.$inject = ['$http'];

angular.module("issueListApp").factory("issueService", issueService);