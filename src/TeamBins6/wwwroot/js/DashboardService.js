var TeamBins;
(function (TeamBins) {
    var DashboardService = (function () {
        function DashboardService($http) {
            this.$http = $http;
        }
        DashboardService.prototype.getActivityStream = function () {
            return this.$http.post("projects/add", {})
                .then(function (response) {
                return response.data;
            });
        };
        return DashboardService;
    })();
    TeamBins.DashboardService = DashboardService;
})(TeamBins || (TeamBins = {}));
