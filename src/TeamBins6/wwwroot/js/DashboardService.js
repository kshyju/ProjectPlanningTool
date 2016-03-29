var TeamBins;
(function (TeamBins) {
    var DashboardService = (function () {
        function DashboardService($http) {
            this.$http = $http;
        }
        DashboardService.prototype.getActivityStream = function () {
            return this.$http.get("api/team/ActivityStream")
                .then(function (response) {
                return response.data;
            });
        };
        DashboardService.prototype.getSummary = function () {
            return this.$http.get("api/team/summary")
                .then(function (response) {
                return response.data;
            });
        };
        return DashboardService;
    })();
    TeamBins.DashboardService = DashboardService;
})(TeamBins || (TeamBins = {}));
