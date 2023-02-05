using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fresh_Farm_Market.Pages
{
    [Authorize(Roles ="Google")]
    public class GoogleUserModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
