namespace Tugas_PAA_TM.Models
{
    public class Panen
    {
        public int Id { get; set; }
        public int TanamanId { get; set; }
        public DateOnly TanggalPanen { get; set; }
        public decimal JumlahKg { get; set; }
        public decimal HargaPerKg { get; set; }
        public string Kualitas { get; set; } = "B";
        public decimal TotalPendapatan => JumlahKg * HargaPerKg;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PanenCreateDto
    {
        public int TanamanId { get; set; }
        public DateOnly TanggalPanen { get; set; }
        public decimal JumlahKg { get; set; }
        public decimal HargaPerKg { get; set; }
        public string Kualitas { get; set; } = "B";
    }
}
