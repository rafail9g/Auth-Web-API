using Microsoft.AspNetCore.Mvc;
using Tugas_PAA_TM.Models;
using Tugas_PAA_TM.Repositories;

// ═══════════════════════════════════════════════════════════
//  TanamanController
// ═══════════════════════════════════════════════════════════
namespace Tugas_PAA_TM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TanamanController : ControllerBase
{
    private readonly TanamanRepository _repo;
    private readonly LahanRepository   _lahanRepo;

    public TanamanController(TanamanRepository repo, LahanRepository lahanRepo)
    {
        _repo      = repo;
        _lahanRepo = lahanRepo;
    }

    // GET api/tanaman?lahanId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? lahanId)
    {
        var data = await _repo.GetAllAsync(lahanId);
        return Ok(ApiResponse<List<Tanaman>>.Ok(data, $"Ditemukan {data.Count} tanaman"));
    }

    // GET api/tanaman/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null)
            return NotFound(ApiResponse<object>.Fail($"Tanaman dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Tanaman>.Ok(item));
    }

    // POST api/tanaman
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TanamanCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        // Validasi referensial: lahan harus ada
        var lahan = await _lahanRepo.GetByIdAsync(dto.LahanId);
        if (lahan is null)
            return BadRequest(ApiResponse<object>.Fail($"Lahan dengan id {dto.LahanId} tidak ditemukan"));

        var validStatus = new[] { "aktif", "panen", "gagal" };
        if (!validStatus.Contains(dto.Status))
            return BadRequest(ApiResponse<object>.Fail("Status harus salah satu dari: aktif, panen, gagal"));

        var created = await _repo.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<Tanaman>.Ok(created, "Tanaman berhasil ditambahkan"));
    }

    // PUT api/tanaman/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TanamanCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        var updated = await _repo.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<object>.Fail($"Tanaman dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Tanaman>.Ok(updated, "Tanaman berhasil diperbarui"));
    }

    // DELETE api/tanaman/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Tanaman dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<object>.Ok(null, "Tanaman berhasil dihapus"));
    }
}
