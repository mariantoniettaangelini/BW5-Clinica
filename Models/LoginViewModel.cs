using System.ComponentModel.DataAnnotations;

namespace BW5.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il campo Nome Utente è obbligatorio.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
