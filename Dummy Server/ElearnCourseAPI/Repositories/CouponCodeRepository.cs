using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ElearnAPI.Repositories
{

    public class CouponCodeRepository : ICouponCodeRepository
    {
        private readonly ElearnDbContext _context;

        public CouponCodeRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task<CouponCode> AddAsync(CouponCode coupon)
        {
            _context.CouponCodes.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<IEnumerable<CouponCode>> GetAllAsync()
        {
            return await _context.CouponCodes.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<CouponCode?> GetByIdAsync(Guid id)
        {
            return await _context.CouponCodes.FindAsync(id);
        }

        public async Task<CouponCode?> GetByCodeAsync(string code)
        {
            return await _context.CouponCodes.FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<bool> UpdateAsync(CouponCode coupon)
        {
            _context.CouponCodes.Update(coupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetActiveStatusAsync(Guid id, bool isActive)
        {
            var coupon = await GetByIdAsync(id);
            if (coupon == null) return false;

            coupon.IsActive = isActive;
            return await _context.SaveChangesAsync() > 0;
        }
    
}

}