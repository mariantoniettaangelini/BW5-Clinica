using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BW5.Models
{
    public class Recovery
    {
        public int Id { get; set; }


        public DateTime RegistrationDate { get; set; }

        [Required]
        public string Name { get; set; }


        public string Type { get; set; }


        public string CoatColor { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string? Microchip { get; set; }


        [DataType(DataType.Date)]
        public DateTime? AdmissionDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DischargeDate { get; set; }

        public string? PhotoUrl { get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }
    }

}
