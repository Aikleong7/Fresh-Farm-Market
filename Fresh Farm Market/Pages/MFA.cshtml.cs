using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MimeKit.Text;


namespace Fresh_Farm_Market.Pages
{
    [Authorize]
    public class Mfa : PageModel
    {
        
        [BindProperty]
        public MFA mfa { get; set; }
        private readonly IConfiguration _config;
        private UserManager<User> userManager { get; }

        public Mfa(UserManager<User> userManager, IConfiguration config)
        {

            this.userManager = userManager;
            _config = config;
        }
        private Random _random = new Random();

        public string GenerateRandomNo()
        {
            return _random.Next(0, 9999).ToString("D4");
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var email = new MimeMessage();
            var otp = GenerateRandomNo();
            user.OTP = otp;
            user.OTPdateTime = DateTime.Now;
            await userManager.UpdateAsync(user);
            email.From.Add(new MailboxAddress("TEST", _config["Gmail:Email"]));
            email.To.Add(new MailboxAddress("User",user.Email));
            email.Subject = "Authentication";
            email.Body = new TextPart("plain") { Text = otp };
            
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate(_config["Gmail:Email"], _config["Gmail:AppPassword"]);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            return Page();
               
        }
        public async Task<IActionResult> OnPostResend()
        {
            
            User user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

            var email = new MimeMessage();
            var otp = GenerateRandomNo();
            user.OTP = "1323";
            user.OTPdateTime = DateTime.Now;

            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                Console.WriteLine("Success");
            }
            email.From.Add(new MailboxAddress("TEST", _config["Gmail:Email"]));
            email.To.Add(new MailboxAddress("User", user.Email));
            email.Subject = "Authentication";
            email.Body = new TextPart("plain") { Text = otp };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate(_config["Gmail:Email"], _config["Gmail:AppPassword"]);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(User.Identity.Name);
                DateTime current = DateTime.Now;
                DateTime otptime = user.OTPdateTime.AddMinutes(15);
                int result = DateTime.Compare(otptime, current);
                if (result < 0)
                {
                    ModelState.AddModelError("", "OTP Token Expired.");
                    return Page();
                }
                if (mfa.code == user.OTP)
                {  
                    user.EmailConfirmed = true;
                    return RedirectToPage("Index");
                }
                else
                {
                    ModelState.AddModelError("code", "Wrong OTP number");
                    return Page();
                }
            }
            return Page();
        }


    }
}
