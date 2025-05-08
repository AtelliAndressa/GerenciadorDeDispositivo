using DeviceManager.API.Models;
using DeviceManager.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispositivosController : ControllerBase
    {
        private readonly IDeviceService _service;

        public DispositivosController(IDeviceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var dispositivo = await _service.GetByIdAsync(id);
            if (dispositivo == null) return NotFound();
            return Ok(dispositivo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dispositivo d)
        {
            if (!await _service.CreateAsync(d)) return BadRequest("Código de referência já existe.");
            return CreatedAtAction(nameof(GetById), new { id = d.Id }, d);
        }

        [HttpPost("lote")]
        public async Task<IActionResult> CreateMany([FromBody] List<Dispositivo> dispositivos)
        {
            var rejeitados = await _service.CreateManyAsync(dispositivos);
            if (rejeitados.Count == 0)
                return Ok(new { message = "Todos os dispositivos foram inseridos com sucesso." });
            return BadRequest(new { message = "Alguns dispositivos não foram inseridos devido a duplicidade de código.", rejeitados });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Dispositivo d)
        {
            d.Id = id;
            if (!await _service.UpdateAsync(d)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
