let ValuesByProperty = function (property, unitsToValues) {
    this.property = property;
    this.unitsToValues = unitsToValues;
}
var ValuesByOrgUnitVM = function (orgUnit) {
    var self = this;
    self.filter = ko.observable('');
    self.valuesByProperties = ko.observableArray();
    self.unitsName = ko.observableArray();
    self.pages = ko.observableArray();
    self.currentPage = ko.observable();
    self.load = function (page) {
        self.currentPage(page);
        let identity = orgUnit.identity();
        $.getJSON('/api/values/byorgunit/' + identity.replace(/\./g, '-') + '/' + self.currentPage() + '/' + self.filter(), function (dataGet) {
            let tempoArrHeader = [];
            let tempoArrData = [];
            let tempoArrIds = [];
            for (var i = 0; i < dataGet.header.length; i++) {
                tempoArrHeader.push(dataGet.header[i]);
                tempoArrIds.push(dataGet.header[i].identity);
            }
            for (var i = 0; i < dataGet.data.length; i++) {
                let tempoIds = [];
                for (var j = 0; j < dataGet.data[i].unitsToValues.length; j++) {
                    tempoIds.push(dataGet.data[i].unitsToValues[j].orgUnitIdentity);
                }
                let diffIds=$(tempoArrIds).not(tempoIds).get();
                for (var i = 0; i < diffIds.length; i++) {
                    dataGet.data[i].unitsToValues.push({ orgUnitIdentity: diffIds[i], value: null });
                }
                tempoArrData.push(new ValuesByProperty(dataGet.data[i].property, dataGet.data[i].unitsToValues));
            }            
            tempoArrHeader.push({ identity: null, tail: 'Name' });
            self.valuesByProperties(tempoArrData);
            self.unitsName(tempoArrHeader.reverse());
        });
    };
    self.buildPages = function () {
        let identity = orgUnit.identity();
        $.get('/api/values/pagesbyorgunit/' + identity.replace(/\./g, '-') + '/' + self.filter(), function (data) {
            let tempoArr = [];
            for (let i = 1; i <= data ; i++) {
                tempoArr.push(i);
            }
            self.pages(tempoArr);
        })
    };
    self.initialize = function () {
        self.buildPages();
        self.load(1);
    }
    self.initialize();
}