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
        DashboardService.prototype.getSummary = function (teamId) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/summary/" + teamId)
                .then(function (response) {
                return response.data;
            });
        };
        DashboardService.prototype.getIssuesGroupedByPriority = function (teamId) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/IssuesGroupedByPrioroty/" + teamId)
                .then(function (response) {
                return response.data;
            });
        };
        DashboardService.prototype.getIssuesGroupedByProject = function (teamId) {
            return this.$http.get(this.appSettings.urls.baseUrl + "/api/team/IssuesGroupedByProject/" + teamId)
                .then(function (response) {
                return response.data;
            });
        };
        return DashboardService;
    }());
    TeamBins.DashboardService = DashboardService;
})(TeamBins || (TeamBins = {}));
