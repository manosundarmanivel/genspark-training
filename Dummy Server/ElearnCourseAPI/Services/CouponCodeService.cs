using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Services
{
    public class CouponCodeService : ICouponCodeService
    {
        private readonly ICouponCodeRepository _repo;
        private readonly IMapper _mapper;

        public CouponCodeService(ICouponCodeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CouponResponseDto> CreateAsync(CreateCouponDto dto)
        {
            var coupon = _mapper.Map<CouponCode>(dto);
            coupon.ExpiryDate = dto.ExpiryDate.ToUniversalTime();

            var created = await _repo.AddAsync(coupon);
            return _mapper.Map<CouponResponseDto>(created);
        }

        public async Task<IEnumerable<CouponResponseDto>> GetAllAsync()
        {
            var coupons = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponResponseDto>>(coupons);
        }

        public async Task<CouponResponseDto?> GetByIdAsync(Guid id)
        {
            var coupon = await _repo.GetByIdAsync(id);
            return coupon == null ? null : _mapper.Map<CouponResponseDto>(coupon);
        }

        public async Task<bool> UpdateAsync(Guid id, CreateCouponDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(dto, existing);
            existing.ExpiryDate = dto.ExpiryDate.ToUniversalTime();
            return await _repo.UpdateAsync(existing);
        }

        public async Task<CouponValidateResponseDto?> ValidateCouponAsync(string code)
        {
            var coupon = await _repo.GetByCodeAsync(code);
            if (coupon == null) return null;

            return new CouponValidateResponseDto
            {
                Id = coupon.Id,
                DiscountAmount = coupon.DiscountAmount,
                DiscountPercentage = coupon.DiscountPercentage
            };
        }

        public Task<bool> EnableAsync(Guid id) => _repo.SetActiveStatusAsync(id, true);
        public Task<bool> DisableAsync(Guid id) => _repo.SetActiveStatusAsync(id, false);
    
    public async Task<bool> IncrementTimesUsedAsync(Guid id)
{
    var coupon = await _repo.GetByIdAsync(id);
    if (coupon == null)
        return false;

    coupon.TimesUsed++;
    await _repo.UpdateAsync(coupon);
    return true;
}
 
}

}