var PropertyVM = function () { 
    var self = this;
    self.filter = ko.observable('');
    self.properties = ko.observableArray();
    self.pagesCount=ko.observable();
    self.currentPage = ko.observable(1);
    self.isLoaded = ko.observable(false);
    self.load = function (page) {
        self.isLoaded(false);
        self.currentPage(Number(page));
        $.getJSON('/api/property/get/' + self.currentPage()+'/'+self.filter(), function (data) {
            self.properties(data);
        }).done(function () {
            self.isLoaded(true);
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
    self.filter.subscribe(function (newFilter) {
        delay(function () {           
                self.initialize();
        }, 300)
    });
    self.initialize();
}