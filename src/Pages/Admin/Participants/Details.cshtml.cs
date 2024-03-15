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
    public class DetailsModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public DetailsModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
        }

        public Participant Participant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants.FirstOrDefaultAsync(m => m.Id == id);
            if (participant == null)
            {
                return NotFound();
            }
            else
            {
                Participant = participant;
            }
            return Page();
        }
    }
}
