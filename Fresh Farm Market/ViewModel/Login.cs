using System.ComponentModel.DataAnnotations;

namespace Fresh_Farm_Market.ViewModel
{
	public class Login
	{
        [Required]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
       
        public string token { get; set; }
    }
}
