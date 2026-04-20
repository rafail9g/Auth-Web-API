using Npgsql;
using Microsoft.Extensions.Configuration;
using Tugas_PAA_TM.Models;

namespace Tugas_PAA_TM.Repositories;

public class TanamanRepository
{
    private readonly string _connStr;

    public TanamanRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connStr);

    public async Task<List<Tanaman>> GetAllAsync(int? lahanId = null)
    {
        var list = new List<Tanaman>();
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = lahanId.HasValue
            ? "SELECT * FROM tanaman WHERE lahan_id = @lahan_id ORDER BY id"
            : "SELECT * FROM tanaman ORDER BY id";
        if (lahanId.HasValue)
            cmd.Parameters.AddWithValue("@lahan_id", lahanId.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    public async Task<Tanaman?> GetByIdAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM tanaman WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<Tanaman> CreateAsync(TanamanCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO tanaman (lahan_id, nama_tanaman, varietas, tanggal_tanam, status)
            VALUES (@lahan_id, @nama_tanaman, @varietas, @tanggal_tanam, @status)
            RETURNING id";
        cmd.Parameters.AddWithValue("@lahan_id",      dto.LahanId);
        cmd.Parameters.AddWithValue("@nama_tanaman",  dto.NamaTanaman);
        cmd.Parameters.AddWithValue("@varietas",      dto.Varietas);
        cmd.Parameters.AddWithValue("@tanggal_tanam", dto.TanggalTanam.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@status",        dto.Status);

        var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return (await GetByIdAsync(newId))!;
    }

    public async Task<Tanaman?> UpdateAsync(int id, TanamanCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE tanaman
               SET lahan_id      = @lahan_id,
                   nama_tanaman  = @nama_tanaman,
                   varietas      = @varietas,
                   tanggal_tanam = @tanggal_tanam,
                   status        = @status
             WHERE id = @id";
        cmd.Parameters.AddWithValue("@id",            id);
        cmd.Parameters.AddWithValue("@lahan_id",      dto.LahanId);
        cmd.Parameters.AddWithValue("@nama_tanaman",  dto.NamaTanaman);
        cmd.Parameters.AddWithValue("@varietas",      dto.Varietas);
        cmd.Parameters.AddWithValue("@tanggal_tanam", dto.TanggalTanam.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@status",        dto.Status);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows == 0 ? null : await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM tanaman WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Tanaman Map(NpgsqlDataReader r) => new()
    {
        Id           = r.GetInt32(r.GetOrdinal("id")),
        LahanId      = r.GetInt32(r.GetOrdinal("lahan_id")),
        NamaTanaman  = r.GetString(r.GetOrdinal("nama_tanaman")),
        Varietas     = r.GetString(r.GetOrdinal("varietas")),
        TanggalTanam = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("tanggal_tanam"))),
        Status       = r.GetString(r.GetOrdinal("status")),
        CreatedAt    = r.GetDateTime(r.GetOrdinal("created_at")),
        UpdatedAt    = r.GetDateTime(r.GetOrdinal("updated_at")),
    };
}
