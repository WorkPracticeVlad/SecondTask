ko.bindingHandlers.bootstrapPopover = {
    init: function (element, valueAccessor, allBindingsAccessor, PropertyUsageVM) {
        var options = valueAccessor();
        $(element).popover({
            placement: 'right',
            content: options.content,
            delay:400,
            html: true,
            trigger: 'hover'
        });
    }
};

var PropertyUsageVM = function (property) {
    var self = this;
    self.filter = ko.observable('');
    self.values = ko.observableArray();
    self.pages = ko.observableArray();
    self.currentPage = ko.observable();
    self.returnPropertyTag = function (value) {
        return '<textarea style="width:250px;height:132px;background-color: #e5eeff;" readonly><property type="' + property.type + '" name="' + property.name + '">' + value + '</property></textarea>';
    };
    self.load = function (page) {
        self.currentPage(page);
        $.getJSON('/api/values/byproperty/' + property.name + '/' + self.currentPage() + '/' + self.filter(), function (data) {
            self.values(data);
        });
    };
    self.buildPages = function () {
        $.get('/api/values/pagesbyproperty/' + property.name + '/' + self.filter(), function (data) {
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