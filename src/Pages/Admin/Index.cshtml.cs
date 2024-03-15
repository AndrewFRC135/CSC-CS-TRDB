using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSCCSTRDB.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace CSCCSTRDB.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public IndexModel(CSCCSTRDB.Models.Database.Open4groupsContext context)
        {
            _context = context;
        }

        public static string CHServerList { get; set; } = default!;

        public string CHPasswordList { get; set; } = default!;

        public string CHConnectionList { get; set; } = default!;

        [BindProperty]
        public int FirstConnectionId { get; set; }

        public async Task OnGetAsync(int FirstConnectionId = 0)
        {
            this.FirstConnectionId = FirstConnectionId;

            var gQuery =
                from g in _context.Groups
                orderby g.Name
                select g;

            if (CHServerList == null)
            {
                CHServerList = "";

                foreach (var group in await gQuery.ToListAsync())
                {
                    CHServerList += "**" + group.Name.ToUpper() + " SERVERS:**\r\n" +
                        "**SERVER 1**\r\n" +
                        "SERVER ADDRESS: customsongscentral.duckdns.org\r\n" +
                        "PORT #: " + group.ChServerPort.ToString() + "\r\n" +
                        "PASSWORD: " + group.ChServerPassword.ToString() + "\r\n" +
                        "\r\n" +
                        "**SERVER 2**\r\n" +
                        "SERVER ADDRESS: customsongscentral.duckdns.org\r\n" +
                        "PORT #: " + (group.ChServerPort + 1).ToString() + "\r\n" +
                        "PASSWORD: " + group.ChServerPassword.ToString() + "\r\n" +
                        "\r\n" +
                        "\r\n";
                }
            }

            CHConnectionList = "";

            FirstConnectionId--;

            foreach (var group in await gQuery.ToListAsync())
            {
                FirstConnectionId++;
                CHConnectionList += "server" + FirstConnectionId.ToString() + " = customsongscentral.duckdns.org:" + group.ChServerPort.ToString() + "\r\n";

                FirstConnectionId++;
                CHConnectionList += "server" + FirstConnectionId.ToString() + " = customsongscentral.duckdns.org:" + (group.ChServerPort + 1).ToString() + "\r\n";
            }

            CHPasswordList = "";

            foreach (var group in await gQuery.ToListAsync())
            {
                CHPasswordList += "Password for " + group.Name.ToString() + ": " + group.ChServerPassword + "\r\n";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // query list of participants ordered first by their bye status, then by qualifier score
            IQueryable<Participant> pQuery =
                from p in _context.Participants
                where p.IsCompetitor == true
                orderby p.ReceivedGroupBye descending, p.QualifierScore descending
                select p;


            List<Participant> pList = await pQuery.ToListAsync();
            int currentQualifierRank = 0;

            foreach (var p in pList)
            {
                currentQualifierRank++;
                p.QualifierRank = currentQualifierRank;

                _context.Attach(p).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();


            // query list of participants who do not have bye's in ranking order
            pQuery =
                from p in _context.Participants
                where p.IsCompetitor == true && p.ReceivedGroupBye == false
                orderby p.QualifierRank ascending
                select p;

            // query list of groups in order
            IQueryable<Group> gQuery =
                from g in _context.Groups
                orderby g.Id ascending
                select g;


            // fetch the list
            pList = await pQuery.ToListAsync();
            List<Group> gList = await gQuery.ToListAsync();


            int currentGroupIndex = -1;
            int currentGroupRank = 1;

            foreach (var p in pList)
            {
                if (currentGroupRank % 2 == 1) 
                {   // current group rank is odd; sweep through 1 to n
                    if (currentGroupIndex >= (gList.Count - 1))
                    {   // currently at the end of the list; advance to the next rank and reverse snake draft
                        currentGroupRank++;
                    }
                    else
                    {   // advance to the next group in the list
                        currentGroupIndex++;
                    }
                }
                else
                {   // current group rank is even; reverse sweep through n to 1
                    if (currentGroupIndex <= 0)
                    {   // currently at the end of the list; advance to the next rank and reverse snake draft
                        currentGroupRank++;
                    }
                    else
                    {   // advance to the next group in the list
                        currentGroupIndex--;
                    }
                }

                // assign the group ID and group Rank to this player
                p.Group = gList[currentGroupIndex];
                p.GroupRank = currentGroupRank;

                _context.Attach(p).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
