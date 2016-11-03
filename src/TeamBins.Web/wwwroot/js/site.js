// Write your Javascript code.
$(function() {

    $("a.team-item")
        .click(function(e) {
            e.preventDefault();
            var _this = $(this);
            $.post(teamBins.urls.baseUrl + '/account/SwitchTeam/' + _this.data("id"),
                function(res) {
                    window.location.href = _this.attr("href");
                });
        });


    // Issue list


});

window.IssueList = (function() {

    console.log('IssueList');

    $(function () {

        //$("#create-issue")
        //    .click(function(e) {
        //        e.preventDefault();
        //        console.log('IssueList');
        //        console.log('IssueList1');
        //    });

    });


})();