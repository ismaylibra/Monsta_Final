$(document).on('click', '#add-product-to-cart', function () {

    var productId = $(this).parent().children().val();
    console.log(productId)
    $.ajax({
        type: "POST",
        url: '/Basket/AddToBasket',
        data: { productId: productId },
        success: function () {
        },
    });

})


$(document).on('change', '#product-quality-from-icon', function () {
    var productId = $(this).parent().children().val();
    var count = $(this).val();

    $.ajax({
        type: "POST",
        url: '/Basket/ChangeProductQuality',
        data: { productId: productId, count: count },
        success: function () {
        },
    });


});