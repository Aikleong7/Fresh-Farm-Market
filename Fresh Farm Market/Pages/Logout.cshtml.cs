using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Fresh_Farm_Market.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Fresh_Farm_Market.Models;
using Fresh_Farm_Market.Service;

namespace Fresh_Farm_Market.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly AuditLogService auditLog;
        private readonly UserManager<User> userManager;
        public LogoutModel(SignInManager<User> signInManager, UserManager<User> userManager, AuditLogService auditLogService)
        {
            this.auditLog = auditLogService;
          
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var audit = new AuditLog()
            {
                Id = Guid.NewGuid().ToString(),
                userId = userManager.GetUserId(User),
                Activity = "Logged Out",
                DateTime = DateTime.Now
            };
            auditLog.AddAudit(audit);
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
