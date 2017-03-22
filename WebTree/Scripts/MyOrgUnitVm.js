let OrgUnit = function (identity, description, isVirtual, parentIdentity) {
    this.Identity = ko.observable(identity);
    this.Description = ko.observable(description);
    this.IsVirtual = ko.observable(isVirtual);
    this.ParentIdentity = ko.observable(parentIdentity);
    this.Children = ko.observableArray();
}
let RecursiveIdentityFinder = function (nestedArr,identity,dataArr) {
    for (let i = 0; i < nestedArr.length; i++) {
        if (identity === nestedArr[i].Identity()) {
            nestedArr[i].Children(dataArr);
            return;
        }
        RecursiveIdentityFinder(nestedArr[i].Children, identity, dataArr);
    }
}
OrgUnitVM = function () {
    var self = this;
    self.units = ko.observableArray([new OrgUnit('Enviroment', 'Enviroment', 'true', '')]);
    self.Load = function (data, event) {
        $.get('/api/units/childrenbyparent/' + event.currentTarget.innerText, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                orgUnitChildrenArr.push(new OrgUnit(dataGet[i].Identity, dataGet[i].Description, dataGet[i].IsVirtual, dataGet[i].ParentIdentity));
            }
            RecursiveIdentityFinder(self.units(), event.currentTarget.innerText, orgUnitChildrenArr);            
        });
    }
}