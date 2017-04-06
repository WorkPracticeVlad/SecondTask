let ValuesByProperty = function (property, unitsToValues) {
    this.property = property;
    this.unitsToValues = unitsToValues;
}
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
    self.isNativeOrgUnitCheckFlag = ko.observable(false);
    self.isResultCheckFlag = ko.observable(false);
    self.load = function (page) {
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
    self.buildPages = function () {
        let identity = orgUnit.identity();
        $.get('/api/values/pagesbyorgunit/' + identity.replace(/\./g, '-') + '/' + self.filter(), function (data) {
            self.pagesCount(data);
        })
    };
    self.initialize = function () {
        self.load(1);
        self.buildPages();  
    }
    self.filter.subscribe(function (newFilter) {
        delay(function () {
            self.initialize();
        }, 300)
    });
    self.initialize();
}