//let baskets = []

//const BASKET_PRODUCTS_KEY = "products";
//let cardBlock = $(".cart-block")


//// #region Initial setups

//setupAddTriggers();

//setupRemoveTriggers();

//function setupAddTriggers() {

//    let addButtons = document.querySelectorAll(".add-product-to-basket-btn")
//    addButtons.forEach(b => b.addEventListener("click", this.addProduct));
//}

//function setupRemoveTriggers() {
//    let removeButtons = document.querySelectorAll(".remove-product-to-basket-btn")
//    removeButtons.forEach(b => b.addEventListener("click", this.removeProduct));
//}

// #endregion



//// #region Add and update products and show

//function addProduct(event) {
//    event.preventDefault();

//    let endpoint = $(event.target).attr("href");
//    console.log(endpoint)

//    $.ajax({
//        url: endpoint,
//        success: function (response) {
//            console.log(response)
//            cardBlock.html(response);
//        }
//    });
//}




//// #endregion


//// #region Remove product and show


//var deleteBtns = document.querySelectorAll(".remove-product-to-basket-btn");

//$(document).on('click', ".remove-product-to-basket-btn", function (e) {
//    e.preventDefault();
//    var url = e.target.href;

//    console.log(url)
//    $.ajax({
//        url: url,
//        success: function (response) {
//            console.log(response)
//            cardBlock.html(response);
//        }
//    });
//})


//// #endregion


let btns = document.querySelectorAll(".add-product-to-basket-btn")

btns.forEach(x => x.addEventListener("click", function (e) {
    e.preventDefault()
    console.log("ceyhun")
    fetch(e.target.href)
        .then(response => response.text())
        .then(data => {
            $('.cart-block').html(data);
        })
}))

$(document).on("click", ".remove-product-to-basket-btn", function (e) {
    e.preventDefault()
    fetch(e.target.href)
        .then(response => response.text())
        .then(data => {
            $('.cart-block').html(data);
        })

})