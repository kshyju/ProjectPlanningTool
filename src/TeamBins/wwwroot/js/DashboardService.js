var TeamBins;
(function (TeamBins) {
    var DashboardService = (function () {
        function DashboardService($http, appSettings) {
            this.$http = $http;
            this.appSettings = appSettings;
        }
        DashboardService.prototype.getActivityStream = function (teamId, count) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/ActivityStream/" + teamId + "?count=" + count)
                .then(function (response) {
                return response.data;
            });
        };
        DashboardService.prototype.getSummary = function () {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/summary")
                .then(function (response) {
                return response.data;
            });
        };
        return DashboardService;
    }());
    TeamBins.DashboardService = DashboardService;
})(TeamBins || (TeamBins = {}));
