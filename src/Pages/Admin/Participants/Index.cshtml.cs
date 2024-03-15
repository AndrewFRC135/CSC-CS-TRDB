using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSCCSTRDB.Models.Database;

namespace CSCCSTRDB.Pages.Admin.Participants
{
    public class IndexModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public IndexModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
        }

        public IList<Participant> Participant { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Participant = await _context.Participants
                .Include(p => p.Group).ToListAsync();
        }
    }
}
