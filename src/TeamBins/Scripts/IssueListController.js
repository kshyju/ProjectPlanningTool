var issueListApp = angular.module('issueListApp', []);
issueListApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);

issueListApp.controller('IssueListCtrl', function ($scope, $http) {
    $scope.loading = true;
    $scope.activities = [];
    $scope.issuesList = [];
    $http.get('../issues?size=25').success(function (data) {
        $scope.issueList = data;
        $scope.loading = false;
        $("#tab-current").addClass("tab-selected");
    });
    $http.get('../../team/stream/' + $("#TeamID").val() + "?size=6").success(function (data) {
        $scope.activities = data;
    });
    $scope.create = function (e) {
        if (e.keyCode == 13) {
            if ($("#NewItemTitle").val() != "") {
                $.post(addIssueUrl, { Title: $("#NewItemTitle").val() }, function (data) {
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

    $scope.updateview = function (iteration, $event) {
        $scope.loading = true;
        var _this = $("#" + $event.target.id);
        $http.get('../issues?size=25&iteration=' + iteration).success(function (data) {
            $scope.issueList = data;
            $("a.aIteration").removeClass("tab-selected");
            _this.addClass("tab-selected");
            $scope.loading = false;
        });
    };

    var chat = $.connection.issuesHub;
    chat.client.addNewTeamActivity = function (comment) {
        console.log(comment);
        $scope.activities.push(comment);
        $scope.$apply();
    };

    chat.client.addIssueToIssueList = function (issue) {
        console.log(issue);
        $scope.issueList.push(issue);
        $scope.$apply();
    };
    $.connection.hub.start().done(function () {
        console.log($("#TeamID").val());
        chat.server.subscribeToTeam($("#TeamID").val());
    });
});
