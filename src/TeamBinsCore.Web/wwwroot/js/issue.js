(function () {

    function bindAssignMembersAutoComplete() {
        var nonIssueMemberUrl = teamBins.urls.baseUrl + "/api/Issues/" + $("#Id").val() + "/noissuemembers";

        $("#txtAssignMember").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: nonIssueMemberUrl,
                    data: { term: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.name, value: item.id, Image: item.gravatarUrl };
                        }));
                    }
                });
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
                $.post(teamBins.urls.baseUrl + "/api/issues/" + $("#Id").val() + "/assignteammember/" + ui.item.value, function (res) {
                    console.log(res);
                    if (res.status === "Success") {
                        $("#txtAssignMember").val("");

                        $("#members").load(teamBins.urls.baseUrl + '/issues/' + $("#Id").val()+"/members");

                    }
                });
                return false;
            }
        });
    }

    $(function () {

        var membersUrl = teamBins.urls.baseUrl + "/issues/" + $("#Id").val() + "/members";
        $.get(membersUrl).success(function (data) {
            $("#members").html(data).promise().done(function () {
                bindAssignMembersAutoComplete();
            });
        });



        $('#IssueDueDate').datepicker({
            onSelect: function (date) {
                var selectedDate = date;//$("#IssueDueDate").val();
                $("span#dueDate").text(selectedDate);
                $("#dueDatePicker").fadeOut(50);
                $.post(teamBins.urls.baseUrl + "/api/issues/" + $("#Id").val() + "/SaveDueDate", { issueDueDate: selectedDate }, function (res) {
                  
                });
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


        $("#issue-star").click(function (e) {
            var _this = $(this);
            console.log('Id:'+$("#Id").val());
            $.post("/api/Issues/" + $("#Id").val() + "/star/" + _this.attr("data-starred"), function (res) {
                if (res.status === "Success") {
                    _this.removeClass("glyphicon-star-empty glyphicon-star").addClass(res.class).attr("data-starred", res.starred);
                }
            });
        });

        $(document).on("click", ".aRemove", function (e) {
            e.preventDefault();
            var _this = $(this);
            $.post(teamBins.urls.baseUrl + "/api/Issues/" + $("#Id").val() + "/removeissuemember/" + _this.data("member"), function (res) {
                $("#members").load(teamBins.urls.baseUrl + '/issues/' + $("#Id").val() + "/members");
            });
        });

        $(document).on("click", "#btnSaveIssue", function (e) {
            e.preventDefault();

            //var _this = $(this);

            var _this = $(this);
            var model = {
                Title: $("#Title").val(),
                Id: $("#Id").val(),
                Description: $("#Description").val(),
                SelectedStatus: $("#SelectedStatus").val(),
                SelectedPriority: $("#SelectedPriority").val(),
                SelectedProject: $("#SelectedProject").val(),
                SelectedCategory: $("#SelectedCategory").val()
            };
            console.log(model);
            $.ajax({
                type: "post",
                contentType: 'application/json',
                url: _this.closest("form").attr("action"),
                data: JSON.stringify(model)
            })
                .done(function (response) {
                    console.log(response);
                    if (response.status === "Success") {
                        $('#modal').modal('hide');
                        window.location.href = window.location.href;
                    } else {
                        alert("Error updating issue!");
                    }
                });

        });


        $(document).on("click", "#btnDeleteIssue", function (e) {
            e.preventDefault();
            var _this = $(this);
            $.post($(this).data("url"), { id: $("#Id").val() }, function (res) {
                if (res.status === "Success") {
                    window.location.href = _this.data("issuelisturl");
                }
            });
        });

    });

})();