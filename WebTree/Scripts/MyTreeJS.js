let urlApies = ["http://localhost:49997/api/values", "http://localhost:49997/api/units", "http://localhost:49997/api/property"];
function ViewModel() {
    var self = this;
    self.units = ko.observableArray([]);
    self.props = ko.observableArray([]);
    self.values = ko.observableArray([]);
    self.pagesProps = ko.observableArray([]);
    self.currentPageProps = ko.observable(1);
    self.numberOfPagesProps = ko.observable();    
    self.viewMode = ko.observable('propertiesMode');
    self.LoadPageProps = function (page) {
        self.currentPageProps(page);
        self.LoadProps();
    }
    self.LoadProps = function () {
        $.getJSON(urlApies[2] + "/"+self.currentPageProps(), function (data) {        
            self.props(data);
        });
    }      
    self.LoadProps();
    self.BuildPages = function () {
        $.get(urlApies[2], function (data) {
            self.numberOfPagesProps(data);
            let tempoArr = [];
            for (let i = 1; i <= self.numberOfPagesProps() ; i++) {
                tempoArr.push(i);
            }
            self.pagesProps(tempoArr);
        })     
    }
    self.BuildPages();
}
ko.applyBindings(new ViewModel());