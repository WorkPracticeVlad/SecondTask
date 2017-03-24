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
        $.getJSON('/api/values/byorgunit/' + identity.replace(/\./g, '-') + '/' + self.currentPage() + '/' + self.filter(), function (data) {
            let tempoArrUnitsName = [];
            let dataKeys = Object.keys(data);
            for (var i = 0; i < dataKeys.length; i++) {
                if (dataKeys[i].startsWith('orgUnitName-')) {
                    tempoArrUnitsName.push(dataKeys[i].substr(dataKeys[i].indexOf('-') + 1));
                    delete data[dataKeys[i]];
                } 
            }
            tempoArrUnitsName.push('Name');
            self.valuesByProperties(data);
            self.unitsName(tempoArrUnitsName.reverse());
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