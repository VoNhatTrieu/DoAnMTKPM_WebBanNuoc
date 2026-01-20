using System;
using System.Collections.Generic;
using WebBanNuoc.Models.Entities;

namespace WebBanNuoc.Models.Strategies
{
    /// <summary>
    /// Strategy Pattern - Chiến lược tính giá
    /// Cho phép thay đổi cách tính giá linh hoạt
    /// </summary>
    public interface IPricingStrategy
    {
        decimal CalculatePrice(Product product, CustomizationOptions options);
    }

    /// <summary>
    /// Tùy chọn tùy chỉnh
    /// </summary>
    public class CustomizationOptions
    {
        public Size Size { get; set; }
        public List<Topping> Toppings { get; set; }
        public int Quantity { get; set; }

        public CustomizationOptions()
        {
            Toppings = new List<Topping>();
            Quantity = 1;
        }
    }

    /// <summary>
    /// Chiến lược tính giá tiêu chuẩn
    /// </summary>
    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal CalculatePrice(Product product, CustomizationOptions options)
        {
            decimal total = product.BasePrice;

            // Cộng giá size
            if (options.Size != null)
            {
                total += options.Size.AdditionalPrice;
            }

            // Cộng giá topping
            foreach (var topping in options.Toppings)
            {
                total += topping.Price;
            }

            // Nhân với số lượng
            total *= options.Quantity;

            return total;
        }
    }

    /// <summary>
    /// Chiến lược tính giá khuyến mãi (VIP, Happy Hour, etc.)
    /// </summary>
    public class PromotionalPricingStrategy : IPricingStrategy
    {
        private readonly decimal _discountPercent;

        public PromotionalPricingStrategy(decimal discountPercent)
        {
            _discountPercent = discountPercent;
        }

        public decimal CalculatePrice(Product product, CustomizationOptions options)
        {
            // Tính giá gốc
            var standardStrategy = new StandardPricingStrategy();
            decimal standardPrice = standardStrategy.CalculatePrice(product, options);

            // Áp dụng giảm giá
            decimal discount = standardPrice * _discountPercent / 100;
            return standardPrice - discount;
        }
    }

    /// <summary>
    /// Strategy Pattern - Chiến lược voucher
    /// </summary>
    public interface IVoucherStrategy
    {
        decimal ApplyDiscount(decimal subtotal);
        bool IsValid(decimal subtotal);
        string GetMessage();
    }

    /// <summary>
    /// Voucher giảm phần trăm
    /// </summary>
    public class PercentageVoucher : IVoucherStrategy
    {
        private readonly decimal _discountPercent;
        private readonly decimal _minOrderAmount;

        public PercentageVoucher(decimal discountPercent, decimal minOrderAmount = 0)
        {
            _discountPercent = discountPercent;
            _minOrderAmount = minOrderAmount;
        }

        public decimal ApplyDiscount(decimal subtotal)
        {
            if (!IsValid(subtotal))
                return 0;

            return subtotal * _discountPercent / 100;
        }

        public bool IsValid(decimal subtotal)
        {
            return subtotal >= _minOrderAmount;
        }

        public string GetMessage()
        {
            return _minOrderAmount > 0
                ? $"Giảm {_discountPercent}% cho đơn từ {_minOrderAmount:N0}₫"
                : $"Giảm {_discountPercent}%";
        }
    }

    /// <summary>
    /// Voucher giảm tiền cố định
    /// </summary>
    public class FixedAmountVoucher : IVoucherStrategy
    {
        private readonly decimal _discountAmount;
        private readonly decimal _minOrderAmount;

        public FixedAmountVoucher(decimal discountAmount, decimal minOrderAmount = 0)
        {
            _discountAmount = discountAmount;
            _minOrderAmount = minOrderAmount;
        }

        public decimal ApplyDiscount(decimal subtotal)
        {
            if (!IsValid(subtotal))
                return 0;

            return Math.Min(_discountAmount, subtotal);
        }

        public bool IsValid(decimal subtotal)
        {
            return subtotal >= _minOrderAmount;
        }

        public string GetMessage()
        {
            return $"Giảm {_discountAmount:N0}₫ cho đơn từ {_minOrderAmount:N0}₫";
        }
    }

    /// <summary>
    /// Voucher miễn phí ship
    /// </summary>
    public class FreeShippingVoucher : IVoucherStrategy
    {
        private readonly decimal _minOrderAmount;

        public FreeShippingVoucher(decimal minOrderAmount = 0)
        {
            _minOrderAmount = minOrderAmount;
        }

        public decimal ApplyDiscount(decimal subtotal)
        {
            // Discount được tính ở shipping fee, không phải subtotal
            return 0;
        }

        public bool IsValid(decimal subtotal)
        {
            return subtotal >= _minOrderAmount;
        }

        public string GetMessage()
        {
            return _minOrderAmount > 0
                ? $"Miễn phí ship cho đơn từ {_minOrderAmount:N0}₫"
                : "Miễn phí ship";
        }
    }

    /// <summary>
    /// Factory Pattern - Tạo voucher strategy
    /// </summary>
    public class VoucherFactory
    {
        private static readonly Dictionary<string, IVoucherStrategy> _vouchers = new Dictionary<string, IVoucherStrategy>
        {
            { "GIAM10", new PercentageVoucher(10, 0) },
            { "GIAM15", new PercentageVoucher(15, 100000) },
            { "GIAM20", new PercentageVoucher(20, 200000) },
            { "FREESHIP", new FreeShippingVoucher(50000) },
            { "NEWUSER", new FixedAmountVoucher(30000, 0) }
        };

        public static IVoucherStrategy GetVoucher(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            code = code.ToUpper().Trim();
            return _vouchers.ContainsKey(code) ? _vouchers[code] : null;
        }

        public static bool IsValidCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            return _vouchers.ContainsKey(code.ToUpper().Trim());
        }
    }
}
