// Products API Integration
(function() {
    const PRODUCTS_API_BASE = 'http://localhost:5299/api';

// Default no-image SVG
const noImageSvg = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="400" height="300"%3E%3Crect fill="%23f0f0f0" width="400" height="300"/%3E%3Ctext fill="%23999" x="50%25" y="50%25" dominant-baseline="middle" text-anchor="middle" font-family="Arial" font-size="20"%3ENo Image%3C/text%3E%3C/svg%3E';

// Load products from API based on category
async function loadProductsFromAPI(categorySlug = '') {
    try {
        let endpoint = '/products';
        
        // Map category slug to categoryId if needed
        if (categorySlug) {
            const categoryMap = {
                'tra-sua': 1,
                'tra': 2,
                'trai-cay': 3,
                'ca-phe': 4,
                'sinh-to': 5,
                'nuoc-ep': 6,
                'soda': 7,
                'banh-ngot': 8
            };
            
            const categoryId = categoryMap[categorySlug];
            if (categoryId) {
                endpoint = `/products/category/${categoryId}`;
            }
        }

        const response = await fetch(`${PRODUCTS_API_BASE}${endpoint}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();
        
        if (result.success && result.data) {
            renderProducts(result.data);
        } else {
            showNoProducts();
        }
    } catch (error) {
        console.error('Lỗi khi tải sản phẩm:', error);
        showErrorMessage('Không thể tải danh sách sản phẩm. Vui lòng thử lại sau.');
    }
}

// Render products to grid
function renderProducts(products) {
    const container = $('#productsContainer');
    
    if (!products || products.length === 0) {
        showNoProducts();
        $('#productCount').text('0');
        return;
    }

    // Update product count
    $('#productCount').text(products.length);

    let html = '<div class="row g-4">';
    
    products.forEach(product => {
        html += `
            <div class="col-lg-4 col-md-6 col-sm-12">
                <div class="product-card h-100" data-product-id="${product.id}">
                    <div class="product-image">
                        <img src="${product.imageUrl || noImageSvg}" 
                             alt="${escapeHtml(product.name)}"
                             onerror="this.src='${noImageSvg}'">
                        ${!product.isAvailable ? '<span class="badge bg-danger position-absolute top-0 end-0 m-2">Hết hàng</span>' : ''}
                    </div>
                    <div class="product-info p-3">
                        <span class="badge bg-primary mb-2">${escapeHtml(product.categoryName || 'Đồ uống')}</span>
                        <h5 class="product-name">${escapeHtml(product.name)}</h5>
                        <p class="product-description text-muted small">
                            ${escapeHtml(product.description || 'Đồ uống thơm ngon, hấp dẫn')}
                        </p>
                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <span class="product-price fw-bold text-primary fs-5">
                                ${formatCurrency(product.basePrice)}
                            </span>
                            ${product.isAvailable ? 
                                `<a href="/Products/ProductDetail/${product.id}" class="btn btn-sm btn-primary">
                                    <i class="fas fa-shopping-cart"></i> Đặt hàng
                                </a>` : 
                                `<button class="btn btn-sm btn-secondary" disabled>Hết hàng</button>`
                            }
                        </div>
                    </div>
                </div>
            </div>
        `;
    });
    
    html += '</div>';
    container.html(html);
}

// Show empty state
function showNoProducts() {
    $('#productsContainer').html(`
        <div class="text-center py-5">
            <i class="fas fa-box-open fa-4x text-muted mb-3"></i>
            <h4>Không có sản phẩm nào</h4>
            <p class="text-muted">Hiện tại chưa có sản phẩm trong danh mục này.</p>
        </div>
    `);
}

// Show error message
function showErrorMessage(message) {
    $('#productsContainer').html(`
        <div class="alert alert-danger text-center" role="alert">
            <i class="fas fa-exclamation-triangle me-2"></i>${message}
        </div>
    `);
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { 
        style: 'currency', 
        currency: 'VND' 
    }).format(amount);
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Filter by price
function filterByPrice(minPrice, maxPrice) {
    const products = $('#productsContainer .product-card');
    
    products.each(function() {
        const priceText = $(this).find('.product-price').text();
        const price = parseInt(priceText.replace(/[^\d]/g, ''));
        
        if (price >= minPrice && price <= maxPrice) {
            $(this).parent().show();
        } else {
            $(this).parent().hide();
        }
    });
}

// Sort products
function sortProducts(sortBy) {
    const container = $('#productsContainer .row');
    const products = container.children('.col-lg-4, .col-md-6').get();
    
    products.sort(function(a, b) {
        const priceA = parseInt($(a).find('.product-price').text().replace(/[^\d]/g, ''));
        const priceB = parseInt($(b).find('.product-price').text().replace(/[^\d]/g, ''));
        
        if (sortBy === 'price-asc') {
            return priceA - priceB;
        } else if (sortBy === 'price-desc') {
            return priceB - priceA;
        } else if (sortBy === 'name-asc') {
            const nameA = $(a).find('.product-name').text();
            const nameB = $(b).find('.product-name').text();
            return nameA.localeCompare(nameB, 'vi');
        } else if (sortBy === 'name-desc') {
            const nameA = $(a).find('.product-name').text();
            const nameB = $(b).find('.product-name').text();
            return nameB.localeCompare(nameA, 'vi');
        }
        return 0;
    });
    
    $.each(products, function(idx, item) {
        container.append(item);
    });
}

// Make functions globally accessible
window.loadProductsFromAPI = loadProductsFromAPI;
window.filterByPrice = filterByPrice;
window.sortProducts = sortProducts;

// Initialize on page load
$(document).ready(function() {
    // Get category from URL or data attribute
    const category = $('#productsContainer').data('category') || '';
    loadProductsFromAPI(category);
    
    // Handle sort dropdown
    $('#sortSelect').on('change', function() {
        sortProducts($(this).val());
    });
});

})(); // End IIFE
