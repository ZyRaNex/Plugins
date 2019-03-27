using System.Globalization;
using Turbo.Plugins.Default;
using System.Linq;
using SharpDX.DirectInput;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Turbo.Plugins.Zy
{
    public class MonsterSets : BasePlugin, IInGameTopPainter, IKeyEventHandler
    {
        public class MonsterSet
        {
            public MonsterSet()
            {
                Name = "unknown";
            }
            public MonsterSet(string name, List<string> monsters)
            {
                Name = name;
                Monsters = monsters;
            }

            public string Name { get; }
            public List<string> Monsters;
        }

        
        private StringBuilder textBuilder;
        private IFont WhiteFont;
        public bool Show { get; set; }
        public IKeyEvent ToggleKeyEvent { get; set; }
        public Key HotKey
        {
            get { return ToggleKeyEvent.Key; }
            set { ToggleKeyEvent = Hud.Input.CreateKeyEvent(true, value, false, false, false); }
        }

        private List<MonsterSet> MonsterSetList = new List<MonsterSet>{
            new MonsterSet("Summoners Dark Berserker", new List<string>
            { "Dark Berserker", "Dark Hellion", "Dark Skeletal Archer",  "Dark Summoner",  "Punisher",
                "Returned Executioner",  "Returned Shieldman",  "Revenant Soldier",  "Savage Fiend" ,  "Skeletal Beast" ,  "Skeleton" ,  "Tomb Guardian" }),

            new MonsterSet("Summoners Ghouls", new List<string>
            { "Blazing Ghoul", "Corpse Worm", "Dark Skeletal Archer", "Deranged Cultist", "Dust Imp", "Grim Wraith", "Grotesque", "Hulking Phasebeast",
                "Quill Fiend", "Risen", "Skeletal Archer", "Skeletal Executioner", "Skeletal Shieldbearer", "Skeleton", "Tomb Guardian", "Unburied"}),

            new MonsterSet("Morlu Hell Witch", new List<string>
            { "Bile Crawler", "Demonic Hellflyer", "Gloom Wraith", "Hell Witch", "Mallet Lord", "Morlu Incinerator", "Morlu Legionnaire"}),

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

            new MonsterSet("Poison Mobs", new List<string>
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
            { "Barbed Lurker", "Blazing Guardian", "Dark Berserker", "Dark Hellion", "Grim Wraith", "Hulking Phasebeast", "Hungry Corpse", "Hungry Torso", "Inferno Zombie",
                "Murderous Fiend", "Returned Archer"}),

            new MonsterSet("Porcupoggers", new List<string>
            { "Frost Maggot", "Glacial Colossus", "Ice Clan Impaler", "Ice Clan Shaman", "Ice Clan Warrior", "Ice Porcupine"}),

            new MonsterSet("Summoners Chargers", new List<string>
            { "Arachnid Horror", "Bone Warrior", "Dark Berserker", "Dark Cultist", "Enraged Phantom", "Fallen Conjurer", "Horned Charger", "Oppressor", "Quill Fiend",
                "Savage Beast", "Scavenger", "Skeleton", "Tomb Guardian"})

            };
        public MonsterSets()
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
                Show = !Show;
            }
        }

        public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Render.UiHidden) return;
            float x = Hud.Window.Size.Width * 0.8f;
            float y = Hud.Window.Size.Height * 0.01f;
			textBuilder.Clear();

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
            }
            foreach (MonsterSet monsterset in RemoveList)
            {
                MatchingMonsterSets.Remove(monsterset);
            }
            if (MatchingMonsterSets.Count < 20)
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
