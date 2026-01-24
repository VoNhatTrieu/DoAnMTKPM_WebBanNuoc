// Dashboard realtime data
let revenueChart = null;
let topProductsChart = null;

// Load dashboard stats
async function loadDashboardStats(period = 'today') {
    try {
        const response = await apiHelper.get(`/Admin/dashboard/stats?period=${period}`);
        
        if (response.success) {
            const stats = response.data;
            
            // Update total orders
            document.getElementById('totalOrders').textContent = stats.totalOrders;
            
            // Update revenue
            document.getElementById('totalRevenue').textContent = formatCurrency(stats.revenue);
            
            // Update customers
            document.getElementById('totalCustomers').textContent = stats.totalCustomers;
            document.getElementById('newCustomers').textContent = `+${stats.newCustomers} mới`;
            
            // Update products
            document.getElementById('totalProducts').textContent = stats.totalProducts;
        }
    } catch (error) {
        console.error('Error loading dashboard stats:', error);
        showToast('Không thể tải thống kê dashboard', 'danger');
    }
}

// Load revenue chart
async function loadRevenueChart() {
    try {
        const response = await apiHelper.get('/Admin/dashboard/revenue-chart');
        
        if (response.success) {
            const revenueData = response.data;
            const labels = revenueData.map(d => d.date);
            const data = revenueData.map(d => d.revenue);
            
            const ctx = document.getElementById('revenueChart');
            if (ctx) {
                if (revenueChart) {
                    revenueChart.destroy();
                }
                
                revenueChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Doanh thu (VND)',
                            data: data,
                            borderColor: '#4e73df',
                            backgroundColor: 'rgba(78, 115, 223, 0.05)',
                            borderWidth: 3,
                            fill: true,
                            tension: 0.4
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: false
                            },
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        return 'Doanh thu: ' + formatCurrency(context.parsed.y);
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function(value) {
                                        return (value / 1000000).toFixed(0) + 'M';
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    } catch (error) {
        console.error('Error loading revenue chart:', error);
    }
}

// Load top products chart
async function loadTopProductsChart() {
    try {
        const response = await apiHelper.get('/Admin/dashboard/top-products?limit=5');
        
        if (response.success) {
            const products = response.data;
            const labels = products.map(p => p.productName);
            const data = products.map(p => p.totalQuantity);
            
            const ctx = document.getElementById('topProductsChart');
            if (ctx) {
                if (topProductsChart) {
                    topProductsChart.destroy();
                }
                
                topProductsChart = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: labels,
                        datasets: [{
                            data: data,
                            backgroundColor: [
                                '#4e73df',
                                '#1cc88a',
                                '#36b9cc',
                                '#f6c23e',
                                '#e74a3b'
                            ],
                            borderWidth: 2,
                            borderColor: '#fff'
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }
                });
            }
        }
    } catch (error) {
        console.error('Error loading top products chart:', error);
    }
}

// Load recent orders
async function loadRecentOrders() {
    try {
        const response = await apiHelper.get('/Admin/orders/recent?limit=5');
        
        if (response.success) {
            const orders = response.data;
            const tbody = document.getElementById('recentOrdersTable');
            
            if (tbody) {
                tbody.innerHTML = orders.map(order => `
                    <tr>
                        <td>#${order.orderId}</td>
                        <td>${order.customerName}</td>
                        <td>${formatCurrency(order.totalAmount)}</td>
                        <td><span class="badge bg-${getStatusBadgeClass(order.status)}">${getStatusText(order.status)}</span></td>
                        <td>${formatDate(order.createdAt)}</td>
                    </tr>
                `).join('');
            }
        }
    } catch (error) {
        console.error('Error loading recent orders:', error);
    }
}

// Change period filter
function loadStats(period) {
    // Update button states
    document.querySelectorAll('.btn-group button').forEach(btn => {
        btn.classList.remove('active');
    });
    event.target.classList.add('active');
    
    // Reload stats
    loadDashboardStats(period);
}

// Initialize dashboard
document.addEventListener('DOMContentLoaded', function() {
    // Load all dashboard data
    loadDashboardStats();
    loadRevenueChart();
    loadTopProductsChart();
    loadRecentOrders();
    
    // Auto refresh every 60 seconds
    setInterval(() => {
        loadDashboardStats();
        loadRecentOrders();
    }, 60000);
});
