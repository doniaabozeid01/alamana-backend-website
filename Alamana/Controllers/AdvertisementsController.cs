using Alamana.Service.Advertisment;
using Alamana.Service.Advertisment.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {



        private readonly IAdvertiseService _advertiseService;

        public AdvertisementsController(IAdvertiseService advertiseService)
        {
            _advertiseService = advertiseService;
        }

        [HttpGet("[controller]/GetAllAdvertisements")]
        public async Task<ActionResult<IReadOnlyList<AdvertiseService>>> GetAllAdvertisements()
        {
            var Advertises = await _advertiseService.GetAllAdvertisements();
            return Ok(Advertises);
        }


        [HttpGet("[controller]/GetAdvertisementById/{id}")]
        public async Task<ActionResult<AdvertiseDto>> GetAdvertisementById(int id)
        {
            var img = await _advertiseService.GetAdvertisementById(id);
            if (img == null)
            {
                return NotFound(new { message = $"Advertisement with ID {id} not found." });
            }
            return Ok(img);
        }



        //[Authorize(Roles = "Admin")]
        [HttpPost("admin/[controller]/AddAdvertisement")]
        public async Task<ActionResult> AddAdvertisement([FromForm] AddAdvertise imageDto)
        {
            if (imageDto.ImageUrl == null || imageDto.ImageUrl.Length == 0)
            {
                return BadRequest(new { message = "Image is required." });
            }

            var addedImg = await _advertiseService.AddAdvertise(imageDto);
            if (addedImg == null)
            {
                return StatusCode(500, "Failed to add advertise.");
            }

            return CreatedAtAction(nameof(GetAdvertisementById), new { id = addedImg.Id }, addedImg);
        }




        //[Authorize(Roles = "Admin")]
        [HttpPut("admin/[controller]/UpdateAdvertisement/{id}")]
        public async Task<ActionResult> UpdateAdvertisement(int id, [FromForm] AddAdvertise imageDto)
        {
            var img = await _advertiseService.GetAdvertisementById(id);
            if (img == null)
            {
                return NotFound(new { message = $"image with ID {id} not found." });
            }

            var updatedImage = await _advertiseService.UpdateAdvertise(id, imageDto);
            if (updatedImage == null)
            {
                return StatusCode(500, "Failed to update image.");
            }

            return Ok(updatedImage);
        }





        //[Authorize(Roles = "Admin")]
        [HttpDelete("admin/[controller]/DeleteAdvertisement/{id}")]
        public async Task<ActionResult> DeleteAdvertisement(int id)
        {
            var image = await _advertiseService.GetAdvertisementById(id);
            if (image == null)
            {
                return NotFound(new { message = $"image with ID {id} not found." });
            }

            var result = await _advertiseService.DeleteAdvertisement(id);
            if (result == 0)
            {
                return StatusCode(500, "Failed to delete image.");
            }

            var images = await _advertiseService.GetAllAdvertisements();
            if (images == null)
            {
                return NotFound(new { message = $"there are no Advertises" });

            }
            return Ok(images);
            // حذف الصورة بعد التأكد من حذف الإعلان
            //_advertiseServices.DeleteImage(advert.ImagePath);

        }











    }
}
