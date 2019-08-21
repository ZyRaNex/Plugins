using System.Globalization;
using Turbo.Plugins.Default;
using System.Linq;
using SharpDX.DirectInput;

using System;
 
 
using System.Text;
using System.Collections.Generic;

namespace Turbo.Plugins.Zy
{

    public class BoneringerStacks : BasePlugin, IInGameTopPainter
    {
        private StringBuilder textBuilder;
        private IFont GreenFont;
        public BoneringerStacks()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            GreenFont = Hud.Render.CreateFont("tahoma", 8, 255, 0, 255, 0, true, false, false);
            textBuilder = new StringBuilder();
        }

        public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Render.UiHidden) return;
            var x = Hud.Window.Size.Width * 0.7f;
            var y = Hud.Window.Size.Height * 0.01f;

            double boneringer = 0;
            var actors = Hud.Game.Actors.Where(a => a.SnoActor.Sno == ActorSnoEnum._p6_necro_commandskeletons_d);
            if (actors.Count() > 0)
            {
                boneringer = actors.First().GetAttributeValue(Hud.Sno.Attributes.Multiplicative_Damage_Percent_Bonus, uint.MaxValue);
            }
            if (boneringer >= 2.4)
            {
                textBuilder.Clear();
				
				var BoneRingerCalc1 = boneringer / 1.5f;     // Command Multiplier
                var BoneRingerCalc2 = BoneRingerCalc1 / 1.6f; // Enforcer Multiplier
                var BoneRingerCalc3 = BoneRingerCalc2 - 1f;
                var BoneRingerTime = BoneRingerCalc3 * 100f / 30.0;
				
                textBuilder.AppendFormat("{0:0.00}", BoneRingerTime);
                textBuilder.AppendLine();

                var layout = GreenFont.GetTextLayout(textBuilder.ToString());
                GreenFont.DrawText(layout, x, y);
            }
        }
    }
}
