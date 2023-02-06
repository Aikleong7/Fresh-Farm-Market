using Fresh_Farm_Market.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fresh_Farm_Market.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Service;

namespace Fresh_Farm_Market.Pages
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public ResetPassword resetpassword { get; set; }

        private AuditLogService auditLog { get; set; }
        private UserManager<User> userManager { get; }
        private PasswordHistoryService passwordHistoryService { get; set; }
        public ResetPasswordModel(UserManager<User> userManager, PasswordHistoryService passwordHistoryService, AuditLogService auditLog)
        {
            this.userManager = userManager;
            this.passwordHistoryService = passwordHistoryService;
            this.auditLog = auditLog;
        }

        public void OnGet(string token, string email)
        {
            resetpassword = new ResetPassword();

            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            if (token != null && email != null )
            {
                resetpassword.Email = email;
                resetpassword.Token = token;
            }
            
            

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(resetpassword.Email);
                if (user != null)
                {
                    var past = passwordHistoryService.GetPasswordHistoriesbyId(user.Id);
                    if (past.Count == 2)
                    {
                        var hashpw = userManager.PasswordHasher.HashPassword(user, resetpassword.Password);
                        var checker = true;
                        foreach (var x in past)
                        {
                            if (x.Password == hashpw)
                            {
                                checker = false;
                            }
                        }
                        if (checker == false)
                        {
                            ModelState.AddModelError("", "Password is used within 2 history");
                        }
                        else
                        {
                            passwordHistoryService.removehistory(past[0].Id);
                            
                        }

                    }
                  
                    var result = await userManager.ResetPasswordAsync(user, resetpassword.Token, resetpassword.Password);
                    if (result.Succeeded)
                    {
                        var passhistroy = new PasswordHistory()
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            Password = user.PasswordHash
                        };
                        var audit = new AuditLog()
                        {
                            userId = user.Id,
                            Activity = "Reset Password",
                            DateTime = DateTime.Now
                        };
                        auditLog.AddAudit(audit);
                        user.PasswordDay = DateTime.Now;    
                        passwordHistoryService.AddPassowrdHistory(passhistroy);
                        Console.WriteLine("Password Reset Suceessfully");
                        RedirectToPage("Login");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return Page();
        }
    }
}
