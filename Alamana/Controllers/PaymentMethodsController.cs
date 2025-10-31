using Alamana.Service.Location;
using Alamana.Service.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodsServices _paymentMethodsServices;

        public PaymentMethodsController(IPaymentMethodsServices paymentMethodsServices)
        {
            _paymentMethodsServices = paymentMethodsServices;
        }


        [HttpGet("GetAllPaymentMethods")]
        public async Task<IActionResult> GetAllPaymentMethods()
        {
            try
            {
                var paymentMethods = await _paymentMethodsServices.GetAllPaymentMethods();
                return Ok(paymentMethods);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}
