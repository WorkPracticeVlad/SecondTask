var PropertyVM = function () { 
    var self = this;
    self.filter = ko.observable('');
    self.properties = ko.observableArray();
    self.pages=ko.observableArray();
    self.currentPage = ko.observable();
    self.load = function (page) {
        self.currentPage(page);
        $.getJSON('/api/property/get/' + self.currentPage()+'/'+self.filter(), function (data) {
            self.properties(data);
        });
    };
    self.buildPages = function () {
        $.get('/api/property/pagesbyfilter/'+self.filter(), function (data) {
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