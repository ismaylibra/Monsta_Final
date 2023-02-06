  $(document).on('click', '#productLink', function () {
            var productId = $(this).children('#productId').val();
            $.ajax({
                type: "POST",
                url: '/WishList/AddProductToWishList',
                data: { productId: productId },
                success: function () {
                    $("#favorite" + productId).removeClass("fa-regular fa-heart");
                    $("#favorite" + productId).addClass("fa-solid fa-heart");
                },
            });
  })
$(document).on('click', '#productLink', function () {
    var productId = $(this).children('#productId').val();
    $.ajax({
        type: "POST",
        url: '/WishList/DeleteProductFromWishList',
        data: { productId: productId },
        success: function () {
            $("#" + productId + "tr").remove();
            
        },
    });
})




