let orgUnit = function (identity, description, isVirtual, parentIdentity,click,isExpanded) {
    this.identity = ko.observable(identity);
    this.description = ko.observable(description);
    this.isVirtual = ko.observable(isVirtual);
    this.parentIdentity = ko.observable(parentIdentity);
    this.click = ko.observable(click);
    this.children = ko.observableArray();
    this.isExpanded = ko.observable(isExpanded);
}
let toggleOrgUnitIsExpanded = function (orgUnit) {
    let tempo = orgUnit.isExpanded();
    orgUnit.isExpanded(!tempo);
}
let toggleClick = function (orgUnit) {
    orgUnit.click(toggleOrgUnitIsExpanded);
}
let recursiveIdentityFind = function (nestedArr,identity,dataArr) {
    for (let i = 0; i < nestedArr.length; i++) {
        if (identity === nestedArr[i].identity()) {
            nestedArr[i].children(dataArr);
            toggleClick(nestedArr[i]);
            return;
        }
        recursiveIdentityFind(nestedArr[i].children(), identity, dataArr);
    }
}

OrgUnitVM = function () {
    var self = this;
    self.load = function (data, event) {
        let identittyToUrl = data.identity().replace(/\./g, '-');
        $.get('/api/units/childrenbyparent/' + identittyToUrl, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                orgUnitChildrenArr.push(new orgUnit(dataGet[i].Identity, dataGet[i].Description, dataGet[i].IsVirtual, dataGet[i].ParentIdentity, self.load, true));
            }
            recursiveIdentityFind(self.units(), data.identity(), orgUnitChildrenArr);
        });
    };
    self.units = ko.observableArray([new orgUnit('Enviroment', 'Enviroment', 'true', '', self.load,true)]);
}