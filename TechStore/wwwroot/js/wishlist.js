document.addEventListener('click', function (event) {
    var removeButton = event.target.closest('.wishlist-remove-btn[data-product-id]');

    if (removeButton) {
        event.preventDefault();
        handleWishlistRemoveOnListPage(removeButton);
        return;
    }

    var button = event.target.closest('.wishlist-heart-btn[data-product-id], .product-detail-wishlist-btn[data-product-id]');

    if (!button) {
        return;
    }

    event.preventDefault();

    if (button.disabled) {
        return;
    }

    var productId = button.getAttribute('data-product-id');
    var isInWishlist = button.getAttribute('data-in-wishlist') === 'true';
    var url = isInWishlist ? '/Wishlist/Remove' : '/Wishlist/Add';

    button.disabled = true;

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: 'productId=' + encodeURIComponent(productId)
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('İstek başarısız oldu.');
            }
            return response.json();
        })
        .then(function (data) {
            if (!data.success) {
                return;
            }

            button.setAttribute('data-in-wishlist', data.isInWishlist ? 'true' : 'false');
            button.classList.toggle('active', data.isInWishlist);

            var textSpan = button.querySelector('span');

            if (textSpan) {
                textSpan.textContent = data.isInWishlist ? 'Favorilerde' : 'Favorilere Ekle';
            }
        })
        .catch(function () {
        })
        .finally(function () {
            button.disabled = false;
        });
});

function handleWishlistRemoveOnListPage(button) {
    if (button.disabled) {
        return;
    }

    var productId = button.getAttribute('data-product-id');

    button.disabled = true;

    fetch('/Wishlist/Remove', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: 'productId=' + encodeURIComponent(productId)
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('İstek başarısız oldu.');
            }
            return response.json();
        })
        .then(function (data) {
            if (!data.success) {
                button.disabled = false;
                return;
            }

            var card = button.closest('.wishlist-item');

            if (card) {
                card.remove();
            }

            var grid = document.querySelector('.wishlist-grid');

            if (grid && grid.children.length === 0) {
                grid.outerHTML = '<div class="wishlist-empty"><span>💔</span><p>Henüz favori ürünün yok.</p><a href="/Home/Index">Alışverişe başla</a></div>';
            }
        })
        .catch(function () {
            button.disabled = false;
        });
}