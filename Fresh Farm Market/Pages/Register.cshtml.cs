using Fresh_Farm_Market.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fresh_Farm_Market.ViewModel;
using Microsoft.Win32;
using Microsoft.AspNetCore.DataProtection;
using System.Web;
using Fresh_Farm_Market.Service;
using Fresh_Farm_Market.Models;
using Microsoft.AspNetCore.Hosting;

namespace Fresh_Farm_Market.Pages
{
    public class RegisterModel : PageModel
    {
        
        [BindProperty]
        public Register rmodel { get; set; }
        private UserManager<User> userManager { get; }
        private SignInManager<User> signInManager { get; }
        private PasswordHistoryService passwordHistory { get; }

        private readonly RoleManager<IdentityRole> roleManager;
        private IWebHostEnvironment webHostEnvironment;


        public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, PasswordHistoryService passwordHistoryService, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.passwordHistory = passwordHistoryService;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (rmodel.Photo != null)
                {
                   
                    if (rmodel.Photo.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Photo", "File size cannot exceed 2MB");
                        return Page();
                    }
                }
                if (!Path.GetExtension(rmodel.Photo.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {

                    ModelState.AddModelError("", "Only JPG files are allowed.");
                    return Page();

                }



                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                var user = new User()
                {
                    UserName = HttpUtility.HtmlEncode(rmodel.Email),
                    Name = HttpUtility.HtmlEncode(rmodel.Name),
                    Email = HttpUtility.HtmlEncode(rmodel.Email),
                    PhoneNumber = HttpUtility.HtmlEncode(rmodel.PhoneNumber),
                    Gender = rmodel.Gender,
                    CreditCard = protector.Protect(HttpUtility.HtmlEncode(rmodel.CreditCard)),
                    Address = HttpUtility.HtmlEncode(rmodel.Address),
                    AboutMe = HttpUtility.HtmlEncode(rmodel.AboutMe),
                    PasswordDay = DateTime.Now,
                    OTPdateTime = DateTime.Now
                };
                var uploadsFolder = "uploads";
                var imageFile = user.Id + ".jpg";
                var imagePath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                using var fileStream = new FileStream(imagePath, FileMode.Create);
                await rmodel.Photo.CopyToAsync(fileStream);
                IdentityRole role = await roleManager.FindByNameAsync("Customer");
                if (role == null)
                {
                    
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Customer"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role Customer failed");
                    }
                }
                var result = await userManager.CreateAsync(user, rmodel.Password);
                if (result.Succeeded)
                {
                    
                    result = await userManager.AddToRoleAsync(user, "Customer");
                    
                    await signInManager.SignInAsync(user, false);
                    var passhistroy = new PasswordHistory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        Password = user.PasswordHash
                    };
                    passwordHistory.AddPassowrdHistory(passhistroy);
                    return RedirectToPage("MFA");
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
