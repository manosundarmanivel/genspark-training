using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System;
using System.Collections.Generic;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly RazorpayClient _client;

        public OrderController()
        {
            _client = new RazorpayClient("rzp_test_b3ElOhVkNMsw8i", "CT8sjQzZV6iOPcJ7mleePNcU");
        }

        [HttpPost("create-order")]
        public IActionResult CreateRazorpayOrder([FromBody] CreateOrderRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Amount must be greater than 0."
                });
            }

            var options = new Dictionary<string, object>
            {
                { "amount", request.Amount * 100 }, // in paise
                { "currency", "INR" },
                { "receipt", $"rcpt_{Guid.NewGuid().ToString("N").Substring(0, 16)}" },
                { "payment_capture", 1 }
            };

            try
            {
                Order order = _client.Order.Create(options);
                return Ok(new { orderId = order["id"].ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    errors = new { exception = ex.Message }
                });
            }
        }
    }

    public class CreateOrderRequest
    {
        public int Amount { get; set; }
    }
}
