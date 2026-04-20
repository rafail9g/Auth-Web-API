namespace Tugas_PAA_TM.Models
{
    public class Lahan
    {
        public int Id { get; set; }
        public string NamaLahan { get; set; } = string.Empty;
        public decimal LuasHektar { get; set; }
        public string Lokasi { get; set; } = string.Empty;
        public string JenisTanah { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class LahanCreateDto
    {
        public string NamaLahan { get; set; } = string.Empty;
        public decimal LuasHektar { get; set; }
        public string Lokasi { get; set; } = string.Empty;
        public string JenisTanah { get; set; } = string.Empty;
    }
}
