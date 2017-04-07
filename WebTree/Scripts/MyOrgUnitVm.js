let OrgUnit = function (identity, description, isVirtual, parentIdentity, click, toggleIsExpanded, isExpanded, pagesCount, currentPage) {
    this.identity = ko.observable(identity);
    this.description = ko.observable(description);
    this.isVirtual = ko.observable(isVirtual);
    this.parentIdentity = ko.observable(parentIdentity);
    this.click = ko.observable(click);
    this.children = ko.observableArray();
    this.toggleIsExpanded = ko.observable(toggleIsExpanded);
    this.isExpanded = ko.observable(isExpanded);
    this.pagesCount = ko.observable(pagesCount);
    this.currentPage = ko.observable(currentPage);
    this.isLoaded = ko.observable(true);
}
let recursiveIdentityFind = function (nestedArr, identity, dataArr) {
    for (let i = 0; i < nestedArr.length; i++) {
        if (identity === nestedArr[i].identity()) {
            nestedArr[i].children(dataArr);
        }
        recursiveIdentityFind(nestedArr[i].children(), identity, dataArr);
    }
}
var OrgUnitVM = function () {
    var self = this;
    self.fistElementsIsLoaded = ko.observable(false);
    self.filter = ko.observable('');
    self.buildBranch = function (arrToFill, dataArr) {
        for (var i = 0; i < dataArr.length; i++) {
            arrToFill.push(new OrgUnit(dataArr[i].identity, dataArr[i].description, dataArr[i].isVirtual,
                dataArr[i].parentIdentity, null, self.toggleIsExpanded, true, 0, 0));
            self.buildBranch(arrToFill[i].children(), dataArr[i].children);
        }
    }
    self.toggleIsExpanded = function (orgUnit) {
        if (orgUnit.children().length == 0) {
            self.loadNodePage(orgUnit, 1, null);
        }
        let tempo = orgUnit.isExpanded();
        orgUnit.isExpanded(!tempo);
    }
    self.loadNodePage = function (parent, data, event) {
        parent.isLoaded(false);
        let identittyToUrl = parent.identity().replace(/\./g, '-');
        parent.currentPage(data);
        $.get('/api/units/rowinnode/' + identittyToUrl + '/' + data, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                let unit = dataGet[i].orgUnit;
                let pagesCount = dataGet[i].pagesCount;
                orgUnitChildrenArr.push(new OrgUnit(unit.identity, unit.description, unit.isVirtual,
                     unit.parentIdentity, self.loadNodePage, self.toggleIsExpanded, false, pagesCount, 1));
            }
            recursiveIdentityFind(self.units(), parent.identity(), orgUnitChildrenArr);
        })
        .done(function () {
            parent.isLoaded(true);
        });
    };
    self.loadFilteredBranches = function () {
        let filter = self.filter().replace(/\./g, '-');
        $.get('/api/units/branchesfiltered/' + filter, function (dataGet) {
            let orgUnitBranches = [];
            self.buildBranch(orgUnitBranches, dataGet[0].children);
            self.units(orgUnitBranches);
        }).done(function () {
            self.fistElementsIsLoaded(true);
        });
    }
    self.filter.subscribe(function (newFilter) {
        self.fistElementsIsLoaded(false);
        delay(function () {
            if (newFilter.length < 3) {
                if (self.units().some(function (u) {
                    return u.children().length == 0
                })) {
                    self.fistElementsIsLoaded(true);
                    return;
                }
                self.units(self.loadEnviromentChildren());
            }
            else {
                self.loadFilteredBranches();
                if (self.units()[0]) {
                    self.units()[0].isExpanded(true);
                    self.units()[0].pagesCount(0);
                    self.units()[0].currentPage(0);
                }  
            }
        }, 300)
    });
    self.units = ko.observableArray();
    self.loadEnviromentChildren = function () {
        let identittyToUrl = 'Enviroment';
        $.get('/api/units/childrenbyparent/' + identittyToUrl, function (dataGet) {
            let orgUnitChildrenArr = [];
            for (var i = 0; i < dataGet.length; i++) {
                orgUnitChildrenArr.push(new OrgUnit(dataGet[i].identity, dataGet[i].description, dataGet[i].isVirtual, dataGet[i].parentIdentity, self.loadNodePage, self.toggleIsExpanded, false, 1, 1));
            }
            self.units(orgUnitChildrenArr);
        })
        .done(function () {
            self.fistElementsIsLoaded(true);
        });
    };
    self.loadEnviromentChildren();
}

