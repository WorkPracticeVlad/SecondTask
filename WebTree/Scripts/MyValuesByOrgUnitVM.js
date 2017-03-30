ko.bindingHandlers.tableValuesByOrgUnit = {
    init: function (element, valueAccessor, allBindings) {
        var identity = ko.unwrap(valueAccessor());
        let page = allBindings.get('page');
        let filter = allBindings.get('filter');
        let tempoArrHeader = [];
        let tempoArrData = [];
        $.getJSON('/api/values/byorgunit/' + identity.replace(/\./g, '-') + '/' + page+ '/' + filter, function (dataGet) {    
            for (var i = 0; i < dataGet.header.length; i++) {
                tempoArrHeader.push(dataGet.header[i]);
            }
            for (var i = 0; i < dataGet.data.length; i++) {
                tempoArrData.push(dataGet.data[i]);
            }
            tempoArrHeader.unshift({ identity: null, tail: 'Name' });
        });
        let headRow = document.createElement('tr');
        for (var i = 0; i < tempoArrHeader.length; i++) {
            var columnData = document.createElement('th');
            columnData.innerText = tempoArrHeader[i].tail;
            headRow.appendChild(columnData);
        }      
        var table = document.createElement('table');
        table.appendChild(headRow);
        element.appendChild(table);
    }
};


let ValuesByProperty = function (property, unitsToValues) {
    this.property = property;
    this.unitsToValues = unitsToValues;
}
var ValuesByOrgUnitVM = function (orgUnit) {
    var self = this;
    self.identityOrgUnit = ko.observable(orgUnit.identity());
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
            for (var i = 0; i < dataGet.header.length; i++) {
                tempoArrHeader.push(dataGet.header[i]);
            }
            for (var i = 0; i < dataGet.data.length; i++) {
                tempoArrData.push(dataGet.data[i]);
            }
            tempoArrHeader.unshift({ identity: null, tail: 'Name' });
            self.valuesByProperties(tempoArrData);
            self.unitsName(tempoArrHeader);
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