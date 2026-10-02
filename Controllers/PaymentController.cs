using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Interfaces.Services;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            var result = await _paymentService.processPaymentAsync(request, GetUserId());
            return Ok(result);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var result = await _paymentService.getPaymentByOrderIdAsync(orderId, GetUserId());
            if (result == null) return NotFound("Payment details not found for this order.");
            return Ok(result);
        }
    }
}
