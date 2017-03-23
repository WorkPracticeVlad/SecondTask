ValuesByOrgUnitVM = function (orgUnit) {
    var self = this;
    self.filter = ko.observable('');
    self.valuesProperties = ko.observableArray();
    self.valuesUnits = ko.observableArray();
    self.pages = ko.observableArray();
    self.currentPage = ko.observable();
    self.load = function (page) {
        self.currentPage(page);
        let identity = orgUnit.identity();
        $.getJSON('/api/values/byorgunit/' + identity.replace(/\./g, '-') + '/' + self.currentPage() + '/' + self.filter(), function (data) {
            let tempoArrProperty = [];
            let tempoArrUnits = [];
            for (var i = 0; i < data.length; i++) {
                if (data[i].NameGroup.startsWith('OrgUnitName-')) {
                    tempoArrUnits.push(data[i])
                } else {
                    tempoArrProperty.push(data[i])
                }
            }

            self.valuesProperties(tempoArrProperty);
            self.valuesUnits(tempoArrUnits);
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