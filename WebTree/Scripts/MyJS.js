let urlApies = ["http://localhost:49997/api/tree", "http://localhost:49997/api/values", "http://localhost:49997/api/units", "http://localhost:49997/api/property"];
let tempo;
function TreeViewModel() {
    var self = this;  
    self.units = ko.observableArray([]);
    self.props = ko.observableArray([]);
    self.values = ko.observableArray([]);
    self.Refresh = function () {
        $.ajax({
            url: urlApies[0],
            type: 'DELETE',
            success: function (result) {
                $.getJSON(urlApies[0], function (data) {
                    self.units(data.OrgUnits);
                    self.props(data.Props);
                    self.values(data.OrgUnitToProps);
                });
            }
        })
    };
    $.getJSON(urlApies[0], function (data) {
        self.units(data.OrgUnits);
        self.props(data.Props);
        self.values(data.OrgUnitToProps);
    });
}
ko.applyBindings(new TreeViewModel());