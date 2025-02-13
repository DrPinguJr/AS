using dit230680c_AS.Data;
using dit230680c_AS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dit230680c_AS.Pages
{
    public class AuditLogsModel : PageModel
    {
        private readonly AppDbContext _context;

        public AuditLogsModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<AuditLog> AuditLogs { get; set; }

        public async Task OnGetAsync()
        {
            AuditLogs = await _context.AuditLogs
                                       .OrderByDescending(log => log.Timestamp)
                                       .Take(100) // Fetch the latest 100 logs
                                       .ToListAsync();
        }
    }
}
