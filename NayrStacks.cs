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
    using System.Text;


    public class NayrStacks : BasePlugin, IInGameTopPainter
    {
        private IFont GreenFont;
		private StringBuilder textBuilder;
		bool BKIngame;
        public NayrStacks()
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
            float x = Hud.Window.Size.Width * 0.75f;
            float y = Hud.Window.Size.Height * 0.01f;

			BKIngame = false;
            foreach (var player in Hud.Game.Players)//others
            {
                if (player.HeroClassDefinition.HeroClass == HeroClass.Necromancer)
                {
                    var Nayrs = player.Powers.GetBuff(476587);
                    if (!(Nayrs == null || !Nayrs.Active))
                    {
                        BKIngame = true;
                        textBuilder.Clear();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[1]);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[2]);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[3]);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[4]);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[5]);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", Nayrs.TimeLeftSeconds[6]);
                        textBuilder.AppendLine();//rat
                    }
                }
            }

            if (BKIngame)
            {
                var layout = GreenFont.GetTextLayout(textBuilder.ToString());
                GreenFont.DrawText(layout, x, y);
            }
        }
    }
}
