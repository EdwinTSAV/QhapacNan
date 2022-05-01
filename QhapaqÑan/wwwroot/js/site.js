// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.addEventListener('load', function () {
    new Glider(document.querySelector('.slides'), {
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: {
            prev: '.flex-prev',
            next: '.flex-next'
        }
    });
});