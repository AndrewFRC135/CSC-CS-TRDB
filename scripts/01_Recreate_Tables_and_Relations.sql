/* Author: Andrew Whiteman
 * Date: 2/11/2024
 * File: 01_Create_Data_Tables_and_Relations.sql
 * 
 * Description: Creates the database schema to organize the involed participants
 *
 */
 
# This block drops all the new table names before creating the schema
SET FOREIGN_KEY_CHECKS=0;

DROP TABLE IF EXISTS `round`;
DROP TABLE IF EXISTS `match`;
DROP TABLE IF EXISTS `song`;
DROP TABLE IF EXISTS `focus`;
DROP TABLE IF EXISTS `participant`;
DROP TABLE IF EXISTS `group`;

SET FOREIGN_KEY_CHECKS=1;

# Create a table to manage each group
CREATE TABLE `group` (
  `id` integer NOT NULL AUTO_INCREMENT,				# Sequential ID number (auto generated)
  `name` varchar(255) NOT NULL,						# Name of the Group
  `discord_channel_id` varchar(255) DEFAULT NULL,	# Discord ID of this group's channel (for bot integration ?)\
  `ch_server_port` integer DEFAULT NULL,			# The port number for this group's clone hero server(s).
  `ch_server_password` varchar(255) DEFAULT NULL,	# The password to access this group's clone hero server(s)
  PRIMARY KEY (`id`),
  KEY (`discord_channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

# Keep track of all participants (both competing and non-competing)
CREATE TABLE `participant` (
  `id` integer NOT NULL AUTO_INCREMENT,			# Sequential ID number (auto generated)
  `name` varchar(255) NOT NULL,					# Participant Name
  `pronouns` varchar(255) DEFAULT NULL,			# Participant's Preferred Pronouns
  `discord_id` varchar(255) DEFAULT NULL,		# Discord ID of this player (for bot integration ?)
  `is_competitor` boolean DEFAULT true,			# set this to TRUE if this user is competing; FALSE otherwise (Default is TRUE)
  `qualifier_rank` integer DEFAULT NULL,		# Rank positon this player received on the qualifier chart
  `qualifier_score` integer DEFAULT NULL,		# Submitted score this player acheived on the qualifier chart
  `advanced_to_playoff` boolean DEFAULT false,	# Set to true if a player has advanced into playoffs
  `received_group_bye` boolean DEFAULT false,	# Set to true if this user received a bye
  `is_referee` boolean DEFAULT false,			# set this to TRUE if this user is a verified trained referee; FALSE otherwise (Default is TRUE)
  `group_id` integer DEFAULT NULL,				# if this user is assigned to a group, this references their group number
  `group_rank` integer DEFAULT NULL,			# if this user is assigned to a group, this is their relaive rank in that group (pot number)
  
  PRIMARY KEY (`id`),
  KEY (`name`),
  KEY (`discord_id`),
  FOREIGN KEY (`group_id`) REFERENCES `group`(`id`) 
	ON UPDATE CASCADE
    ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `focus` (
  `id` integer NOT NULL AUTO_INCREMENT,			# Sequential ID number (auto generated)
  `name` varchar(255) NOT NULL,					# name of this song focus type
  `tiebreak_song_id` integer DEFAULT NULL,		# specify which song serves as the tiebreak for this focus
  PRIMARY KEY (`id`),
  KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


INSERT INTO `focus` (`name`)
VALUES
('Solo'),  	# Solo Focus Group (ID #1)
('Strum'), 	# Strum Focus Group (ID #2)
('Hybrid');	# Hybrid Focus Group (ID #3)


# Stores the information about each song available to select/ban
CREATE TABLE `song` (
  `id` integer NOT NULL AUTO_INCREMENT,			# Sequential ID number (auto genrated
  `name` varchar(255) NOT NULL,					# The name of the song
  `artist` varchar(255) NOT NULL,				# The artist(s) of the song
  `modifiers` varchar(255) DEFAULT NULL,		# The required list of modifiers (if any)
  `charter` varchar(255) NOT NULL,				# The charter(s) of this song
  `source` varchar(255) NOT NULL,				# The release source of this song
  `focus_id` integer NOT NULL,					# the focus group that this song belongs to
  PRIMARY KEY (`id`),
  KEY (`name`),
  FOREIGN KEY (`focus_id`) REFERENCES `focus`(`id`) 
	ON UPDATE CASCADE
    ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

# add the foreign key constraint to the song table
ALTER TABLE `focus`
  ADD FOREIGN KEY (`tiebreak_song_id`) REFERENCES `song`(`id`) 
    ON UPDATE CASCADE
    ON DELETE SET NULL;    
    
CREATE TABLE `match` (
  `id` integer NOT NULL AUTO_INCREMENT,			# Sequential ID number (auto generated)
  `start_timestamp` timestamp NOT NULL DEFAULT current_timestamp(), # timestamp of match record creation
  `group_id` integer NOT NULL,					# group for this match
  `player1_id` integer NOT NULL,				# player 1 (higher seed)
  `player2_id` integer NOT NULL,				# player 2 (lower seed)
  `referee_id` integer NOT NULL,				# referee
  `player1_ban_song_id` integer DEFAULT NULL,			# player 1's song ban
  `player2_ban_song_id` integer DEFAULT NULL,			# player 2's song ban
  `player1_win_count` integer NOT NULL DEFAULT 0,		# The number of rounds (songs) this player won during the match
  `player2_win_count` integer NOT NULL DEFAULT 0,		# The number of rounds (songs) this player won during the match
  `winning_player_num` integer DEFAULT NULL,			# The player number who won this match
  `end_timestamp` timestamp DEFAULT NULL,		# match winning time
  `referee_notes` text DEFAULT NULL,			# any typed notes from the referee
  
  PRIMARY KEY (`id`),
  FOREIGN KEY (`group_id`) REFERENCES `group`(`id`)
	ON UPDATE CASCADE
	ON DELETE RESTRICT,
  FOREIGN KEY (`player1_id`) REFERENCES `participant`(`id`)
	ON UPDATE CASCADE
	ON DELETE RESTRICT,
   FOREIGN KEY (`player2_id`) REFERENCES `participant`(`id`)
	ON UPDATE CASCADE
	ON DELETE RESTRICT,
  FOREIGN KEY (`referee_id`) REFERENCES `participant`(`id`)
	ON UPDATE CASCADE
	ON DELETE RESTRICT,
  FOREIGN KEY (`player1_ban_song_id`) REFERENCES `song`(`id`)
    ON UPDATE CASCADE
	ON DELETE RESTRICT,
  FOREIGN KEY (`player2_ban_song_id`) REFERENCES `song`(`id`)
    ON UPDATE CASCADE
	ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `round` (
  `match_id` integer NOT NULL,					# Match ID Number
  `round_num` integer NOT NULL,					# Round number within this match
  `seq_num` integer NOT NULL DEFAULT 1,			# usually always 1, but if a song is re-done/restarted this should be incremented
  `timestamp` timestamp NOT NULL DEFAULT current_timestamp(), # timestamp of round record creation
  `picking_player_num` integer DEFAULT NULL,	# The player who gets to pick this round (1, 2, or NULL if no player gets to choose (tie breaker))
  `picked_song_id` integer DEFAULT NULL,		# The song that was chosen
  `results_screenshot` mediumblob DEFAULT NULL, 	# submitted in-game screenshot (16MB or less!)
  `player1_score` integer DEFAULT NULL,			# score player 1 earned
  `player2_score` integer DEFAULT NULL,			# score player 2 earned
  `winning_player_num` integer DEFAULT NULL,	# the player who won!
  `forefit_player_num` integer DEFAULT NULL,	# if a player disconnects/forefits the match, they are noted here. Otherwise leave as null
  `referee_notes` text DEFAULT NULL,			# any typed notes from the referee

  PRIMARY KEY (`match_id`, `round_num`, `seq_num`),
  FOREIGN KEY (`match_id`) REFERENCES `match`(`id`)
    ON UPDATE CASCADE
	ON DELETE CASCADE,
  FOREIGN KEY (`picked_song_id`) REFERENCES `song`(`id`)
    ON UPDATE CASCADE
	ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8;