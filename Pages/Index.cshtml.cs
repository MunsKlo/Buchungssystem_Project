using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Buchungsystem_WindowsAuth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string StatusMessage { get; set; } = string.Empty;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ControlVariables.URL = $"{Request.Scheme}://{Request.Host.Value}";

            StatusMessage = Request.Query["status"].Count > 0 ? Request.Query["status"][0] : string.Empty;
        }
    }
}
