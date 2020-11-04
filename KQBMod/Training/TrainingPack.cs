using GameLogic;
using GameLogic.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KQBMod.Training
{

    public class TrainingPack
    {
        public LevelId levelId;
        public List<Shot> shots;

        public TrainingPack(LevelId levelId, List<Shot> shots)
        {
            this.levelId = levelId;
            this.shots = shots;
        }

        // TODO should come from a JSON
        // All shots from https://www.reddit.com/r/KillerQueen/comments/hr6dc8/berry_compendium_postpatch/?utm_medium=android_app&utm_source=share
        public static TrainingPack BasicHelixShots()
        {
            List<Shot> shots = new List<Shot>()
            {
                // Page 3
                new Shot(ShotType.RunningOGToss, 7, new Vector2(-2.94f, -1.74f), true, false, new int[]{  }, "#3 Snail platform, center berries, hit gound"),

                new Shot(ShotType.RunningOGToss, 8, new Vector2(-5.0f, -1.74f), true, false, new int[]{  }, "#3 Snail platform, passing berries"),
                new Shot(ShotType.RunningOGToss, 7, new Vector2(-5.0f, -1.74f), true, false, new int[]{  }, "#3 Snail platform, passing berries"),
                new Shot(ShotType.RunningOGToss, 6, new Vector2(-5.0f, -1.74f), true, false, new int[]{  }, "#3 Snail platform, passing berries"),

                new Shot(ShotType.RunningOGToss, 5, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#3 Snail platform, entering base"),
                new Shot(ShotType.RunningOGToss, 4, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#3 Snail platform, entering base"),
                new Shot(ShotType.RunningOGToss, 6, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 4 }, "#3 Snail platform, entering base"),
                new Shot(ShotType.RunningOGToss, 8, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 4, 6 }, "#3 Snail platform, entering base, shoot from egg, hit wall"),
                new Shot(ShotType.RunningOGToss, 10, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 4, 6, 8 }, "#3 Snail platform, entering base, shoot from egg, hit wall"),

                new Shot(ShotType.StationaryOGToss, 12, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#3 Egg floating platform"),
                new Shot(ShotType.StationaryOGToss, 11, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#3 Egg floating platform"),
                new Shot(ShotType.StationaryOGToss, 10, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#3 Egg floating platform"),

                new Shot(ShotType.StationaryJumpToss, 12, new Vector2(-7.58f, -11.50f), true, false, new int[]{ }, "#3 Bottom ground"),
                new Shot(ShotType.StationaryJumpToss, 9, new Vector2(-7.58f, -11.50f), true, false, new int[]{ 12 }, "#3 Bottom ground"),
                new Shot(ShotType.StationaryJumpToss, 5, new Vector2(-19.92f, -8.55f), true, true, new int[]{ }, "#3 Hit snail goal ledge"),
                new Shot(ShotType.StationaryJumpToss, 10, new Vector2(-21.30f, -11.50f), true, true, new int[]{ }, "#3 Bottom ground"),

                // Tops throws
                new Shot(ShotType.StationaryOGToss, 3, new Vector2(-4.15f, 4.18f), true, false, new int[]{ }, "#1 Upper floating platform"),
                new Shot(ShotType.StationaryOGToss, 9, new Vector2(-4.15f, 4.18f), true, false, new int[]{ }, "#1 Upper floating platform, touch ground"),

                new Shot(ShotType.StationaryOGToss, 2, new Vector2(-9.53f, 1.23f), true, false, new int[]{ }, "#2 Small floating platform"),
                new Shot(ShotType.StationaryOGToss, 4, new Vector2(-9.53f, 1.23f), true, false, new int[]{ }, "#2 Small floating platform"),
                new Shot(ShotType.StationaryOGToss, 6, new Vector2(-9.53f, 1.23f), true, false, new int[]{ 4 }, "#2 Small floating platform"),

                new Shot(ShotType.StationaryOGToss, 8, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#2 Snail platform, entering base, shoot from egg"),
                new Shot(ShotType.StationaryOGToss, 7, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#2 Snail platform, entering base, shoot from egg"),
                new Shot(ShotType.StationaryOGToss, 5, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#2 Snail platform, entering base, shoot from egg"),
                new Shot(ShotType.StationaryOGToss, 6, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 5 }, "#2 Snail platform, entering base, shoot from egg"),

                new Shot(ShotType.RunningOGToss, 3, new Vector2(5.77f, 4.18f), true, false, new int[]{ }, "#2 Upper floating platform"),

                new Shot(ShotType.StationaryBindToss, 3, new Vector2(-5.0f, -1.74f), true, false, new int[]{  }, "#2 Snail platform, front of berries"),
                new Shot(ShotType.StationaryBindToss, 9, new Vector2(-5.0f, -1.74f), true, false, new int[]{ 3 }, "#2 Snail platform, front of berries"),
                new Shot(ShotType.StationaryBindToss, 7, new Vector2(-9.86f, -1.74f), true, false, new int[]{  }, "#2 Snail platform, entering base, hit ceiling"),

                // first page
                new Shot(ShotType.RunningBindToss, 3, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#1 Snail platform, entering base, shoot from egg"),
                new Shot(ShotType.RunningBindToss, 2, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#1 Snail platform, entering base, shoot from egg"),
                new Shot(ShotType.RunningBindToss, 1, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#1 Snail platform, entering base, shoot from egg"),

                new Shot(ShotType.RunningBindToss, 7, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 2 }, "#3 Snail platform, entering base, shoot from egg, hit wall"),

                new Shot(ShotType.RunningBindToss, 5, new Vector2(6.28f, -1.74f), true, false, new int[]{ }, "#1 Drop onto snail platform"),
                new Shot(ShotType.RunningBindToss, 7, new Vector2(6.28f, -1.74f), true, false,  new int[]{ 5 }, "#1 Drop onto snail platform"),
                new Shot(ShotType.RunningBindToss, 8, new Vector2(6.28f, -1.74f), true, false, new int[]{ 5, 7 }, "#1 Drop onto snail platform"),

                new Shot(ShotType.RunningBindToss, 2, new Vector2(-2.94f, -1.74f), true, false, new int[]{ }, "#1 Snail platform, berry start"),
                new Shot(ShotType.RunningBindToss, 4, new Vector2(-2.94f, -1.74f), true, false,  new int[]{ }, "#1 Snail platform, berry center"),

                new Shot(ShotType.RunningBindToss, 6, new Vector2(-2.94f, -1.74f), true, false, new int[]{ 4 }, "#1 Snail platform, berry center"),
                new Shot(ShotType.RunningBindToss, 6, new Vector2(-5.94f, -1.74f), true, false,  new int[]{ }, "#1 Snail platform, hit ceiling, bounce"),

                // Bottoms throws
                new Shot(ShotType.RunningBindToss, 8, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#1 Egg floating platform"),
                new Shot(ShotType.RunningBindToss, 9, new Vector2(-7.0f, -5.55f), true, false,  new int[]{ }, "#1 Egg floating platform"),
                new Shot(ShotType.RunningOGToss, 10, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#1 Egg floating platform"),

                new Shot(ShotType.StationaryBindToss, 12, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#2 Egg floating platform"),
                new Shot(ShotType.StationaryBindToss, 11, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#2 Egg floating platform"),
                new Shot(ShotType.StationaryBindToss, 10, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#2 Egg floating platform"),
                new Shot(ShotType.StationaryBindToss, 9, new Vector2(-7.0f, -5.55f), true, false, new int[]{ }, "#2 Egg floating platform"),

                new Shot(ShotType.StationaryJumpToss, 6, new Vector2(-14.40f, -8.55f), true, false, new int[]{ }, "#1 Base platform"),
                new Shot(ShotType.StationaryJumpToss, 7, new Vector2(-14.40f, -8.55f), true, false, new int[]{ }, "#1 Base platform"),
                new Shot(ShotType.StationaryJumpToss, 4, new Vector2(-14.10f, -8.55f), true, false,  new int[]{ }, "#1 Base platform, narrow shot"),

                new Shot(ShotType.StationaryJumpToss, 6, new Vector2(-19.92f, -8.55f), true, false, new int[]{ }, "#1 Base platform, hit wall"),
                new Shot(ShotType.StationaryJumpToss, 4, new Vector2(-19.92f, -8.55f), true, false,  new int[]{ 6 }, "#1 Base platform, hit wall"),

                new Shot(ShotType.StationaryBindToss, 10, new Vector2(-14.40f, -8.55f), true, false,  new int[]{ }, "#1 Base platform, one hole away"),
                new Shot(ShotType.StationaryBindToss, 11, new Vector2(-14.40f, -8.55f), true, false, new int[]{ }, "#1 Base platform, one hole away"),
                new Shot(ShotType.StationaryBindToss, 12, new Vector2(-19.62f, -8.55f), true, true,  new int[]{ }, "#1 Base platform, one hole away"),

                new Shot(ShotType.RunningBindToss, 12, new Vector2(3.20f, -8.55f), true, false,  new int[]{ }, "#1 Bottom gate platform"),

                new Shot(ShotType.RunningJumpToss, 8, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#3 Snail platform, entering base, shoot from egg, hit ceiling, wall"),
                new Shot(ShotType.RunningJumpToss, 11, new Vector2(-1.42f, -8.55f), true, false, new int[]{ }, "#3 Gate platfom hit ceiling"),

                new Shot(ShotType.RunningJumpToss, 9, new Vector2(-9.86f, -1.74f), true, false, new int[]{ }, "#2 Shoot from end of egg, ceiling, wall"),
                new Shot(ShotType.RunningJumpToss, 11, new Vector2(-9.86f, -1.74f), true, false, new int[]{ 9 }, "#2 Shoot from end of egg, ceiling, wall"),

                new Shot(ShotType.RunningJumpToss, 12, new Vector2(-7.58f, -11.50f), true, false, new int[]{ }, "#2 Bottom ground"),
                new Shot(ShotType.RunningJumpToss, 9, new Vector2(-7.58f, -11.50f), true, false, new int[]{ 12 }, "#2 Bottom ground"),
                new Shot(ShotType.RunningJumpToss, 11, new Vector2(-7.58f, -11.50f), true, false, new int[]{ }, "#2 Bottom ground"),
                new Shot(ShotType.RunningJumpToss, 8, new Vector2(-7.58f, -11.50f), true, false, new int[]{ 11 }, "#2 Bottom ground"),

                new Shot(ShotType.RunningJumpToss, 4, new Vector2(-14.10f, -8.55f), true, false,  new int[]{ }, "#2 Hit wall"),
                new Shot(ShotType.RunningJumpToss, 5, new Vector2(-14.10f, -8.55f), true, false,  new int[]{ }, "#2 Hit wall"),

                new Shot(ShotType.RunningJumpToss, 10, new Vector2(15.75f, -11.50f), true, true, new int[]{ }, "#1 Through the wrap"),

                new Shot(ShotType.RunningFlatToss, 1, new Vector2(-2.94f, -1.74f), true, false, new int[]{ }, "#2 Snail platform, berry center"),
                new Shot(ShotType.RunningFlatToss, 11, new Vector2(-1.42f, -8.55f), true, false, new int[]{ }, "#2 Gate platform"),
                new Shot(ShotType.RunningFlatToss, 10, new Vector2(-1.42f, -8.55f), true, false, new int[]{ }, "#2 Gate platform"),

                new Shot(ShotType.RunningFlatToss, 7, new Vector2(-10.0f, -5.55f), true, false, new int[]{ }, "#3 Egg floating platform"),
                new Shot(ShotType.RunningFlatToss, 6, new Vector2(-10.0f, -5.55f), true, false, new int[]{ 7 }, "#3 Egg floating platform"),

                new Shot(ShotType.FallingToss, 3, new Vector2(-0.94f, 10.10f), true, false, new int[]{ }, "#3 top most platform"),
                new Shot(ShotType.FallingToss, 7, new Vector2(-0.94f, 10.10f), true, false, new int[]{ }, "#3 top most platform"),

                new Shot(ShotType.FallingToss, 3, new Vector2(-4.15f, 4.18f), false, false, new int[]{ }, "#3 Upper floating platform, bounce top platform"),
                new Shot(ShotType.FallingToss, 3, new Vector2(-6.65f, 4.18f), true, false, new int[]{ }, "#3 Upper floating platform, bounce lower platform"),
                new Shot(ShotType.FallingToss, 2, new Vector2(-2.70f, 4.18f), true, false, new int[]{ 3 }, "#3 Upper floating platform, bounce lower platform"),
                new Shot(ShotType.FallingToss, 8, new Vector2(-6.65f, 4.18f), true, false, new int[]{ }, "#3 Upper floating platform, drop onto lower platform, no bounce"),
            };

            return new TrainingPack(LevelId.HelixTemple, shots);
        }

        public static TrainingPack TallyShots()
        {
            List<Shot> shots = new List<Shot>()
            {
                new Shot(ShotType.RunningBindToss, 2, new Vector2(-16.50f, -2.63f), true, true, new int[]{ }, "#1 Side platfrom"),
                new Shot(ShotType.RunningBindToss, 1, new Vector2(-17.50f, -2.63f), true, true, new int[]{ }, "#1 Side platfrom"),
                new Shot(ShotType.RunningBindToss, 5, new Vector2(-18.50f, -2.63f), true, true, new int[]{ }, "#1 Side platfrom"),
                new Shot(ShotType.RunningBindToss, 4, new Vector2(-21.0f, -2.63f), true, true, new int[]{ }, "#1 Side platfrom"),
                new Shot(ShotType.RunningBindToss, 3, new Vector2(-21.0f, -2.63f), true, true, new int[]{ }, "#1 Side platfrom"),

                new Shot(ShotType.RunningBindToss, 9, new Vector2(-21.0f, -2.63f), true, true, new int[]{ 4 }, "#2 Side platfrom"),
                new Shot(ShotType.RunningBindToss, 8, new Vector2(-21.0f, -2.63f), true, true, new int[]{ 3 }, "#2 Side platfrom"),

                new Shot(ShotType.RunningOGToss, 7, new Vector2(-16.50f, -2.63f), true, true, new int[]{ }, "#2 Side platfrom"),
                new Shot(ShotType.RunningOGToss, 6, new Vector2(-16.50f, -2.63f), true, true, new int[]{ }, "#2 Side platfrom"),

                new Shot(ShotType.RunningOGToss, 11, new Vector2(-19.0f, -2.63f), true, true, new int[]{ 6 }, "#2 Side platfrom"),
                new Shot(ShotType.RunningOGToss, 10, new Vector2(-20.0f, -2.63f), true, true, new int[]{  }, "#2 Side platfrom"),
                new Shot(ShotType.RunningOGToss, 12, new Vector2(-21.0f, -2.63f), true, true, new int[]{  }, "#2 Side platfrom"),

                new Shot(ShotType.RunningOGToss, 12, new Vector2(-15.8f, -5.59f), true, true, new int[]{ }, "#1 Lower platform"),

                new Shot(ShotType.RunningOGToss, 12, new Vector2(-3.97f, -5.59f), true, false, new int[]{ }, "#1 Lower platform"),

                new Shot(ShotType.StationaryBindToss, 6, new Vector2(-14.8f, -5.59f), true, true, new int[]{ }, "#1 Lower platform"),
                new Shot(ShotType.StationaryBindToss, 7, new Vector2(-14.8f, -5.59f), true, true, new int[]{ 6 }, "#1 Lower platform"),

                new Shot(ShotType.StationaryBindToss, 7, new Vector2(-12.8f, -5.59f), true, true, new int[]{ }, "#1 Lower platform"),
                new Shot(ShotType.StationaryBindToss, 8, new Vector2(-12.8f, -5.59f), true, true, new int[]{ 7 }, "#1 Lower platform"),

                new Shot(ShotType.StationaryBindToss, 10, new Vector2(-10.8f, -5.59f), true, true, new int[]{ }, "#1 Lower platform"),
                new Shot(ShotType.StationaryBindToss, 8, new Vector2(-10.8f, -5.59f), true, true, new int[]{ 10 }, "#1 Lower platform"),
                new Shot(ShotType.StationaryBindToss, 9, new Vector2(-10.8f, -5.59f), true, true, new int[]{ 8, 10 }, "#1 Lower platform"),

                new Shot(ShotType.StationaryBindToss, 11, new Vector2(-8.8f, -5.59f), true, true, new int[]{ }, "#1 Lower platform"),
                new Shot(ShotType.StationaryBindToss, 9, new Vector2(-8.8f, -5.59f), true, true, new int[]{ 11 }, "#1 Lower platform"),


                new Shot(ShotType.StationaryJumpToss, 1, new Vector2(1.45f, -2.63f), true, true, new int[]{ }, "#1 Center platform"),

                new Shot(ShotType.StationaryJumpToss, 3, new Vector2(-8.8f, -5.59f), true, true, new int[]{ 10 }, "#2 Lower platform"),
                new Shot(ShotType.StationaryJumpToss, 1, new Vector2(-8.8f, -5.59f), true, true, new int[]{ 7, 12 }, "#2 Lower platform"),
                new Shot(ShotType.StationaryJumpToss, 4, new Vector2(-7.8f, -5.59f), true, true, new int[]{ 11 }, "#2 Lower platform"),
                new Shot(ShotType.StationaryJumpToss, 2, new Vector2(-7.8f, -5.59f), true, true, new int[]{ 8 }, "#2 Lower platform"),
                new Shot(ShotType.StationaryJumpToss, 5, new Vector2(-6.8f, -5.59f), true, true, new int[]{ }, "#2 Lower platform"),


                new Shot(ShotType.StationaryOGToss, 6, new Vector2(-16.0f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryOGToss, 10, new Vector2(-16.0f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryOGToss, 12, new Vector2(-16.0f, -2.63f), true, true, new int[]{ 10 }, "#3 Side platfrom"),

                new Shot(ShotType.RunningBindToss, 6, new Vector2(-15.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 7, new Vector2(-14.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 8, new Vector2(-13.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 9, new Vector2(-12.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),

                new Shot(ShotType.RunningBindToss, 9, new Vector2(8.8f, -5.59f), true, false, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 11, new Vector2(8.8f, -5.59f), true, false, new int[]{ 9 }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 8, new Vector2(7.8f, -5.59f), true, false, new int[]{ 9 }, "#3 Lower platform"),
                new Shot(ShotType.RunningBindToss, 10, new Vector2(7.8f, -5.59f), true, false, new int[]{ 8, 9 }, "#3 Lower platform"),

                new Shot(ShotType.RunningJumpToss, 2, new Vector2(7.8f, -5.59f), true, false, new int[]{ }, "#3 Lower platform"),

                new Shot(ShotType.RunningJumpToss, 5, new Vector2(-10.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningJumpToss, 2, new Vector2(-11.8f, -5.59f), true, true, new int[]{ 4, 8, 11, 12 }, "#3 Lower platform"),
                new Shot(ShotType.RunningJumpToss, 4, new Vector2(-11.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningJumpToss, 3, new Vector2(-12.8f, -5.59f), true, true, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningJumpToss, 1, new Vector2(-12.8f, -5.59f), true, true, new int[]{ 3 }, "#3 Lower platform"),

                new Shot(ShotType.RunningJumpToss, 11, new Vector2(1.57f, -8.54f), true, false, new int[]{  }, "#3 Gate platform"),
                new Shot(ShotType.RunningJumpToss, 6, new Vector2(1.57f, -8.54f), true, false, new int[]{ 11 }, "#3 Gate platform"),
                new Shot(ShotType.RunningJumpToss, 7, new Vector2(1.57f, -8.54f), true, false, new int[]{ 11 }, "#3 Gate platform"),

                new Shot(ShotType.StationaryJumpToss, 8, new Vector2(-1.57f, -8.54f), true, false, new int[]{ }, "#3 Gate platform"),


                new Shot(ShotType.StationaryFlatToss, 12, new Vector2(1.57f, -8.54f), true, false, new int[]{ }, "#3 Gate platform"),
                new Shot(ShotType.StationaryFlatToss, 11, new Vector2(1.57f, -8.54f), true, false, new int[]{ }, "#3 Gate platform"),
                new Shot(ShotType.StationaryFlatToss, 10, new Vector2(1.57f, -8.54f), true, false, new int[]{ 11 }, "#3 Gate platform"),


                new Shot(ShotType.RunningFlatToss, 5, new Vector2(8.8f, -5.59f), true, false, new int[]{ }, "#3 Lower platform"),
                new Shot(ShotType.RunningFlatToss, 4, new Vector2(8.8f, -5.59f), true, false, new int[]{ 5 }, "#3 Lower platform"),
                new Shot(ShotType.RunningFlatToss, 3, new Vector2(8.8f, -5.59f), true, false, new int[]{ 4, 5 }, "#3 Lower platform"),

                new Shot(ShotType.RunningFlatToss, 5, new Vector2(-16.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom, ceiling"),
                new Shot(ShotType.RunningFlatToss, 4, new Vector2(-17.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom, ceiling"),
                new Shot(ShotType.RunningFlatToss, 3, new Vector2(-18.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom, ceiling"),

                new Shot(ShotType.StationaryBindToss, 4, new Vector2(-16.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 9, new Vector2(-16.50f, -2.63f), true, true, new int[]{ 4 }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 3, new Vector2(-17.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 8, new Vector2(-17.50f, -2.63f), true, true, new int[]{ 3 }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 7, new Vector2(-18.50f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 11, new Vector2(-18.50f, -2.63f), true, true, new int[]{ 7 }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 12, new Vector2(-21.0f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 6, new Vector2(-21.0f, -2.63f), true, true, new int[]{ }, "#3 Side platfrom"),
                new Shot(ShotType.StationaryBindToss, 10, new Vector2(-21.0f, -2.63f), true, true, new int[]{ 6 }, "#3 Side platfrom"),
            };

            return new TrainingPack(LevelId.TallyFields, shots);
        }
    }
}
