using System.ComponentModel.DataAnnotations;

namespace BW5.Models
{
    public class Sales
    {
        public int Id { get; set; }

        public string CustomerFiscalCode { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string PrescriptionNumber { get; set; } // Numero della ricetta medica

        public DateTime SaleDate { get; set; }
    }
}
