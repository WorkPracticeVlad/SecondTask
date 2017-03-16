let urlApies = ['http://localhost:49997/api/values', 'http://localhost:49997/api/units', 'http://localhost:49997/api/property'];
function ViewModel() {
    var self = this;
    self.values = ko.observableArray([]);
    self.units = ko.observableArray([]);
    self.properties = ko.observableArray([]);
    self.pages = ko.observableArray([]);
    self.currentPage= ko.observable(1);  
    self.viewMode = ko.observable(urlApies[1]);
    self.Toggle = function (data, event) {
        $("#page" + urlApies.indexOf(self.viewMode())).removeClass("btn-warning").addClass("btn-info");
        $("#" + event.target.id).removeClass("btn-info").addClass("btn-warning");
        self.viewMode(urlApies[Number(event.target.id.slice(-1))]);
        self.LoadPage(1);
        self.BuildPages();
    };
    self.LoadPage = function (page) {
        self.currentPage(page);
        self.Load();
    }
    self.Load = function () {
        $.getJSON(self.viewMode() + "/" + self.currentPage(), function (data) {
            switch (self.viewMode()) {
                case urlApies[1]:
                    self.units(data);
                    break;
                case urlApies[2]:
                    self.properties(data);
                    break;
                default:
                    self.values(data);
                    break;
            }
        });
    }
    self.LoadUnitValues = function (row) {
        $.getJSON(self.viewMode() + "/" + self.currentPage() + '/' + row.Identity.replace(/\./g, '|'), function (data) {
            var x = data;
        });
    }
    self.Load();
    self.BuildPages = function () {
        $.get(self.viewMode(), function (data) {
            let tempoArr = [];
            for (let i = 1; i <= data ; i++) {
                tempoArr.push(i);
            }
            self.pages(tempoArr);
        })     
    }
    self.BuildPages();
}
ko.applyBindings(new ViewModel());