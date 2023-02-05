using System.ComponentModel.DataAnnotations;

namespace Fresh_Farm_Market.ViewModel
{
	public class GoogleRegister
	{
        [Required]
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }
        [Required]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
        [Required]
        public string AboutMe { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }


    }
}
