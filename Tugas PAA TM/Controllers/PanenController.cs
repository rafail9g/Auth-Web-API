using Microsoft.AspNetCore.Mvc;
using Tugas_PAA_TM.Models;
using Tugas_PAA_TM.Repositories;

namespace Tugas_PAA_TM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PanenController : ControllerBase
{
    private readonly PanenRepository   _repo;
    private readonly TanamanRepository _tanamanRepo;

    public PanenController(PanenRepository repo, TanamanRepository tanamanRepo)
    {
        _repo        = repo;
        _tanamanRepo = tanamanRepo;
    }

    // GET api/panen?tanamanId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? tanamanId)
    {
        var data = await _repo.GetAllAsync(tanamanId);
        return Ok(ApiResponse<List<Panen>>.Ok(data, $"Ditemukan {data.Count} catatan panen"));
    }

    // GET api/panen/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null)
            return NotFound(ApiResponse<object>.Fail($"Catatan panen dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Panen>.Ok(item));
    }

    // POST api/panen
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PanenCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        // Validasi referensial: tanaman harus ada
        var tanaman = await _tanamanRepo.GetByIdAsync(dto.TanamanId);
        if (tanaman is null)
            return BadRequest(ApiResponse<object>.Fail($"Tanaman dengan id {dto.TanamanId} tidak ditemukan"));

        if (dto.JumlahKg <= 0)
            return BadRequest(ApiResponse<object>.Fail("Jumlah kg harus lebih dari 0"));

        if (dto.HargaPerKg <= 0)
            return BadRequest(ApiResponse<object>.Fail("Harga per kg harus lebih dari 0"));

        var validKualitas = new[] { "A", "B", "C" };
        if (!validKualitas.Contains(dto.Kualitas.ToUpper()))
            return BadRequest(ApiResponse<object>.Fail("Kualitas harus A, B, atau C"));

        var created = await _repo.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<Panen>.Ok(created, "Catatan panen berhasil ditambahkan"));
    }

    // PUT api/panen/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PanenCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        var updated = await _repo.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<object>.Fail($"Catatan panen dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Panen>.Ok(updated, "Catatan panen berhasil diperbarui"));
    }

    // DELETE api/panen/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Catatan panen dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<object>.Ok(null, "Catatan panen berhasil dihapus"));
    }
}
