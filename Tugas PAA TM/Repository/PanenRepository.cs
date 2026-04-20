using Npgsql;
using Microsoft.Extensions.Configuration;
using Tugas_PAA_TM.Models;

namespace Tugas_PAA_TM.Repositories;

public class PanenRepository
{
    private readonly string _connStr;

    public PanenRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connStr);

    public async Task<List<Panen>> GetAllAsync(int? tanamanId = null)
    {
        var list = new List<Panen>();
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = tanamanId.HasValue
            ? "SELECT * FROM panen WHERE tanaman_id = @tanaman_id ORDER BY tanggal_panen DESC"
            : "SELECT * FROM panen ORDER BY tanggal_panen DESC";
        if (tanamanId.HasValue)
            cmd.Parameters.AddWithValue("@tanaman_id", tanamanId.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    public async Task<Panen?> GetByIdAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM panen WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<Panen> CreateAsync(PanenCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO panen (tanaman_id, tanggal_panen, jumlah_kg, harga_per_kg, kualitas)
            VALUES (@tanaman_id, @tanggal_panen, @jumlah_kg, @harga_per_kg, @kualitas)
            RETURNING id";
        cmd.Parameters.AddWithValue("@tanaman_id",    dto.TanamanId);
        cmd.Parameters.AddWithValue("@tanggal_panen", dto.TanggalPanen.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@jumlah_kg",     dto.JumlahKg);
        cmd.Parameters.AddWithValue("@harga_per_kg",  dto.HargaPerKg);
        cmd.Parameters.AddWithValue("@kualitas",      dto.Kualitas);

        var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return (await GetByIdAsync(newId))!;
    }

    public async Task<Panen?> UpdateAsync(int id, PanenCreateDto dto)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE panen
               SET tanaman_id    = @tanaman_id,
                   tanggal_panen = @tanggal_panen,
                   jumlah_kg     = @jumlah_kg,
                   harga_per_kg  = @harga_per_kg,
                   kualitas      = @kualitas
             WHERE id = @id";
        cmd.Parameters.AddWithValue("@id",            id);
        cmd.Parameters.AddWithValue("@tanaman_id",    dto.TanamanId);
        cmd.Parameters.AddWithValue("@tanggal_panen", dto.TanggalPanen.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@jumlah_kg",     dto.JumlahKg);
        cmd.Parameters.AddWithValue("@harga_per_kg",  dto.HargaPerKg);
        cmd.Parameters.AddWithValue("@kualitas",      dto.Kualitas);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows == 0 ? null : await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM panen WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Panen Map(NpgsqlDataReader r) => new()
    {
        Id           = r.GetInt32(r.GetOrdinal("id")),
        TanamanId    = r.GetInt32(r.GetOrdinal("tanaman_id")),
        TanggalPanen = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("tanggal_panen"))),
        JumlahKg     = r.GetDecimal(r.GetOrdinal("jumlah_kg")),
        HargaPerKg   = r.GetDecimal(r.GetOrdinal("harga_per_kg")),
        Kualitas     = r.GetString(r.GetOrdinal("kualitas")),
        CreatedAt    = r.GetDateTime(r.GetOrdinal("created_at")),
        UpdatedAt    = r.GetDateTime(r.GetOrdinal("updated_at")),
    };
}
