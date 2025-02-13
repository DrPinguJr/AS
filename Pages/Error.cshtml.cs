using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace dit230680c_AS.Pages
{

    public class ErrorModel : PageModel
    {
        [BindProperty]
        public int? ErrorCode { get; set; }

        public void OnGet(int? code)
        {
            // Capture the error code from the query string
            ErrorCode = code ?? 500; // Default to 500 if no specific error code is provided
        }
    }
}
