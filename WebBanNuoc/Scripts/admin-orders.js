// Orders Page - Realtime Data
let currentPage = 1;
const pageSize = 20;
let currentStatus = '';

// Load orders from API
async function loadOrders(page = 1, status = '') {
    try {
        currentPage = page;
        currentStatus = status;
        
        const response = await apiHelper.get(`/Admin/orders?page=${page}&pageSize=${pageSize}&status=${status}`);
        
        if (response.success) {
            const data = response.data;
            displayOrders(data.items);
            updatePagination(data.totalPages, page);
        }
    } catch (error) {
        console.error('Error loading orders:', error);
        showToast('Không thể tải danh sách đơn hàng', 'danger');
    }
}

// Display orders in table
function displayOrders(orders) {
    const tbody = document.getElementById('ordersTableBody');
    
    if (!tbody) return;
    
    if (orders.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-center">Không có đơn hàng nào</td></tr>';
        return;
    }
    
    tbody.innerHTML = orders.map(order => `
        <tr>
            <td><strong>#${order.orderId}</strong></td>
            <td>${order.customerName}</td>
            <td>${order.customerPhone || 'N/A'}</td>
            <td><strong>${formatCurrency(order.totalAmount)}</strong></td>
            <td>
                <span class="order-status status-${order.status.toLowerCase()}">
                    ${getStatusText(order.status)}
                </span>
            </td>
            <td>${formatDate(order.createdAt)}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-info" onclick="viewOrderDetail(${order.orderId})" title="Xem chi tiết">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn btn-success" onclick="updateOrderStatus(${order.orderId}, 'Completed')" title="Hoàn thành">
                        <i class="fas fa-check"></i>
                    </button>
                    <button class="btn btn-danger" onclick="updateOrderStatus(${order.orderId}, 'Cancelled')" title="Hủy">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// Update pagination
function updatePagination(totalPages, currentPage) {
    const pagination = document.getElementById('ordersPagination');
    if (!pagination) return;
    
    let html = '';
    
    // Previous button
    html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="loadOrders(${currentPage - 1}, '${currentStatus}'); return false;">«</a>
    </li>`;
    
    // Page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="loadOrders(${i}, '${currentStatus}'); return false;">${i}</a>
            </li>`;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }
    
    // Next button
    html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="loadOrders(${currentPage + 1}, '${currentStatus}'); return false;">»</a>
    </li>`;
    
    pagination.innerHTML = html;
}

// View order detail
async function viewOrderDetail(orderId) {
    try {
        const response = await apiHelper.get(`/Admin/orders/${orderId}`);
        
        if (response.success) {
            const order = response.data;
            showOrderDetailModal(order);
        }
    } catch (error) {
        console.error('Error loading order detail:', error);
        showToast('Không thể tải chi tiết đơn hàng', 'danger');
    }
}

// Show order detail modal
function showOrderDetailModal(order) {
    const modal = document.getElementById('orderDetailModal');
    if (!modal) return;
    
    // Update modal content
    document.getElementById('modalOrderId').textContent = `#${order.orderId}`;
    document.getElementById('modalCustomerName').textContent = order.customerName;
    document.getElementById('modalCustomerEmail').textContent = order.customerEmail;
    document.getElementById('modalCustomerPhone').textContent = order.customerPhone;
    document.getElementById('modalDeliveryAddress').textContent = order.deliveryAddress;
    document.getElementById('modalOrderDate').textContent = formatDate(order.createdAt);
    document.getElementById('modalOrderStatus').innerHTML = `
        <span class="order-status status-${order.status.toLowerCase()}">
            ${getStatusText(order.status)}
        </span>
    `;
    
    // Update order items
    const itemsContainer = document.getElementById('modalOrderItems');
    itemsContainer.innerHTML = order.items.map(item => `
        <div class="order-item">
            <div class="d-flex justify-content-between align-items-start mb-2">
                <div>
                    <h6 class="mb-1">${item.productName}</h6>
                    <small class="text-muted">
                        ${item.size ? `Size: ${item.size}` : ''}
                        ${item.toppings ? ` | Topping: ${item.toppings}` : ''}
                    </small>
                </div>
                <span class="badge bg-primary">x${item.quantity}</span>
            </div>
            <div class="d-flex justify-content-between">
                <span>Đơn giá: ${formatCurrency(item.unitPrice)}</span>
                <strong>Thành tiền: ${formatCurrency(item.unitPrice * item.quantity)}</strong>
            </div>
        </div>
    `).join('');
    
    // Update total
    document.getElementById('modalTotalAmount').textContent = formatCurrency(order.totalAmount);
    
    // Show modal
    const bsModal = new bootstrap.Modal(modal);
    bsModal.show();
}

// Update order status
async function updateOrderStatus(orderId, newStatus) {
    if (!confirm(`Bạn có chắc muốn ${newStatus === 'Completed' ? 'hoàn thành' : 'hủy'} đơn hàng này?`)) {
        return;
    }
    
    try {
        const response = await apiHelper.put(`/Admin/orders/${orderId}/status`, { status: newStatus });
        
        if (response.success) {
            showToast('Cập nhật trạng thái thành công', 'success');
            loadOrders(currentPage, currentStatus);
        }
    } catch (error) {
        console.error('Error updating order status:', error);
        showToast('Không thể cập nhật trạng thái', 'danger');
    }
}

// Filter orders
function filterOrders() {
    const status = document.getElementById('filterStatus').value;
    currentStatus = status;
    loadOrders(1, status);
}

// Search orders
function searchOrders() {
    const searchTerm = document.getElementById('searchInput').value;
    // Implement search logic
    console.log('Searching for:', searchTerm);
}

// Export orders to Excel
function exportOrders() {
    showToast('Chức năng xuất Excel đang được phát triển', 'info');
}

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    // Load initial data
    loadOrders();
    
    // Setup filter listeners
    const filterStatus = document.getElementById('filterStatus');
    if (filterStatus) {
        filterStatus.addEventListener('change', filterOrders);
    }
    
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('keyup', function(e) {
            if (e.key === 'Enter') {
                searchOrders();
            }
        });
    }
    
    // Auto refresh every 30 seconds
    setInterval(() => {
        loadOrders(currentPage, currentStatus);
    }, 30000);
});
