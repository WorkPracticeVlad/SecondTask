PrpoertyVM = function () {
    var self = this;
    self.filter = ko.observable('');
    self.values = ko.observableArray();
    self.pages = ko.observableArray();
    self.currentPage = ko.observable();
    self.Load = function (page) {
        self.currentPage(page);
        $.getJSON('/api/values/get/' + self.currentPage()  , function (data) {
            self.values(data);
        });
    };
    self.BuildPages = function () {
        $.get('/api/values/get/' , function (data) {
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