   MainVM = function(){
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
           panel: 'units-template',
           data: panelViewModels.units
       });       
       self.goToTeplate = function(name) {
           self.currentPanelData().panel = name + "-template";
           self.currentPanelData().data = panelViewModels[name];
           self.currentPanelData.valueHasMutated();
       }
   }
   
mainViewModel = new MainVM();
ko.applyBindings(mainViewModel);