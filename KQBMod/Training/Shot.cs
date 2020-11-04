using GameLogic.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KQBMod.Training
{
    public enum ShotType : int
    {
        StationaryOGToss,
        StationaryBindToss,
        StationaryJumpToss,
        StationaryFlatToss,
        RunningOGToss,
        RunningBindToss,
        RunningJumpToss,
        RunningFlatToss,
        FallingToss,
    }

    public class Shot
    {
        public ShotType type;
        public int slot;
        public GameLogic.Math.Vector2 pos;
        public bool holdingBerry;
        public bool facingRight;
        public int[] occupiedSlots;
        public string description;

        public Shot(ShotType type, int slot, Vector2 pos, bool holdingBerry, bool facingRight, int[] occupiedSlots, string description)
        {
            this.type = type;
            this.slot = slot;
            this.pos = pos;
            this.holdingBerry = holdingBerry;
            this.facingRight = facingRight;
            this.occupiedSlots = occupiedSlots;
            this.description = description;
        }

        public static UnityEngine.Color GetColor(int r, int g, int b)
        {
            return new UnityEngine.Color(r / 255f, g / 255f, b / 255f, 1.0f);
        }

        public static Dictionary<ShotType, UnityEngine.Color> ShotColor = new Dictionary<ShotType, UnityEngine.Color>
        {
            [ShotType.StationaryOGToss] = GetColor(129, 49, 215),
            [ShotType.StationaryBindToss] = GetColor(227, 226, 21),
            [ShotType.StationaryJumpToss] = GetColor(211, 49, 81),
            [ShotType.RunningOGToss] = GetColor(139, 32, 32),
            [ShotType.RunningBindToss] = GetColor(25, 225, 46),
            [ShotType.RunningJumpToss] = GetColor(26, 224, 226),
            [ShotType.StationaryFlatToss] = GetColor(140, 79, 20),
            [ShotType.RunningFlatToss] = GetColor(222, 165, 25),
            [ShotType.FallingToss] = GetColor(27, 27, 220)
        };
    }
}
