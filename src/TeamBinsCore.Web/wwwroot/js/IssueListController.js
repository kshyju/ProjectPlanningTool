
issueListApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);


var issueListCtrl = function ($http, issueService, appSettings, currentTeam) {

    var vm = this;
    vm.newIssue = "";
    vm.loading = true;
    vm.activities = [];
    vm.issuesList = [];
    vm.issuesGrouped = [];
     

    issueService.getActivityStream(currentTeam,25)
    .then(function (response) {
        vm.activities = response;
    });


    vm.getIssues = function () {
        issueService.getIssues(currentTeam,25)
        .then(function (response) {

            vm.issuesGrouped = response;
            vm.currentlyShowingGroup = response[0];

            vm.loading = false;
        });
    }


    vm.getIssues();

    vm.create = function (e) {
        if (e.keyCode === 13) {
            if (vm.newIssue !== "") {


                issueService.create({ Title: vm.newIssue, IncludeIssueInResponse: true })
                    .then(function (res) {
                        if (res.status === "Success") {
                            vm.newIssue = "";
                       
                            if (vm.issuesGrouped.length > 0) {
                                vm.issuesGrouped[0].issues.push(res.data.result);
                            } else {
                                //very first issue
                                vm.getIssues();
                            }
                        } else {
                            alert(res.message);
                        }
                    });
            }
        }
    };

    vm.updateview = function (group, $event) {
        vm.currentlyShowingGroup = group;
    };

}


issueListCtrl.inject = ['$http', 'issueService','appSettings', 'currentTeam'];
issueListApp.controller("IssueListCtrl", issueListCtrl);

//    var chat = $.connection.issuesHub;
//    chat.client.addNewTeamActivity = function (comment) {
//        console.log(comment);
//        $scope.activities.push(comment);
//        $scope.$apply();
//    };

//    chat.client.addIssueToIssueList = function (issue) {
//        console.log(issue);

//        angular.forEach($scope.issuesGrouped, function(value, key) {
//            if (value.GroupCode === issue.StatusGroupCode) {
//                value.Issues.push(issue);
//                $scope.$apply();
//            }
//        });

//       // $scope.issueList
//    };
//    $.connection.hub.start().done(function () {
//        console.log($("#TeamId").val());
//        chat.server.subscribeToTeam($("#TeamId").val());
//    });
//});
