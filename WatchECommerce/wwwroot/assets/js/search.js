var searchInput = document.getElementById("search-products");
if (searchInput) {
    searchInput.addEventListener("keyup", function () {

        let text = this.value
        let courseList = document.querySelector("#product-list")

        fetch('Product/Search?searchText=' + text)
            .then((response) => response.text())
            .then((data) => {
                console.log(data)
                courseList.innerHTML = data
            });

    });
}