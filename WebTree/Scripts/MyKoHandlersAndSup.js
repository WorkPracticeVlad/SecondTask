var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

ko.bindingHandlers.bootstrapPopover = {
    init: function (element, valueAccessor, allBindingsAccessor, PropertyUsageVM) {
        var options = valueAccessor();
        $(element).popover({
            placement: 'right',
            title: 'Xml tag', 
            content: options.content,
            delay: 100,
            html: true,
            trigger: 'click'
        });
    }
};

ko.bindingHandlers.tableValuesByOrgUnit = {
    update: function (element, valueAccessor, allBindings) {
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
        let inputData = ko.unwrap(valueAccessor());
        let isResultCheckFlag = inputData.isResultCheckFlag;
        var dataGet = inputData.dataGet;
        if (!dataGet) {
            return;
        }
        let tempoArrHeader = [];
        let tempoArrData = [];
        for (var i = 0; i < dataGet.data.length; i++) {
            let tempo = { property: dataGet.data[i].property, unitsToValues: [] };
            for (var j = 0; j < dataGet.data[i].unitsToValues.length; j++) {
                tempo.unitsToValues.push({ orgUnitIdentity: dataGet.data[i].unitsToValues[j].orgUnitIdentity, value: dataGet.data[i].unitsToValues[j].value });
            }
            tempoArrData.push(tempo);
        }
        for (var i = 0; i < tempoArrData.length; i++) {
            if (isResultCheckFlag) {
                tempoArrData[i].unitsToValues = tempoArrData[i].unitsToValues.splice(-1, 1);
            }
        }
        for (var i = 0; i < dataGet.header.length; i++) {
            if ((isResultCheckFlag && i != dataGet.header.length - 1))
                continue;
            tempoArrHeader.push(dataGet.header[i]);
        }
        tempoArrHeader.unshift({ identity: null, tail: 'Name' });
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
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
        var dataGet = ko.unwrap(valueAccessor());
        let pagesCount = dataGet.pagesCount;
        let currentPage = dataGet.currentPage;
        let onPageClick = dataGet.onPageClick;
        let node = dataGet.node;
        if (!pagesCount || currentPage === 0 || pagesCount === 1) {
            return;
        }
        let pagesOnScreen = [currentPage - 2, currentPage - 1, currentPage, currentPage + 1, currentPage + 2];
        pagesOnScreen = pagesOnScreen.filter(function (number, index, self) {
            return number > 0 && number < pagesCount + 1 && self.indexOf(number) == index;
        });
        let appendButtonPage = function (data, text, parent) {
            let page = data;
            let button = document.createElement('button');
            button.innerText = text;
            button.addEventListener("click", function () {
                if (!node) {
                    onPageClick(page);
                } else {
                    onPageClick(node, page);
                }
            }, false);
            if (page !== currentPage) {
                button.className = "btn btn-info btn-xs";
            } else {
                button.className = "btn btn-warning btn-xs";
            }

            parent.appendChild(button);
        }
        appendButtonPage(1, 'First', element);
        if (currentPage - 1 >= 1) {
            appendButtonPage(currentPage - 1, 'Previous', element);
        }
        for (var i = 0; i < pagesOnScreen.length; i++) {
            appendButtonPage(pagesOnScreen[i], pagesOnScreen[i], element);
        }
        if (currentPage + 1 <= pagesCount) {
            appendButtonPage(currentPage + 1, 'Next', element);
        }
        appendButtonPage(pagesCount, 'Last', element);
    }
};
