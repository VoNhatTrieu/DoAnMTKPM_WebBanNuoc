/**
 * DESIGN PATTERNS IMPLEMENTATION IN JAVASCRIPT
 * Hệ thống bán đồ uống Sip & Savor
 */

// ========================================
// SINGLETON PATTERN - Cart Manager
// ========================================
const CartManager = (function() {
    let instance;

    function createInstance() {
        return {
            cart: [],
            
            getCart() {
                if (this.cart.length === 0) {
                    this.cart = JSON.parse(localStorage.getItem('cart') || '[]');
                }
                return this.cart;
            },

            addItem(item) {
                const existingIndex = this.cart.findIndex(i => 
                    i.id === item.id && 
                    i.size === item.size && 
                    i.sugar === item.sugar && 
                    i.ice === item.ice && 
                    JSON.stringify(i.toppings) === JSON.stringify(item.toppings)
                );

                if (existingIndex > -1) {
                    this.cart[existingIndex].quantity += item.quantity;
                } else {
                    this.cart.push(item);
                }

                this.saveCart();
                this.notifyObservers('itemAdded', item);
            },

            removeItem(index) {
                const item = this.cart[index];
                this.cart.splice(index, 1);
                this.saveCart();
                this.notifyObservers('itemRemoved', item);
            },

            updateQuantity(index, quantity) {
                if (quantity > 0) {
                    this.cart[index].quantity = quantity;
                    this.saveCart();
                    this.notifyObservers('quantityUpdated', this.cart[index]);
                }
            },

            clear() {
                this.cart = [];
                this.saveCart();
                this.notifyObservers('cartCleared', null);
            },

            saveCart() {
                localStorage.setItem('cart', JSON.stringify(this.cart));
            },

            getTotalItems() {
                return this.cart.reduce((sum, item) => sum + item.quantity, 0);
            },

            getSubtotal() {
                return this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
            },

            // Observer Pattern - Thông báo khi cart thay đổi
            observers: [],

            subscribe(observer) {
                this.observers.push(observer);
            },

            notifyObservers(event, data) {
                this.observers.forEach(observer => observer(event, data));
            }
        };
    }

    return {
        getInstance() {
            if (!instance) {
                instance = createInstance();
            }
            return instance;
        }
    };
})();

// ========================================
// BUILDER PATTERN - Drink Builder
// ========================================
class DrinkBuilder {
    constructor(product) {
        this.product = product;
        this.size = { code: 'M', name: 'Medium', price: 5000 };
        this.sugar = '50%';
        this.ice = 'Vừa';
        this.toppings = [];
        this.quantity = 1;
    }

    withSize(size) {
        this.size = size;
        return this;
    }

    withSugar(level) {
        this.sugar = level;
        return this;
    }

    withIce(level) {
        this.ice = level;
        return this;
    }

    addTopping(topping) {
        this.toppings.push(topping);
        return this;
    }

    withQuantity(qty) {
        this.quantity = qty;
        return this;
    }

    calculatePrice() {
        let total = this.product.basePrice;
        total += this.size.price;
        
        this.toppings.forEach(topping => {
            total += topping.price;
        });
        
        return total * this.quantity;
    }

    build() {
        return {
            id: this.product.id,
            name: this.product.name,
            image: this.product.image,
            price: this.calculatePrice() / this.quantity, // Unit price
            quantity: this.quantity,
            size: this.size.code,
            sugar: this.sugar,
            ice: this.ice,
            toppings: this.toppings.map(t => t.name)
        };
    }
}

// ========================================
// STRATEGY PATTERN - Pricing Strategy
// ========================================
class PricingStrategy {
    calculatePrice(basePrice, options) {
        throw new Error('Must implement calculatePrice method');
    }
}

class StandardPricing extends PricingStrategy {
    calculatePrice(basePrice, options) {
        let total = basePrice;
        total += options.size?.price || 0;
        
        if (options.toppings) {
            options.toppings.forEach(topping => {
                total += topping.price;
            });
        }
        
        return total * (options.quantity || 1);
    }
}

class PromotionalPricing extends PricingStrategy {
    constructor(discountPercent) {
        super();
        this.discountPercent = discountPercent;
    }

    calculatePrice(basePrice, options) {
        const standardPricing = new StandardPricing();
        const standardPrice = standardPricing.calculatePrice(basePrice, options);
        const discount = standardPrice * this.discountPercent / 100;
        return standardPrice - discount;
    }
}

class HappyHourPricing extends PricingStrategy {
    calculatePrice(basePrice, options) {
        const hour = new Date().getHours();
        const isHappyHour = hour >= 14 && hour <= 16; // 2PM - 4PM
        
        const standardPricing = new StandardPricing();
        const standardPrice = standardPricing.calculatePrice(basePrice, options);
        
        return isHappyHour ? standardPrice * 0.8 : standardPrice; // 20% off
    }
}

// ========================================
// STRATEGY PATTERN - Voucher Strategy
// ========================================
class VoucherStrategy {
    applyDiscount(subtotal) {
        throw new Error('Must implement applyDiscount method');
    }

    isValid(subtotal) {
        return true;
    }

    getMessage() {
        return '';
    }
}

class PercentageVoucher extends VoucherStrategy {
    constructor(percent, minAmount = 0) {
        super();
        this.percent = percent;
        this.minAmount = minAmount;
    }

    applyDiscount(subtotal) {
        if (!this.isValid(subtotal)) return 0;
        return Math.round(subtotal * this.percent / 100);
    }

    isValid(subtotal) {
        return subtotal >= this.minAmount;
    }

    getMessage() {
        return this.minAmount > 0 
            ? `Giảm ${this.percent}% cho đơn từ ${this.minAmount.toLocaleString('vi-VN')}₫`
            : `Giảm ${this.percent}%`;
    }
}

class FixedAmountVoucher extends VoucherStrategy {
    constructor(amount, minAmount = 0) {
        super();
        this.amount = amount;
        this.minAmount = minAmount;
    }

    applyDiscount(subtotal) {
        if (!this.isValid(subtotal)) return 0;
        return Math.min(this.amount, subtotal);
    }

    isValid(subtotal) {
        return subtotal >= this.minAmount;
    }

    getMessage() {
        return `Giảm ${this.amount.toLocaleString('vi-VN')}₫ cho đơn từ ${this.minAmount.toLocaleString('vi-VN')}₫`;
    }
}

// ========================================
// FACTORY PATTERN - Voucher Factory
// ========================================
class VoucherFactory {
    static vouchers = {
        'GIAM10': new PercentageVoucher(10, 0),
        'GIAM15': new PercentageVoucher(15, 100000),
        'GIAM20': new PercentageVoucher(20, 200000),
        'NEWUSER': new FixedAmountVoucher(30000, 0),
        'VIP50': new FixedAmountVoucher(50000, 150000)
    };

    static getVoucher(code) {
        const upperCode = code.toUpperCase().trim();
        return this.vouchers[upperCode] || null;
    }

    static isValid(code) {
        return code && this.vouchers.hasOwnProperty(code.toUpperCase().trim());
    }
}

// ========================================
// FACTORY PATTERN - Payment Factory
// ========================================
class PaymentProcessor {
    processPayment(order) {
        throw new Error('Must implement processPayment method');
    }

    getPaymentUrl(order) {
        return null;
    }
}

class CODPayment extends PaymentProcessor {
    processPayment(order) {
        return {
            success: true,
            message: 'Đơn hàng sẽ được thanh toán khi nhận hàng',
            transactionId: `COD-${Date.now()}`
        };
    }
}

class MoMoPayment extends PaymentProcessor {
    processPayment(order) {
        return {
            success: true,
            message: 'Đang chuyển hướng đến MoMo...',
            transactionId: `MOMO-${Date.now()}`,
            redirectUrl: this.getPaymentUrl(order)
        };
    }

    getPaymentUrl(order) {
        return `https://test-payment.momo.vn/pay?orderId=${order.orderNumber}&amount=${order.total}`;
    }
}

class BankingPayment extends PaymentProcessor {
    processPayment(order) {
        return {
            success: true,
            message: 'Đang chuyển hướng đến ngân hàng...',
            transactionId: `BANK-${Date.now()}`,
            redirectUrl: this.getPaymentUrl(order)
        };
    }

    getPaymentUrl(order) {
        return `https://payment.bank.vn/checkout?order=${order.orderNumber}`;
    }
}

class PaymentFactory {
    static createProcessor(method) {
        switch(method) {
            case 'COD':
                return new CODPayment();
            case 'MOMO':
                return new MoMoPayment();
            case 'BANKING':
                return new BankingPayment();
            default:
                throw new Error(`Unsupported payment method: ${method}`);
        }
    }
}

// ========================================
// OBSERVER PATTERN - Order Observer
// ========================================
class OrderSubject {
    constructor(order) {
        this.order = order;
        this.observers = [];
    }

    attach(observer) {
        this.observers.push(observer);
    }

    detach(observer) {
        const index = this.observers.indexOf(observer);
        if (index > -1) {
            this.observers.splice(index, 1);
        }
    }

    notify(event, data) {
        this.observers.forEach(observer => observer.update(event, data));
    }

    updateStatus(newStatus) {
        const oldStatus = this.order.status;
        this.order.status = newStatus;
        this.notify('statusChanged', { oldStatus, newStatus, order: this.order });
    }
}

class NotificationObserver {
    update(event, data) {
        console.log(`[Notification] ${event}:`, data);
        
        if (event === 'statusChanged') {
            this.showNotification(`Đơn hàng đã chuyển sang: ${data.newStatus}`);
        }
    }

    showNotification(message) {
        // Hiển thị toast notification
        console.log(`🔔 ${message}`);
    }
}

// ========================================
// USAGE EXAMPLES
// ========================================

// Singleton: Cart Manager
const cart = CartManager.getInstance();
cart.subscribe((event, data) => {
    console.log('Cart event:', event, data);
    updateCartCount();
});

// Builder: Tạo drink tùy chỉnh
function createCustomDrink() {
    const product = {
        id: 1,
        name: 'Trà Sữa Trân Châu',
        basePrice: 35000,
        image: 'image.jpg'
    };

    const drink = new DrinkBuilder(product)
        .withSize({ code: 'L', name: 'Large', price: 10000 })
        .withSugar('70%')
        .withIce('Ít đá')
        .addTopping({ name: 'Trân châu đen', price: 8000 })
        .addTopping({ name: 'Kem cheese', price: 10000 })
        .withQuantity(2)
        .build();

    cart.addItem(drink);
}

// Strategy: Áp dụng pricing
function calculateDrinkPrice(product, options, pricingType = 'standard') {
    let strategy;
    
    switch(pricingType) {
        case 'promotional':
            strategy = new PromotionalPricing(15); // 15% off
            break;
        case 'happyHour':
            strategy = new HappyHourPricing();
            break;
        default:
            strategy = new StandardPricing();
    }

    return strategy.calculatePrice(product.basePrice, options);
}

// Factory: Xử lý thanh toán
function processPayment(order, method) {
    const processor = PaymentFactory.createProcessor(method);
    const result = processor.processPayment(order);
    
    if (result.redirectUrl) {
        window.location.href = result.redirectUrl;
    }
    
    return result;
}

// Observer: Theo dõi đơn hàng
function trackOrder(order) {
    const orderSubject = new OrderSubject(order);
    const notificationObserver = new NotificationObserver();
    
    orderSubject.attach(notificationObserver);
    
    // Giả lập thay đổi trạng thái
    setTimeout(() => orderSubject.updateStatus('Đã xác nhận'), 2000);
    setTimeout(() => orderSubject.updateStatus('Đang pha chế'), 5000);
    setTimeout(() => orderSubject.updateStatus('Đang giao hàng'), 8000);
}

// Export để sử dụng
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        CartManager,
        DrinkBuilder,
        PricingStrategy,
        VoucherFactory,
        PaymentFactory,
        OrderSubject
    };
}
