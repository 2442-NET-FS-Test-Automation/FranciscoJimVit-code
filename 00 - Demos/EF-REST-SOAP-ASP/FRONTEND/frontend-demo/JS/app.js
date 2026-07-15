// Using hard coded items until we call our API
// The same catalog shape js/app.js renders - and GET /api/Inventory returns.
let catalogItems = [
    { sku: "BK-101", name: "Clean Code",                price: 29.99, currentStock: 12 },
    { sku: "BK-102", name: "The Pragmatic Programmer",  price: 34.99, currentStock: 7 },
    { sku: "BK-103", name: "Design Patterns",           price: 44.99, currentStock: 3 },
    { sku: "BK-104", name: "Refactoring",               price: 39.99, currentStock: 0 },
];

// Lets do some rendering
function renderCards(items) {
    // In JS the HTML page that the JS is running against is treated as an object
    // called the DOM.

    // I want to grab the elemenyt that contains my cards
    const container = document.querySelector("#catalog-cards")
                                // .getElementsByID

    if (items.length === 0) {
        container.innerHTML = `<p class="hint">nothing matches </p>`;
        return;
    }

    // Using a map to generate a whole blovk of HTML for each item in our array
    container.innerHTML = items.map(item => `
            <article class="card" data-sku="${item.sku}">
                <h3>${item.name}</h3>
                <dl>
                    <dt>SKU</dt><dd>${item.sku}</dd>
                    <dt>Price</dt><dd>${item.price.toFixed(2)}</dd>
                    <dt>In Stock</dt><dd>${item.currentStock}</dd>
                </dl>
                <button class="price-btn data-sku="${item.sku}">Supplier price</button>
                <p class="supplier-price"></p>
            </article>
        `
    ).join("");
}

// Event listenner section

// Event listeners let us "listen" for certain actions/states in the HTML page
// Elements loading, buttons being clicked, even hovering over a certain element.

// Adding an event listener for our cards. This will handle every click
// event that happens within the card. the card has a button - when it's clicked
// the event bubbles up - and we can catch it at it's partner/container
document.querySelector("#catalog-cards").addEventListener("click", (e) => {
    if (e.target.matches(".priece-btn")) {
        console.log("clicked", e.target.dataset.sku); // eventually we will call the API here
        
    }
});

document.querySelector("#search").addEventListener("input", (e) => {
    // Grab the string value in the search bar
    const search = e.target.value.trim().toLowerCase();

    // Allowing for search by name or sku using a filter.
    renderCards(catalogItems.filter(item => 
        item.name.toLowerCase().includes(search) || item.sku.toLowerCase().includes(search)
    ));
});

document.addEventListener("DOMContentLoaded", () => {
    renderCards(catalogItems);
});