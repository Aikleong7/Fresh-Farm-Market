﻿using System.ComponentModel.DataAnnotations;

namespace Fresh_Farm_Market.ViewModel
{
    public class Register
    {
        [Required]
        
        public string Name { get; set; }
        [Required]
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }
        [Required]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(12)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\\s).{12,}$", ErrorMessage = "Min 12 chars,Use combination of lower-case, upper-case, Numbers\r\nand special characters")]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
        [Required]
        public string AboutMe { get; set; }


    }
}
