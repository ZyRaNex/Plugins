using System.Globalization;
using Turbo.Plugins.Default;
using System.Linq;
using SharpDX.DirectInput;

using System;
using System.Text;
using System.Collections.Generic;

namespace Turbo.Plugins.Zy
{
    public class MapAndMobtype : BasePlugin, IInGameTopPainter, IKeyEventHandler
    {
        public class MonsterSet
        {
            public MonsterSet()
            {
                Name = "unknown";
            }
            public MonsterSet(string Name_, List<string> Monsters_)
            {
                Name = Name_;
                Monsters = Monsters_;
            }

            public string Name { get; }
            public List<string> Monsters;
        }
        public class MapName
        {
            public MapName()
            {
                Name = "unknown";
            }
            public MapName(string Name_, List<string> Monsters_)
            {
                Name = Name_;
                Scenes = Monsters_;
            }

            public string Name { get; }
            public List<string> Scenes;
        }

        private StringBuilder textBuilder;
        private IFont WhiteFont;
        public bool Show { get; set; }
        public IKeyEvent ToggleKeyEvent { get; set; }
        public string LastMap;
        public Key HotKey
        {
            get { return ToggleKeyEvent.Key; }
            set { ToggleKeyEvent = Hud.Input.CreateKeyEvent(true, value, false, false, false); }
        }
        HashSet<string> SceneDebug = new HashSet<string>();

        private List<MonsterSet> MonsterSetList = new List<MonsterSet>{
            new MonsterSet("Dark Berserker", new List<string>
            { "Dark Berserker", "Dark Hellion", "Dark Skeletal Archer",  "Dark Summoner",  "Punisher",
                "Returned Executioner",  "Returned Shieldman",  "Revenant Soldier",  "Savage Fiend" ,  "Skeletal Beast" ,  "Skeleton" ,  "Tomb Guardian" }),

            new MonsterSet("Summoners", new List<string>
            { "Blazing Ghoul", "Corpse Worm", "Dark Skeletal Archer", "Deranged Cultist", "Dust Imp", "Grim Wraith", "Grotesque", "Hulking Phasebeast",
                "Quill Fiend", "Risen", "Skeletal Archer", "Skeletal Executioner", "Skeletal Shieldbearer", "Skeleton", "Tomb Guardian", "Unburied"}),

            new MonsterSet("Morlu Hell Witch", new List<string>
            { "Bile Crawler","Corrupted Angel", "Demonic Hellflyer", "Gloom Wraith", "Hell Witch", "Mallet Lord", "Morlu Incinerator", "Morlu Legionnaire"}),

            new MonsterSet("Boggen Trappers ", new List<string>
            { "Bogan Trapper", "Boggit", "Dark Moon Clan Impaler", "Dark Moon Clan Shaman", "Dark Moon Clan Warrior", "Fallen Hellhound", "Fallen Lunatic",
                "Fallen Slavelord", "Lacuni Slasher", "Lacuni Stalker", "Tusked Bogan"}),

            new MonsterSet("Spearman", new List<string>
            { "Blood Clan Sorcerer", "Blood Clan Spearman", "Blood Clan Warrior", "Colossal Golgor", "Death Maiden", "Fallen Soldier", "Herald of Pestilence",
                "Revenant Shield Guard", "Summoned Archer", "Summoned Shield Guard", "Tormented Stinger"}),

            new MonsterSet("Unburieds", new List<string>
            { "Carrion Bat", "Dark Moon Clan Shaman", "Darkmoth", "Ghoul", "Moon Clan Impaler", "Moon Clan Shaman", "Moon Clan Warrior", "Skeleton", "Unburied"}),

            new MonsterSet("Red Spiders", new List<string>
            { "Armored Destroyer", "Colossal Golgor", "Corrupted Angel", "Dark Hellion", "Disentombed Hulk", "Enslaved Nightmare", "Exarch", "Fiery Spiderling",
                "Lacuni Huntress", "Lacuni Stalker", "Mallet Lord", "Oppressor", "Revenant Soldier", "Scorching Creeper", "Summoned Archer", "Winged Assassin"}),

            new MonsterSet("Big Spiders", new List<string>
            { "Dark Hellion", "Darkmoth", "Death Maiden", "Moon Clan Shaman", "Stinging Swarm", "Stygian Crawler", "Summoned Archer", "Summoned Shield Guard",
                "Tormented Stinger", "Toxic Lurker", "Webspitter Spider"}),

            new MonsterSet("Hulks", new List<string>
            { "Dark Skeletal Archer", "Disentombed Hulk", "Dust Biter", "Dust Eater", "Dust Imp", "Enraged Phantom", "Frost Guardian", "Inferno Zombie", "Lacuni Stalker",
                "Murderous Fiend", "Ravenous Dead", "Retching Cadaver", "Revenant Shield Guard", "Sand Dweller", "Smoldering Construct", "Vicious Mangler"}),

            new MonsterSet("Transformers", new List<string>
            { "Dark Berserker", "Dark Cultist", "Dark Hellion", "Dark Summoner", "Dark Vessel", "Death Maiden", "Quill Fiend", "Revenant Archer", "Revenant Shield Guard", "Revenant Soldier",
                "Unburied", "Unholy Thrall"}),

            new MonsterSet("Lickers", new List<string>
            { "Abomination", "Brood Hatchling", "Copperfang Lurker", "Deathly Haunt", "Fallen Grunt", "Fallen Mongrel", "Fallen Overlord", "Fallen Prophet",
                "Noxious Guardian", "Soul Lasher", "Spellwinder"}),

            new MonsterSet("Blue Fallen", new List<string>
            { "Blazing Swordwielder", "Chilling Construct", "Demon Raider", "Exorcist", "Fallen Master", "Returned Executioner", "Reviled", "Skeletal Archer",
                "Skeletal Raider", "Spine Lasher", "Vengeful Summoner", "Vile Revenant"}),

            new MonsterSet("Lacuni Phasebeast", new List<string>
            { "Corpse Worm", "Grotesque", "Hungry Corpse", "Hungry Torso", "Imp", "Lacuni Huntress", "Ravenous Dead", "Retching Cadaver", "Returned", "Returned Archer",
                "Returned Executioner", "Returned Shieldman", "Returned Summoner", "Summoned Soldier", "Unholy Thrall", "Warping Horror"}),

            new MonsterSet("Ghosts", new List<string>
            { "Dark Zealot", "Enraged Phantom", "Ghastly Gravedigger", "Ghostly Murderer", "Horned Charger", "Moon Clan Ghost", "Pain Monger", "Shadow Vermin",
                "Summoned Archer", "Warping Horror", "Winged Assassin"}),

            new MonsterSet("Red Fallen", new List<string>
            { "Anarch", "Carrion Bat", "Death Maiden", "Fallen", "Fallen Hound", "Fallen Lunatic", "Fallen Overseer", "Fallen Shaman", "Scarab", "Skeletal Archer", "Summoned Archer",
                "Summoned Shield Guard", "Summoned Soldier"}),

            new MonsterSet("Smoldering Construct", new List<string>
            { "Accursed Hellion", "Dark Hellion", "Decayer", "Demon Trooper", "Enraged Phantom", "Foul Conjurer", "Quill Fiend", "Smoldering Construct", "Spewing Horror",
                "Stygian Crawler", "Voracious Zombie"}),

            new MonsterSet("Red Archers", new List<string>
            { "Betrayed", "Bile Crawler", "Canine Bones", "Corpse Raiser", "Corrupted Angel", "Darkmoth", "Death Maiden", "Exarch", "Exorcist", "Hellhide Tremor",
                "Hungering Bones", "Risen Bones", "Skeletal Ranger", "Spitting Bones", "Summoned Archer", "Summoned Shield Guard", "Tormented Stinger",
                "Vile Bat", "Winged Assassin"}),

            new MonsterSet("Lightning Snakes", new List<string>
            { "Blood Clan Mauler", "Blood Clan Sorcerer", "Canine Bones", "Corpse Raiser", "Corpse Worm", "Death Maiden", "Electric Eel", "Exarch", "Frenzied Lunatic",
                "Grotesque", "Hell Witch", "Hulking Phasebeast", "Hungering Bones", "Risen Bones", "Sand Wasp", "Scarab", "Serpent Magus", "Skeleton", "Spitting Bones",
                "Summoned Archer", "Summoned Shield Guard", "Terror Demon", "Vile Bat", "Writhing Deceiver"}),

            new MonsterSet("Slashers", new List<string>
            { "Accursed Hellion", "Blood Clan Impaler", "Blood Clan Mauler", "Bogan Trapper", "Fallen", "Fallen Shaman", "Foul Conjurer", "Herald of Pestilence",
                "Horror", "Lacuni Slasher", "Skeletal Raider", "Zap Worm"}),

            new MonsterSet("Small Spiders", new List<string>
            { "Arachnid Horror", "Bile Crawler", "Corpse Worm", "Exorcist", "Lacuni Huntress", "Moon Clan Impaler", "Spiderling"}),

            new MonsterSet("Big Bogans", new List<string>
            { "Armored Destroyer", "Bogan Trapper", "Boggit", "Demon Trooper", "Enslaved Nightmare", "Lacuni Huntress", "Maggot", "Maggot Brood", "Mallet Lord", "Moon Clan Impaler",
                "Moon Clan Warrior", "Oppressor", "Shrieking Terror", "Tusked Bogan", "Vile Bat"}),

            new MonsterSet("Hellions", new List<string>
            { "Accursed", "Accursed Hellion", "Barbed Lurker", "Blood Clan Impaler", "Blood Clan Mauler", "Crazed Cultist", "Dark Hellion", "Deranged Cultist", "Foul Conjurer",
                "Lacuni Huntress", "Lacuni Warrior", "Mallet Lord", "Pain Monger", "Savage Fiend", "Shadow of Death", "Vicious Mangler"}),

            new MonsterSet("Swarms", new List<string>
            { "Blood Clan Impaler", "Brood Hatchling", "Darkmoth", "Dune Stinger", "Great Horned Goliath", "Hellhide Tremor", "Scarab", "Stygian Crawler", "Vile Swarm",
                "Vile Temptress", "Webspitter Spider", "Winged Assassin"}),

            new MonsterSet("Anarch Exarch", new List<string>
            { "Anarch", "Death Maiden", "Demon Trooper", "Exarch", "Executioner", "Summoned Archer", "Summoned Shield Guard", "Summoned Soldier", "Winged Assassin"}),

            new MonsterSet("Bee Accursed", new List<string>
            { "Accursed", "Corpse Worm", "Dune Stinger", "Fallen Conjurer", "Fallen Cur", "Fallen Peon", "Grotesque", "Noxious Guardian", "Reviled"}),

            new MonsterSet("Zombie Grotesque", new List<string>
            { "Corpse Worm", "Dark Hellion", "Dark Summoner", "Decayer", "Grotesque", "Hungry Corpse", "Hungry Torso", "Imp", "Punisher", "Ravenous Dead", "Retching Cadaver",
                "Returned", "Returned Archer", "Returned Executioner", "Returned Shieldman", "Returned Summoner"}),

            new MonsterSet("Frogs", new List<string>
            { "Anarch", "Burrowing Leaper", "Dark Hellion", "Dark Skeletal Archer", "Enraged Phantom", "Hungry Corpse", "Hungry Torso", "Maniacal Golgor",
                "Returned Executioner", "Scavenger", "Shock Guardian", "Skeletal Archer", "Skeleton", "Tomb Guardian"}),

            new MonsterSet("Shamans", new List<string>
            { "Brood Hatchling", "Crazed Summoner", "Dark Conjurer", "Fallen", "Fallen Firemage", "Fallen Shaman", "Fallen Slavelord", "Frenzied Hellion",
                "Herald of Pestilence", "Quill Fiend", "Skeletal Ranger", "Soul Ripper", "Subjugator", "Unburied", "Vicious Ghoul", "Vicious Hellion"}),

            new MonsterSet("Armored Destroyer", new List<string>
            { "Anarch", "Armored Destroyer", "Blood Clan Warrior", "Canine Bones", "Corpse Raiser", "Demonic Hellflyer", "Demon Trooper", "Hungering Bones", "Morlu Incinerator",
                "Plague Carrier", "Risen Bones", "Sand Wasp", "Shade Stalker", "Skeletal Beast", "Spitting Bones", "Terror Demon", "Vile Bat", "Winged Assassin"}),

            new MonsterSet("Oppressor Hell Witch", new List<string>
            { "Blazing Ghoul", "Fallen Overseer", "Moon Clan Impaler", "Oppressor", "Subjugator", "Vile Temptress"}),

            new MonsterSet("Blazing Guardian", new List<string>
            { "Barbed Lurker", "Bile Crawler", "Blazing Guardian", "Dark Berserker", "Dark Hellion", "Grim Wraith", "Hulking Phasebeast", "Hungry Corpse", "Hungry Torso", "Inferno Zombie",
                "Murderous Fiend", "Returned Archer", "Skeletal Raider"}),

            new MonsterSet("Porcupoggers", new List<string>
            { "Frost Maggot", "Glacial Colossus", "Ice Clan Impaler", "Ice Clan Shaman", "Ice Clan Warrior", "Ice Porcupine"}),

            new MonsterSet("Petting Zoo", new List<string>
            { "Arachnid Horror", "Bone Warrior", "Dark Berserker", "Dark Cultist", "Enraged Phantom", "Fallen Conjurer", "Horned Charger", "Oppressor", "Quill Fiend",
                "Savage Beast", "Scavenger", "Skeleton", "Tomb Guardian"})

            };

        private List<MapName> MapNameList = new List<MapName>{

            new MapName("Battlefields", new List<string>
            {"x1_pand_ext_120_edge_nw_entrance_01","x1_pand_ext_120_edge_sw_01","x1_pand_ext_120_edge_sew_02","x1_pand_ext_120_edge_elbow_sw_01","x1_pand_ext_120_edge_new_02","x1_pand_ext_120_edge_elbow_se_01",
                "x1_pand_ext_120_edge_new_03","x1_pand_ext_120_edge_ne_01","x1_pand_ext_120_edge_se_01","x1_pand_ext_120_edge_elbow_ne_01","x1_pand_ext_120_edge_sew_01","x1_pand_ext_120_edge_sew_03",
                "x1_pand_ext_120_edge_elbow_nw_02","x1_pand_ext_120_edge_nse_02", "x1_pand_ext_120_edge_se_entrance_01","x1_pand_ext_120_edge_nsw_02","x1_pand_ext_120_edge_nse_03","x1_pand_ext_120_edge_nse_01",
                "x1_pand_ext_120_edge_new_01","x1_pand_ext_120_edge_nw_01","x1_pand_ext_120_edge_elbow_se_02","x1_pand_ext_120_edge_nw_02","x1_pand_ext_120_edge_ne_entrance_01","x1_pand_ext_120_edge_nsw_01",
                "x1_pand_ext_120_edge_elbow_nw_01","x1_pand_ext_120_edge_nw_exit_01", "x1_pand_ext_120_edge_ne_exit_01", "x1_pand_ext_120_edge_nse_04", "x1_pand_ext_120_edge_elbow_ne_02","x1_pand_ext_120_edge_ne_02",
                "x1_pand_ext_120_edge_nsw_04","x1_pand_ext_120_edge_se_exit_01", "x1_pand_ext_120_edge_nsw_03", }),

            new MapName("?????????? Cave", new List<string>
            {"a2dun_cave_w_entrance_01","a2dun_cave_ew_01","a2dun_cave_sew_01","a2dun_cave_new_01","a2dun_cave_e_exit_01",}),

            new MapName("Blue Cave", new List<string>
            {"a2dun_cave_flooded_e_entrance_01","a2dun_cave_flooded_nw_01","a2dun_cave_flooded_nse_01","a2dun_cave_flooded_s_01","a2dun_cave_flooded_sew_01","a2dun_cave_flooded_n_01","a2dun_cave_flooded_new_01",
                "a2dun_cave_flooded_s_exit_01", "a2dun_cave_flooded_s_entrance_01","a2dun_cave_flooded_w_01","a2dun_cave_flooded_ew_01","a2dun_cave_flooded_e_01", "a2dun_cave_flooded_n_entrance_01",
                "a2dun_cave_flooded_ne_01", "a2dun_cave_flooded_w_entrance_01","a2dun_cave_flooded_nsw_01","a2dun_cave_flooded_se_01","a2dun_cave_flooded_n_dungeonstone_exit_01", "a2dun_cave_flooded_e_exit_01",
                "a2dun_cave_flooded_ns_01", "a2dun_cave_flooded_w_exit_01","a2dun_cave_flooded_sw_01", }),

            new MapName("Brown Cave", new List<string>
            {"trdun_cave_n_entrance_01","trdun_cave_nsw_02","trdun_cave_sw_01","trdun_cave_nse_01","trdun_cave_n_01","trdun_cave_se_01","trdun_cave_w_exit_01", "trdun_cave_w_entrance_01", "trdun_cave_e_entrance_01",
                "trdun_cave_s_entrance_01", "trdun_cave_ew_01","trdun_cave_nw_01","trdun_cave_sew_01","trdun_cave_new_01", "trdun_cave_w_01","trdun_cave_nsw_01","trdun_cave_ne_01","trdun_cave_e_exit_01", "trdun_cave_s_exit_01", }),

            new MapName("Cathedral", new List<string>
            {"trdun_cath_n_entrance_01","trdun_cath_sw_hall_02","trdun_cath_new_01","trdun_cath_s_dead_end_01","trdun_cath_ne_02","trdun_cath_sw_01","trdun_cath_e_exit_02", "trdun_cath_ns_hall_01","trdun_cath_sew_01",
                "trdun_cath_e_dead_end_01","trdun_cath_nw_hall_01","trdun_cath_se_hall_01","trdun_cath_w_exit_01", "trdun_cath_n_entrance_02","trdun_cath_se_02","trdun_cath_nw_hall_02","trdun_cath_e_dungeonstone_exit_01",
                "trdun_cath_s_entrance_01","trdun_cath_nse_01","trdun_cath_ne_01","trdun_cath_nsw_01","trdun_cath_s_exit_01", "trdun_cath_w_entrance_01","trdun_cath_ew_hall_02","trdun_cath_sew_02","trdun_cath_n_dead_end_01",
                "trdun_cath_ew_02", "trdun_cath_ne_hall_01", "trdun_cath_e_entrance01", "trdun_cath_ew_hall_01","trdun_cath_ne_hall_03","trdun_cath_se_01", "trdun_cath_e_entrance_02","trdun_cath_ew_01",
                "trdun_cath_n_exit_01", "trdun_cath_sw_hall_01", "trdun_cath_s_exit_03", "trdun_cath_se_hall_02","trdun_cath_w_dead_end_01","trdun_cath_ne_hall_02", "trdun_cath_nw_01","trdun_cath_ns_01",
                "trdun_cath_ns_02", }),

            new MapName("Cave", new List<string>
            {"x1_bogcave_w_entrance_01","x1_bogcave_ne_01","x1_bogcave_nsew_02","x1_bogcave_nse_01","x1_bogcave_n_01","x1_bogcave_se_01","x1_bogcave_sw_01","x1_bogcave_w_exit_01", "x1_bogcave_e_01","x1_bogcave_nw_01",
                "x1_bogcave_sew_01","x1_bogcave_e_exit_01", "x1_bogcave_new_02","x1_bogcave_w_01","x1_bogcave_s_01","x1_bogcave_new_01", "x1_bogcave_n_entrance_01","x1_bogcave_s_exit_01",}),

            new MapName("Church", new List<string>
            {"p6_church_w_entrance_01","p6_church_ne_01","p6_church_ns_01","p6_church_sw_01","p6_church_e_exit_01", "p6_church_n_entrance_01", "p6_church_se_01", "p6_church_n_exit_01", "p6_church_ew_01","p6_church_w_exit_01",
                "p6_church_s_exit_01", "p6_church_s_entrance_01", "p6_church_nw_01", "p6_church_e_entrance_01", "p6_church_n_dungeonstone_exit_01", }),

            new MapName("Corvus", new List<string>
            {"x1_catacombs_n_entrance_04","x1_catacombs_nse_01","x1_catacombs_s_01","x1_catacombs_sw_01","x1_catacombs_ns_01","x1_catacombs_n_exit_01", "x1_catacombs_s_entrance_01","x1_catacombs_ns_02","x1_catacombs_nw_01",
                "x1_catacombs_se_02","x1_catacombs_ns_06","x1_catacombs_ew_02","x1_catacombs_ew_01","x1_catacombs_e_exit_02", "x1_catacombs_w_entrance_01","x1_catacombs_se_01","x1_catacombs_new_02","x1_catacombs_w_01",
                "x1_catacombs_ne_01","x1_catacombs_s_exit_01", "x1_catacombs_e_entrance_01", "x1_catacombs_new_01","x1_catacombs_ne_02", "x1_catacombs_w_exit_01", "x1_catacombs_e_01","x1_catacombs_nw_02","x1_catacombs_sw_02",
                "x1_catacombs_e_dungeonstone_exit_01", "x1_catacombs_nse_02", "x1_catacombs_sew_01", "x1_catacombs_nsw_03", }),

            new MapName("Crater", new List<string>
            {"a3dun_crater_entrance_02_w01_s01","a3dun_crater_entrance_02_e01_n01","a3dun_crater_entrance_02_e01_s01","a3dun_crater_entrance_02_w01_n01","a3dun_crater_sw_01_w01_s01","a3dun_crater_sw_01_e01_s01",
                "a3dun_crater_sw_01_w01_n01","a3dun_crater_sw_01_e01_n01","a3dun_crater_ne_01_e01_n01","a3dun_crater_ne_01_e01_s01","a3dun_crater_ne_01_w01_n01","a3dun_crater_ne_01_w01_s01","a3dun_crater_s_exit_02_w01_s01",
                "a3dun_crater_s_exit_02_e01_s01","a3dun_crater_s_exit_02_e01_n01", "a3dun_crater_entrance_01_w01_n01","a3dun_crater_entrance_01_e01_n01","a3dun_crater_entrance_01_e01_s01","a3dun_crater_ns_05_w01_s01",
                "a3dun_crater_ns_05_e01_s01","a3dun_crater_ns_05_w01_n01","a3dun_crater_ns_05_e01_n01","a3dun_crater_ns_03_e01_s01","a3dun_crater_ns_03_w01_s01","a3dun_crater_ns_03_w01_n01","a3dun_crater_ns_03_e01_n01",
                "a3dun_crater_ns_04_e01_s01","a3dun_crater_ns_04_w01_s01","a3dun_crater_ns_04_w01_n01", "a3dun_crater_se_03_w01_s01","a3dun_crater_se_03_w01_n01","a3dun_crater_se_03_e01_s01","a3dun_crater_se_03_e01_n01",
                "a3dun_crater_nw_03_w01_n01","a3dun_crater_nw_03_e01_n01","a3dun_crater_nw_03_e01_s01", "a3dun_crater_ns_04_e01_n01", "a3dun_crater_ne_02_e01_n01","a3dun_crater_ne_02_e01_s01","a3dun_crater_ne_02_w01_s01",
                "a3dun_crater_ne_02_w01_n01", }),

            new MapName("Crypt", new List<string>
            {"trdun_crypt_s_entrance_01","trdun_crypt_ns_01","trdun_crypt_nw_01","trdun_crypt_ew_01","trdun_crypt_nse_01","trdun_crypt_s_02","trdun_crypt_n_exit_down_01", "trdun_crypt_ne_hall_01","trdun_crypt_nw_hall_01",
                "trdun_crypt_nsw_01","trdun_crypt_s_exit_down_01", "trdun_crypt_se_hall_01","trdun_crypt_new_01","trdun_crypt_e_exit_down_01", "trdun_crypt_s_entrance_02", "trdun_crypt_n_entrance_04","trdun_crypt_sw_hall_01",
                "trdun_crypt_sew_02", "trdun_crypt_e_entrance_02","trdun_crypt_w_02","trdun_crypt_se_01", "trdun_crypt_sw_01","trdun_crypt_n_02", "trdun_crypt_w_entrance_02", "trdun_crypt_ew_hall_01", "trdun_crypt_ne_01",
                "trdun_crypt_w_dungeonstone_exit_01", "trdun_crypt_e_02", "trdun_crypt_w_exit_down_01", }),

            new MapName("Desert", new List<string>
            {"px_desert_120_border_nse_entrance_01","px_desert_120_border_sw_01","px_desert_120_border_se_01","px_desert_120_border_nsw_01","px_desert_120_border_elbow_ne_01","px_desert_120_border_nse_01",
                "px_desert_120_border_elbow_se_01","px_desert_120_border_ne_01","px_desert_120_border_new_01","px_desert_120_border_elbow_nw_01","px_desert_120_border_elbow_sw_01","px_desert_120_border_sew_01",
                "px_desert_120_border_new_exit_01","px_desert_120_border_nw_01", "px_desert_120_border_new_entrance_01","px_desert_120_border_sew_exit_01", "px_desert_120_border_nsw_entrance_01",
                "px_desert_120_border_nsw_exit_01", "px_desert_120_border_sew_entrance_01", "px_desert_120_border_nse_exit_01","px_desert_120_border_nse_dungeonstone_exit_01", }),

            new MapName("Festering", new List<string>
            {"px_festeringwoods_border_sew_01_entrance","px_festeringwoods_border_new_01","px_festeringwoods_border_ne_01","px_festeringwoods_border_se_01","px_festeringwoods_border_elbow_nw_01",
                "px_festeringwoods_border_elbow_ne_01","px_festeringwoods_border_elbow_se_01","px_festeringwoods_border_nse_01","px_festeringwoods_border_sew_01","px_festeringwoods_border_elbow_sw_01",
                "px_festeringwoods_border_nsw_01","px_festeringwoods_border_nse_01_exit","px_festeringwoods_border_sw_01", "px_festeringwoods_border_nsw_01_entrance","px_festeringwoods_border_nw_01",
                "px_festeringwoods_border_sew_01_exit","px_festeringwoods_border_new_01_exit", "px_festeringwoods_border_nse_01_entrance", "px_festeringwoods_border_new_01_entrance", "px_festeringwoods_border_nsw_01_exit", }),

            new MapName("Forgotten Ruins", new List<string>
            {"a2dun_zolt_random_s_entrance_01","a2dun_zolt_random_nw_01","a2dun_zolt_random_nse_01","a2dun_zolt_random_s_exit_01","a2dun_zolt_random_n_02", "a2dun_zolt_random_ne_01","a2dun_zolt_random_nsw_01",
                "a2dun_zolt_random_s_02","a2dun_zolt_random_n_exit_01", "a2dun_zolt_random_sw_02", "a2dun_zolt_random_w_entrance_01", "a2dun_zolt_random_sew_02","a2dun_zolt_random_e_02","a2dun_zolt_random_e_exit_01",
                "a2dun_zolt_random_w_exit_01", }),

            new MapName("Green Cave", new List<string>
            {"px_cave_a_e_entrance_01","px_cave_a_sw_01","px_cave_a_nse_01","px_cave_a_new_02","px_cave_a_nsw_01","px_cave_a_s_01","px_cave_a_nw_01","px_cave_a_e_exit_01", "px_cave_a_s_entrance_01",
                "px_cave_a_sew_01","px_cave_a_ew_01", "px_cave_a_n_01","px_cave_a_e_01", }),

            new MapName("Halls of Agony", new List<string>
            {"a1dun_leor_s_entrance_01","a1dun_leor_nw_02","a1dun_leor_ne_03","a1dun_leor_nsw_02","a1dun_leor_sew_01","a1dun_leor_w_dead_end_01","a1dun_leor_sew_02","a1dun_leor_e_exit_01", "a1dun_leor_w_entrance_01",
                "a1dun_leor_nw_01","a1dun_leor_nse_02","a1dun_leor_s_dead_end_01","a1dun_leor_new_01","a1dun_leor_ew_01", "a1dun_leor_e_entrance_01","a1dun_leor_sw_hall_02","a1dun_leor_new_02","a1dun_leor_e_dead_end_01",
                "a1dun_leor_ns_01","a1dun_leor_nse_01","a1dun_leor_se_01","a1dun_leor_w_exit_01", "a1dun_leor_se_02","a1dun_leor_n_entrance_01","a1dun_leor_n_dead_end_01","a1dun_leor_n_exit_01",  "a1dun_leor_sw_hall_01",
                "a1dun_leor_ns_02","a1dun_leor_ew_02","a1dun_leor_sw_hall_01_c", "a1dun_leor_sw_hall_01_b", "a1dun_leor_ne_02", "a1dun_leor_nsw_01", "a1dun_leor_s_exit_01", "a1dun_leor_ne_01", }),

            new MapName("Hell Rift", new List<string>
            {"a4dun_hellportal_n_dead_end_01_e01_n01","a4dun_hellportal_s_dead_end_01_e01_s01","a4dun_hellportal_s_dead_end_01_w01_s01","a4dun_hellportal_n_dead_end_01_w01_n01","a4dun_hellportal_s_dead_end_01_e01_n01",
                "a4dun_hellportal_w_dead_end_01_e01_s01","a4dun_hellportal_w_dead_end_01_w01_s01","a4dun_hellportal_w_dead_end_01_w01_n01","a4dun_hellportal_w_dead_end_01_e01_n01",
                "a3dun_crater_e_dead_end_02_e01_n01","a3dun_crater_e_dead_end_02_e01_s01",//NOT SURE
                "a4dun_hellportal_e_dead_end_01_e01_s01","a4dun_hellportal_e_dead_end_01_e01_n01",
            "a3dun_crater_s_dead_end_02_w01_s01","a3dun_crater_s_dead_end_02_e01_s01","a3dun_crater_s_dead_end_02_w01_n01","a3dun_crater_s_dead_end_02_e01_n01",//NOT SURE
            "a4dun_hellportal_s_dead_end_01_e01_n01_dungeonstone",
            }),

            new MapName("Ice Cave", new List<string>
            {"a3dun_icecaves_n_entrance_01","a3dun_icecaves_nse_01","a3dun_icecaves_w_01","a3dun_icecaves_ns_01","a3dun_icecaves_se_01","a3dun_icecaves_nsw_01","a3dun_icecaves_n_01","a3dun_icecaves_s_exit_01",
                "a3dun_icecaves_w_entrance_01","a3dun_icecaves_ne_01","a3dun_icecaves_nsew_01","a3dun_icecaves_sw_01","a3dun_icecaves_e_exit_01", "a3dun_icecaves_e_entrance_01", "a3dun_icecaves_s_entrance_01",
                "a3dun_icecaves_nsew_holes_01","a3dun_icecaves_ew_01","a3dun_icecaves_n_exit_01", }),

            new MapName("Keeps", new List<string>
            {"a3dun_keep_s_entrance_01","a3dun_keep_ne_01","a3dun_keep_new_02","a3dun_keep_ns_02","a3dun_keep_s_exit_01","a3dun_keep_e_entrance_01","a3dun_keep_ew_02","a3dun_keep_sew_01","a3dun_keep_ne_02","a3dun_keep_w_exit_01",
                "a3dun_keep_new_01","a3dun_keep_w_01", "a3dun_keep_nsw_01","a3dun_keep_s_01","a3dun_keep_nw_01","a3dun_keep_e_exit_01", "a3dun_keep_sw_01","a3dun_keep_e_01", "a3dun_keep_w_entrance_01", "a3dun_keep_n_01",
                "a3dun_keep_sw_02", "a3dun_keep_n_entrance_02", "a3dun_keep_ew_03_forge","a3dun_keep_nw_02_forge", "a3dun_keep_ew_01", "a3dun_keep_se_01", "a3dun_keep_se_02","a3dun_keep_new_04_river", "a3dun_keep_se_04_forge",
                "a3dun_keep_nse_01", "a3dun_keep_ns_03_forge", "a3dun_keep_ns_01","a3dun_keep_n_exit_01", }),

             new MapName("Pest Tunnel", new List<string>
            {"x1_abattoir_n_entrance_01","x1_abattoir_nse_05","x1_abattoir_se_05","x1_abattoir_nsew_03","x1_abattoir_s_01","x1_abattoir_w_01","x1_abattoir_nsw_05","x1_abattoir_n_exit_01","x1_abattoir_nsew_04",
                 "x1_abattoir_w_entrance_01", "x1_abattoir_ns_02","x1_abattoir_n_01","x1_abattoir_ew_02","x1_abattoir_nw_05","x1_abattoir_new_05","x1_abattoir_sw_05","x1_abattoir_e_exit_01", "x1_abattoir_sew_05","x1_abattoir_e_01",
                 "x1_abattoir_e_entrance_01", "x1_abattoir_ne_05","x1_abattoir_s_exit_01", "x1_abattoir_s_entrance_01","x1_abattoir_w_exit_01", "x1_abattoir_n_dungeonstone_exit_01", }),

            new MapName("Sewers", new List<string>
            {"a2dun_swr_s_entrance_01","a2dun_swr_nsw_02","a2dun_swr_e_01","a2dun_swr_ns_02","a2dun_swr_nse_02","a2dun_swr_n_01","a2dun_swr_sw_02","a2dun_swr_ne_02","a2dun_swr_s_01","a2dun_swr_n_exit_01", "a2dun_swr_e_entrance_01",
                "a2dun_swr_w_01","a2dun_swr_nw_02","a2dun_swr_se_02","a2dun_swr_ew_02", "a2dun_swr_n_entrance_01","a2dun_swr_sew_02", "a2dun_swr_new_02","a2dun_swr_w_exit_01", "a2dun_swr_e_exit_01","a2dun_swr_w_entrance_01", }),

            new MapName("Sewers (water)", new List<string>
            {"a2dun_aqd_e_entrance_01","a2dun_aqd_nw_02","a2dun_aqd_ns_02","a2dun_aqd_se_02","a2dun_aqd_ew_02","a2dun_aqd_new_02","a2dun_aqd_s_01","a2dun_aqd_nsw_02","a2dun_aqd_e_01","a2dun_aqd_sew_02","a2dun_aqd_sw_02",
                "a2dun_aqd_e_exit_01","a2dun_aqd_s_entrance_01", "a2dun_aqd_nse_02","a2dun_aqd_ne_02","a2dun_aqd_w_dungeonstone_exit_01", "a2dun_aqd_n_entrance_01", "a2dun_aqd_s_exit_01", }),

            new MapName("Shocktowers", new List<string>
            {"x1_fortress_n_entrance_01","x1_fortress_sw_01","x1_fortress_sew_01","x1_fortress_n_01","x1_fortress_se_01","x1_fortress_n_exit_01", "x1_fortress_s_entrance_01","x1_fortress_nw_01","x1_fortress_se_02","x1_fortress_ns_01",
                "x1_fortress_ns_02", "x1_fortress_w_entrance_01","x1_fortress_ew_01","x1_fortress_new_02","x1_fortress_w_01","x1_fortress_ew_02","x1_fortress_e_exit_01","x1_fortress_e_entrance_01","x1_fortress_sew_02","x1_fortress_nw_02",
                "x1_fortress_nsw_01","x1_fortress_s_01","x1_fortress_ne_01", "x1_fortress_s_exit_01", "x1_fortress_e_01", "x1_fortress_nse_02","x1_fortress_ne_02","x1_fortress_sw_02", "x1_fortress_nse_01",
                "x1_fortress_w_dungeonstone_exit_01", "x1_fortress_nsw_02", "x1_fortress_new_01", "x1_fortress_s_dungeonstone_exit_01", }),

            new MapName("Shrouded Moors", new List<string>
            {"p6_moor_ne_120_entrance_01","p6_moor_nse_01","p6_moor_nsw_01","p6_moor_border_elbow_sw_01","p6_moor_border_elbow_se_01","p6_moor_sew_01","p6_moor_ne_01", "p6_moor_se_01","p6_moor_border_elbow_nw_01","p6_moor_new_01",
                "p6_moor_border_elbow_ne_01","p6_moor_nw_01","p6_moor_sw_120_exit_01", "p6_moor_sw_01","p6_moor_ne_120_exit_01", "p6_moor_sw_120_entrance_01", }),

             new MapName("Snow Forest (no snow)", new List<string>
            {"p4_forest_coast_border_entrance_nsw_01","p4_forest_coast_border_sw_01","p4_forest_coast_border_se_01","p4_forest_coast_border_nse_01","p4_forest_coast_border_elbow_ne_02","p4_forest_coast_border_elbow_se_02",
                 "p4_forest_coast_border_ne_01","p4_forest_coast_border_new_01","p4_forest_coast_border_elbow_nw_02","p4_forest_coast_border_nw_01","p4_forest_coast_border_nsw_01","p4_forest_coast_border_elbow_sw_02",
                 "p4_forest_coast_border_exit_nsw_01", "p4_forest_coast_border_entrance_new_01","p4_forest_coast_border_sew_01", "p4_forest_coast_border_entrance_nse_01", "p4_forest_coast_border_entrance_sew_01",
                 "p4_forest_coast_border_exit_nse_01", "p4_forest_coast_border_sew_dungeonstone_exit_01", "p4_forest_coast_border_exit_sew_01", "p4_forest_coast_border_exit_new_01", }),

            new MapName("Snow Forest", new List<string>
            {"p4_forest_snow_border_entrance_sew_01","p4_forest_snow_border_sw_01","p4_forest_snow_border_sew_01","p4_forest_snow_border_new_01","p4_forest_snow_border_elbow_ne_02","p4_forest_snow_border_nse_01",
                "p4_forest_snow_border_ne_01","p4_forest_snow_border_elbow_se_02","p4_forest_snow_border_elbow_sw_02","p4_forest_snow_border_nw_01","p4_forest_snow_border_exit_nsw_01","p4_forest_snow_border_se_01",
                "p4_forest_snow_border_entrance_nse_01","p4_forest_snow_border_nsw_01","p4_forest_snow_border_elbow_nw_02","p4_forest_snow_border_exit_nse_01", "p4_forest_snow_border_entrance_nsw_01",
                "p4_forest_snow_border_entrance_new_01", "p4_forest_snow_border_exit_sew_01", }),

            new MapName("Spaghetti", new List<string>
            {"x1_pand_hexmaze_entrance_e_01","x1_pand_hexmaze_ew_01","x1_pand_hexmaze_nsew_05","x1_pand_hexmaze_edge_n_01","x1_pand_hexmaze_exit_w_01", "x1_pand_hexmaze_entrance_n_01","x1_pand_hexmaze_ns_01","x1_pand_hexmaze_edge_w_01",
                "x1_pand_hexmaze_edge_e_01","x1_pand_hexmaze_exit_s_01","x1_pand_hexmaze_entrance_w_01","x1_pand_hexmaze_nsew_04","x1_pand_hexmaze_edge_s_01","x1_pand_hexmaze_exit_n_01", "x1_pand_hexmaze_entrance_s_01",
                "x1_pand_hexmaze_nsew_03","x1_pand_hexmaze_exit_e_01",  "x1_pand_hexmaze_nsew_02", "x1_pand_hexmaze_nsew_a_01", }),

            new MapName("Spider Cavern", new List<string>
            {"a2dun_spider_w_entrance_01","a2dun_spider_ew_01","a2dun_spider_nsew_02","a2dun_spider_ns_01","a2dun_spider_s_01","a2dun_spider_e_02","a2dun_spider_n_exit_01", "a2dun_spider_n_entrance_01", "a2dun_spider_e_entrance_01",
                "a2dun_spider_w_exit_01", }),

            new MapName("Spire", new List<string>
            {"a4dun_spire_corrupt_w_entrance_01","a4dun_spire_corrupt_ew_02","a4dun_spire_corrupt_ew_01","a4dun_spire_corrupt_sew_01","a4dun_spire_corrupt_nsw_01","a4dun_spire_corrupt_se_02","a4dun_spire_corrupt_ne_01",
                "a4dun_spire_corrupt_n_exit_01", "a4dun_spire_corrupt_s_entrance_01","a4dun_spire_corrupt_ns_01","a4dun_spire_corrupt_nw_01", "a4dun_spire_corrupt_new_01","a4dun_spire_corrupt_w_exit_01", "a4dun_spire_corrupt_nse_01",
                "a4dun_spire_corrupt_sw_01","a4dun_spire_corrupt_e_dungeonstone_exit_01", "a4dun_spire_corrupt_w_01","a4dun_spire_corrupt_ne_02", "a4dun_spire_corrupt_se_01", "a4dun_spire_corrupt_s_01", "a4dun_spire_corrupt_n_entrance_01",
                "a4dun_spire_corrupt_s_exit_01", "a4dun_spire_corrupt_e_exit_01", "a4dun_spire_corrupt_e_entrance_01","a4dun_spire_corrupt_ns_02", "a4dun_spire_corrupt_e_01", "a4dun_spire_corrupt_n_01",
                "a4dun_spire_corrupt_s_dungeonstone_exit_01", }),

            new MapName("Westmarch", new List<string>
            {"x1_westm_w_entrance_01","x1_westm_ew_01","x1_westm_ew_03","x1_westm_sew_04","x1_westm_e_01_vista","x1_westm_nw_01","x1_westm_e_exit_01","x1_westm_s_entrance_01","x1_westm_nw_02","x1_westm_new_01","x1_westm_se_05",
                "x1_westm_nse_01","x1_westm_ne_01","x1_westm_nsw_02","x1_westm_nsw_04", "x1_westm_ne_03","x1_westm_sw_04","x1_westm_ne_04", "x1_westm_nse_02","x1_westm_sw_03", "x1_westm_n_entrance_01", "x1_westm_nsw_03",
                "x1_westm_s_exit_01","x1_westm_w_exit_01","x1_westm_n_dungeonstone_exit_01","x1_westm_n_exit_01","x1_westm_e_entrance_01", "x1_westm_n_02", "x1_westm_sew_01","x1_westm_se_01", "x1_westm_ns_02",
                "x1_westm_ns_04","x1_westm_sew_02", "x1_westm_w_02", }),

            new MapName("Westmarch Ruins", new List<string>
            {"x1_westm_s_entrance_01_fire","x1_westm_nw_01_fire","x1_westm_sew_03_fire","x1_westm_e_02_fire","x1_westm_ns_03_fire", "x1_westm_e_entrance_01_fire","x1_westm_ew_03_fire","x1_westm_ew_01_fire",
                "x1_westm_nsw_02_fire","x1_westm_s_01_fire","x1_westm_ne_04_fire","x1_westm_new_02_fire","x1_westm_w_01_fire","x1_westm_sw_03_fire","x1_westm_ne_02_fire", "x1_westm_w_entrance_01_fire",
                "x1_westm_nse_01_fire","x1_westm_n_01_fire","x1_westm_se_01_fire", "x1_westm_n_entrance_01_fire","x1_westm_sew_04_fire","x1_westm_se_02_fire","x1_westm_new_01_fire",
                 }),

            new MapName("Zoltun Kulle", new List<string>
            {"a2dun_zolt_w_entrance","a2dun_zolt_ew_04","a2dun_zolt_ne_01","a2dun_zolt_se_02","a2dun_zolt_new_01","a2dun_zolt_w_01","a2dun_zolt_sew_01","a2dun_zolt_w_02","a2dun_zolt_e_03","a2dun_zolt_nsw_01","a2dun_zolt_s_01",
                "a2dun_zolt_e_exit", "a2dun_zolt_e_entrance","a2dun_zolt_nsw_01_library","a2dun_zolt_w_exit", "a2dun_zolt_n_03","a2dun_zolt_n_01","a2dun_zolt_e_01","a2dun_zolt_sw_library","a2dun_zolt_hall_ns_03","a2dun_zolt_hall_ns_02",
                "a2dun_zolt_n_exit", "a2dun_zolt_nw_01","a2dun_zolt_nse_02","a2dun_zolt_s_exit", "a2dun_zolt_n_entrance", "a2dun_zolt_sw_02","a2dun_zolt_s_02", "a2dun_zolt_se_01",
                "a2dun_zolt_nse_library", "a2dun_zolt_sew_02","a2dun_zolt_hall_ew_04_library", "a2dun_zolt_sw_01", }),


            new MapName("Town", new List<string>
            {"x1_westmarch_hub_e01_n03", "px_trout_tristram_e10_s15", })
            
            };


        public MapAndMobtype()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);

            HotKey = Key.F11;
            Show = false;
            WhiteFont = Hud.Render.CreateFont("tahoma", 8, 255, 200, 200, 200, true, false, false);
            textBuilder = new StringBuilder();
        }

        public void OnKeyEvent(IKeyEvent keyEvent)
        {
            if (keyEvent.IsPressed && ToggleKeyEvent.Matches(keyEvent))
            {
                Hud.TextLog.Log("Scenes", "start", false, false);
                foreach (var Debug in SceneDebug)
                {
                    string debugtext = "\"" + Debug + "\"" + ", ";
                    Hud.TextLog.Log("Scenes", debugtext, false, true);
                }

                Show = !Show;
            }
        }

        public void PaintTopInGame(ClipState clipState)
        {
            if (clipState != ClipState.BeforeClip)
                return;
            if (Hud.Render.UiHidden)
                return;
            if (Hud.Game.SpecialArea != SpecialArea.GreaterRift)
                return;
            float x = Hud.Window.Size.Width * 0.8f;
            float y = Hud.Window.Size.Height * 0.000f;
            textBuilder.Clear();

            //Mob Type
            HashSet<string> MonstersOnTheScreen = new HashSet<string>();

            var monsters = Hud.Game.AliveMonsters.Where(m => (!m.IsElite));
            foreach (var monster in monsters)
            {
                if (monster.Rarity == ActorRarity.Normal)
                {
                    MonstersOnTheScreen.Add(monster.SnoMonster.NameEnglish);
                }
            }

            var BadMonsters = new List<string>
            { "Shock Tower", "Hell Bringer", "Blazing Swordwielder", "Bloated Corpse", "Spiderling"};

            foreach (string monstername in BadMonsters)
            {
                if (MonstersOnTheScreen.Contains(monstername))
                {
                    MonstersOnTheScreen.Remove(monstername);
                }
            }
            //DEBUG
            if (Show)
            {
                foreach (string monstername in MonstersOnTheScreen)
                {
                    textBuilder.AppendFormat("{0}", monstername);
                    textBuilder.AppendLine();
                }
                textBuilder.AppendLine();
            }
            List<MonsterSet> MatchingMonsterSets = new List<MonsterSet>();
            foreach (MonsterSet monsterset in MonsterSetList)
            {
                MatchingMonsterSets.Add(new MonsterSet(monsterset.Name, monsterset.Monsters));
            }


            List<MonsterSet> RemoveList = new List<MonsterSet>();

            foreach (string monster in MonstersOnTheScreen)
            {
                foreach (MonsterSet monsterset in MatchingMonsterSets)//see if the monster fits the monsterset
                {
                    //if monster is not in the monsterset delete the monsterset
                    if (!monsterset.Monsters.Contains(monster))
                    {
                        RemoveList.Add(monsterset);
                    }
                }
                foreach (MonsterSet monsterset in RemoveList)
                {
                    MatchingMonsterSets.Remove(monsterset);
                }
            }





            HashSet<string> ScenesOnTheScreen = new HashSet<string>();

            /*var actors = Hud.Game.Actors;
            foreach (var actor in actors)
            {
                if (actor == null)continue;
                if (actor.Scene == null)continue;
                if (actor.Scene.SnoScene == null)continue;
                ScenesOnTheScreen.Add(actor.Scene.SnoScene.Code);
            }*/

            if (Hud.Game.Me.Scene != null)
            {
                ScenesOnTheScreen.Add(Hud.Game.Me.Scene.SnoScene.Code);
            }

            var BadMaps = new List<string>
            { };

            foreach (string mapname in BadMaps)
            {
                if (ScenesOnTheScreen.Contains(mapname))
                {
                    ScenesOnTheScreen.Remove(mapname);
                }
            }
            //DEBUG
            
            List<MapName> MatchingMapNames = new List<MapName>();
            foreach (MapName mapset in MapNameList)
            {
                MatchingMapNames.Add(new MapName(mapset.Name, mapset.Scenes));
            }


            List<MapName> RemoveListMap = new List<MapName>();

            foreach (var Scene in ScenesOnTheScreen)
            {
                foreach (MapName mapset in MatchingMapNames)//see if the Scene fits the Map
                {
                    //if Scene is not in the Map delete the Map
                    if (!mapset.Scenes.Contains(Scene))
                    {
                        RemoveListMap.Add(mapset);
                    }
                }
                foreach (MapName mapset in RemoveListMap)
                {
                    MatchingMapNames.Remove(mapset);
                }
            }

            foreach (string Scene in ScenesOnTheScreen)
            {
                bool found = false;
                foreach (MapName Map in MapNameList)
                {
                    if (Map.Scenes.Contains(Scene))
                    {
                        found = true;
                    }
                }
                if (found)
                {
                    if (Show)
                    {
                        textBuilder.AppendFormat("{0}", Scene);
                        textBuilder.AppendLine();
                    }
                }
                else
                {
                    if (Show)
                    {

                        textBuilder.AppendFormat("NEW SCENE");
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0}", Scene);
                        textBuilder.AppendLine();
                    }
                    if (LastMap != null)
                    {
                        SceneDebug.Add(LastMap);
                    }
                    SceneDebug.Add(Scene);
                }
            }
            textBuilder.AppendLine();

            foreach (MapName map in MatchingMapNames)
            {
                textBuilder.AppendFormat("{0}", map.Name);
                textBuilder.AppendLine();
                LastMap = map.Name;
            }
            textBuilder.AppendLine();






            if (MatchingMonsterSets.Count == 1)
            {
                foreach (MonsterSet monsterset in MatchingMonsterSets)
                {
                    textBuilder.AppendFormat("{0}", monsterset.Name);
                    textBuilder.AppendLine();
                }
            }
            else if (MatchingMonsterSets.Count < 20)
            {
                foreach (MonsterSet monsterset in MatchingMonsterSets)
                {
                    textBuilder.AppendFormat("{0}", monsterset.Name);
                    textBuilder.AppendLine();
                }
            }

            var layout = WhiteFont.GetTextLayout(textBuilder.ToString());
            WhiteFont.DrawText(layout, x, y);
        }
    }
}
