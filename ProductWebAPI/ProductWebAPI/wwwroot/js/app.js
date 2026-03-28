const API_BASE = '/api/products';

// ─────────────────────────────────────────
// API FUNCTIONS
// ─────────────────────────────────────────

async function getAllProducts() {
    const res = await fetch(API_BASE);
    return await res.json();
}

async function getProductById(id) {
    const res = await fetch(`${API_BASE}/${id}`);
    if (!res.ok) return null;
    return await res.json();
}

async function createProduct(product) {
    const res = await fetch(API_BASE, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(product)
    });
    return res;
}

async function updateProduct(id, product) {
    const res = await fetch(`${API_BASE}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(product)
    });
    return res;
}

async function deleteProduct(id) {
    const res = await fetch(`${API_BASE}/${id}`, {
        method: 'DELETE'
    });
    return res;
}

// ─────────────────────────────────────────
// HELPER FUNCTIONS
// ─────────────────────────────────────────

function getQueryParam(param) {
    return new URLSearchParams(window.location.search).get(param);
}

function showAlert(elementId, message, type = 'success') {
    document.getElementById(elementId).innerHTML =
        `<div class="alert alert-${type}">${message}</div>`;
}

function showFieldError(elementId, message) {
    document.getElementById(elementId).textContent = message;
}

function clearErrors(...errorIds) {
    errorIds.forEach(id => {
        document.getElementById(id).textContent = '';
    });
}

function validate(name, price, category) {
    clearErrors('nameErr', 'priceErr', 'categoryErr');
    let valid = true;

    if (!name.trim()) {
        showFieldError('nameErr', 'Please enter product name');
        valid = false;
    }
    if (!price || isNaN(price) || price < 0.01 || price > 10000) {
        showFieldError('priceErr', 'Price must be between 0.01 and 10000');
        valid = false;
    }
    if (!category.trim()) {
        showFieldError('categoryErr', 'Please enter product category');
        valid = false;
    }
    return valid;
}

// ─────────────────────────────────────────
// PAGE: INDEX — Load Product List
// ─────────────────────────────────────────

async function loadIndex() {
    if (!document.getElementById('productTable')) return;

    // Show success message if redirected
    const msg = getQueryParam('msg');
    if (msg) showAlert('alert', decodeURIComponent(msg), 'success');

    const products = await getAllProducts();
    const tbody = document.getElementById('productTable');
    tbody.innerHTML = '';

    if (products.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" style="text-align:center;color:#888;">
                            No products found.</td></tr>`;
        return;
    }

    products.forEach(p => {
        tbody.innerHTML += `
            <tr>
                <td>${p.id}</td>
                <td>${p.name}</td>
                <td>₹${parseFloat(p.price).toFixed(2)}</td>
                <td>${p.category}</td>
                <td class="actions">
                    <a href="/details.html?id=${p.id}" class="btn btn-primary">Details</a>
                    <a href="/edit.html?id=${p.id}"    class="btn btn-warning">Edit</a>
                    <a href="/delete.html?id=${p.id}"  class="btn btn-danger">Delete</a>
                </td>
            </tr>`;
    });
}

// ─────────────────────────────────────────
// PAGE: CREATE — Handle Create Form
// ─────────────────────────────────────────

async function loadCreate() {
    const form = document.getElementById('createForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const name = document.getElementById('name').value;
        const price = parseFloat(document.getElementById('price').value);
        const category = document.getElementById('category').value;

        if (!validate(name, price, category)) return;

        const res = await createProduct({ name, price, category });

        if (res.ok) {
            window.location.href = '/index.html?msg=' +
                encodeURIComponent('Product created successfully!');
        } else {
            showAlert('alert', 'Failed to create product. Please try again.', 'danger');
        }
    });
}

// ─────────────────────────────────────────
// PAGE: EDIT — Load Product + Handle Update
// ─────────────────────────────────────────

async function loadEdit() {
    const form = document.getElementById('editForm');
    if (!form) return;

    const id = getQueryParam('id');
    const product = await getProductById(id);

    if (!product) {
        window.location.href = '/index.html';
        return;
    }

    // Pre-fill form fields
    document.getElementById('name').value = product.name;
    document.getElementById('price').value = product.price;
    document.getElementById('category').value = product.category;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const name = document.getElementById('name').value;
        const price = parseFloat(document.getElementById('price').value);
        const category = document.getElementById('category').value;

        if (!validate(name, price, category)) return;

        const res = await updateProduct(id, { id: parseInt(id), name, price, category });

        if (res.ok) {
            window.location.href = '/index.html?msg=' +
                encodeURIComponent('Product updated successfully!');
        } else {
            showAlert('alert', 'Failed to update product. Please try again.', 'danger');
        }
    });
}

// ─────────────────────────────────────────
// PAGE: DETAILS — Load Single Product
// ─────────────────────────────────────────

async function loadDetails() {
    const details = document.getElementById('details');
    if (!details) return;

    const id = getQueryParam('id');
    const product = await getProductById(id);

    if (!product) {
        window.location.href = '/index.html';
        return;
    }

    details.innerHTML = `
        <dt>ID</dt>       <dd>${product.id}</dd>
        <dt>Name</dt>     <dd>${product.name}</dd>
        <dt>Price</dt>    <dd>₹${parseFloat(product.price).toFixed(2)}</dd>
        <dt>Category</dt> <dd>${product.category}</dd>
    `;

    document.getElementById('actions').innerHTML = `
        <a href="/edit.html?id=${product.id}"  class="btn btn-warning">Edit</a>
        <a href="/index.html"                  class="btn btn-secondary"
           style="margin-left:8px;">Back to List</a>
    `;
}

// ─────────────────────────────────────────
// PAGE: DELETE — Load Product + Confirm
// ─────────────────────────────────────────

async function loadDelete() {
    const details = document.getElementById('details');
    if (!details) return;

    const id = getQueryParam('id');
    const product = await getProductById(id);

    if (!product) {
        window.location.href = '/index.html';
        return;
    }

    details.innerHTML = `
        <dt>Name</dt>     <dd>${product.name}</dd>
        <dt>Price</dt>    <dd>₹${parseFloat(product.price).toFixed(2)}</dd>
        <dt>Category</dt> <dd>${product.category}</dd>
    `;

    document.getElementById('confirmBtn').addEventListener('click', async () => {
        const res = await deleteProduct(id);
        if (res.ok) {
            window.location.href = '/index.html?msg=' +
                encodeURIComponent('Product deleted successfully!');
        } else {
            showAlert('alert', 'Failed to delete product.', 'danger');
        }
    });
}

// ─────────────────────────────────────────
// AUTO-RUN — Detects which page is loaded
// ─────────────────────────────────────────

document.addEventListener('DOMContentLoaded', () => {
    loadIndex();
    loadCreate();
    loadEdit();
    loadDetails();
    loadDelete();
});