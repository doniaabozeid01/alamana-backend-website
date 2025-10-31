using System.Text;
using Alamana.Data.Entities;
using Alamana.Service.Carts;
using Alamana.Service.Carts.Dtos;
using Alamana.Service.Email;
using Alamana.Service.Location;
using Alamana.Service.Orders;
using Alamana.Service.Orders.Dtos;
using Alamana.Service.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IEmailSender _emailSender;
        private readonly ILocationServices _locationService;
        private readonly IPaymentMethodsServices _paymentService;

        public OrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager, ICartService cartService , IEmailSender emailSender,ILocationServices locationServices,IPaymentMethodsServices paymentService)
        {
            _orderService = orderService;
            _userManager = userManager;
            _cartService = cartService;
            _emailSender = emailSender;
            _locationService = locationServices;
            _paymentService = paymentService;
        }

                    

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null) return NotFound(new { message = "User Not Found" });

                var cart = await _cartService.GetCartByUserId(request.UserId);
                if (cart == null) return NotFound(new { message = "Cart Not Found" });

                var orderId = await _orderService.CreateOrderAsync(request);


                var country = await _locationService.GetCountryById(request.CountryId);
                var governorate = await _locationService.GetGovernorateById(request.GovernorateId);
                var city = await _locationService.GetCityById(request.CityId);
                var district = await _locationService.GetDistrictById(request.DistrictId);
                var paymentMethod = await _paymentService.GetPaymentMethodById(request.PaymentMethodId);

                // بناء الإيميل بعد الطلب
                var emailBody = BuildOrderEmail(
                    user, request, cart,
                    country.country_name, governorate.governorate_name, city.city_name, district.district_name, paymentMethod.Name
                );

                var subject = "New Order from " + request.FullName;

                await _emailSender.SendEmailAsync(
                    toEmail: request.Email,
                    subject: subject,
                    body: emailBody
                );


                return Ok(new { success = true, orderId = orderId, message = "تم تأكيد الطلب بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
       

        }





        private string BuildOrderEmail(
            ApplicationUser user,
            OrderRequest request,
            GetCartDto cart,
            string countryName,
            string governorateName,
            string cityName,
            string districtName,
            string paymentMethodName)
        {
            var sb = new StringBuilder();

            sb.Append(@"
    <div style='font-family:Arial, sans-serif; color:#333;'>
        <h2 style='color:#4CAF50;'>طلب جديد من الموقع</h2>

        <h3 style='margin-top:30px;'>بيانات العميل:</h3>
        <table style='width:100%; border-collapse:collapse; margin-bottom:30px;'>
            <tbody>");

            void Row(string label, string value) =>
                sb.Append($"<tr><td style='padding:6px 12px; font-weight:bold;'>{label}</td><td style='padding:6px 12px;'>{value}</td></tr>");

            Row("الاسم", request.FullName);
            Row("رقم الهاتف", request.Phone);
            Row("البريد الإلكتروني", request.Email);
            Row("الدولة", countryName);
            Row("المحافظة", governorateName);
            Row("المدينة", cityName);
            Row("المنطقة", districtName);
            Row("الشارع", request.Street);
            Row("رقم المبنى", request.BuildingNumber);
            Row("الدور", request.Floor);
            Row("الشقة", request.Apartment);
            Row("علامة مميزة", request.Landmark);
            Row("طريقة الدفع", paymentMethodName);

            sb.Append("</tbody></table>");

            sb.Append(@"
        <h3>تفاصيل السلة:</h3>
        <table style='width:100%; border-collapse:collapse;'>
            <thead>
                <tr style='background-color:#f8f8f8; border-bottom:2px solid #ddd;'>
                    <th style='padding:10px; text-align:left;'>المنتج</th>
                    <th style='padding:10px; text-align:center;'>الكمية</th>
                    <th style='padding:10px; text-align:left;'>السعر</th>
                </tr>
            </thead>
            <tbody>");

            foreach (var item in cart.cartItems)
            {
                sb.Append("<tr style='border-bottom:1px solid #eee;'>");
                sb.Append($"<td style='padding:8px 10px; text-align:left;'>{item.Name}</td>");
                sb.Append($"<td style='padding:8px 10px; text-align:center;'>{item.Quantity}</td>");
                sb.Append($"<td style='padding:8px 10px; text-align:left;'>{item.Price:C}</td>");
                sb.Append("</tr>");
            }

            sb.Append($@"
            </tbody>
        </table>

        <div style='margin-top:20px; font-size:16px; font-weight:bold;'>
            الإجمالي: {cart.TotalAmount:C}
        </div>
    </div>");

            return sb.ToString();
        }







        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new { message = "User Not Found" });

                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }







    }
}
