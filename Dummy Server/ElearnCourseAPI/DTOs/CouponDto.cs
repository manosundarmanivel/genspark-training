using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
    public class CreateCouponDto
    {
        public string Code { get; set; } = null!;
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } = 1;
    }

    public class CouponResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

public class CouponValidateResponseDto
{
     public Guid Id { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
}


}
