using CSCCSTRDB.Models;
using CSCCSTRDB.Models.Database;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Policy;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CSCCSTRDB.Pages
{
    public class RefSheetModel : PageModel
    {
        private readonly CSCCSTRDB.Models.Database.Open4groupsContext _context;

        public Models.MatchModel MatchModel;

        public static SelectList GroupList { get; set; } = default!;
        public SelectList Player1List { get; set; } = default!;
        public SelectList Player2List { get; set; } = default!;
        public SelectList RefereeList { get; set; } = default!;

        [BindProperty]
        public int GroupId { get; set; }
        [BindProperty]
        public int Player1Id { get; set; }
        [BindProperty]
        public int Player2Id { get; set; }
        [BindProperty]
        public int RefereeId { get; set; }

        [BindProperty]
        public int MatchId { get; set; }

        public RefSheetModel(Open4groupsContext context)
        {
            _context = context;
            MatchModel = new MatchModel(_context);

            if (GroupList is null || GroupList.ToList().Count() == 0)
            {   // setlist not populated; populate it now!

                // query for all non-tiebreak songs ordered by focus, then id
                var gQuery =
                    from g in _context.Groups
                    orderby g.Id
                    select g;

                // populate the Group List and cache the result
                GroupList = new SelectList(gQuery.AsEnumerable(), nameof(Models.Database.Group.Id), nameof(Models.Database.Group.Name));
            }

            // get the list of refs
            var pQuery =
                from p in _context.Participants
                where p.IsReferee == true
                orderby p.Name
                select p;

            // create the select list
            RefereeList = new SelectList(pQuery.AsEnumerable(), nameof(Participant.Id), nameof(Participant.Name));
        }

        public async Task OnGetAsync(int GroupId = 0, int Player1Id = 0, int Player2Id = 0, int RefereeId = 0, int MatchId = 0)
        {
            if (MatchId == 0)
            {   // the match ID is unknown; fill in the select boxes
                if (GroupId == 0)
                {   // no group selected yet; return
                    return;
                }
                // save the value
                this.GroupId = GroupId;

                var p1Query =
                    from p in _context.Participants
                    where p.IsCompetitor == true && p.GroupId == GroupId
                    orderby p.GroupRank, p.QualifierRank, p.Id
                    select p;

                var p1List = await p1Query.ToListAsync();

                Player1List = new SelectList(p1List, nameof(Participant.Id), nameof(Participant.Name));

                // check if the provided player index exists in this list
                if (!p1List.Exists(p => p.Id == Player1Id))
                {   // no player 1 selected yet; return
                    return;
                }
                // save this value
                this.Player1Id = Player1Id;

                var p2Query =
                    from p in p1List
                    where p.GroupRank > p1List.First(q => q.Id == Player1Id).GroupRank
                    select p;

                var p2List = p2Query.ToList();

                Player2List = new SelectList(p2List, nameof(Participant.Id), nameof(Participant.Name));

                if (!p2List.Exists(p => p.Id == Player2Id))
                {   // no player 2 selected yet; return
                    return;
                }
                // save the player 2 Id
                this.Player2Id = Player2Id;

                // search for a pre-existing match
                this.MatchId = await MatchModel.FindMatch(GroupId, Player1Id, Player2Id);

                if (this.MatchId == 0)
                {   // no match exists yet
                    this.RefereeId = RefereeId;
                }
                else
                {   // match does exist
                    this.RefereeId = MatchModel.Match.RefereeId;
                }
            }
            else
            {
                this.MatchId = MatchId;
            }

            if (this.MatchId > 0)
            {   // a match is selected; load it
                await MatchModel.LoadMatch(this.MatchId);
            }

        }
        public async Task OnPostStartMatchAsync(int GroupId = 0, int Player1Id = 0, int Player2Id = 0, int RefereeId = 0)
        {
            this.GroupId = GroupId;
            this.Player1Id = Player1Id;
            this.Player2Id = Player2Id;
            this.RefereeId = RefereeId;

            // generate a new match entry
            this.MatchId = (await MatchModel.NewMatch(GroupId, Player1Id, Player2Id, RefereeId)).Id;

            // load this new match entry now
            await MatchModel.LoadMatch(this.MatchId);
        }

        public async Task OnPostUpdateMatchAsync(int MatchId = 0, int Player1Selection = 0, int Player2Selection = 0, int RoundWinnerSelection = 0)
        {
            this.MatchId = MatchId;

            // load this match's current state
            await MatchModel.LoadMatch(this.MatchId);

            if (MatchModel.Phase == MatchModel.MatchState.AwaitingPlayer1Ban && Player1Selection > 0)
            {   // received player 1's ban, apply and save to database
                MatchModel.Match.Player1BanSongId = Player1Selection;
                await MatchModel.SaveMatch();
            }
            else if (MatchModel.Phase == MatchModel.MatchState.AwaitingPlayer2Ban && Player2Selection > 0)
            {   // received player 1's ban, apply and save to database
                MatchModel.Match.Player2BanSongId = Player2Selection;
                await MatchModel.SaveMatch();
            }
            else if (MatchModel.Phase == MatchModel.MatchState.AwaitingPlayer1Pick && Player1Selection > 0)
            {   // received a pick from player 1, add this new round
                await MatchModel.NewRound(Player1Selection, 1);
            }
            else if (MatchModel.Phase == MatchModel.MatchState.AwaitingPlayer2Pick && Player2Selection > 0)
            {   // received a pick from player 1, add this new round
                await MatchModel.NewRound(Player2Selection, 2);
            }
            else if (MatchModel.Phase == MatchModel.MatchState.AwaitingRoundResult && RoundWinnerSelection > 0)
            {   // received a round winner selection
                var round = MatchModel.GetCurrentRound();
                round.WinningPlayerNum = RoundWinnerSelection;
                await MatchModel.SaveRound(round);

                if (MatchModel.Match.Player1WinCount >= 4)
                {   // player 1 has won! match over
                    MatchModel.Match.WinningPlayerNum = 1;
                    await MatchModel.SaveMatch();
                }
                else if (MatchModel.Match.Player2WinCount >= 4)
                {   // player 2 has won! match over
                    MatchModel.Match.WinningPlayerNum = 2;
                    await MatchModel.SaveMatch();
                }
                else if (MatchModel.Match.Player1WinCount == 3 && MatchModel.Match.Player2WinCount == 3)
                {   // tiebreak time! auto choose the tie breaker
                    int soloSongCount = 0, strumSongCount = 0;
                    int tiebreakSongId = 0;

                    foreach (Round r in MatchModel.Match.Rounds)
                    {
                        if (r.PickedSong!.Focus.Name.Contains("Solo"))
                        {
                            soloSongCount++;
                        }
                        else if (r.PickedSong!.Focus.Name.Contains("Strum"))
                        {
                            strumSongCount++;
                        }
                    }

                    if (soloSongCount > strumSongCount)
                    {   // more solos played, so strum tiebreak
                        tiebreakSongId = (await _context.Foci.FirstAsync(f => f.Name.Contains("Strum"))).TiebreakSongId!.Value;
                    }
                    else if (soloSongCount < strumSongCount)
                    {   // more strums played, so solo tiebreak
                        tiebreakSongId = (await _context.Foci.FirstAsync(f => f.Name.Contains("Solo"))).TiebreakSongId!.Value;
                    }
                    else
                    {   // balanced picks, so hybrid tiebreak
                        tiebreakSongId = (await _context.Foci.FirstAsync(f => f.Name.Contains("Hybrid"))).TiebreakSongId!.Value;
                    }

                    await MatchModel.NewRound(tiebreakSongId);
                }
            }
        }

        public async Task OnPostUndoMatchAsync(int MatchId = 0)
        {
            this.MatchId = MatchId;

            // load this match's current state
            await MatchModel.LoadMatch(this.MatchId);
        }
    }
}
