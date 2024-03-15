/* Author: Andrew Whiteman
 * Date: 2/11/2024
 * File: 02_Create_Data_Tables_and_Relations.sql
 * 
 * Description: Creates the database schema to organize the involed participants
 *
 */
 
 # Add Participants that either have byes, are competing, and/or an official referee
INSERT INTO `participant` (`name`, `pronouns`, `qualifier_rank`, `qualifier_score`, `received_group_bye`, `is_competitor`, `is_referee`)
VALUES
# These competitors received a bye and are referees
('jrh_62','she/they',2,1352280, true, true, true),
('symphony_x (Brad)','he/him',4,1350272, true, true, true),
('Lilithuwu','She/her',8,1348252, true, true, true),
('RandomDays','he/they',9,1347400, true, true, true),
('Hodor1355','He/They',39,1266269, true, true, true),

# These competitors received a bye and are not referees
('FingerQuick',NULL,7,1349484, true, true, false),
('josch404','he/they',12,1345836, true, true, false),
('SkullJoke','She/Her',13,1345644, true, true, false),

# These competitors did not receive a bye, but are referees
('taka_does_stuff','he/him',27,1301161, false, true, true),
('WingsOfSpeed',NULL,5,1349684, false, true, true),
('Craez Firebird','He/Him',11,1346612, false, true, true),
('jdsmitty1','they/them',3,1351232, false, true, true),
('JhavA','he/him',59,1189882, false, true, true),

# These are non-competitors (referee only)
('AtomicGH','He/Him',NULL,NULL, false, false, true),
('Tris','He/Him',NULL,NULL, false, false, true),
('KhromaticScale','She/Her',NULL,NULL, false, false, true),
('3-UP','He/Him',NULL,NULL, false, false, true),
('Real Savage Jef','he/him',NULL,NULL, false, false, true);

# Add Competitors that did not receive a bye
INSERT INTO `participant` (`name`, `pronouns`, `qualifier_rank`, `qualifier_score`)
VALUES
('GiometriQ','He/Him',1,1352512),
('DaMan2413','he/him',6,1349528),
('darkwolf1029','he/him',10,1346664),
('deanozord5000','He/Him',14,1338944),
('Trojans05','He/Him/His',15,1334639),
('xdhaqwen','He/Him',16,1334417),
('sniqma','he/him',17,1332942),
('mchcr125',NULL,18,1332086),
('mjonti912','he/him',19,1322728),
('mufloncito','he/him',20,1320885),
('zoopernickel','they/them',21,1319552),
('Duckkiie','She/Her',22,1316830),
('Spyro (saxmanadam)','He/Him',23,1316829),
('thattaikoguy','He/Him',24,1312188),
('Luis Mureddu','He/Him',25,1309202),
('Final','he/him',26,1306862),
('norrvox','he/him',28,1300432),
('nikkovanessa','he/him',29,1299608),
('Joquanmax','He/him',30,1298933),
('areneternal','She / Her',31,1295723),
('deltarak','she/they',32,1291955),
('guacodi1e','he/him',33,1289648),
('Leftygod999','FOC/EPIC',34,1288687),
('RadRog1','he/him',35,1284832),
('pepelepew000','pepelepew',36,1271758),
('camlikescow','He/Him',37,1269787),
('MyanRatte','He/Him',38,1267904),
('darkbloom187','He/Him',40,1264254),
('CaptnMario','He/Him',41,1261746),
('soulspirit','He/Him',42,1247191),
('xElves','He/him',43,1235543),
('Vark','He/Him',44,1229132),
('Burdee','he/him/his',45,1227121),
('PicklePirate','he/him',46,1226429),
('CheetoSpeedo','He/Him',47,1222597),
('xlumpypotato','He/Him DO NOT call me \"\"they\"\" please.',48,1221005),
('Littlejth','He/Him',49,1217184),
('mechon','he/him',50,1212333),
('serooj','he/him',51,1209525),
('bozygrzmot','he/him',52,1207151),
('WhYYZ','He/Him',53,1204958),
('Buckeye637','He/Him',54,1201921),
('bluebro827','he/him/they',55,1199341),
('CattyAttie','She/Her',56,1196458),
('HhHero','He/Him',57,1192872),
('SaKage','he him',58,1191375),
('hunterm2230',NULL,60,1183870),
('AccuracyBE','he/him',61,1183778),
('pepperoni_playboy','He/Him',62,1164766),
('IamOverlord999','he/him',63,1155873),
('FerchoH4x','he/him',64,1149647),
('markmrm',NULL,65,1145917),
('JoshCH','JoshCH',66,1142632),
('ViLLn','He/Him',67,1141741),
('masonjar13',NULL,68,1141648),
('technoflare','He/Him',69,1138681),
('Wiking','He/Him',70,1132283),
('fabeyon1027','He/Him',71,1129886),
('kostiaf','he/him',72,1129224),
('Jobo69','He/Him',73,1125769),
('acecaptainslow','He/Him',74,1121994),
('Noc1997',NULL,75,1118505),
('TheMisfitPond','He/Him',76,1101549),
('assivore','he/him',77,1095213),
('clintfneastwood','He/Him',78,1094011),
('thegoochspot','he/him',79,1082124),
('julyguy27',NULL,80,1081109),
('LyserStorm','He/Him',81,1078894),
('wakizashi','he/him',82,1077194),
('imcasual92','he/him',83,1077022),
('lostqk','He/Him',84,1070909),
('eating.seeds',NULL,85,1064437),
('baconguy','he',87,1057606),
('katie','she/her',88,1046334),
('09rene09','he/him',89,1026451),
('Solo','He/Him',90,1024311),
('Waffles2005','They/Them',91,1021368),
('Almond or probablyalmond','He/him',92,1019435),
('septox','he/him',93,1018699),
('Py_xel','He-Him',94,1011756),
('Theycallmeham_','He/him',95,1002574),
('Ph3nom3non','He/Him',96,993512),
('AvengedSevenfoldEnthusiast','He/Him',97,992244),
('wesleyjn','He/Him',98,987646),
('DragonPianist','He/Him',99,983364),
('aksfalcon','He/Him They/Them',100,983159),
('.fmtrir','He/Him',101,982639),
('Jambles','He / Him',102,958273),
('Arbit3r','he/him',103,955362),
('Goose','he/him/his',104,946661),
('7wander','He/Him',105,945913),
('xxwhytrymexx','Not sure what this is asking? Name: Tyler',106,938398),
('avillainousone','He/him',107,934524),
('threepercentmilk','He/Him',108,925608),
('MobilaKotila','Blue/mallard',109,925321),
('Koloss09','He/Him',110,908607),
('jayz_','He/Him',111,906713),
('furriichan','She/They',112,905211),
('DemonPossesser',NULL,113,900474),
('stranded42','he/him',114,885496),
('red3517','he/him',115,884840),
('Danfish98','he/him',116,865052),
('skeleshmoke','he/him',117,858177),
('Scrawa','He him',118,855828),
('Laterowlus','he/him',119,852090),
('Drazeb','He/Him',120,840017),
('Spider602','he/him',121,833711),
('randommexican33','He/Him',122,826420),
('nyarlath0tep.','He/They',123,798097),
('DirtyHertz','He/him',124,791383),
('captain_yata','She/Her',125,787606),
('musicnerd69','He/Him ',126,784639),
('gonzalorp','Gon',127,775453),
('gianfranco','He/Him',128,770950),
('Thermikz','He/Him',129,769385),
('eraze13','He/Him',130,736600),
('TheJuggernaut6789','He/Him',131,709228),
('buldy_01','he/him',132,692008),
('Cepitore',NULL,133,676401),
('Andrey1206','He/Him',134,650152),
('maskedkitsune','He/him or They/Them',135,617445),
('Heavy Hand','He / Him',136,557943),
('Niesse','He/Him',137,555723),
('@sonofgod_1998','he/him',138,536234),
('KoffeeKay','They/Them',139,518623),
('zuther1123','He/Him',140,514980),
('miles_finch','he/him',141,511815),
('blackbelt','He/Him',142,494522),
('Smidees','Him/He',143,493596),
('Bxccz','she/her',144,479693),
('dunkalunk117',NULL,145,472389),
('coppolao666','he/him or they/them interchangeably',146,466957),
('Java-12','he/him',147,463297),
('RowdyRandy','He/Him',148,446837),
('zemoo','he/him',149,408497),
('Bird5','he/him',150,392958),
('luxicityy','he/they',151,345951),
('Lunar\'s Z\'s','He/Him',152,336427),
('knoxler','he/him',153,271941),
('syke_l3tdown','he/him',154,197538),
('thedumbconnorfan','he/him',155,179249);

# Add Participants that either have byes, are competing, and/or an official referee
INSERT INTO `group` (`name`,  `ch_server_port`, `ch_server_password`)
VALUES
('Group A', 27540, 'kdh48'),
('Group B', 27542, 'i3w9r'),
('Group C', 27544, 'h468x'),
('Group D', 27546, '92pjk'),
('Group E', 27548, '4c2zh'),
('Group F', 27550, '9mqa4'),
('Group G', 27552, 'zidnc'),
('Group H', 27554, 'h6gpd'),
('Group I', 27556, 'pdhzc'),
('Group J', 27558, 'u6s52'),
('Group K', 27560, 'tnkeh'),
('Group L', 27562, 'rn5wz'),
('Group M', 27564, 'sfaqt'),
('Group N', 27566, '6daek'),
('Group O', 27568, 'r49ph'),
('Group P', 27570, '5pycw'),
('Group Q', 27572, '1km5r'),
('Group R', 27574, 'cdpf8'),
('Group S', 27576, 'skcav'),
('Group T', 27578, '9xuwa'),
('Group U', 27580, 'zmy6d'),
('Group V', 27582, 'i62md'),
('Group W', 27584, 'cbfe8'),
('Group X', 27586, '7m2gh');

INSERT INTO `song` (`name`, `artist`, `modifiers`, `charter`, `source`, `focus_id`)
VALUES
('Tilt', 'Richie Kotzen, Greg Howe', NULL, 'Tris255', 'CTH3 DLC1', 1),
('The Second Loudest Guitar in the World', 'Paul Gilbert', NULL, 'Supahfast198', 'CTH3 DLC2', 1),
('Midnight', 'Joe Satriani', NULL, 'highfine', '04-2023', 1),

('Life Controller', 'Gorod', NULL, 'OHM', 'DISGOROD', 2),
('Saturnine', 'Allegaeon', NULL, 'Miscellany', 'S Hero', 2),
('Nitrus', 'Dick Dale', NULL, 'Haggis', 'CTH3', 2),

('Trust', 'DGM', NULL, 'Peddy', 'CTH3', 3),
('Death Perception', 'Æternity', NULL, 'Aren Eternal & Jackie', 'Fuse Box', 3),
('L\'Entité', 'First Fragment', NULL, 'Miscellany', 'BH', 3),
('Groundhog (Beat Juggle)', 'Noisia', NULL, 'Aren Eternal', 'CB', 3),

('Forgotten Trail', 'Buckethead', NULL, 'Tris255', 'MH2 DLC', 1),
('Charging the Void', 'Vektor', NULL, 'Miscellany, xX760Xx, Inventor211', 'Miscellany', 2),
('See the Light of Freedom', 'Galneryus', NULL, 'Peddy', 'S Hero', 3);

UPDATE `focus` SET `tiebreak_song_id` = 11 WHERE `id`=1;
UPDATE `focus` SET `tiebreak_song_id` = 12 WHERE `id`=2;
UPDATE `focus` SET `tiebreak_song_id` = 13 WHERE `id`=3;