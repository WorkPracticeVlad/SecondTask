let OrgUnit = function (identity, description, isVirtual, parentIdentity, click, toggleIsExpanded, isExpanded, pages, currentPage) {
    this.identity = ko.observable(identity);
    this.description = ko.observable(description);
    this.isVirtual = ko.observable(isVirtual);
    this.parentIdentity = ko.observable(parentIdentity);
    this.click = ko.observable(click);
    this.children = ko.observableArray();
    this.toggleIsExpanded = ko.observable(toggleIsExpanded);
    this.isExpanded = ko.observable(isExpanded);
    this.pages = ko.observableArray(pages);
    this.currentPage = ko.observable(currentPage);
}
let recursiveIdentityFind = function (nestedArr, identity, dataArr) {
    for (let i = 0; i < nestedArr.length; i++) {
        if (identity === nestedArr[i].identity()) {
            nestedArr[i].children(dataArr);
        }
        recursiveIdentityFind(nestedArr[i].children(), identity, dataArr);
    }
}
var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer =setTimeout(callback, ms);
    };
})();//?
var OrgUnitVM = function () {
    var self = this;
    self.filter = ko.observable('');
    self.buildPages = function (data) {
        let tempoArr = [];
        for (let i = 1; i <= data ; i++) {
            tempoArr.push(i);
        }
        return tempoArr;
    };
    self.buildBranch = function (arrToFill, dataArr) {
        for (var i = 0; i < dataArr.length; i++) {
            arrToFill.push(new OrgUnit(dataArr[i].identity,
                dataArr[i].description,
                dataArr[i].isVirtual, dataArr[i].parentIdentity, null, self.toggleIsExpanded, true, [], 0));
            self.buildBranch(arrToFill[i].children(), dataArr[i].children);
        }
    }
    self.toggleIsExpanded=function (orgUnit) {
        let tempo = orgUnit.isExpanded();
        orgUnit.isExpanded(!tempo);
    }
    self.loadNodePage = function (parent, data, event) {
        let identittyToUrl = parent.identity().replace(/\./g, '-');
        parent.currentPage(data);
        $.get('/api/units/rowinnode/' + identittyToUrl + '/' + data, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                let unit = dataGet[i].orgUnit;
                let pagesArr = self.buildPages(dataGet[i].pagesCount);
                orgUnitChildrenArr.push(new OrgUnit(unit.identity, unit.description, unit.isVirtual,
                     unit.parentIdentity, self.loadNodePage, self.toggleIsExpanded, true, pagesArr, 1));
            }
            recursiveIdentityFind(self.units(), parent.identity(), orgUnitChildrenArr);
        });
    };
    self.loadFilteredBranches = function () {
        let filter = self.filter().replace(/\./g, '-');
        $.get('/api/units/branchesfiltered/' + filter, function (dataGet) {
            let orgUnitBranches = [];
            self.buildBranch(orgUnitBranches, dataGet[0].children);
            self.units()[0].children(orgUnitBranches);
        });
    }
    self.filter.subscribe(function (newFilter) {
        delay(function () {
            if (newFilter.length < 3) {
                self.units([new OrgUnit('Enviroment', 'Enviroment', 'true', '', self.loadNodePage,self.toggleIsExpanded ,true, [1], 1)]);
            }
            else {
                self.units()[0].pages([]);
                self.units()[0].currentPage(0);
                self.loadFilteredBranches();
            }
        },750)        
    });
    self.units = ko.observableArray([new OrgUnit('Enviroment', 'Enviroment', 'true', '', self.loadNodePage, self.toggleIsExpanded, true, [1], 1)]);
    self.loadChildren = function (data, event) {
        let identittyToUrl = data.identity().replace(/\./g, '-');
        $.get('/api/units/childrenbyparent/' + identittyToUrl, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                orgUnitChildrenArr.push(new OrgUnit(dataGet[i].identity, dataGet[i].description, dataGet[i].isVirtual, dataGet[i].parentIdentity, self.loadChildren, true));
            }
            recursiveIdentityFind(self.units(), data.identity(), orgUnitChildrenArr);
        });
    };
}
