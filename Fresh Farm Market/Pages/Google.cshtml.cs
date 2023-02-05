using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Fresh_Farm_Market.Model;
using Microsoft.AspNetCore.Identity;
using Fresh_Farm_Market.ViewModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Fresh_Farm_Market.Service;
using Fresh_Farm_Market.Models;

namespace Fresh_Farm_Market.Pages
{
    public class GoogleModel : PageModel
    {
        //[BindProperty]
        public User user { get; set; }

        [BindProperty]
        public GoogleRegister GoogleRegister { get; set; }
        private readonly RoleManager<IdentityRole> roleManager;
        private UserManager<User> userManager { get; }
        private AuditLogService auditLog { get; set; }

        private SignInManager<User> signInManager { get; }
        private IWebHostEnvironment webHostEnvironment;
        public GoogleModel(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager, AuditLogService auditLogService)
        {
            this.auditLog = auditLogService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.webHostEnvironment = webHostEnvironment;
            this.roleManager = roleManager;
        }

        public  async Task<IActionResult> OnGet()

        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            
            var result = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            if (result != null)
            {
                var role = await userManager.GetRolesAsync(result);

                if (role[0].ToString() == "Google")
                {

                    var res = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                    await signInManager.SignInAsync(result, false);
                    var audit = new AuditLog()
                    {
                        userId = result.Id,
                        Activity = "Logged In",
                        DateTime = DateTime.Now
                    };
                    auditLog.AddAudit(audit);
                    //RedirectToPage("Index");
                    return RedirectToPage("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Email Registered with our database");
                    return Page();
                }
            }
            return Page();

            

        }

        public async Task<IActionResult> OnPost()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            var res = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            user = new User();
            user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            user.Name = info.Principal.FindFirstValue(ClaimTypes.Name);
            user.UserName = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (ModelState.IsValid)
            {
                if (GoogleRegister.Photo != null)
                {

                    if (GoogleRegister.Photo.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Photo", "File size cannot exceed 2MB");
                        return Page();
                    }
                }
                if (!Path.GetExtension(GoogleRegister.Photo.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                   
                    ModelState.AddModelError("", "Only JPG files are allowed.");
                    return Page();
                    
                }
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var uploadsFolder = "uploads";
                var imageFile = user.Id + ".jpg";
                var imagePath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                using var fileStream = new FileStream(imagePath, FileMode.Create);
                await GoogleRegister.Photo.CopyToAsync(fileStream);
                user.CreditCard = protector.Protect(GoogleRegister.CreditCard);
                user.AboutMe = GoogleRegister.AboutMe;
                user.Address = GoogleRegister.Address;
                user.PhoneNumber = GoogleRegister.PhoneNumber;
                user.Gender = GoogleRegister.Gender;
                await userManager.CreateAsync(user);
                IdentityRole role = await roleManager.FindByNameAsync("Google");
                if (role == null)
                {

                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Google"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role Google failed");
                    }
                }
                var result = await userManager.AddToRoleAsync(user, "Google");
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return Page();
        }

    }
}
