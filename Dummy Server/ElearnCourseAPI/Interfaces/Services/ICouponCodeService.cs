using ElearnAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Models;

namespace ElearnAPI.Interfaces.Services
{

    public interface ICouponCodeService
    {
        Task<CouponResponseDto> CreateAsync(CreateCouponDto dto);
        Task<IEnumerable<CouponResponseDto>> GetAllAsync();
        Task<CouponResponseDto?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, CreateCouponDto dto);
        Task<bool> EnableAsync(Guid id);
        Task<bool> DisableAsync(Guid id);


        Task<CouponValidateResponseDto?> ValidateCouponAsync(string code);

        Task<bool> IncrementTimesUsedAsync(Guid id);

}


}