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
        if (e.keyCode == 13)
        {
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
        var _this = $("#" +  $event.target.id);
        $http.get('../issues?size=25&iteration=' + iteration).success(function (data) {
            $scope.issueList = data;
            $("a.aIteration").removeClass("tab-selected");
            _this.addClass("tab-selected");
            $scope.loading = false;
        });       
    };
  
    var chat = $.connection.issuesHub;
    chat.client.addNewTeamActivity = function (comment) {
        $scope.activities.push(comment);
        $scope.$apply();
    };

    chat.client.addIssueToIssueList = function (issue) {       
        $scope.issueList.push(issue);
        $scope.$apply();       
    };
    $.connection.hub.start().done(function () {
        chat.server.subscribeToTeam($("#TeamID").val())
    })   
});
    

$(function () {
    
    $("#txtAssignMember").autocomplete({       
        source: function (request, response) {                  
            $.ajax({
                url: "../../Issues/NonIssueMembers",
                data: { term: request.term ,issueId: $("#ID").val() },
                success: function (data) {
                    response($.map(data, function (item) {                   
                        return { label: item.Name, value: item.MemberID, Image: item.AvatarHash };
                    }))
                }
            })
        },
        create: function () {
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li>')
                    .append("<a><div class='autocomplete-item'><img src='" + item.Image + "?size=28' />" + item.label + "</div></a>")
                    .appendTo(ul);
            };
        },
        focus: function (event, ui) {   
            $("#txtAssignMember").val(ui.item.label);
        },
        select: function (event, ui) {                       
            $("#txtAssignMember").val(ui.item.label);
            $.post(addMemberToIssueUrl, { memberId: ui.item.value, issueId: $("#ID").val() }, function (res) {
                if(res.Status==="success")
                {
                    $("#txtAssignMember").val("");
                    var domElement = $("#memberList");
                    var scope = angular.element(domElement).scope();
                    $.get('../../issues/members/' + $("#ID").val()).success(function (data) {
                        scope.$apply(function () {
                            scope.members = data;
                        });
                    });
                }
            });
            return false;
        }
    });

    $('#IssueDueDate').datepicker({
        onSelect: function (date) {
            var selectedDate = date;//$("#IssueDueDate").val();
            $("span#dueDate").text(selectedDate);
            $("#dueDatePicker").fadeOut(50);
            $.post("../../Issues/SaveDueDate", { issueDueDate: selectedDate, issueId: $("#ID").val() });
        }
    });

    $("#aChangeDueDate").click(function (e) {
        e.preventDefault();
        $("#dueDatePicker").fadeIn(50);
    });
    $(".changableWidget").hover(function () {
        $(this).find("a.hiddenChangeLink").show();
    },
    function () {
        $(this).find("a.hiddenChangeLink").hide();
    });
    $("#saveComment").click(function (e) {
        e.preventDefault();
        var _this = $(this);
       
        if ($("#newComment").val() !== "") {
            _this.attr("value", "Saving...").attr("disabled", true);
            $.ajax({
                url: "../../Issues/Comment",
                type: "post",
                data: {
                    CommentBody: $("#newComment").val(),
                    IssueID: $("#ID").val()
                },
                success: function (res, textStatus, jqXHR) {                   
                    $("#newComment").val("");
                    _this.attr("disabled", false).attr("value","Post");
                }
            });
        }
        else {
            $("#newComment").focus();
        }
    });
    

    $("#issue-star").click(function (e) {
        var _this = $(this);
        $.post("../Star/" + $("#ID").val()+"?mode="+_this.attr("data-starred"), function (res) {
            if (res.Status === "Success") {
                _this.removeClass().addClass("glyphicon").addClass(res.StarClass).attr("data-starred",res.Mode);
            }
        });
    });

    $(document).on("click", "#btnDeleteIssue", function (e) {
        e.preventDefault();
        $.post(issueDeleteUrl+"/"+ $("#ID").val(), function (res) {
            if (res.Status === "Success") {
                window.location.href = issuesUrl;
            }
        });
    });

});