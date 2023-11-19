window.Cart = (function ($) {

    function _addToCart(productId) {
        $.post('/Home/Add/' + productId, function () {
            showCart();
        });
    }

    function _removeFromCart(productId) {
        $.post('/Home/Remove/' + productId, function () {
            showCart();
        });
    }

    function _showCart() {
        var shoppingList = sessionStorage.getItem('shoppingCart');
        var cartContainer = $('#cart-container');

        if (shoppingList) {
            var productIds = JSON.parse(shoppingList);

            $.ajax({
                url: '/Home/GetCart',
                type: 'GET',
                success: function (data) {
                    cartContainer.html(data);
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching cart details:', status, error);
                }
            });
        } else {
            cartContainer.empty();
        }
    }

    $(document).ready(function () {
        _showCart();
    });

    return {
        addToCart: _addToCart,
        removeFromCart: _removeFromCart,
        showCart: _showCart
    };

}($));
