// Admin Products Management - Realtime Data từ API
$(document).ready(function () {
    loadProducts();
});

// Load danh sách sản phẩm
async function loadProducts() {
    try {
        const response = await adminAPI.get('/admin/products');
        if (response.success && response.data) {
            renderProductsTable(response.data);
        } else {
            showEmptyState();
        }
    } catch (error) {
        console.error('Lỗi khi tải sản phẩm:', error);
        showErrorState('Không thể tải dữ liệu sản phẩm. Vui lòng thử lại.');
    }
}

// Render bảng sản phẩm
function renderProductsTable(products) {
    const tbody = $('#productsTableBody');
    
    if (!products || products.length === 0) {
        showEmptyState();
        return;
    }

    const noImageSvg = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="60" height="60"%3E%3Crect fill="%23ddd" width="60" height="60"/%3E%3Ctext fill="%23999" x="50%25" y="50%25" dominant-baseline="middle" text-anchor="middle" font-family="Arial" font-size="10"%3ENo Image%3C/text%3E%3C/svg%3E';
    
    let html = '';
    products.forEach(product => {
        const statusBadge = getStatusBadge(product.isActive);
        const categoryBadge = getCategoryBadge(product.categoryName);
        
        html += `
            <tr data-product-id="${product.productId}">
                <td>
                    <img src="${product.imageUrl || noImageSvg}" 
                         alt="${product.name}" 
                         class="product-image"
                         onerror="this.src='${noImageSvg}'">
                </td>
                <td>
                    <div class="fw-bold">${escapeHtml(product.name)}</div>
                    <small class="text-muted">${escapeHtml(product.description || '')}</small>
                </td>
                <td>${categoryBadge}</td>
                <td>
                    <div class="fw-bold">${formatCurrency(product.basePrice)}</div>
                    <small class="text-muted">Giá cơ bản</small>
                </td>
                <td>${statusBadge}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-info" onclick="viewProduct(${product.productId})" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-warning" onclick="editProduct(${product.productId})" title="Chỉnh sửa">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-danger" onclick="deleteProduct(${product.productId})" title="Xóa">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    });

    tbody.html(html);
}

// Get status badge
function getStatusBadge(isAvailable) {
    if (isAvailable) {
        return '<span class="badge bg-success">Còn bán</span>';
    } else {
        return '<span class="badge bg-danger">Hết hàng</span>';
    }
}

// Get category badge
function getCategoryBadge(categoryName) {
    const colors = {
        'Trà Sữa': 'primary',
        'Trà': 'success',
        'Cà Phê': 'warning',
        'Sinh Tố': 'info',
        'Nước Ép': 'danger'
    };
    const color = colors[categoryName] || 'secondary';
    return `<span class="badge bg-${color}">${escapeHtml(categoryName || 'Khác')}</span>`;
}

// Show empty state
function showEmptyState() {
    $('#productsTableBody').html(`
        <tr>
            <td colspan="6" class="text-center py-5">
                <i class="fas fa-box-open fa-3x text-muted mb-3"></i>
                <p class="text-muted">Chưa có sản phẩm nào</p>
                <button class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#productModal" onclick="openAddModal()">
                    <i class="fas fa-plus"></i> Thêm sản phẩm đầu tiên
                </button>
            </td>
        </tr>
    `);
}

// Show error state
function showErrorState(message) {
    $('#productsTableBody').html(`
        <tr>
            <td colspan="6" class="text-center py-5">
                <i class="fas fa-exclamation-triangle fa-3x text-warning mb-3"></i>
                <p class="text-muted">${message}</p>
                <button class="btn btn-primary" onclick="loadProducts()">
                    <i class="fas fa-redo"></i> Thử lại
                </button>
            </td>
        </tr>
    `);
}

// Open add modal
function openAddModal() {
    $('#productModalLabel').text('Thêm sản phẩm mới');
    $('#productForm')[0].reset();
    $('#productId').val('');
    uploadedImageBase64 = '';
    $('#imagePreview').html('');
    $('#productModal').modal('show');
}

// View product details
function viewProduct(id) {
    // TODO: Implement view product details modal
    adminAPI.get(`/admin/products/${id}`)
        .then(response => {
            if (response.success) {
                alert('Chi tiết sản phẩm: ' + JSON.stringify(response.data, null, 2));
            }
        })
        .catch(error => {
            adminAPI.showToast('error', 'Không thể tải thông tin sản phẩm');
        });
}

// Edit product
async function editProduct(id) {
    try {
        const response = await adminAPI.get(`/admin/products/${id}`);
        if (response.success && response.data) {
            const product = response.data;
            
            $('#productModalLabel').text('Chỉnh sửa sản phẩm');
            $('#productId').val(product.productId);
            $('#productName').val(product.name);
            $('#productCategory').val(product.categoryId);
            $('#productDescription').val(product.description);
            $('#productStatus').val(product.isActive ? 'available' : 'out-of-stock');
            
            // Set uploaded image for editing
            uploadedImageBase64 = product.imageUrl;
            if (product.imageUrl) {
                $('#imagePreview').html(`<img src="${product.imageUrl}" class="img-fluid rounded" style="max-height: 200px;" onerror="this.src='${noImageSvg}'">`);
            }
            
            $('#productModal').modal('show');
        }
    } catch (error) {
        adminAPI.showToast('error', 'Không thể tải thông tin sản phẩm');
    }
}

// Delete product
async function deleteProduct(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) {
        return;
    }

    try {
        const response = await adminAPI.delete(`/admin/products/${id}`);
        if (response.success) {
            adminAPI.showToast('success', 'Đã xóa sản phẩm thành công');
            loadProducts(); // Reload table
        } else {
            adminAPI.showToast('error', response.message || 'Không thể xóa sản phẩm');
        }
    } catch (error) {
        console.error('Lỗi khi xóa sản phẩm:', error);
        adminAPI.showToast('error', 'Lỗi khi xóa sản phẩm');
    }
}

// Save product (create or update)
async function saveProduct() {
    const productId = $('#productId').val();
    const categorySlug = $('#productCategory').val();
    
    // Map slug to categoryId
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
    
    // Use uploaded image or default SVG
    let imageUrl = uploadedImageBase64 || 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="400" height="400"%3E%3Crect fill="%23f0f0f0" width="400" height="400"/%3E%3Ctext fill="%23999" x="50%25" y="50%25" dominant-baseline="middle" text-anchor="middle" font-family="Arial" font-size="24"%3EProduct Image%3C/text%3E%3C/svg%3E';
    
    const productData = {
        name: $('#productName').val(),
        categoryId: categoryMap[categorySlug] || 1,
        description: $('#productDescription').val(),
        basePrice: parseFloat($('#priceM').val()) || parseFloat($('#priceS').val()) || 0,
        imageUrl: imageUrl,
        isAvailable: $('#productStatus').val() === 'available'
    };

    // Validation
    if (!productData.name || !productData.categoryId) {
        adminAPI.showToast('error', 'Vui lòng điền đầy đủ thông tin bắt buộc');
        return;
    }

    try {
        let response;
        if (productId) {
            // Update
            response = await adminAPI.put(`/admin/products/${productId}`, productData);
        } else {
            // Create
            response = await adminAPI.post('/admin/products', productData);
        }

        if (response.success) {
            adminAPI.showToast('success', productId ? 'Cập nhật sản phẩm thành công' : 'Thêm sản phẩm thành công');
            $('#productModal').modal('hide');
            loadProducts(); // Reload table
        } else {
            adminAPI.showToast('error', response.message || 'Không thể lưu sản phẩm');
        }
    } catch (error) {
        console.error('Lỗi khi lưu sản phẩm:', error);
        adminAPI.showToast('error', 'Lỗi khi lưu sản phẩm');
    }
}

// Filter products
function filterProducts() {
    const category = $('#filterCategory').val();
    const status = $('#filterStatus').val();
    const search = $('#searchProduct').val().toLowerCase();

    // Get all rows
    const rows = $('#productsTableBody tr');
    
    rows.each(function() {
        const row = $(this);
        const productName = row.find('td:eq(1) .fw-bold').text().toLowerCase();
        const productCategory = row.find('td:eq(2)').text();
        const productStatus = row.find('td:eq(4) .badge').text();
        
        let show = true;
        
        if (category && !productCategory.includes(category)) {
            show = false;
        }
        
        if (status) {
            if (status === 'available' && !productStatus.includes('Còn bán')) {
                show = false;
            }
            if (status === 'out-of-stock' && !productStatus.includes('Hết hàng')) {
                show = false;
            }
        }
        
        if (search && !productName.includes(search)) {
            show = false;
        }
        
        row.toggle(show);
    });
}

// Add topping to form
function addTopping() {
    const toppingHtml = `
        <div class="topping-item">
            <input type="checkbox" class="form-check-input" checked>
            <input type="text" class="form-control form-control-sm" placeholder="Tên topping">
            <input type="number" class="form-control form-control-sm" placeholder="Giá">
            <button type="button" class="btn btn-sm btn-danger" onclick="this.parentElement.remove()">
                <i class="fas fa-trash"></i>
            </button>
        </div>
    `;
    $('#toppingList').append(toppingHtml);
}

// Image upload and preview
let uploadedImageBase64 = '';
$('#productImage').change(function(e) {
    const file = e.target.files[0];
    if (file) {
        // Check file size (max 5MB)
        if (file.size > 5 * 1024 * 1024) {
            adminAPI.showToast('error', 'Kích thước ảnh không được vượt quá 5MB');
            $(this).val('');
            return;
        }
        
        const reader = new FileReader();
        reader.onload = function(e) {
            uploadedImageBase64 = e.target.result;
            $('#imagePreview').html(`<img src="${uploadedImageBase64}" class="img-fluid rounded" style="max-height: 200px;">`);
        };
        reader.readAsDataURL(file);
    } else {
        uploadedImageBase64 = '';
        $('#imagePreview').html('');
    }
});

// Escape HTML to prevent XSS
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
