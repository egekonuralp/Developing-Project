document.addEventListener('click', function (event) {
    var thumbnail = event.target.closest('.product-thumbnail[data-image-url]');

    if (!thumbnail) {
        return;
    }

    var mainImage = document.getElementById('mainProductImage');

    if (!mainImage) {
        return;
    }

    mainImage.src = thumbnail.getAttribute('data-image-url');

    document.querySelectorAll('.product-thumbnail.active').forEach(function (el) {
        el.classList.remove('active');
    });

    thumbnail.classList.add('active');
});