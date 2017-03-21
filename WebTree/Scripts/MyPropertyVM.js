PropertyVM = function () { 
    var self = this;
    self.filter = ko.observable('');
    self.properties = ko.observableArray();
    self.pages=ko.observableArray();
    self.currentPage = ko.observable();
    self.Load = function (page) {
        self.currentPage(page);
        $.getJSON('/api/property/get/' + self.currentPage()+'/'+self.filter(), function (data) {
            self.properties(data);
        });
    };
    self.BuildPages = function () {
        $.get('/api/property/pagesbyfilter/'+self.filter(), function (data) {
            let tempoArr = [];
            for (let i = 1; i <= data ; i++) {
                tempoArr.push(i);
            }
            self.pages(tempoArr);
        })
    };
    self.Initialize = function () {
        self.BuildPages();
        self.Load(1);
    }
    self.Initialize();
}