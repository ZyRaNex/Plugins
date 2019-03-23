using Turbo.Plugins.Default;
using System.Linq;

namespace Turbo.Plugins.Zy
{
    public class OculusColor : BasePlugin, IInGameWorldPainter, ICustomizer
    {
        public WorldDecoratorCollection DecoratorBase { get; set; }
        public WorldDecoratorCollection DecoratorClosest { get; set; }
        public WorldDecoratorCollection DecoratorInside { get; set; }
        public OculusColor()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);

            DecoratorBase = new WorldDecoratorCollection(
                new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 128, 255, 0, -2),
                    Radius = 10.0f,
                },
                new GroundLabelDecorator(Hud)
                {
                    CountDownFrom = 7,
                    TextFont = Hud.Render.CreateFont("tahoma", 11, 255, 96, 255, 96, true, false, 128, 0, 0, 0, true),
                },
                new GroundTimerDecorator(Hud)
                {
                    CountDownFrom = 7,
                    BackgroundBrushEmpty = Hud.Render.CreateBrush(128, 0, 0, 0, 0),
                    BackgroundBrushFill = Hud.Render.CreateBrush(200, 0, 192, 0, 0),
                    Radius = 30,
                }
                );
            DecoratorClosest = new WorldDecoratorCollection(
                new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 0, 0, 255, -2),
                    Radius = 10.0f,
                },
                new GroundLabelDecorator(Hud)
                {
                    CountDownFrom = 7,
                    TextFont = Hud.Render.CreateFont("tahoma", 11, 255, 96, 255, 96, true, false, 128, 0, 0, 0, true),
                },
                new GroundTimerDecorator(Hud)
                {
                    CountDownFrom = 7,
                    BackgroundBrushEmpty = Hud.Render.CreateBrush(128, 0, 0, 0, 0),
                    BackgroundBrushFill = Hud.Render.CreateBrush(200, 0, 192, 0, 0),
                    Radius = 30,
                }
                );
            DecoratorInside = new WorldDecoratorCollection(
                new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 0, 0, -2),
                    Radius = 10.0f,
                },
                new GroundLabelDecorator(Hud)
                {
                    CountDownFrom = 7,
                    TextFont = Hud.Render.CreateFont("tahoma", 11, 255, 96, 255, 96, true, false, 128, 0, 0, 0, true),
                },
                new GroundTimerDecorator(Hud)
                {
                    CountDownFrom = 7,
                    BackgroundBrushEmpty = Hud.Render.CreateBrush(128, 0, 0, 0, 0),
                    BackgroundBrushFill = Hud.Render.CreateBrush(200, 0, 192, 0, 0),
                    Radius = 30,
                }
                );
        }
        public void Customize()
        {
            Hud.TogglePlugin<OculusPlugin>(false);
        }
        public void PaintWorld(WorldLayer layer)
        {
            if (Hud.Game.IsInTown) return;

            IWorldCoordinate WizPosition = Hud.Game.Players.First().FloorCoordinate;
            int WizardsIngame = 0;
            foreach (var player in Hud.Game.Players)
            {
                if (player.HeroClassDefinition.HeroClass == HeroClass.Wizard)
                {
                    WizPosition = player.FloorCoordinate;
                    WizardsIngame++;
                }
            }

            float mindist = float.MaxValue;
            var actors = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._generic_proxy && x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_None, Hud.Sno.SnoPowers.OculusRing.Sno) == 1);
            if (actors.Count() > 0)
            {
                var closest = actors.First();

                foreach (var actor in actors)
                {
                    float dist = actor.FloorCoordinate.XYDistanceTo(WizPosition);
                    if (dist < mindist)
                    {
                        closest = actor;
                        mindist = dist;
                    }
                }
                foreach (var actor in actors)
                {
                    if (WizardsIngame == 1)
                    {
                        if (actor == closest)
                        {
                            if (mindist < 13.3f)
                            {
                                DecoratorInside.Paint(layer, actor, actor.FloorCoordinate, null); 
                            }
                            else
                            {
                                DecoratorClosest.Paint(layer, actor, actor.FloorCoordinate, null);
                            }
                        }
                        else
                        {
                            DecoratorBase.Paint(layer, actor, actor.FloorCoordinate, null);
                        }
                    }
                    else
                    {
                        DecoratorBase.Paint(layer, actor, actor.FloorCoordinate, null);
                    }
                }
            }
        }
    }
}
