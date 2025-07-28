using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _service;

        public OrderDetailsController(IOrderDetailService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _service.GetAllAsync();
            return Ok(details);
        }

        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> GetById(int orderId, int productId)
        {
            var detail = await _service.GetByIdAsync(orderId, productId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDetailDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { orderId = created.OrderID, productId = created.ProductID }, created);
        }

        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> Delete(int orderId, int productId)
        {
            var deleted = await _service.DeleteAsync(orderId, productId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
