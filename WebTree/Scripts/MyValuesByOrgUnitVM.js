var ValuesByOrgUnitVM = function (orgUnit) {
    var self = this;
    self.filter = ko.observable('');
    self.valuesByProperties = ko.observableArray();
    self.orgUnitIdentity = ko.observable();
    self.unitsName = ko.observableArray();
    self.pagesCount = ko.observable();
    self.currentPage = ko.observable();
    self.dataForTable = ko.observable();
    self.isLoaded = ko.observable(false);
    self.isCurrentOrgUnitCheckFlag = ko.observable(false);
    self.isResultCheckFlag = ko.observable(false);
    self.loadToAncestors = function (page) {
        self.currentPage(Number(page));
        self.isLoaded(false);
        let identity = orgUnit.identity();
        self.orgUnitIdentity(identity);
        $.getJSON('/api/values/byorgunit/' + identity.replace(/\./g, '-') + '/' + self.currentPage() + '/' + self.filter(), function (dataGet) {
            let tempoArrHeader = [];
            let tempoArrData = [];
            for (var i = 0; i < dataGet.header.length; i++) {
                tempoArrHeader.push(dataGet.header[i]);
            }
            for (var i = 0; i < dataGet.data.length; i++) {
                tempoArrData.push(dataGet.data[i]);
            }
            tempoArrHeader.unshift({ identity: null, tail: 'Name' });
            self.valuesByProperties(tempoArrData);
            self.unitsName(tempoArrHeader);
            self.dataForTable(dataGet);
        }).done(function () {
            self.isLoaded(true);
        });
    };
    self.loadCurrent = function (page) {
        self.currentPage(Number(page));
        self.isLoaded(false);
        let identity = orgUnit.identity();
        self.orgUnitIdentity(identity);
        $.getJSON('/api/values/bycurrentorgunit/' + identity.replace(/\./g, '-') + '/' + self.currentPage() + '/' + self.filter(), function (dataGet) {
            let tempoArrHeader = [];
            let tempoArrData = [];
            for (var i = 0; i < dataGet.header.length; i++) {
                tempoArrHeader.push(dataGet.header[i]);
            }
            for (var i = 0; i < dataGet.data.length; i++) {
                tempoArrData.push(dataGet.data[i]);
            }
            tempoArrHeader.unshift({ identity: null, tail: 'Name' });
            self.valuesByProperties(tempoArrData);
            self.unitsName(tempoArrHeader);
            self.dataForTable(dataGet);
        }).done(function () {
            self.isLoaded(true);
        });
    };
    self.buildToAncestorsPages = function () {
        let identity = orgUnit.identity();
        $.get('/api/values/pagesbyorgunit/' + identity.replace(/\./g, '-') + '/' + self.filter(), function (data) {
            self.pagesCount(data);
        })
    };
    self.buildCurrentPages= function () {
        let identity = orgUnit.identity();
        $.get('/api/values/pagesbycurrentorgunit/' + identity.replace(/\./g, '-') + '/' + self.filter(), function (data) {
            self.pagesCount(data);
        })
    };
    self.load =  self.loadToAncestors;
    self.buildPages =self.buildToAncestorsPages;
    self.initialize = function () {
        self.load(1);
        self.buildPages();
    };
    self.filter.subscribe(function (newFilter) {
        delay(function () {
            if (newFilter.length == 0 || newFilter.length > 3) {
                self.initialize();
            }
        }, 300)
    });
    self.isCurrentOrgUnitCheckFlag.subscribe(function (newFlag) {
        if (newFlag) {
            self.load = self.loadCurrent;
            self.buildPages = self.buildCurrentPages;
            self.initialize();
        } else {
            self.load = self.loadToAncestors;
            self.buildPages = self.buildToAncestorsPages;
            self.initialize();
        }
    })
    self.initialize();
}