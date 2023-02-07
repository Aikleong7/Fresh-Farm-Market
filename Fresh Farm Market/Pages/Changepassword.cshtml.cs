using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Service;
using Fresh_Farm_Market.ViewModel;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Fresh_Farm_Market.Pages
{
    public class Index1Model : PageModel
        
    {
        private UserManager<User> userManager { get; }
        private readonly IConfiguration _config;

        [BindProperty]
        public Changepassword Changepassword { get; set; }

        public Index1Model(UserManager<User> usermanager, IConfiguration config)
        {
            this.userManager = usermanager;
            _config = config;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.FindByEmailAsync(HttpUtility.HtmlEncode(Changepassword.EmailAddress));
            if (user == null)
            {
                ModelState.AddModelError("", "Email does not exist");
                return Page();
            }

            DateTime userdate = user.PasswordDay;
            DateTime current = DateTime.Now;
            int resilt = DateTime.Compare(current, userdate.AddSeconds(user.MinimumPasswordAge));
            if (resilt > 0)
            {


                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var url = Url.PageLink("/ResetPassword", values: new { email = Changepassword.EmailAddress, token = token });
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Forget Password", "aikleong0713@gmail.com"));
                email.To.Add(new MailboxAddress("User", user.Email));
                email.Subject = "Forget Password";
                email.Body = new TextPart("plain") { Text = url };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate(_config["Gmail:Email"], _config["Gmail:AppPassword"]);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return Page();
            }
            else
            {
                ModelState.AddModelError("", "Minimum Password Age limit has not reached you are not allowed to change password");
                return Page();
            }
            return Page();
        }

    }
}
