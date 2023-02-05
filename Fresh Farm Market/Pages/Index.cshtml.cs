using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Fresh_Farm_Market.Model;
using Microsoft.AspNetCore.DataProtection;

namespace Fresh_Farm_Market.Pages
{
    [Authorize(Roles = "Customer, Google")]


    public class IndexModel : PageModel
    {
        private readonly UserManager<User> userManager;
        public User user { get; set; }
        public string imgsrc { get;set; }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, UserManager<User> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
           
        }

        public async Task<IActionResult> OnGet()
        {
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            HttpContext.Session.SetString("sess", "hello");
          
            imgsrc = "../uploads/" + user.Id + ".jpg";
            user.CreditCard = protector.Unprotect(user.CreditCard);
            //if (HttpContext.Session.GetString("SessionTimeout") == null)
            //{
            //    return RedirectToPage("Login");
            //}
            return Page();
            //user.CreditCard = "ABC";
        }
    }
}