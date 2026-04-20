using Microsoft.AspNetCore.Mvc;
using Tugas_PAA_TM.Models;
using Tugas_PAA_TM.Repositories;

namespace Tugas_PAA_TM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LahanController : ControllerBase
{
    private readonly LahanRepository _repo;
    public LahanController(LahanRepository repo) => _repo = repo;

    // GET api/lahan
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Lahan>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAllAsync();
        return Ok(ApiResponse<List<Lahan>>.Ok(data, $"Ditemukan {data.Count} lahan"));
    }

    // GET api/lahan/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Lahan>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var lahan = await _repo.GetByIdAsync(id);
        if (lahan is null)
            return NotFound(ApiResponse<object>.Fail($"Lahan dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Lahan>.Ok(lahan));
    }

    // POST api/lahan
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Lahan>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Create([FromBody] LahanCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        if (dto.LuasHektar <= 0)
            return BadRequest(ApiResponse<object>.Fail("Luas hektar harus lebih dari 0"));

        var created = await _repo.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<Lahan>.Ok(created, "Lahan berhasil ditambahkan"));
    }

    // PUT api/lahan/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Lahan>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] LahanCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Data tidak valid"));

        var updated = await _repo.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<object>.Fail($"Lahan dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<Lahan>.Ok(updated, "Lahan berhasil diperbarui"));
    }

    // DELETE api/lahan/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Lahan dengan id {id} tidak ditemukan"));

        return Ok(ApiResponse<object>.Ok(null, "Lahan berhasil dihapus"));
    }
}
