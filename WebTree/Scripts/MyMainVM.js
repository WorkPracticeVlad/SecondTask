   MainVM = function(){
       var self = this;
       self.goToPropertiesUsage = function (row) {
           self.currentPanelData().panel = 'propertiesUsage-template';
           self.currentPanelData().data = panelViewModels.propertiesUsage(row);
           self.currentPanelData.valueHasMutated();
       }
       self.panels = [
           "units",
           "properties",
       ];
       var panelViewModels = {
           "units": new OrgUnitVM(),
           "properties": new PropertyVM(),
           "propertiesUsage": function SetPropertyUsageVM(name) {
               return new PropertyUsageVM(name);       
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