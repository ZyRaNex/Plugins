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

    public class BossTimer
    {
        public BossTimer()
        {
            Animation = 0;
            Duration = 0;
            Timer = 0;
            Name = "";
            LastTimer = 0;
        }
        public BossTimer(AnimSnoEnum Animation_, double Duration_, string Name_)
        {
            Animation = Animation_;
            Duration = Duration_;
            Timer = 0;
            Name = Name_;
            LastTimer = 0;
        }


        public AnimSnoEnum Animation;
        public double Duration;
        public double Timer;
        public string Name;
        public double LastTimer;
    }

    public class BossCooldowns : BasePlugin, IInGameTopPainter
    {
        private StringBuilder textBuilder;
        private StringBuilder DebugtextBuilder;
        private IFont GreenFont;
        public IBrush BackgroundBrush { get; set; }
        public IBrush BarBrush { get; set; }
        public IBrush BorderBrush { get; set; }
        public const float BorderSize = 1.2f;
        public List<BossTimer> BossTimers = new List<BossTimer>
        {
            new BossTimer (0, 10.0, "Wormhole"),
            new BossTimer (AnimSnoEnum._skeletonking_whirlwind_start, 22.0, "TRIPLE SWING"),
            new BossTimer (AnimSnoEnum._p4_ratking_roar_summon, 22.0, "Plagued Arena"),
            new BossTimer (AnimSnoEnum._x1_squigglet_taunt_01, 20.0, "PLAGUED CIRCLE"),
            new BossTimer (AnimSnoEnum._morluspellcaster_attack_aoe_01, 8.0, "METEOR"),
            new BossTimer (AnimSnoEnum._bigred_firebreath_combo_01, 20.0, "Frozen Nova"),
            new BossTimer (AnimSnoEnum._bigred_charge_01, 10.0, "Charge"),
            new BossTimer (AnimSnoEnum._sandmonsterblack_attack_03_sandwall, 12.0, "Summoning"),
            new BossTimer (AnimSnoEnum._sandmonster_temp_rock_throw, 13.0, "Shovel"),
            new BossTimer (AnimSnoEnum._p6_envy_teleport_start_02, 13.0, "Gateway"),
            new BossTimer (AnimSnoEnum._skeletonsummoner_attack_01, 11.0, "Arcane Bolt"),
            new BossTimer (0, 25.0, "Orlash Summon"),
            new BossTimer (0, 11.0, "Lightning Breath"),
            new BossTimer (AnimSnoEnum._gluttony_attack_areaeffect, 11.0, "Fart Cloud"),
            new BossTimer (AnimSnoEnum._zoltunkulle_aoe_01, 11.0, "Cave In"),
            new BossTimer (AnimSnoEnum._zoltunkulle_omni_cast_01, 6.0, "Twister"),
            new BossTimer (AnimSnoEnum._zoltunkulle_omni_cast_05_fadeout, 9.0, "Teleport"),
            new BossTimer (AnimSnoEnum._zoltunkulle_omni_cast_04, 12.0, "Slow Time"),
            new BossTimer (AnimSnoEnum._demonflyer_mega_firebreath_01, 8.0, "Flame Breath"),
            new BossTimer (AnimSnoEnum._x1_dark_angel_cast, 8.0, "Repulsion Wave"),
            new BossTimer (AnimSnoEnum._lordofdespair_attack_teleport_full, 10.0, "Blink Strike"),
            new BossTimer (AnimSnoEnum._x1_westmarchbrute_taunt, 20.0, "Leap"),
            new BossTimer (AnimSnoEnum._x1_westmarchbrute_attack_02_out, 15.0, "Leap?"),
            new BossTimer (AnimSnoEnum._x1_westmarchbrute_b_attack_06_in, 6.0, "Leaping Strike"),
            new BossTimer (0, 15.0, "Small Fields ??????????????"),
            new BossTimer (0, 8.0, "Cave In"),
            new BossTimer (0, 20.0, "Good Attack"),
            new BossTimer (0, 20.0, "Bad Attack"),
            new BossTimer (0, 20.0, "Line Attack"),
            new BossTimer (AnimSnoEnum._x1_deathmaiden_attack_04_aoe, 14.0, "Overhead Attack"),
            new BossTimer (AnimSnoEnum._x1_deathmaiden_attack_special_360_01, 14.0, "Spinning Attack"),
            new BossTimer (AnimSnoEnum._butcher_attack_charge_01_in, 20.0, "CHARGE"),
            new BossTimer (AnimSnoEnum._butcher_attack_chain_01_in, 14.0, "Sickle Grab"),
            new BossTimer (AnimSnoEnum._butcher_attack_fanofchains, 14.0, "Fan of Spears"),
            new BossTimer (0, 12.0, "Ground Effect"),
            new BossTimer (0, 14.0, "Heavy Smash"),
            //new BossTimer (AnimSnoEnum._angel_corrupt_attack_dash_in, 14.0, "Dash"),
            //new BossTimer (0, 10.0, "Poison Circle"),
            new BossTimer (AnimSnoEnum._x1_sniperangel_firebomb_01, 7.5, "Lightning Orb"),
            new BossTimer (AnimSnoEnum._x1_sniperangel_temp_cast_01, 10.0, "Holy Bolt Nova")
    };
        public BossCooldowns()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            GreenFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 255, 255, true, false, false);
            textBuilder = new StringBuilder();
            DebugtextBuilder = new StringBuilder();
            BackgroundBrush = Hud.Render.CreateBrush(240, 0, 0, 0, 0);
            BackgroundBrush.Opacity = 0.25f;
            BarBrush = Hud.Render.CreateBrush(240, 100, 100, 100, 0);
            BorderBrush = Hud.Render.CreateBrush(240, 244, 169, 80, 0);
        }


        public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Render.UiHidden)
                return;
			if (Hud.Game.SpecialArea != SpecialArea.GreaterRift)
                return;
            var bosses = Hud.Game.AliveMonsters.Where(m => m.Rarity == ActorRarity.Boss);
            bool BossSpawned = false;
            foreach (IMonster m in bosses)
            {
                BossSpawned = true;
                foreach (var bosstimer in BossTimers)
                {
                    if (m.SnoMonster.NameEnglish == "Ember")
                    {
                        var healthpercent = (m.CurHealth / m.MaxHealth);
                        if (healthpercent > 0.5)
                        {
                            bosstimer.Duration = 7.5;
                        }
                        else
                        {
                            bosstimer.Duration = 4.0;
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Stonesinger")
                    {
                        if (bosstimer.Animation == AnimSnoEnum._sandmonsterblack_attack_03_sandwall)
                        {
                            var healthpercent = (m.CurHealth / m.MaxHealth);
                            bosstimer.Duration = 12.0 * healthpercent + 5.5 * (1 - healthpercent);
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Bone Warlock")
                    {
                        if (bosstimer.Name == "Wormhole")
                        {
                            bosstimer.Duration = 16.0;
                            var wormhole = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_monsteraffix_teleportmines); // 337109
                            if (wormhole.Count() > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 7)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Orlash")
                    {
                        if (m.Animation == AnimSnoEnum._terrordemon_generic_cast && bosstimer.Name == "Orlash Summon")
                        {
                            var Clones = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_lr_boss_terrordemon_a_breathminion); // 337109
                            if (Clones.Count() <= 1)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 8)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                        else if (m.Animation == AnimSnoEnum._terrordemon_attack_firebreath && bosstimer.Name == "Lightning Breath")
                        {
                            var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                            if (TimeElapsed > 6)
                            {
                                //DebugtextBuilder.Append(bosstimer.Name);
                                //DebugtextBuilder.Append(" ");
                                //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                //DebugtextBuilder.AppendLine();
                                bosstimer.Timer = Hud.Game.CurrentGameTick;
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Bloodmaw")
                    {
                        if (m.Animation == bosstimer.Animation && bosstimer.Animation == AnimSnoEnum._x1_westmarchbrute_attack_02_out)
                        {
                            var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                            if (TimeElapsed > 7)
                            {
                                //DebugtextBuilder.Append(bosstimer.Name);
                                //DebugtextBuilder.Append(" ");
                                //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                //DebugtextBuilder.AppendLine();
                                //DebugtextBuilder.AppendLine();
                            }
                            bosstimer.Timer = Hud.Game.CurrentGameTick;
                        }
                        if (m.Animation == AnimSnoEnum._x1_westmarchbrute_taunt && bosstimer.Animation == AnimSnoEnum._x1_westmarchbrute_attack_02_out)
                        {
                            var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                            if (TimeElapsed > 7)
                            {
                                //DebugtextBuilder.Append(bosstimer.Name);
                                //DebugtextBuilder.Append(" ");
                                //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                //DebugtextBuilder.AppendLine();
                                //DebugtextBuilder.AppendLine();
                            }
                            bosstimer.Timer = Hud.Game.CurrentGameTick;
                        }

                        /*if (m.Animation == bosstimer.Animation && bosstimer.Animation == AnimSnoEnum._x1_westmarchbrute_taunt)
                        {
                            var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                            if (TimeElapsed > 4)
                            {
                                DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                DebugtextBuilder.AppendLine();
                                bosstimer.LastTimer = bosstimer.Timer;
                                bosstimer.Timer = Hud.Game.CurrentGameTick;
                            }
                        }*/
                    }
                    else if (m.SnoMonster.NameEnglish == "Rime")
                    {
                        if (bosstimer.Name == "Small Fields")
                        {
                            var Circles = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_unique_monster_generic_aoe_dot_cold_10foot); // 337109
                            if (Circles.Count() > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 7)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Perendi")
                    {
                        if (bosstimer.Animation == 0 && bosstimer.Name == "Cave In")
                        {
                            var Circles = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_lr_boss_malletdemon_fallingrocks);
                            if (Circles.Count() > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 5)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Blighter")
                    {
                        if (bosstimer.Animation == 0 && bosstimer.Name == "Good Attack")
                        {
                            var Attack = m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_5_Visual_Effect_None, 429291);
                            if (Attack > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 10)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                        else if (bosstimer.Animation == 0 && bosstimer.Name == "Bad Attack")
                        {
                            var Attack = m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_5_Visual_Effect_None, 309921);
                            if (Attack > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 10)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                        else if (bosstimer.Animation == 0 && bosstimer.Name == "Line Attack")
                        {
                            var Attack = m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_5_Visual_Effect_None, 429077);
                            if (Attack > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 10)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Man Carver")
                    {
                        if (bosstimer.Name == "Ground Effect")
                        {
                            var Circles = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_unique_monster_generic_aoe_dot_fire_10foot);
                            if (Circles.Count() > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 15)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                        else if (bosstimer.Name == "Heavy Smash")
                        {
                            if (m.Animation == AnimSnoEnum._butcher_attack_05_telegraph)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 6)
                                {
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.LastTimer = bosstimer.Timer;
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Erethon")
                    {
                        if (bosstimer.Name == "Poison Circle")
                        {
                            var Circles = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._generic_proxy);
                            if (Circles.Count() > 0)
                            {
                                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                                if (TimeElapsed > 7)
                                {
                                    //DebugtextBuilder.Append(bosstimer.Name);
                                    //DebugtextBuilder.Append(" ");
                                    //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                                    //DebugtextBuilder.AppendLine();
                                    bosstimer.Timer = Hud.Game.CurrentGameTick;
                                }
                            }
                        }
                    }
                    else if (m.SnoMonster.NameEnglish == "Raiziel")
                    {
                        if (m.Animation == AnimSnoEnum._x1_sniperangel_firebomb_01)
                        {
                            var healthpercent = (m.CurHealth / m.MaxHealth);
                            if (healthpercent > 0.75)
                            {
                                bosstimer.Duration = 7.5;
                            }
                            else
                            {
                                bosstimer.Duration = 4.0;
                            }
                        }
                    }

                    if (m.Animation == bosstimer.Animation && bosstimer.Animation != 0)
                    {
                        var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                        if (TimeElapsed > 4)
                        {
                            //DebugtextBuilder.Append(bosstimer.Name);
                            //DebugtextBuilder.Append(" ");
                            //DebugtextBuilder.Append((Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d);
                            //DebugtextBuilder.AppendLine();
                            bosstimer.LastTimer = bosstimer.Timer;
                            bosstimer.Timer = Hud.Game.CurrentGameTick;
                        }
                    }
                }
            }
            if (!BossSpawned)
                return;
            float ydelta = Hud.Window.Size.Height * 0.03f;
            int i = 0;
            foreach (var bosstimer in BossTimers)
            {
                var TimeElapsed = (Hud.Game.CurrentGameTick - bosstimer.Timer) / 60.0d;
                if (TimeElapsed > 0 && TimeElapsed < 120.0d)
                {
                    textBuilder.Clear();
                    textBuilder.AppendFormat("{0:0}", TimeElapsed);

                    if (TimeElapsed > bosstimer.Duration)
                    {
                        TimeElapsed = bosstimer.Duration;
                    }

                    float x = Hud.Window.Size.Width * 0.14f;
                    float y = Hud.Window.Size.Height * 0.05f;
                    float sizex = Hud.Window.Size.Width * 0.16f;
                    float sizey = Hud.Window.Size.Height * 0.02f;
                    float textx = Hud.Window.Size.Width * (0.14f + 0.498f - 0.42f);
                    float texty = Hud.Window.Size.Height * (0.05f + 0.111f - 0.11f);

                    y += i * ydelta;
                    texty += i * ydelta;

                    BackgroundBrush.DrawRectangle(x, y, sizex, sizey);
                    BarBrush.DrawRectangle(x, y, sizex * (float)(TimeElapsed / bosstimer.Duration), sizey);
                    //BorderBrush.DrawLine(x, y, x + sizex, y, 0.6f);
                    BorderBrush.DrawLine(x + sizex, y, x + sizex, y + sizey, BorderSize);
                    //BorderBrush.DrawLine(x, y + sizey, x + sizex, y + sizey, BorderSize);
                    //BorderBrush.DrawLine(x, y, x, y + sizey, 0.6f);

                    var layout = GreenFont.GetTextLayout(textBuilder.ToString());
                    GreenFont.DrawText(layout, textx, texty);

                    textBuilder.Clear();
                    textBuilder.Append(bosstimer.Name);

                    var layout2 = GreenFont.GetTextLayout(textBuilder.ToString());
                    GreenFont.DrawText(layout2, x, texty);

                    
                    //var layout3 = GreenFont.GetTextLayout(DebugtextBuilder.ToString());
                    //GreenFont.DrawText(layout3, Hud.Window.Size.Width * 0.35f, Hud.Window.Size.Height * (0.05f + 0.111f - 0.11f));
                    i++;
                }
            }
        }
    }
}
