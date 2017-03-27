let TreeViewModel = function() {
    var self = this;
    self.panels = [
           "units",
           "properties",
    ];
    self.goToTeplate = function (name) {
        window.location.replace("/home/homepages/"+name);;
    }
    self.units = ko.observableArray([]);
    self.props = ko.observableArray([]);
    self.values = ko.observableArray([]);
    self.refresh = function () {
        $.ajax({
            url: "/api/tree/delete",
            type: 'DELETE',
            success: function (result) {
                $.getJSON("/api/tree/get", function (data) {
                    self.units(data.orgUnits);
                    self.props(data.properties);
                    self.values(data.values);
                });
            }
        })
    };
    $.getJSON("/api/tree/get", function (data) {
        self.units(data.orgUnits);
        self.props(data.properties);
        self.values(data.values);
    });
}
ko.applyBindings(new TreeViewModel());