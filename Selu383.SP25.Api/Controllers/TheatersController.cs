using Microsoft.AspNetCore.Mvc;
using Selu383.SP25.Api.Data.Services;
using Selu383.SP25.Api.Entity;

namespace Selu383.SP25.Api.Controllers
{
    [ApiController]
    [Route("api/theaters")]
    public class TheatersController : ControllerBase
    {
        private readonly ITheatersService _service;

        public TheatersController(ITheatersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var theaters = await _service.GetAll();
            return Ok(theaters);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var theaterDetails = await _service.GetByIdAsync(id);
            if (theaterDetails == null) return NotFound();
            return Ok(theaterDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Theater theater)
        {
            if (theater == null)
            {
                return BadRequest("Theater data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.AddAsync(theater);
            return CreatedAtAction(nameof(GetById), new { id = theater.Id }, theater);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Theater theater)
        {
            if (theater == null || id != theater.Id)
            {
                return BadRequest("Invalid request data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTheater = await _service.GetByIdAsync(id);
            if (existingTheater == null)
            {
                return NotFound("Theater not found.");
            }

            existingTheater.Name = theater.Name;
            existingTheater.Address = theater.Address;
            existingTheater.SeatCount = theater.SeatCount;

            await _service.UpdateAsync(id, existingTheater);
            return Ok(existingTheater);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingTheater = await _service.GetByIdAsync(id);
            if (existingTheater == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}