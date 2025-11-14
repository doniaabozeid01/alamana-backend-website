using Alamana.Service.Category.Dtos;
using Alamana.Service.Video;
using Alamana.Service.Video.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {

        private readonly IVideoService _svc;
        public VideosController(IVideoService svc) => _svc = svc;

        [HttpGet("GetAllVideos")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetAllVideos()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("GetVideoById/{id:int}")]
        public async Task<ActionResult<VideoDto>> GetVideoById(int id)
        {
            var v = await _svc.GetByIdAsync(id);
            return v is null ? NotFound() : Ok(v);
        }





        [HttpPost("AddVideo")]
        public async Task<IActionResult> AddVideo([FromForm] CreateVideoDto dto)
        {
            var video = await _svc.CreateAsync(dto);
            return Ok(video);
        }





        [HttpPut("UpdateVideo/{id:int}")]
        public async Task<IActionResult> UpdateVideo(int id, [FromForm] CreateVideoDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return Ok();
        }







        [HttpDelete("DeleteVideo/{id:int}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            await _svc.DeleteAsync(id);
            return Ok("تم حذف الفيديو بنجاح");
        }







        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            await _svc.SetDefaultAsync(id);
            return Ok();
        }

    }
}
