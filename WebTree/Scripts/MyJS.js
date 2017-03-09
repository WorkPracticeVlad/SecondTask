﻿let urlApi = "http://localhost:49997/api/values";
function ViewModel() {
    var self = this;
    self.shouldShowUnits = ko.observable(true);
    self.shouldShowProps = ko.observable(false);
    self.shouldShowValues = ko.observable(false);
    self.shouldShowUnitBtn = ko.observable(false);
    self.shouldShowPropBtn = ko.observable(true);
    self.shouldShowValueBtn = ko.observable(true);
    self.ShowUnits = function () {
        self.shouldShowUnits(true);
        self.shouldShowProps(false);
        self.shouldShowValues(false);
        self.shouldShowValueBtn(self.shouldShowUnitBtn());
        self.shouldShowPropBtn(self.shouldShowUnitBtn());
        self.shouldShowUnitBtn(!self.shouldShowUnitBtn());
    };
    self.ShowProps = function () {
        self.shouldShowUnits(false);
        self.shouldShowProps(true);
        self.shouldShowValues(false);
        self.shouldShowUnitBtn(self.shouldShowPropBtn());
        self.shouldShowValueBtn(self.shouldShowPropBtn());
        self.shouldShowPropBtn(!self.shouldShowPropBtn());
    };
    self.ShowValues = function () {
        self.shouldShowUnits(false);
        self.shouldShowProps(false);
        self.shouldShowValues(true);
        self.shouldShowUnitBtn(self.shouldShowValueBtn());
        self.shouldShowPropBtn(self.shouldShowValueBtn());
        self.shouldShowValueBtn(!self.shouldShowValueBtn());
    };
    self.units = ko.observableArray([]);
    self.props = ko.observableArray([]);
    self.orgUnitProps = ko.observableArray([]);
    $.getJSON(urlApi, function (data) {
        self.units(data.OrgUnits);
        self.props(data.Props);
        self.orgUnitProps(data.OrgUnitToProps);
    })   
}
$("#refresh").click(function () {
    $.ajax({
        url: urlApi,
        type: 'DELETE',
        success: function (result) {
            $.getJSON(urlApi, function (data) {
                self.units(data.OrgUnits);
                self.props(data.Props);
                self.orgUnitProps(data.OrgUnitToProps);
            })
        }
    });
});
ko.applyBindings(new ViewModel());