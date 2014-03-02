
var issueDetailApp = angular.module('issueDetialApp', []);
issueDetailApp.controller("IssueDetailsCtrl", function ($scope, $http) {
    $scope.hover = false;
    $scope.members = [];
    $http.get(issueCommentsUrl+'/' + $("#ID").val()).success(function (data) {       
        $scope.commentCount = data.length;
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
        console.log("beofre");
        $http.post("../../issues/removemember", { memberid: member.MemberID, id: issueId }).success(function(data) {
            if (data.Status === "Success") {               
                $scope.members.splice($scope.members.indexOf(member), 1);
            }
        });
    };

    var chat = $.connection.issuesHub;       
    chat.client.addNewComment = function (comment) {        
        $scope.comments.push(comment);
        $scope.commentCount++;
        $scope.$apply();
    };
    $.connection.hub.start().done(function () {           
        chat.server.subscribeToTeam($("#TeamID").val())
    })  

});

