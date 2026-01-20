using System;
using System.Collections.Generic;
using System.Linq;

namespace WebBanNuoc.Models.Entities
{
    /// <summary>
    /// Cart Item - Mục trong giỏ hàng
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string SugarLevel { get; set; }
        public string IceLevel { get; set; }
        public List<string> Toppings { get; set; }
        
        public decimal TotalPrice => UnitPrice * Quantity;

        public CartItem()
        {
            Toppings = new List<string>();
        }
    }

    /// <summary>
    /// Shopping Cart - Giỏ hàng
    /// Singleton Pattern có thể áp dụng cho Cart Service
    /// </summary>
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; }
        public string VoucherCode { get; set; }
        public decimal VoucherDiscount { get; set; }

        public ShoppingCart()
        {
            Items = new List<CartItem>();
        }

        public decimal GetSubtotal()
        {
            return Items.Sum(item => item.TotalPrice);
        }

        public decimal GetShippingFee()
        {
            // Strategy Pattern - Tính phí ship
            return GetSubtotal() >= 100000 ? 0 : 20000;
        }

        public decimal GetDiscount()
        {
            if (string.IsNullOrEmpty(VoucherCode))
                return 0;

            // Strategy Pattern - Áp dụng voucher
            return GetSubtotal() * VoucherDiscount / 100;
        }

        public decimal GetTotal()
        {
            return GetSubtotal() + GetShippingFee() - GetDiscount();
        }

        public void AddItem(CartItem item)
        {
            // Kiểm tra xem sản phẩm với cùng tùy chọn đã có chưa
            var existingItem = Items.FirstOrDefault(i =>
                i.ProductId == item.ProductId &&
                i.Size == item.Size &&
                i.SugarLevel == item.SugarLevel &&
                i.IceLevel == item.IceLevel &&
                string.Join(",", i.Toppings) == string.Join(",", item.Toppings));

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
            }
        }

        public void UpdateQuantity(int index, int quantity)
        {
            if (index >= 0 && index < Items.Count && quantity > 0)
            {
                Items[index].Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
            VoucherCode = null;
            VoucherDiscount = 0;
        }
    }
}
