ko.bindingHandlers.bootstrapPopover = {
    init: function (element, valueAccessor, allBindingsAccessor, PropertyUsageVM) {
        var options = valueAccessor();
        $(element).popover({
            placement: 'right',
            content: options.content,
            delay: 400,
            html: true,
            trigger: 'hover'
        });
    }
};

ko.bindingHandlers.tableValuesByOrgUnit = {
    update: function (element, valueAccessor, allBindings) {
        var dataGet = ko.unwrap(valueAccessor());
        if (!dataGet) {
            return;
        }
        let tempoArrHeader = [];
        let tempoArrData = [];
        for (var i = 0; i < dataGet.header.length; i++) {
            tempoArrHeader.push(dataGet.header[i]);
        }
        for (var i = 0; i < dataGet.data.length; i++) {
            tempoArrData.push(dataGet.data[i]);
        }
        tempoArrHeader.unshift({ identity: null, tail: 'Name' });
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
        var table = document.createElement('table');
        let tableHead = document.createElement('thead');
        let headRow = document.createElement('tr');
        for (var i = 0; i < tempoArrHeader.length; i++) {
            let columnData = document.createElement('th');
            columnData.innerText = tempoArrHeader[i].tail;
            headRow.appendChild(columnData);
        }
        tableHead.appendChild(headRow);
        table.appendChild(tableHead);
        let tableBody = document.createElement('tbody');
        for (var n = 0; n < tempoArrData.length; n++) {
            let row = document.createElement('tr');
            let propertyColumnData = document.createElement('th');
            propertyColumnData.innerText = tempoArrData[n].property;
            row.appendChild(propertyColumnData);
            for (var j = 0; j < tempoArrData[n].unitsToValues.length; j++) {
                let columnData = document.createElement('td');
                columnData.innerText = tempoArrData[n].unitsToValues[j].value;
                row.appendChild(columnData);
            }
            tableBody.appendChild(row);
        }
        table.appendChild(tableBody);
        table.className += "table table-bordered";
        element.appendChild(table);
    }
};

ko.bindingHandlers.pagesBuilder = {
    update: function (element, valueAccessor, allBindings) {
        var pages = ko.unwrap(valueAccessor());
        let currentPage = allBindings.get('currentPage');
        let onPageClick = allBindings.get('onPageClick');
        let node = allBindings.get('node');
        if (!pages || currentPage === 0) {
            return;
        }
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
        let pagesOnScreen = [1, currentPage - 2, currentPage - 1, currentPage, currentPage + 1, currentPage + 2, pages.length];
        pagesOnScreen = pagesOnScreen.filter(function (number, index, self) {
            return number > 0 && number < pages.length + 1 && self.indexOf(number) == index;
        });
        for (var i = 0; i < pagesOnScreen.length; i++) {
            let page = pagesOnScreen[i];
            let button = document.createElement('button');
            button.innerText = page;
            button.addEventListener("click", function () {
                if (!node) {
                    onPageClick(page);
                } else {
                    onPageClick(node, page);
                }
            }, false);
            if (page === currentPage) {
                button.className = "btn btn-primary btn-xs";
            }
            element.appendChild(button);
        }
        //for (var page in pages) {
        //    let y =page;
        //    let tempo = Number(page) + 1;
        //    let button = document.createElement('button');
        //    button.innerText = tempo;
        //    button.addEventListener("click", function () {
        //        onPageClick(tempo);
        //    }, false);
        //    if (tempo === currentPage) {
        //        button.className = "btn-primary";
        //    }
        //    element.appendChild(button);
        //};
    }
};
