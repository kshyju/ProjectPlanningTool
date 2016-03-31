



var issueDetailsCtrl = function ($http, issue, issueDetailService) {



    var vm = this;
    vm.issue = issue;
    vm.comments = [];
    vm.newComment = '';

    issueDetailService.getComments(vm.issue.Id)
        .then(function (response) {
            vm.comments = response;
        });

    vm.removeComment = function (comment, issueId, $event) {

        var yes = window.confirm("Are you sure to delete this comment ? This cannot be undone.");
        if (yes) {
            issueDetailService.deleteComment(issueId, comment.Id)
            .then(function (data) {
                if (data.Status === "Success") {
                    vm.comments.splice(vm.comments.indexOf(comment), 1);
                }
            });
        }
    };

    vm.saveComment = function () {
        issueDetailService.saveComment(vm.newComment, vm.issue.Id)
            .then(function (response) {
                if (response.Status === "Success") {
                    vm.comments.push(response.Data);
                }
            });
    };



    //vm.deleteIssue = function () {
    //    issueDetailService.deleteIssue(vm.issue.Id)
    //        .then(function (response) {
    //             if (response.Status === "Success") {
    //                 vm.comments.push(response.Data);
    //             }
    //        });

    //};


}


issueDetailsCtrl.inject = ['$http', 'issueDetailService'];
issueListApp.controller("issueDetailsCtrl", issueDetailsCtrl);