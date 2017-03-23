ValesByOrgUnitVM = function (orgUnit) {
    var self = this;
    self.filter = ko.observable('');
    self.values = ko.observableArray();
    self.pages = ko.observableArray();
    self.currentPage = ko.observable();
    self.load = function (page) {
        self.currentPage(page);
        $.getJSON('/api/values/byorgunit/' + orgUnit.Identity + '/' + self.currentPage() + '/' + self.filter(), function (data) {
            self.values(data);
        });
    };
    //self.buildPages = function () {
    //    $.get('/api/values/pagesbyproperty/' + property.Name + '/' + self.filter(), function (data) {
    //        let tempoArr = [];
    //        for (let i = 1; i <= data ; i++) {
    //            tempoArr.push(i);
    //        }
    //        self.pages(tempoArr);
    //    })
    //};
    self.initialize = function () {
        //self.buildPages();
        self.load(1);
    }
    self.initialize();
}