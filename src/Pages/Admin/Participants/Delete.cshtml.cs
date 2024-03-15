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
    public class DeleteModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public DeleteModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants.FindAsync(id);
            if (participant != null)
            {
                Participant = participant;
                _context.Participants.Remove(Participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
