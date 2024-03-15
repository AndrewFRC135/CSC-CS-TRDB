using CSCCSTRDB.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CSCCSTRDB.Pages.Participants
{
    public class IndexModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public IndexModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
        }

        public IList<Participant> pList { get; set; } = default!;
        public int maxSeed { get; set; } = 0;
        public int sortCol { get; set; } = 0;

        public async Task OnGetAsync(int sort_col)
        {
            IQueryable<Participant> pQuery = 
                from p in _context.Participants
                where p.IsCompetitor == true
                select p;

            sortCol = sort_col;

            switch (sort_col)
            {
                case 1:
                    pQuery = pQuery.OrderBy(p => p.Name.ToUpper());
                    break;
                
                case 3:
                    pQuery = pQuery.OrderByDescending(p => p.QualifierScore);
                    break;
                case 4:
                    pQuery = pQuery.OrderBy(p => (p.GroupId == null)).ThenBy(p => p.GroupId).ThenBy(p => p.GroupRank).ThenBy(p => p.QualifierRank);
                    break;

                default:
                    sortCol = 2;
                    pQuery = pQuery.OrderBy(p => p.QualifierRank);
                    break;
            }

            pList = await pQuery.Include(p => p.Group).ToListAsync();

            maxSeed = pList.Max(p => p.GroupRank) ?? 0;
        }
    }
}
