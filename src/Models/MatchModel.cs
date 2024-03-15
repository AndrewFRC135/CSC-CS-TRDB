using CSCCSTRDB.Models.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;
using System.Collections;
using System.Collections.Concurrent;
using System.Configuration;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CSCCSTRDB.Models
{
    public class MatchModel
    {
        // reference to the database context
        private readonly Open4groupsContext _context;

        // store the state of the database
        public enum MatchState
        {
            NotStarted = 0,
            AwaitingPlayer1Ban = 1,         // Player gets to pick their ban
			AwaitingPlayer2Ban = 2,
			AwaitingPlayer1Pick = 3,        // Player gets to pick next rounds's song
			AwaitingPlayer2Pick = 4,
            AwaitingRoundResult = 5,        // Round is on-going, results not yet submitted
            AwaitingRoundScores = 6,        // Round result reported by ref, but scores not uploaded yet
            Completed = 7,                  // this match is done
            Error = 99                      // something went wrong
        }

		/// <summary>
        /// Song info contains the setlist for this tournament
        /// </summary>
		public class SongInfo
		{
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="songDb">The song database entry to use to populate this info. Include the Focus reference with this information</param>
            public SongInfo(Song songDb)
            {
                Id = songDb.Id;
                Name = songDb.Name;
                Artist = songDb.Artist;
                Modifiers = songDb.Modifiers ?? ""; // if no modifiers, empty string
                Charter = songDb.Charter;
                Source = songDb.Source;
                FocusId = songDb.FocusId;
                FocusName = songDb.Focus.Name;
                isTiebreak = (songDb.Focus.TiebreakSongId == songDb.Id); // song is a tiebreak if the ID's match
                isBoss = false; // TODO: Implement boss logic
            }

            public int Id { get; }
			public string Name { get; } = "";
			public string Artist { get; } = "";
			public string Modifiers { get; } = "";
			public string Charter { get; } = "";
			public string Source { get; } = "";
			public int FocusId { get; } = 0;
			public string FocusName { get; } = "";
			public bool isTiebreak { get; } = false;
            public bool isBoss { get; } = false;
		};

		/// <summary>
		/// A song state is information specific to each song in the current match
		/// </summary>
		public class SongState
		{
            public enum Player
            {
                None = 0,       // This song has not been banned/picked/won yet
                Player1 = 1,    // This song has been banned/picked/won by Player1
                Player2 = 2,    // This song has been banned/picked/won by Player2
                Math = 3        // This song has been banned/picked by math (tiebreaker seletion)
            }					    // NOTE: A song cannot be won by math
			public enum Tiebreaker
			{
				NormalOrNone = 0,   // this is not a tiebreaker, or it is not leaning nor leaning
				Leaning = 2,        // this tiebreaker is likely by math
				Locked = 3,         // this tiebreaker is locked by math
			}

			public int Id { get; } = 0;
            public int RoundNum { get; } = 0;
			public Player BannedByPlayer { get; } = Player.None;
			public Player PickedByPlayer { get; } = Player.None;
			public Player WonByPlayer { get; } = Player.None;
			public Tiebreaker TiebreakerStatus { get; } = Tiebreaker.NormalOrNone;

			public SongState(int id)
			{
				Id = id;
			}
		}

        // Publically accessible information about the match
        public int Id { get; } = 0;
        public int GroupId { get;  } = 0;
        public string GroupName { get; } = "";
        public int Player1Id { get; } = 0;
        public string Player1Name { get; } = "";
		public string Player1Pronouns { get; } = "";
		public int Player1Rank { get; } = 0;
        public int Player1Seed { get; } = 0;
		public int Player2Id { get; } = 0;
		public string Player2Name { get; } = "";
		public string Player2Pronouns { get; } = "";
		public int Player2Rank { get; } = 0;
		public int Player2Seed { get; } = 0;
        public int RefereeId { get; } = 0;
		public string RefereeName { get; } = "";
		public string RefereePronouns { get; } = "";
        public MatchState state { get; } = MatchState.NotStarted;
        public List<SongInfo> SongInfoList { get; } = new List<SongInfo>();
		public List<SongState> SongStateList { get; } = new List<SongState>();


		/// <summary>
		/// Call this funciton instead of the constructor
		/// </summary>
		/// <param name="context">The database context to use</param>
		/// <param name="isReadOnly">Set this value to false if this is the referee's input screen (shows radio buttons)</param>
		/// <param name="matchId">If specified, the match information to load</param>
		async public static Task<MatchModel> BuildMatchModelAsync(Open4groupsContext context, bool isReadOnly = true, int matchId = 0)
        {   
            // Generate the setlist
            // query for all non-tiebreak songs ordered by focus, then by id
            var setListQuery = (
                from s in context.Songs
                join f in context.Foci on s.FocusId equals f.Id
                where f.TiebreakSongId != s.Id
                orderby s.FocusId, s.Id
                select s
            // join the tiebreak songs, only by id
            ).Union(
                from s in context.Songs
                join f in context.Foci on s.FocusId equals f.Id
                where f.TiebreakSongId == s.Id
                orderby s.FocusId, s.Id
                select s
            );

            // get the setlist songs (and foci)
            var setList = await setListQuery.Include(s => s.Focus).ToListAsync();

            foreach (var song in setList)
            {   // populate the song info
                SongInfo info = new SongInfo (song);
            }

            // clear the SetListCache
            SetList.Clear();
            foreach (var song in setList)
            {   // add all setlist songs to the cache
                SetList.Add(new SetlistItem 
                { 
                    Data = song, 
                    isTiebreak = (song.Focus.TiebreakSongId == song.Id) // this song is tiebreak if it's ID matches the one specified by it's focus
                });
            }

            if (matchId != 0)
            {   // a match ID was provided, try to find it
                bool matchCacheLockAcquired = false;

				// check if this match is listed in the cache
				match = MatchList.GetValueOrDefault(matchId)!;

                if (match != null)
                {   // a match was found in cache, verify it is current
                    // try to acquire the match cache mutex lock
                    matchCacheLockAcquired = match.Lock.WaitOne(1000);

                    if (matchCacheLockAcquired)
					{   // *** LOCK ACQUIRED ***
						// calculate the age of this cache entry in ticks
						long matchCacheTTL = ActiveMatchListTTL; // assume this is an active match as first for TTL

						if (match.State == MatchState.NotStarted || match.State == MatchState.Completed)
						{   // match is inactive
							matchCacheTTL = InactiveMatchListTTL;
						}

						if (DateTime.UtcNow.Ticks - match.RetrievalTick < matchCacheTTL)
						{   // match cache entry is not stale, use the cached value
							// release the mutex
							match.Lock.ReleaseMutex();
							// *** LOCK RELEASED ***

							// return cached value
							return (new MatchModel(context, isReadOnly, matchId, match));
						}
					}
                }
                else
                {   // the match was not found in cache; create an empty match object and acquire it's mutex
                    match = new MatchListItem();
					// try to acquire the mutex for this new cache entry
					matchCacheLockAcquired = match.Lock.WaitOne();
					// *** LOCK ACQUIRED ***

					// add or update the retrieved match value into the list
					if (MatchList.TryAdd(matchId, match) == false)
                    {   // could not add this new entry to the cache
                        match.Lock.ReleaseMutex();
                        matchCacheLockAcquired = false;
						// *** LOCK RELEASED ***
					}
				}

				// at this point, either we have acquired the cache lock and need to retrieve the latest value
                // from the database, or we don't and need to not use the cache at all

                // asyncronous query the matches to find the correct one by the provided ID
				var newMatchData = await context.Matches
                    .Include(m => m.Group)              // a lot of objects are joined to this match,
                    .Include(m => m.Player1)            // this is a slow operation, and why it should be cached
                    .Include(m => m.Player2)
                    .Include(m => m.Referee)
                    .Include(m => m.Rounds)
                        .ThenInclude(r => r.PickedSong)
                            .ThenInclude(s => s!.Focus)
                    .FirstOrDefaultAsync(m => m.Id == matchId); // get only the match with the *matching* id

                if (newMatchData != null)
				{   // found match data for this id
                    // generate this new match data
					var newMatch = new MatchListItem()
                    {
                        Data = newMatchData,
                        State = MatchState.NotStarted, // TODO: match phase routine calculator
                    };

                    if (matchCacheLockAcquired)
                    {   // we have the cache lock; update the values in the cached object and release the lock
                        match.Data = newMatch.Data;
                        match.State = newMatch.State;

						match.Lock.ReleaseMutex();
						matchCacheLockAcquired = false;
                        // *** LOCK RELEASED ***

                        // return the constructed object
                        return new MatchModel(context, isReadOnly, matchId, match);
					}
                    else
                    {   // we never had the cache lock, so return the new match object outside of cache
						return new MatchModel(context, isReadOnly, matchId, newMatch);
					}
				}
                else
                {   // match does not exist (or no longer exists)
					if (matchCacheLockAcquired)
					{   // we have the cache lock; update the values in the cached object and release the lock

                        MatchListItem removedMatch;
                        
                        if (MatchList.TryRemove(matchId, out removedMatch!))
                        {   // the object was removed from the cache successfully
                            removedMatch.Lock.ReleaseMutex();
							matchCacheLockAcquired = false;
							// *** LOCK RELEASED ***
						}
					}
					// return the constructed object with no match data
					return new MatchModel(context, isReadOnly, matchId, match);
				}
			}
			// return the constructed object with no match data
			return (new MatchModel(context, isReadOnly, matchId));
		}

        private MatchModel(Open4groupsContext context, bool isReadOnly, int matchId, MatchListItem match = null!)
        {
            _context = context;
            IsReadOnly = isReadOnly;
            _matchId = matchId;
            Match = match;
        }

		/// <summary>
		/// Given the provided Group, Player 1, and Player 2, find if a Match for this already exists
		/// </summary>
		/// <returns>true if match is found; false otherwise</returns>
		public async Task<int> FindMatch(int groupId, int player1Id, int player2Id)
        {
            //// search for any matches with this group, player1, player2 combination
            //var mQuery =
            //    from m in _context.Matches
            //    where m.GroupId == groupId &&
            //        m.Player1Id == player1Id &&
            //        m.Player2Id == player2Id
            //    orderby m.Id descending // the first one in the list will be the largest ID value (newest one)
            //    select m;

            //// run query
            //Match = (await mQuery.FirstOrDefaultAsync())!;

            //if (Match != null)
            //{   // found the match; return its ID
            //    return Match.Id;
            //}
            // no match found; return zero
            return 0;
		}

        public async Task<Database.Match> NewMatch(int groupId, int player1Id, int player2Id, int refereeId)
        {
            //Match = new Database.Match();

            //// create a new match
            //Match.GroupId = groupId;
            //Match.Player1Id = player1Id;
            //Match.Player2Id = player2Id;
            //Match.RefereeId = refereeId;
            //Match.StartTimestamp = DateTime.UtcNow;

            //// add the match to the database and sa ve changes
            //_context.Matches.Add(Match);
            //await _context.SaveChangesAsync();

            //// return the new match
            //return Match;

            return null;
        }

        public async Task<MatchState> LoadMatch(int matchId)
        {
            //// find the match by ID
            //var mQuery =
            //from m in _context.Matches
            //    where m.Id == matchId
            //    orderby m.Id descending // the first one in the list will be the largest ID value (newest one)
            //    select m;

            //// get the first matching record
            //Match = (await mQuery.Include(m => m.Group)
            //    .Include(m => m.Player1)
            //    .Include(m => m.Player2)
            //    .Include(m => m.Referee)
            //    .Include(m => m.Rounds)
            //        .ThenInclude(r => r.PickedSong)
            //            .ThenInclude(s => s!.Focus)
            //    .FirstOrDefaultAsync())!;

            return UpdatePhase();
		}

        /// <summary>
        /// Save the changes to the Match database entry only
        /// </summary>
        /// <returns></returns>
        public async Task<MatchState> SaveMatch()
        {
            //if (Match == null)
            //{   // No match data to save
            //    return Phase;
            //}

            // add the match to the database and sa ve changes
            _context.Attach(Match).State = EntityState.Modified;
            await _context.SaveChangesAsync();

			return UpdatePhase();
		}

        /// <summary>
        /// Update the match phase variable
        /// </summary>
        private MatchState UpdatePhase()
        {
			//if (Match == null)
			//{   // no match located; return not started/does not exist
			//	return Phase = MatchPhase.NotStartedOrError;
			//}
			//// determine the current match phase

			//// first, check if this match is already over
			//if (Match.WinningPlayerNum != null)
			//{   // it is
			//	return Phase = MatchPhase.Completed;
			//}

			//// next, check the match setup data
			//if (Match.Player1BanSongId == null)
			//{   // player 1's ban not in yet
			//	return Phase = MatchPhase.AwaitingPlayer1Ban;
			//}
			//if (Match.Player2BanSongId == null)
			//{   // player 2's ban not in yet
			//	return Phase = MatchPhase.AwaitingPlayer2Ban;
			//}
			//if (Match.Rounds.Count == 0)
			//{   // no rounds started yet; player 1 must get to pick
			//	return Phase = MatchPhase.AwaitingPlayer1Pick;
			//}

			//// at this point, we know that as least one round exists
			//// get the most recent round to determine the match phase
			//var latestRound = Match.Rounds.MaxBy(r => r.RoundNum);

			//if (latestRound?.PickedSongId == null)
			//{   // song not yet picked, see who's picking
			//	if (latestRound?.PickingPlayerNum == null)
			//	{   // no player is picking; an error has occured!
			//		return Phase = MatchPhase.NotStartedOrError;
			//	}
			//	if (latestRound.PickingPlayerNum == 1)
			//	{   // player 1 gets to pick
			//		return Phase = MatchPhase.AwaitingPlayer1Pick;
			//	}
			//	// otherwise; it must be player 2's pick
			//	return Phase = MatchPhase.AwaitingPlayer2Pick;
			//}

			//// there must be a picked song ID if this point is reached; check on result
			//if (latestRound?.WinningPlayerNum == null)
			//{   // the winning player has not yet been selected
			//	return Phase = MatchPhase.AwaitingRoundResult;
			//}


   //         if (latestRound.WinningPlayerNum == 1)
   //         {
   //             return Phase = MatchPhase.AwaitingPlayer2Pick;
   //         }
			//// the winner is known, but scores must not be saved yet
			//return Phase = MatchPhase.AwaitingPlayer1Pick;

            return MatchState.NotStarted;
		}

        public async Task<Database.Round> NewRound(int songId, int picking_player_num = 0)
        {
            //         if (Match == null)
            //         {   // no active match; cannot add a round
            //             return null!;
            //         }

            //         // get the next round number
            //         int newRoundNum = 1;

            //         if (Match.Rounds.Count > 0)
            //         {   // find new round num from largest previous
            //             newRoundNum = Match.Rounds.Max(r => r.RoundNum) + 1;
            //         }

            //         // create the new round
            //         var newRound = new Database.Round();

            //         // apply known parameters
            //         newRound.MatchId = Match.Id;
            //         newRound.RoundNum = newRoundNum;
            //         newRound.PickedSongId = songId;
            //         newRound.SeqNum = 1; // reserved for future use
            //         if (picking_player_num != 0)
            //         {   // if zero, this was a forced tiebreaker
            //	newRound.PickingPlayerNum = picking_player_num;
            //}
            //         newRound.Timestamp = DateTime.UtcNow;

            //         // add the match to the database and save changes
            //         _context.Rounds.Add(newRound);
            //         await _context.SaveChangesAsync();

            //         // need to reload match data structure
            //         await LoadMatch(Match.Id);

            //         // return the new match
            //         return newRound;
            return null;
        }

        /// <summary>
        /// Return the current active round
        /// </summary>
        /// <returns></returns>
        public Database.Round GetCurrentRound()
        {
            //if (Match == null || Match.Rounds.Count <= 0)
            //{   // no active match; cannot add a round
            //    return null!;
            //}

            //return Match.Rounds.OrderByDescending(r => r.RoundNum).First();

            return null;
        }

        /// <summary>
        /// Save the changes to the Match database entry only
        /// </summary>
        /// <returns></returns>
        public async Task<MatchState> SaveRound(Round round)
        {
            //if (Match == null)
            //{   // No match data to save
            //    return Phase;
            //}

            //int player1wins = 0, player2wins = 0;
            
            //foreach (var r in Match.Rounds)
            //{
            //    if (r.WinningPlayerNum == 1)
            //    {
            //        player1wins++;
            //    }
            //    else if (r.WinningPlayerNum == 2)
            //    {
            //        player2wins++;
            //    }
            //}

            //Match.Player1WinCount = player1wins;
            //Match.Player2WinCount = player2wins;

            //// add the match to the database and sa ve changes
            //_context.Attach(Match).State = EntityState.Modified;
            //_context.Attach(round).State = EntityState.Modified;
            //await _context.SaveChangesAsync();

            return UpdatePhase();
        }
    }
}
