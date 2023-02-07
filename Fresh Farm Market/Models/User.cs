
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.Security.Cryptography.Xml;

namespace Fresh_Farm_Market.Model
{
    public class User: IdentityUser
    {
        public string Name { get; set; }
        public string CreditCard { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string AboutMe { get; set; }

        public int MinimumPasswordAge { get; } = 1;

        public int MaximumPasswordAge { get; } = 5;
        public string OTP { get; set; }
        public DateTime OTPdateTime { get; set; }
        

        public DateTime PasswordDay { get; set; }

    }
}
