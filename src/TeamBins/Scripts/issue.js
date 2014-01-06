$(function () {
    $("input#NewItemTitle").keyup(function (e) {
        
        if(e.keyCode == 13)        {
            $(this).trigger("enterKey");
        }
    });
    $("input#NewItemTitle").bind("enterKey", function (e) {
        var _this = $(this);
       
        e.preventDefault();
        $.post(addIssueUrl, { Title: _this.val() }, function (data) {
            if (data.Status == "Success") {
                _this.val("");
                var secondRow = $("#issueTbl tr").eq(1);
                var newRowClass="trOdd";
                if (secondRow.hasClass("trOdd"))
                {
                    newRowClass="trEven";
                }               
                var newRow = "<tr class='" + newRowClass + "'><td>" + data.Item.ID + "</td><td><a class='popup' href='" +editIssueUrl+ "/"+data.Item.ID + "'>" + data.Item.Title + "</a></td><td>" + data.Item.OpenedBy + "</td><td>" + data.Item.Priority + "</td><td>" + data.Item.Status + "</td><td>" + data.Item.CreatedDate + "</td></tr>";

                secondRow.before(newRow);
                if ($("#myonoffswitch").is(":checked")) {
                    ShowModel(editIssueUrl + "/" + data.Item.ID, data.Item.Title);
                }
            }
        });
    });

    //Show popup on clicking the link
    $(document).on("click", "a.popup", function (e) {
        e.preventDefault();
        var _this = $(this);
        ShowModel(_this.attr("href"), _this.text());
    });
    //Save the user preference to session
    $("#myonoffswitch").click(function () {
        var switchVal = $(this).is(":checked");
        $.post(savePrefUrl, { CreateAndEditMode: switchVal });
    });

 
    $(document).on("click", "a.aRemove", function (e) {
        e.preventDefault();
        var _this = $(this);
        $.post(_this.attr("href"), function (res) {
            if (res.Status == "Success") {
                $("#members").load("../../Issues/IssueMembers/" + $("#ID").val(), function () {
                    InitializeMemberCombo();
                });
                
            }
        });
    });

    /*
    $("#IssueDueDate").width(300).kendoDatePicker({
        change: function (e) {
            selectedDate = $("#IssueDueDate").val();
            $("span#dueDate").text(selectedDate);
            $("#dueDatePicker").fadeOut(50);
            $.post("../../Issues/SaveDueDate", { issueDueDate: selectedDate, issueId: $("#ID").val() });
        }
    });*/
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
        if ($("#newComment").val() != "") {
            $.ajax({
                url: "../../Issues/Comment",
                type: "post",
                data: {
                    CommentBody: $("#newComment").val(),
                    IssueID: $("#ID").val()
                },
                success: function (res, textStatus, jqXHR) {
                    if (res.Status === "Success") {
                        $.get("../../Issues/Comment/" + res.NewCommentID, function (dat) {
                            $(dat).hide().appendTo("#commentList").fadeIn(350);                            
                            $("#newComment").val("");
                            var editor = $("#newComment").data("kendoEditor");

                            // set value
                            editor.value("");
                        });
                    }
                }
            });
        }
        else {
            $("#newComment").focus();
        }

       /* $.post("../../Issues/Comment", { body: $("#newComment").val(), issueId: $("#ID").val() }, function (res) {
            if (res.Status == "Success") {
                window.location.href = window.location.href;
            }
        });*/
    });
    /*
    if ($("#activityStream").length) {
       // $("#activityStream").load("../../Team/ActivityStream");
    }*/


});


