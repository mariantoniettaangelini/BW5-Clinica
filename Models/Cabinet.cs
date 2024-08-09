using System.ComponentModel.DataAnnotations;

namespace BW5.Models
{
    public class Cabinet
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } // Codice numerico univoco
        public ICollection<Drawer>? Drawers { get; set; }
    }
}
