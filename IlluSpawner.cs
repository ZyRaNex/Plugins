using Turbo.Plugins.Default;
using System.Linq;
using System.Collections.Generic;

namespace Turbo.Plugins.Zy
{
    public class IlluSpawner : BasePlugin, IInGameWorldPainter
    {
        public IFont TextFont { get; set; }
        public IBrush RareBrush { get; set; }
        public IBrush BorderBrush { get; set; }
        public IBrush BackgroundBrush { get; set; }
        public WorldDecoratorCollection CircleDecorator { get; set; }
        public IlluSpawner()
        {
            Enabled = true;
        }

        private List<string> IlluSpawners = new List<string>
            { "Maggot Brood", "Tomb Guardian", "Deathspitter", "Retching Cadaver", "Enslaved Nightmare", "Rat Caller"};

    public override void Load(IController hud)
        {
            base.Load(hud);
            TextFont = Hud.Render.CreateFont("tahoma", 9, 255, 255, 255, 255, false, false, true);
            RareBrush = Hud.Render.CreateBrush(255, 20, 255, 20, 0);
            BorderBrush = Hud.Render.CreateBrush(255, 0, 100, 0, -1);
            BackgroundBrush = Hud.Render.CreateBrush(255, 100, 100, 100, 0);
            CircleDecorator = new WorldDecoratorCollection(
            new GroundCircleDecorator(Hud)
            {
                Brush = Hud.Render.CreateBrush(50, 255, 100, 100, 0),
                Radius = 2f,
           }
           );
        }
        public void PaintWorld(WorldLayer layer)
        {
            if (Hud.Game.IsInTown) return;

            var h = 17;
            var w1 = 35;
            var py = Hud.Window.Size.Height / 600;
            var monsters = Hud.Game.AliveMonsters.Where(x => x.IsAlive);
            List<IMonster> monstersElite = new List<IMonster>();
            foreach (var monster in monsters)
            {
                if (monster.SummonerAcdDynamicId == 0)
                {
                    if (monster.Rarity == ActorRarity.RareMinion)
                    {
                        if (IlluSpawners.Contains(monster.SnoMonster.NameEnglish))
                        {
                            foreach (var snoMonsterAffix in monster.AffixSnoList)
                            {
                                if (snoMonsterAffix.Affix == MonsterAffix.Illusionist)
                                {
                                    monstersElite.Add(monster);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var monster in monstersElite)
            {
                var wint = monster.CurHealth / monster.MaxHealth;
                var hptext = "";
                if ((wint < 0) || (wint > 1))
                { wint = 1; hptext = "bug"; }
                else
                { hptext = ValueToString(wint * 100, ValueFormat.NormalNumberNoDecimal); }
                var w = wint * w1;
                var layout = TextFont.GetTextLayout(hptext);
                var monsterX = monster.FloorCoordinate.ToScreenCoordinate().X - w1 / 2;
                var monsterY = monster.FloorCoordinate.ToScreenCoordinate().Y - py * 8;
                if (monsterY < 0)
                { monsterY = monster.FloorCoordinate.ToScreenCoordinate().Y; }
                monsterY -= 15.0f;
                var ShieldingX = monsterX - w1 / 2;
                var hauntX = monsterX + w1 + 5;
                var buffY = monsterY - 1;
                var hpX = monsterX + 7;

                BorderBrush.DrawRectangle(monsterX, monsterY, w1, h);
                BackgroundBrush.DrawRectangle(monsterX, monsterY, w1, h);

                RareBrush.DrawRectangle(monsterX, monsterY, (float)w, h);
                TextFont.DrawText(layout, hpX, buffY);
            }
        }
    }
}
