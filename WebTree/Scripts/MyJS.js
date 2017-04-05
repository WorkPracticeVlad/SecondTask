let TreeViewModel = function() {
    var self = this;
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
            type: 'DELETE'          
        })
    }; 
}
ko.applyBindings(new TreeViewModel());