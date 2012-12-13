// paging.js

// registers a knockout binding handler to change the visiblity of the previous page
// and next page buttons depending on whether the page exists
ko.bindingHandlers.hidden = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        element.style.visibility = value ? "hidden" : "visible";
    }
}

// the URL of the first page to retrieve
var startPage = "api/Books?$orderby=ISBN";

var viewModel = new Object();
viewModel.books = ko.observable();
viewModel.currentPage = ko.observable(startPage);
viewModel.previousPages = ko.observableArray();
viewModel.nextPage = ko.observable();

// On initialization, make a request for the first page
$(document).ready(function () {
    OData.read(viewModel.currentPage(), function (data) {
        viewModel.nextPage(data.__next);
        viewModel.books(data.results);
        ko.applyBindings(viewModel);
    });
});

// When the previous button is clicked, make a request to get the previous page viewed
var previousButtonClicked = function () {
    var previousPageLink = viewModel.previousPages.pop();
    OData.read(previousPageLink, function (data) {
        viewModel.nextPage(viewModel.currentPage());
        viewModel.currentPage(previousPageLink);
        viewModel.books(data.results);
    });
}

// When the next button is clicked, make a request to get the next page using the link
// from the current page's request
var nextButtonClicked = function () {
    var nextPageLink = viewModel.nextPage();
    OData.read(nextPageLink, function (data) {
        viewModel.previousPages.push(viewModel.currentPage());
        viewModel.currentPage(nextPageLink);
        viewModel.nextPage(data.__next);
        viewModel.books(data.results);
    });
}