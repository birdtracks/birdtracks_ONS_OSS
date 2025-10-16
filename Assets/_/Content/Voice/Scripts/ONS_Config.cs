
using System;

namespace BirdTracks.Game.ONS
{
    public class ONS_Config
    {
        public enum AnimationState
        {
            EntryPoint,
            Intro,
            Name_ListenClosely,
            Name_ListenBegging,
            Name_CuppedIdle,
            Name_CuppedIdleBeg,
            NameReact,
            Square,
            SquareIdle,
            Square_ListenClosely,
            Square_ListenBegging,
            Square_CuppedIdle,
            Square_CuppedIdleBeg,
            SquareToCircle,
            CircleIdle,
            Circle_ListenClosely,
            Circle_ListenBegging,
            Circle_CuppedIdle,
            Circle_CuppedIdleBeg,
            CircleToTriangle,
            TriangleIdle,
            Triangle_ListenClosely,
            Triangle_ListenBegging,
            Triangle_CuppedIdle,
            Triangle_CuppedIdleBeg,
            TriangleToShapesGame,
            ExitPoint
            
        }

        public enum ResponseType
        {
            Positive,
            Negative
        }
    }
}
