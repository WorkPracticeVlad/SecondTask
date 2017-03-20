let urlApies = ['/api/values/get/', '/api/units/get/', '/api/property/get'];
function ViewModel() {
    var self = this;
    self.values = ko.observableArray([]);
    self.valuesPerUnit = ko.observableArray([]);
    self.units = ko.observableArray([]);
    self.properties = ko.observableArray([]);
    self.pages = ko.observableArray([]);
    self.currentPage = ko.observable(1);
    self.viewMode = ko.observable(urlApies[1]);
    self.showUnitsButton = ko.observable(true);
    self.ToggleTables = function (data, event) {
        self.showUnitsButton(true);
        $("#navButtons button").not("#backToUnit3").removeClass("btn-warning").addClass("btn-info");
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
        self.showUnitsButton(!self.showUnitsButton());
        $.getJSON(self.viewMode() + "/" + self.currentPage() + '/' + row.Identity.replace(/\./g, '|'), function (data) {
            self.valuesPerUnit(data);
        });
        self.viewMode('valuesPerUnitMode3');
    }
    self.BuildPages = function () {
        $.get(self.viewMode(), function (data) {
            let tempoArr = [];
            for (let i = 1; i <= data ; i++) {
                tempoArr.push(i);
            }
            self.pages(tempoArr);
        })
    }
    self.BackToUnits = function () {
        self.showUnitsButton(!self.showUnitsButton());
        self.viewMode(urlApies[1]);
        self.LoadPage(self.currentPage());
    }
    self.BuildPages();
    self.Load();
}
ko.applyBindings(new ViewModel());