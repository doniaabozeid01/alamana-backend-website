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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VideoDto>> GetById(int id)
        {
            var v = await _svc.GetByIdAsync(id);
            return v is null ? NotFound() : Ok(v);
        }





        //[HttpPost]
        //public async Task<ActionResult<int>> Create([FromBody] CreateVideoDto dto)
        //{
        //    var id = await _svc.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetById), new { id }, id);
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateVideoDto dto)
        //{
        //    await _svc.UpdateAsync(id, dto);
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    await _svc.DeleteAsync(id);
        //    return NoContent();
        //}







        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            await _svc.SetDefaultAsync(id);
            return NoContent();
        }

    }
}
