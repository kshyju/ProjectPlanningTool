var dashboardApp = angular.module('teamBins', []);
dashboardApp.controller("dashboardCtrl", function ($scope, $http, issueService) {
   
    issueService.getActivityStream(8)
    .then(function (response) {
        $scope.activities = response;
        $scope.loading = false;
    });


    $http.get('/dashboard/GetDashBoardItemSummary').success(function (data) {
        RenderPieChart(data.NewItems, data.ItemsInProgress, data.CompletedItems, true);
        $scope.summary = data;
    });
    var chat = $.connection.issuesHub;
    chat.client.addNewTeamActivity = function (comment) {
        $scope.activities.push(comment);
        $scope.$apply();
    };
    chat.client.updateDashboardSummary = function (item) {
        RenderPieChart(item.NewItems, item.ItemsInProgress, item.CompletedItems, false);
        $scope.summary = item;
        $scope.$apply();
    };
    $.connection.hub.start().done(function () {
        chat.server.subscribeToTeam($("#TeamID").val());
    })
});