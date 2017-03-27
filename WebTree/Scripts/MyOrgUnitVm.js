let OrgUnit = function (identity, description, isVirtual, parentIdentity, click, isExpanded) {
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
let recursiveIdentityFind = function (nestedArr, identity, dataArr) {
    for (let i = 0; i < nestedArr.length; i++) {
        if (identity === nestedArr[i].identity()) {
            nestedArr[i].children(dataArr);
            toggleClick(nestedArr[i]);
        }
        recursiveIdentityFind(nestedArr[i].children(), identity, dataArr);
    }
}
let BuildBranch = function (arrToFill,dataArr) {
    for (var i = 0; i < dataArr.length; i++) {
        arrToFill.push(new OrgUnit(dataArr[i].identity,
            dataArr[i].description,
            dataArr[i].isVirtual, dataArr[i].parentIdentity, toggleOrgUnitIsExpanded, true));
            BuildBranch(arrToFill[i].children(), dataArr[i].children);
    }
}
OrgUnitVM = function () {
    var self = this;
    self.filter = ko.observable('');
    self.load = function (data, event) {
        let identittyToUrl = data.identity().replace(/\./g, '-');
        $.get('/api/units/childrenbyparent/' + identittyToUrl, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                orgUnitChildrenArr.push(new OrgUnit(dataGet[i].identity, dataGet[i].description, dataGet[i].isVirtual, dataGet[i].parentIdentity, self.load, true));
            }
            recursiveIdentityFind(self.units(), data.identity(), orgUnitChildrenArr);
        });
    };
    self.loadFiltered = function () {
        let identittyToUrl = self.filter().replace(/\./g, '-');
        $.get('/api/units/branchesfiltered/' + identittyToUrl, function (dataGet) {
            let orgUnitBranches = [];
            BuildBranch(orgUnitBranches, dataGet[0].children);
            self.units()[0].children(orgUnitBranches);
        });     
    }
    self.filter.subscribe(function (newFilter) {
        if (newFilter=='') {
            self.units()[0].children(null);
        }      
    });
    self.units = ko.observableArray([new OrgUnit('Enviroment', 'Enviroment', 'true', '', self.load, true)]);
}