//OrgUnit = function (identity, description, isVirtual, parentIdentity) {
//    this.identity = ko.observable(identity);
//    this.description = ko.observable(description);
//    this.isVirtual = ko.observable(isVirtual);
//    this.parentIdentity = ko.observable(parentIdentity);
//}
OrgUnitVM = function () {
    var self = this;
    self.units = ko.observableArray();
    self.Load = function () {
        $.getJSON('/api/units/get/1', function (data) {
            self.units(data);
        });
    };
    self.Load();
}