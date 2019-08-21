using Turbo.Plugins.Default;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Plugins.Zy
{
    public class CursbleInside : BasePlugin, IInGameWorldPainter, ICustomizer
    {
        private StringBuilder textBuilder;
        private IFont RedFont;
        public CursbleInside()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            RedFont = Hud.Render.CreateFont("tahoma", 9, 255, 255, 0, 0, false, false, 250, 0, 0, 0, true);
            textBuilder = new StringBuilder();
        }
        public void Customize()
        {
        }
        public void PaintWorld(WorldLayer layer)
        {
            if (Hud.Render.UiHidden)
                return;
            var x = Hud.Window.Size.Width * 0.47f;
            var y = Hud.Window.Size.Height * 0.015f;

            textBuilder.Clear();
            int CursableCount = 0;
            var monsters = Hud.Game.AliveMonsters.Where(m => m.FloorCoordinate.XYDistanceTo(Hud.Game.Me.FloorCoordinate) <= 40);
            foreach (var monster in monsters)
            {
                if (monster.CurHealth <= monster.MaxHealth * 0.18)
                {
                    CursableCount++;
                }
            }
            if (CursableCount > 0)
            {
                textBuilder.AppendFormat("Cursable inside");
                textBuilder.AppendLine();
            }
            var layout = RedFont.GetTextLayout(textBuilder.ToString());
            RedFont.DrawText(layout, x, y);
        }
    }
}
