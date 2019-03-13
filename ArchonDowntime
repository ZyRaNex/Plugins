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


    public class ArchonDowntime : BasePlugin, IInGameTopPainter
    {
        private readonly int[] _skillOrder = { 2, 3, 4, 5, 0, 1 };
        private StringBuilder textBuilder;
        private IFont GreenFont;
        private IFont RedFont;
        double ArchonTimeLeft;
        double Cooldown;
        bool WizIngame;
        public ArchonDowntime()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            GreenFont = Hud.Render.CreateFont("tahoma", 8, 255, 0, 255, 0, true, false, false);
            RedFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 0, 0, true, false, false);
            textBuilder = new StringBuilder();
        }


        public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Render.UiHidden) return;
            float x = Hud.Window.Size.Width * 0.7f;
            float y = Hud.Window.Size.Height * 0.01f;
			textBuilder.Clear();
			WizIngame = false;
            foreach (var player in Hud.Game.Players)
            {
                if (player.HeroClassDefinition.HeroClass == HeroClass.Wizard)
                {
                    WizIngame = true;
                    foreach (var i in _skillOrder)
                    {
                        var skill = player.Powers.SkillSlots[i];
                        if (skill == null || skill.SnoPower.Sno != 134872) continue; //Archon

                        Cooldown = (skill.CooldownFinishTick - Hud.Game.CurrentGameTick) / 60.0d;

                        var buff = player.Powers.GetBuff(Hud.Sno.SnoPowers.Wizard_Archon.Sno);
                        if (buff != null)
                        {
                            ArchonTimeLeft = buff.TimeLeftSeconds[2];
                        }
                        if (Cooldown < 0)
                        {
                            if (skill.Rune == 3.0)
                            {
                                Cooldown = skill.CalculateCooldown(100) - 20 + ArchonTimeLeft;
                            }
                            else
                            {
                                Cooldown = skill.CalculateCooldown(120) - 20 + ArchonTimeLeft;
                            }

                            
                            if(ArchonTimeLeft == 0)
                            {
                                Cooldown = 0;
                            }
                        }
                        textBuilder.AppendFormat("{0:0.00}", Cooldown);
                    }
                }
            }

            if (WizIngame)
            {
                if (ArchonTimeLeft == 0)
                {
                    var layout = RedFont.GetTextLayout(textBuilder.ToString());
                    RedFont.DrawText(layout, x, y);
                }
                else
                {
                    var layout = GreenFont.GetTextLayout(textBuilder.ToString());
                    GreenFont.DrawText(layout, x, y);
                }
            }
        }
    }
}
