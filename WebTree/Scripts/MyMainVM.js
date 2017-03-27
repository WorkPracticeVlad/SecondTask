﻿var MainVM = function () {
    var self = this;
    self.goToPropertiesUsage = function (row) {
        self.currentPanelData().panel = 'propertiesUsage-template';
        self.currentPanelData().data = panelViewModels.propertiesUsage(row);
        self.currentPanelData.valueHasMutated();
    }
    self.goToValuesByOrgUnit = function (orgUnit) {
        self.currentPanelData().panel = 'valuesByOrgUnit-template';
        self.currentPanelData().data = panelViewModels.valuesByOrgUnit(orgUnit);
        self.currentPanelData.valueHasMutated();
    }
    self.panelFromUrl = function () {
        let url = window.location.href.toString();
        switch (url.substr(url.lastIndexOf('#') + 1)) {
            case '':
                return 'units';
                break;
            case 'units':
                return 'units';
                break;
            case 'properties':
                return 'properties';
                break;
            default:
                return 'units';

        }
    }
    self.panels = [
        "units",
        "properties",
    ];
    var panelViewModels = {
        "units": new OrgUnitVM(),
        "properties": new PropertyVM(),
        "propertiesUsage": function setPropertyUsageVM(name) {
            return new PropertyUsageVM(name);
        },
        "valuesByOrgUnit": function setValuesByOrgUnitVM(orgUnit) {
            return new ValuesByOrgUnitVM(orgUnit);
        }
    };
    self.currentPanelData = ko.observable({
        panel: self.panelFromUrl() + '-template',
        data: panelViewModels.units
    });
    self.goToTeplate = function (name) {
        self.currentPanelData().panel = name + "-template";
        self.currentPanelData().data = panelViewModels[name];
        self.currentPanelData.valueHasMutated();
    }
}

mainViewModel = new MainVM();
ko.applyBindings(mainViewModel);