using System;

namespace ElearnAPI.Models
{
    public class CouponCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = null!; // e.g., "WELCOME10", "SAVE2025"

        public decimal? DiscountAmount { get; set; }  // e.g., ₹200 off

        public decimal? DiscountPercentage { get; set; }  // e.g., 10% off

        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; } = 1; // Total allowed usages

        public int TimesUsed { get; set; } = 0; // Tracks how many times used

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
