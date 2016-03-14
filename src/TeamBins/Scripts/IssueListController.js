var issueListApp = angular.module('teamBins', []);
issueListApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);

issueListApp.controller('IssueListCtrl', function ($scope, $http, issueService) {

    var vm = this;

    $scope.loading = true;
    $scope.activities = [];
    $scope.issuesList = [];
    $scope.issuesGrouped = [];
   
    $scope.currentlyShowingGroup = {};

    issueService.getIssues(25)
        .then(function (response) {
            console.log(response);
            $scope.issuesGrouped = response;
            $scope.currentlyShowingGroup = response[0];
            $scope.loading = false;
        });

    issueService.getActivityStream(25)
    .then(function (response) {
        $scope.activities = response;
        $scope.loading = false;
    });


    $scope.create = function (e) {
        if (e.keyCode === 13) {
            if ($("#NewItemTitle").val() !== "") {
                $.post(addIssueUrl, { Title: $("#NewItemTitle").val(), includeIssueInResponse:true }, function (data) {
                    if (data.Status === "Error") {
                        alert(data.Message);
                    }
                    else {
                        $("#NewItemTitle").val("");

                    }
                });
            }
        }
    };

    $scope.updateview = function (group, $event) {
       
        $scope.currentlyShowingGroup = group;
        
    };

    var chat = $.connection.issuesHub;
    chat.client.addNewTeamActivity = function (comment) {
        console.log(comment);
        $scope.activities.push(comment);
        $scope.$apply();
    };

    chat.client.addIssueToIssueList = function (issue) {
        console.log(issue);

        angular.forEach($scope.issuesGrouped, function(value, key) {
            if (value.GroupCode === issue.StatusGroupCode) {
                value.Issues.push(issue);
                $scope.$apply();
            }
        });

       // $scope.issueList
    };
    $.connection.hub.start().done(function () {
        console.log($("#TeamID").val());
        chat.server.subscribeToTeam($("#TeamID").val());
    });
});
