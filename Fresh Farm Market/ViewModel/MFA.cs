using System.ComponentModel.DataAnnotations;

namespace Fresh_Farm_Market.ViewModel
{
	public class MFA
	{
        [Required]
        [DataType(DataType.Text)]
        public string code { get; set; }
    }
}
