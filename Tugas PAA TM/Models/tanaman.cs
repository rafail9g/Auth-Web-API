namespace Tugas_PAA_TM.Models
{
    public class Tanaman
    {
        public int Id { get; set; }
        public int LahanId { get; set; }
        public string NamaTanaman { get; set; } = string.Empty;
        public string Varietas { get; set; } = string.Empty;
        public DateOnly TanggalTanam { get; set; }
        public string Status { get; set; } = "aktif";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TanamanCreateDto
    {
        public int LahanId { get; set; }
        public string NamaTanaman { get; set; } = string.Empty;
        public string Varietas { get; set; } = string.Empty;
        public DateOnly TanggalTanam { get; set; }
        public string Status { get; set; } = "aktif";
    }
}
