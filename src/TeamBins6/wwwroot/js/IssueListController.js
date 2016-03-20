
issueListApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);


var issueListCtrl = function ($http, issueService, appSettings) {

    console.log(appSettings);


    var vm = this;
    vm.newIssue = "";
    vm.loading = true;
    vm.activities = [];
    vm.issuesList = [];
    vm.issuesGrouped = [];
     

    issueService.getActivityStream(25)
    .then(function (response) {
        console.log(response);
        vm.activities = response;

    });


    vm.getIssues = function () {
        issueService.getIssues(25)
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
                        if (res.Status === "Success") {
                            vm.newIssue = "";
                            if (vm.issuesGrouped.length > 0) {
                                vm.issuesGrouped[0].Issues.push(res.Data);
                            } else {
                                //very first issue
                                vm.getIssues();
                            }
                        } else {
                            alert(res.Message);
                        }
                        console.log(res);

                    });

            }
        }
    };


    vm.updateview = function (group, $event) {

        vm.currentlyShowingGroup = group;

    };


}


issueListCtrl.inject = ['$http', 'issueService'];
issueListApp.controller("IssueListCtrl", issueListCtrl);
//issueListApp.controller('IssueListCtrl', function ($scope, $http, issueService) {

//    var vm = this;

//    $scope.loading = true;
//    $scope.activities = [];
//    $scope.issuesList = [];
//    $scope.issuesGrouped = [];

//    $scope.currentlyShowingGroup = {};

//    issueService.getIssues(25)
//        .then(function (response) {
//            console.log(response);
//            $scope.issuesGrouped = response;
//            $scope.currentlyShowingGroup = response[0];
//            $scope.loading = false;
//        });

//    issueService.getActivityStream(25)
//    .then(function (response) {
//        $scope.activities = response;
//        $scope.loading = false;
//    });


//    $scope.create = function (e) {
//        if (e.keyCode === 13) {
//            if ($("#NewItemTitle").val() !== "") {
//                $.post(addIssueUrl, { Title: $("#NewItemTitle").val() }, function (data) {
//                    if (data.Status === "Error") {
//                        alert(data.Message);
//                    }
//                    else {
//                        $("#NewItemTitle").val("");
//                    }
//                });
//            }
//        }
//    };

//    $scope.updateview = function (group, $event) {

//        $scope.currentlyShowingGroup = group;

//    };

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
//        console.log($("#TeamID").val());
//        chat.server.subscribeToTeam($("#TeamID").val());
//    });
//});
