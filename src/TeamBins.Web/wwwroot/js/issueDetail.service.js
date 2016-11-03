var issueDetailService = function ($http) {

    return {
        getComments: getComments,
        getIssueMembers: getIssueMembers,
        saveComment: saveComment,
        deleteComment :deleteComment,
        starIssue: starIssue
    }




    function deleteComment(issueId, commentId) {
        return $http.post('../api/comments/' + commentId + '/delete')
        .then(deleteCommentCompleted)
        .catch(deleteCommentFailed);

        function deleteCommentCompleted(response) {
            // console.log("r");
            //console.log(response.data);
            return response.data;
        }

        function deleteCommentFailed(error) {
            console.log("error");
            return error;
        }
    }
        function starIssue(issueId) {
            return $http.post('../api/issues/' + issueId + '/star', {
                IssueID: issueId
            })
    .then(issueStarredCompleted)
    .catch(issueStarringFailed);

            function issueStarredCompleted(response) {
                // console.log("r");
                //console.log(response.data);
                return response.data;
            }

            function issueStarringFailed(error) {
                console.log("error");
                return error;
            }
        }

        function saveComment(commentText, issueId, signalRConnection) {
            return $http.post('../api/Commentsapi', {
                IssueID: issueId,
                CommentText: commentText,
                Connection: signalRConnection
            })
        .then(saveCommentsCompleted)
        .catch(saveCommentsFailed);

            function saveCommentsCompleted(response) {
                // console.log("r");
                //console.log(response.data);
                return response.data;
            }

            function saveCommentsFailed(error) {
                console.log("error");
                return error;
            }
        }
        function getComments(issueId) {
            return $http.get('../api/issue/' + issueId + "/comments")
          .then(getCommentsCompleted)
          .catch(getCommentsFailed);

            function getCommentsCompleted(response) {
                // console.log("r");
                //console.log(response.data);
                return response.data;
            }

            function getCommentsFailed(error) {
                console.log("error");
                return error;
            }
        }

        function getIssueMembers(issueId) {
            return $http.get('../../issues/members/' + issueId)
          .then(getIssueMembersCompleted)
          .catch(getIssueMembersFailed);

            function getIssueMembersCompleted(response) {
                // console.log("r");
                //console.log(response.data);
                return response.data;
            }

            function getIssueMembersFailed(error) {
                console.log("error");
                return error;
            }
        }

    }

    issueDetailService.$inject = ['$http'];

    angular.module("teamBins").factory("issueDetailService", issueDetailService);