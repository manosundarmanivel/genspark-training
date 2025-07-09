
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ElearnAPI.Interfaces.Repositories
{
    public interface ICouponCodeRepository
    {
        Task<CouponCode> AddAsync(CouponCode coupon);
        Task<IEnumerable<CouponCode>> GetAllAsync();
        Task<CouponCode?> GetByIdAsync(Guid id);
        Task<CouponCode?> GetByCodeAsync(string code);
        Task<bool> UpdateAsync(CouponCode coupon);
        Task<bool> SetActiveStatusAsync(Guid id, bool isActive);

    }

}
