using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web_applogs2.Pages.Shared
{
    public class Trigger500Model : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Error500");

        }
    }
}
