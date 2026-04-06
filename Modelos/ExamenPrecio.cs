using System.ComponentModel.DataAnnotations;

namespace BackendLimpio.Models
{
    public class ExamenPrecio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExamenId { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = "medico"; // "medico" o "dueno"

        public decimal Precio { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}