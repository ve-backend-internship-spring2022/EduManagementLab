// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function submitForm() {
    let form = document.getElementById("form__submit");
    form.submit();
}

$(function () {
    var placeholderElement = $('#RemoveModal-placeholder');

    $('button[data-toggle="ajax-RemoveModal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.removemodal').modal('show');
        });
    });
});