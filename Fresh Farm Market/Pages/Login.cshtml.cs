using Microsoft.AspNetCore.Mvc;
using Fresh_Farm_Market.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fresh_Farm_Market.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Fresh_Farm_Market.Models;
using System.Web;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Fresh_Farm_Market.Service;

namespace Fresh_Farm_Market.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private UserManager<User> userManager { get; }
        private AuditLogService auditLog { get; set; }
        private SignInManager<User> signInManager { get; }
        private readonly reCaptchaService _reCaptchaService;
        public LoginModel(UserManager<User> userManager, SignInManager<User> signInManager, reCaptchaService reCaptchaService,AuditLogService auditLogService)
        {
            this.auditLog = auditLogService;
            _reCaptchaService = reCaptchaService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPostLoginG()
        {
            string redirectUrl = "/Google";

            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            
            return new ChallengeResult("Google", properties);
        }

          
        
            
            //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           
            //var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            //{
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type,
            //    claim.Value
               
            //});
            //return JsonSerializer.Serialize(claims);

        public async Task<IActionResult> OnPostAsync()
            
        {

            var reCaptcharesult = _reCaptchaService.tokenVerify(LModel.token);
            if (!reCaptcharesult.Result.success || reCaptcharesult.Result.score <= 0.5)
            {
                ModelState.AddModelError("", "you are not a human.");
                return Page();

            }
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, true);
                if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account is locked out");
                    return Page();
                }
                if (identityResult.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(LModel.Email);
                    if (user != null)
                    {
                        DateTime current = DateTime.Now;
                        DateTime userMax = user.PasswordDay.AddDays(user.MaximumPasswordAge);
                        int result = DateTime.Compare(userMax, current);
                       
                        if (result <= 0)
                        {
                            ModelState.AddModelError("", "Maximum Password Age reached password need to be reset");
                            return Page();
                        }
                        var audit = new AuditLog()
                        {
                            Id = Guid.NewGuid().ToString(),
                            userId = user.Id,
                            Activity = "Logged In",
                            DateTime = DateTime.Now
                        };
                        auditLog.AddAudit(audit);
                        return RedirectToPage("Index");
                    }


                    
                }


                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();
        }
    }
}
