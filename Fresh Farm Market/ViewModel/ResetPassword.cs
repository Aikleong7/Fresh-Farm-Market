using System.ComponentModel.DataAnnotations;

namespace Fresh_Farm_Market.ViewModel
{
	public class ResetPassword
	{

	
		[DataType(DataType.EmailAddress)]

		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name ="Confirm password")]
		[Compare("Password", ErrorMessage ="Password and Confirm Password must match")]
		public	string CfmPassword { get; set; }

		public string Token { get; set; }

	}
}
