using Alamana.Service.ContactUs;
using Alamana.Service.ContactUs.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {

        private readonly IContactUsServices _contactUsService;

        public ContactUsController(IContactUsServices contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] ContactUsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _contactUsService.SendContactMessageAsync(dto);
            return Ok(new { message = "تم إرسال رسالتك بنجاح" });
        }

    }
}
