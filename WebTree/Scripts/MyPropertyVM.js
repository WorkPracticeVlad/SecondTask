var PropertyVM = function () { 
    var self = this;
    self.filter = ko.observable('');
    self.properties = ko.observableArray();
    self.pagesCount=ko.observable();
    self.currentPage = ko.observable(1);
    self.load = function (page) {
        self.currentPage(Number(page));
        $.getJSON('/api/property/get/' + self.currentPage()+'/'+self.filter(), function (data) {
            self.properties(data);
        });
    };
    self.buildPages = function () {
        $.get('/api/property/pagesbyfilter/'+self.filter(), function (data) {
            self.pagesCount(data);
        })
    };
    self.initialize = function () {
        self.currentPage(1);
        self.load(self.currentPage());
        self.buildPages();
    }
    self.initialize();
}