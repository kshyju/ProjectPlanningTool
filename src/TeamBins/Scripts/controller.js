var IssueDetails = IssueDetails || {};
var IssueDetails.gIssueDetailConnectionID = "";

var issueDetailApp = angular.module('issueDetialApp', ['ngSanitize']);
issueDetailApp.controller("IssueDetailsCtrl", function ($scope, $http) {
    $scope.hover = false;
    $scope.members = [];
    $http.get(issueCommentsUrl+'/' + $("#ID").val()).success(function (data) {    
        $scope.comments = data;       
    });
    $http.get('../../issues/members/' + $("#ID").val()).success(function (data) {
        $scope.members = data;
        $scope.isEditableForCurrentUser = true;        
    });
    $scope.hover = function (member) {       
        return member.ShowDelete = !member.ShowDelete;
    };
    $scope.removeIssueMember = function (issueId, member, $event) {        
        $http.post("../../issues/removemember", { memberid: member.MemberID, id: issueId }).success(function(data) {
            if (data.Status === "Success") {               
                $scope.members.splice($scope.members.indexOf(member), 1);
            }
        });
    };
    $scope.removeComment = function (comment, issueId, $event) {
        var yes = window.confirm("Are you sure to delete this comment ? This cannot be undone.");
        if (yes) {
            $http.post("../../issues/removecomment", { id: comment.ID }).success(function (data) {
                if (data.Status === "Success") {
                    $scope.comments.splice($scope.comments.indexOf(comment), 1);
                }
            });
        }
    };
    var chat = $.connection.issuesHub;
    console.log("connection");
    console.log(chat);
    chat.client.addNewComment = function (comment) {        
        $scope.comments.push(comment);
        $scope.commentCount++;
        $scope.$apply();
    };
    $.connection.hub.start().done(function () {           
        chat.server.subscribeToTeam($("#TeamID").val())
        gIssueDetailConnectionID=$.connection.hub.id;
        console.log("connection id " + gIssueDetailConnectionID);
    })  

});

