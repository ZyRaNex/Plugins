using System.Globalization;
using Turbo.Plugins.Default;
using System.Linq;
using SharpDX.DirectInput;

using System;
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
        public double ArchonTimeLeft;
        public double Cooldown;
        public bool WizIngame;
        public double ArchonLeft;
        public IBrush BackgroundBrush { get; set; }
        public IBrush TimerArchonBrush { get; set; }
        public IBrush TimerArchonWarningBrush { get; set; }
        public IBrush TimerOutsideBrush { get; set; }
        public IBrush BorderBrush { get; set; }
        public const float BorderSize = 1.2f;
        public ArchonDowntime()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            GreenFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 255, 255, true, false, false);
            RedFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 255, 255, true, false, false);
            textBuilder = new StringBuilder();
			BackgroundBrush = Hud.Render.CreateBrush(240, 0, 0, 0, 0);
            BackgroundBrush.Opacity = 0.25f;
            TimerArchonBrush = Hud.Render.CreateBrush(240, 79, 38, 113, 0);
            TimerArchonWarningBrush = Hud.Render.CreateBrush(240, 200, 0, 0, 0);
            TimerOutsideBrush = Hud.Render.CreateBrush(240, 100, 100, 100, 0);
            BorderBrush = Hud.Render.CreateBrush(240, 244, 169, 80, 0);
        }
        

        public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Render.UiHidden) return;
            float x = Hud.Window.Size.Width * 0.42f;
            float y = Hud.Window.Size.Height * 0.11f;
            float sizex = Hud.Window.Size.Width * 0.16f;
            float sizey = Hud.Window.Size.Height * 0.02f;
            float textx = Hud.Window.Size.Width * 0.498f;
            float texty = Hud.Window.Size.Height * 0.111f;
            float MeteorOcux = Hud.Window.Size.Width * 0.505f;
            float MeteorOcuy = Hud.Window.Size.Height * 0.131f;
            float Meteorx = Hud.Window.Size.Width * 0.421f;
            float Meteory = Hud.Window.Size.Height * 0.131f;
            textBuilder.Clear();
            
            var ATleft = (ArchonLeft - Hud.Game.CurrentGameTick) / 60.0d;
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
                            if (player.HasValidActor && buff.TimeLeftSeconds[2] > 0.0)
                            {
                                ArchonLeft = Hud.Game.CurrentGameTick + ArchonTimeLeft * 60.0d;
                            }
                        }
                        ATleft = (ArchonLeft - Hud.Game.CurrentGameTick) / 60.0d;
                        if (ATleft > 0)
                        {
                            textBuilder.AppendFormat("{0:0}", ATleft);
                        }
                        else
                        {
                            textBuilder.AppendFormat("{0:0}", 12.0 + ATleft);
                        }
                        // DEBUG
                        /*
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", ATleft);
                        textBuilder.AppendLine();
                        textBuilder.AppendFormat("{0:0.00}", ArchonLeft);*/
                    }
                }
            }

            if (WizIngame)
            {
                if (ATleft <= 0.0)//outside
                {
                    if (ATleft >= -12.0)//not bugged
                    {
                        BackgroundBrush.DrawRectangle(x, y, sizex, sizey);
                        TimerOutsideBrush.DrawRectangle(x, y, sizex * (float)((12.0 + ATleft) / 12.0), sizey);
                        BorderBrush.DrawLine(x, y, x + sizex, y, 0.6f);
                        BorderBrush.DrawLine(x + sizex, y, x + sizex, y + sizey, BorderSize);
                        BorderBrush.DrawLine(x, y + sizey, x + sizex, y + sizey, BorderSize);
                        BorderBrush.DrawLine(x, y, x, y + sizey, 0.6f);

                        Hud.Texture.GetTexture(Hud.Sno.GetSnoPower(69190).NormalIconTextureId).Draw(MeteorOcux, MeteorOcuy, 28.0f, 28.0f);//Wizard_Meteor { get; } // 69190
                        Hud.Texture.GetTexture(Hud.Sno.GetSnoPower(69190).NormalIconTextureId).Draw(Meteorx, Meteory, 28.0f, 28.0f);//Wizard_Meteor { get; } // 69190
                    }
                    var layout = RedFont.GetTextLayout(textBuilder.ToString());
                    RedFont.DrawText(layout, textx, texty);
                }
                else//in archon
                {
                    if (ATleft <= 20)//not bugged
                    {
                        BackgroundBrush.DrawRectangle(x, y, sizex, sizey);
                        if (ATleft >= 5.0)
                        {
                            TimerArchonBrush.DrawRectangle(x, y, sizex * (float)(ATleft / 20.0), sizey);
                        }
                        else
                        {
                            TimerArchonWarningBrush.DrawRectangle(x, y, sizex * (float)(ATleft / 20.0), sizey);
                        }
                        BorderBrush.DrawLine(x, y, x + sizex, y, 0.6f);
                        BorderBrush.DrawLine(x + sizex, y, x + sizex, y + sizey, BorderSize);
                        BorderBrush.DrawLine(x, y + sizey, x + sizex, y + sizey, BorderSize);
                        BorderBrush.DrawLine(x, y, x, y + sizey, 0.6f);
                    }
                    var layout = GreenFont.GetTextLayout(textBuilder.ToString());
                    GreenFont.DrawText(layout, textx, texty);
                }
            }
        }
    }
}
