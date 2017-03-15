let urlApi = "http://localhost:49997/api/tree";
function TreeViewModel() {
    var self = this;  
    self.units = ko.observableArray([]);
    self.props = ko.observableArray([]);
    self.values = ko.observableArray([]);
    self.Refresh = function () {
        $.ajax({
            url: urlApi,
            type: 'DELETE',
            success: function (result) {
                $.getJSON(urlApi, function (data) {
                    self.units(data.OrgUnits);
                    self.props(data.Properties);
                    self.values(data.Values);
                });
            }
        })
    };
    $.getJSON(urlApi, function (data) {
        self.units(data.OrgUnits);
        self.props(data.Properties);
        self.values(data.Values);
    });
}
ko.applyBindings(new TreeViewModel());