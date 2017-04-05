var PropertyUsageVM = function (property) {
    var self = this;
    self.propertyName = ko.observable(property.name);
    self.filter = ko.observable('');
    self.values = ko.observableArray();
    self.pagesCount = ko.observable();
    self.currentPage = ko.observable();
    self.returnPropertyTag = function (value) {
        return '<textarea style="width:250px;height:132px;background-color: #e5eeff;" readonly><property type="' + property.type + '" name="' + property.name + '">' + value + '</property></textarea>';
    };
    self.load = function (page) {
        self.currentPage(Number(page));
        $.getJSON('/api/values/byproperty/' + property.name + '/' + self.currentPage() + '/' + self.filter(), function (data) {
            self.values(data);
        });
    };
    self.buildPages = function () {
        $.get('/api/values/pagesbyproperty/' + property.name + '/' + self.filter(), function (data) {
            self.pagesCount(data);
        })
    };
    self.initialize = function () {
        self.load(1);
        self.buildPages();
    }
    self.initialize();
}