let TreeViewModel = function() {
    var self = this;
    self.isRefreshed= ko.observable(false);
    self.panels = [
           "units",
           "properties",
    ];
    self.goToTeplate = function (name) {
        window.location.replace("/home/homepages#"+name);;
    }
    self.refresh = function () {
        $.ajax({
            url: "/api/tree/delete",
            type: 'DELETE',
            success: function (result) {
                self.isRefreshed(result);
            }
        })
    }; 
}
ko.applyBindings(new TreeViewModel());