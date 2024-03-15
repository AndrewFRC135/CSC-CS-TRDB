using CSCCSTRDB.Models;
using CSCCSTRDB.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CSCCSTRDB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public MatchModel MatchModel { get; set; }
        public IndexModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
            MatchModel = new MatchModel(_context);
        }

        public IList<Participant> pList { get; set; } = default!;
        public int maxSeed { get; set; } = 0;
        public int sortCol { get; set; } = 0;

        [BindProperty]
        public int MatchId { get; set; } = 0;

        public async Task OnGetAsync(int MatchId)
        {
            this.MatchId = MatchId;

            await MatchModel.LoadMatch(MatchId);
        }
    }
}
