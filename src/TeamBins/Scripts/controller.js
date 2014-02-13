
var issueDetailApp = angular.module('issueDetialApp', []);
issueDetailApp.controller("IssueDetailsCtrl", function ($scope, $http) {   
    $http.get(issueCommentsUrl+'/' + $("#ID").val()).success(function (data) {
        $scope.comments = data;
    });

    var chat = $.connection.issuesHub;       
    chat.client.addNewComment = function (comment) {
        $scope.comments.push(comment);
        $scope.$apply();
    };

    $.connection.hub.start().done(function () {           
        chat.server.subscribeToTeam($("#TeamID").val())
    })  

});

