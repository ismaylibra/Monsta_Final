const addToCartBtns = document.querySelectorAll(".add_to_cartBtn");


function addToCart(ev) {
    
    const productId = ev.target.getAttribute("data-id");

    fetch(`/basket/AddToBasket?productId=${productId}`).then(response => {
        console.log(response);
    })
}
addToCartBtns.forEach(addToCartBtn => addToCartBtn.addEventListener("click", addToCart))

