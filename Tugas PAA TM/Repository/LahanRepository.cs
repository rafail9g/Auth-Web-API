using Npgsql;
using Microsoft.Extensions.Configuration;
using Tugas_PAA_TM.Models;

namespace Tugas_PAA_TM.Repositories;

public class LahanRepository
{
    private readonly string _connStr;

    public LahanRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connStr);

    public async Task<List<Lahan>> GetAllAsync()
    {
        var list = new List<Lahan>();
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nama_lahan, luas_hektar, lokasi, jenis_tanah, created_at, updated_at FROM lahan ORDER BY id";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    public async Task<Lahan?> GetByIdAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nama_lahan, luas_hektar, lokasi, jenis_tanah, created_at, updated_at FROM lahan WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<Lahan> CreateAsync(LahanCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO lahan (nama_lahan, luas_hektar, lokasi, jenis_tanah)
            VALUES (@nama_lahan, @luas_hektar, @lokasi, @jenis_tanah)
            RETURNING id";
        cmd.Parameters.AddWithValue("@nama_lahan",  dto.NamaLahan);
        cmd.Parameters.AddWithValue("@luas_hektar", dto.LuasHektar);
        cmd.Parameters.AddWithValue("@lokasi",      dto.Lokasi);
        cmd.Parameters.AddWithValue("@jenis_tanah", dto.JenisTanah);

        var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return (await GetByIdAsync(newId))!;
    }

    public async Task<Lahan?> UpdateAsync(int id, LahanCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE lahan
               SET nama_lahan  = @nama_lahan,
                   luas_hektar = @luas_hektar,
                   lokasi      = @lokasi,
                   jenis_tanah = @jenis_tanah
             WHERE id = @id";
        cmd.Parameters.AddWithValue("@id",          id);
        cmd.Parameters.AddWithValue("@nama_lahan",  dto.NamaLahan);
        cmd.Parameters.AddWithValue("@luas_hektar", dto.LuasHektar);
        cmd.Parameters.AddWithValue("@lokasi",      dto.Lokasi);
        cmd.Parameters.AddWithValue("@jenis_tanah", dto.JenisTanah);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows == 0 ? null : await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM lahan WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Lahan Map(NpgsqlDataReader r) => new()
    {
        Id         = r.GetInt32(r.GetOrdinal("id")),
        NamaLahan  = r.GetString(r.GetOrdinal("nama_lahan")),
        LuasHektar = r.GetDecimal(r.GetOrdinal("luas_hektar")),
        Lokasi     = r.GetString(r.GetOrdinal("lokasi")),
        JenisTanah = r.GetString(r.GetOrdinal("jenis_tanah")),
        CreatedAt  = r.GetDateTime(r.GetOrdinal("created_at")),
        UpdatedAt  = r.GetDateTime(r.GetOrdinal("updated_at")),
    };
}
