using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Interfaces.Services;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
        {
            var result = await _couponService.createCouponAsync(request);
            return CreatedAtAction(nameof(GetActiveCoupons), new { }, result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var result = await _couponService.getAllActiveCouponsAsync();
            return Ok(result);
        }

        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            var result = await _couponService.applyCouponAsync(request);
            return Ok(result);
        }
    }
}
