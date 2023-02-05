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
        private UserManager<User> userManager { get; }

        public Mfa(UserManager<User> userManager)
        {
        
            this.userManager = userManager;
        }
        private Random _random = new Random();

        public string GenerateRandomNo()
        {
            return _random.Next(0, 9999).ToString("D4");
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultProvider);
            var email = new MimeMessage();
            var otp = GenerateRandomNo();

            HttpContext.Session.SetString("OTP",otp);
            Console.WriteLine(HttpContext.Session.GetString("OTP"));
            email.From.Add(new MailboxAddress("TEST", "aikleong0713@gmail.com"));
            email.To.Add(new MailboxAddress("aik",user.Email));
            email.Subject = "Authentication";
            email.Body = new TextPart("plain") { Text = otp };
            
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("aikleong0713@gmail.com", "ynvinbhvfetivlcn");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            return Page();
               
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(HttpContext.Session.GetString("OTP"));
                Console.WriteLine(mfa.code);
                    Console.WriteLine(HttpContext.Session.GetString("OTP"));
                if (mfa.code == HttpContext.Session.GetString("OTP"))
                {
                    Console.WriteLine("ram");
                    var user = await userManager.FindByEmailAsync(User.Identity.Name);
                    user.EmailConfirmed = true;
                    return RedirectToPage("Privacy");
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
