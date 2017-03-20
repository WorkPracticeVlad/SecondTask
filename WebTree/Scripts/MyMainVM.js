   MainVM = function(){
       var self = this;
       self.checkClick = function (page) {
           let y = page;
          let x = 1;
       }
       self.panels = [
           "units",
           "properties"
       ];
       var panelViewModels = {
           "units": new OrgUnitVM(),
           "properties": new PrpoertyVM()
       };
       self.currentPanelData = ko.observable({
           panel: 'units-template',
           data: panelViewModels["units"]
       });       
       self.goToTeplate = function(name) {
           self.currentPanelData()['panel'] = name + "-template";
           self.currentPanelData()['data'] = panelViewModels[name];
           self.currentPanelData.valueHasMutated();
       }
   }
   
mainViewModel = new MainVM();
ko.applyBindings(mainViewModel);