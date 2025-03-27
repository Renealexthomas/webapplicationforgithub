using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web_applogs2.Pages.Shared
{
    public class Error500Model : PageModel
    {
        public void OnGet()
        {
            throw new Exception("This is an intentional 500 error");

        }
    }
}
