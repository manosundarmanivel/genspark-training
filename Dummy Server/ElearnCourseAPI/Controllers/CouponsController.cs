using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog; // Serilog logger
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/coupons")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponCodeService _service;

        public CouponsController(ICouponCodeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var coupon = await _service.GetByIdAsync(id);
            return coupon == null ? NotFound() : Ok(coupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCouponDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> Enable(Guid id)
        {
            var success = await _service.EnableAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> Disable(Guid id)
        {
            var success = await _service.DisableAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Coupon code is required.");

            var coupon = await _service.ValidateCouponAsync(code);
            if (coupon == null)
                return NotFound(new { message = "Invalid or expired coupon." });

            return Ok(coupon);
        }

        [HttpPatch("{id}/increment-usage")]
public async Task<IActionResult> IncrementUsage(Guid id)
{
    var success = await _service.IncrementTimesUsedAsync(id);
    return success ? NoContent() : NotFound();
}

 
    }




}