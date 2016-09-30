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

        //var popOverSettings = {
        //    placement: 'bottom',
        //    container: 'body',
        //    html: true,
        //    selector: '[data-toggle="popover"]', //Sepcify the selector here
        //    content: function () {
        //        return $('#popover-content').html();
        //    }
        //}

        //$('body').popover(popOverSettings);


        //$('[data-toggle="popover"]').popover();
    });


})();