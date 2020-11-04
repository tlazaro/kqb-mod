using GameLogic;
using GameLogic.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KQBMod.Training
{
    public enum LevelId : int
    {
        ThePod = 2,
        BlackQueensKeep = 4,
        HelixTemple = 7,
        TallyFields = 11,
        Spire = 14,
        SplitJuniper = 15,
        NestingFlats = 17
    }

    public class LevelInfo
    {
        public readonly LevelId level;
        private Dictionary<int, Entity> blueBerrySlots;
        private Dictionary<int, Entity> goldBerrySlots;

        public LevelInfo(LevelId level)
        {
            this.level = level;

            {
                List<Entity> blueDepositEntities = FreePlay.GetBerryDeposits(Team.Blue);
                blueBerrySlots = new Dictionary<int, Entity>();
                Vector2[] blueDepositLocations = BerrySlotPositions[new Pair<LevelId, Team.Color>(this.level, Team.Blue)];

                Main.Logger.Log("Blue deposits are: " + String.Join(", ", blueDepositEntities.Select(d => d.pos)));

                for (int i = 0; i < blueDepositLocations.Length; i++)
                {
                    var entity = blueDepositEntities.Find(d => d.pos.InsideCircle(blueDepositLocations[i], 0.1f));
                    if (entity == null)
                    {
                        throw new Exception($"Could find berry slot location {blueDepositLocations[i]} for team {Team.Blue}");
                    }

                    blueBerrySlots.Add(i + 1, entity);
                }
            }

            {
                List<Entity> goldDepositEntities = FreePlay.GetBerryDeposits(Team.Red);
                Vector2[] goldDepositLocations = BerrySlotPositions[new Pair<LevelId, Team.Color>(this.level, Team.Red)];
                goldBerrySlots = new Dictionary<int, Entity>();

                for (int i = 0; i < goldDepositLocations.Length; i++)
                {
                    var entity = goldDepositEntities.Find(d => d.pos.InsideCircle(goldDepositLocations[i], 0.1f));
                    if (entity == null)
                    {
                        throw new Exception($"Could find berry slot location {goldDepositLocations[i]} for team {Team.Red}");
                    }

                    goldBerrySlots.Add(i + 1, entity);
                }
            }
        }

        public Entity getDeposit(Team.Color team, int number)
        {
            if (number < 1 || number > 12)
            {
                throw new Exception("Berry slot number must be between 1 and 12");
            }

            return (team == Team.Blue) ? blueBerrySlots[number] : goldBerrySlots[number];
        }

        public List<Entity> getDeposits(Team.Color team)
        {
            return FreePlay.GetBerryDeposits(team);
        }

        static Dictionary<Team.Color, Vector2[]> HelixDeposits = new Dictionary<Team.Color, Vector2[]>
        {
            [Team.Blue] = new Vector2[]
            {
                new Vector2(-18.77277f, 0.9974738f),
                new Vector2(-16.95459f, 0.9974738f),
                new Vector2(-15.13641f, 0.9974738f),
                new Vector2(-19.68187f, -0.820708f),
                new Vector2(-17.86368f, -0.820708f),
                new Vector2(-20.59096f, -2.63889f),
                new Vector2(-18.77277f, -2.63889f),
                new Vector2(-19.68187f, -4.457072f),
                new Vector2(-17.86368f, -4.457072f),
                new Vector2(-18.77277f, -6.275254f),
                new Vector2(-16.95459f, -6.275254f),
                new Vector2(-15.13641f, -6.275254f)
            },
            [Team.Red] = new Vector2[]
            {
                new Vector2(18.77225f, 0.9974738f),
                new Vector2(16.95407f, 0.9974738f),
                new Vector2(15.13589f, 0.9974738f),
                new Vector2(19.68134f, -0.820708f),
                new Vector2(17.86316f, -0.820708f),
                new Vector2(20.59043f, -2.63889f),
                new Vector2(18.77225f, -2.63889f),
                new Vector2(19.68134f, -4.457072f),
                new Vector2(17.86316f, -4.457072f),
                new Vector2(18.77225f, -6.275254f),
                new Vector2(16.95407f, -6.275254f),
                new Vector2(15.13589f, -6.275254f)
            }
        };

        static Dictionary<Team.Color, Vector2[]> TallyDeposits = new Dictionary<Team.Color, Vector2[]>
        {
            [Team.Blue] = new Vector2[]
            {
                new Vector2(-3.466141f, 0.7928657f),
                new Vector2(-1.761595f, 0.7928657f),

                new Vector2(-6.022938f, -0.568688f),
                new Vector2(-4.318393f, -0.568688f),
                new Vector2(-2.613847f, -0.568688f),

                new Vector2(-8.579735f, -1.934374f),
                new Vector2(-6.875189f, -1.934374f),
                new Vector2(-5.170644f, -1.934374f),
                new Vector2(-3.466098f, -1.934374f),

                new Vector2(-7.78427f, -3.297995f),
                new Vector2(-6.079725f, -3.297995f),

                new Vector2(-6.931997f, -4.661614f)
            },
            [Team.Red] = new Vector2[]
            {
                new Vector2(3.465616f, 0.7928657f),
                new Vector2(1.76107f, 0.7928657f),
                new Vector2(6.022414f, -0.568688f),
                new Vector2(4.317868f, -0.568688f),
                new Vector2(2.613323f, -0.568688f),
                new Vector2(8.579211f, -1.934374f),
                new Vector2(6.874666f, -1.934374f),
                new Vector2(5.17012f, -1.934374f),
                new Vector2(3.465575f, -1.934374f),
                new Vector2(7.783744f, -3.297995f),
                new Vector2(6.079199f, -3.297995f),
                new Vector2(6.931472f, -4.661614f)
            }
        };

        static Dictionary<Pair<LevelId, Team.Color>, Vector2[]> BerrySlotPositions = new Dictionary<Pair<LevelId, Team.Color>, Vector2[]>
        {
            [new Pair<LevelId, Team.Color>(LevelId.HelixTemple, Team.Blue)] = HelixDeposits[Team.Color.Blue],
            [new Pair<LevelId, Team.Color>(LevelId.HelixTemple, Team.Red)] = HelixDeposits[Team.Color.Red],
            [new Pair<LevelId, Team.Color>(LevelId.TallyFields, Team.Blue)] = TallyDeposits[Team.Color.Blue],
            [new Pair<LevelId, Team.Color>(LevelId.TallyFields, Team.Red)] = TallyDeposits[Team.Color.Red],
        };
    }

    /*
The pod

[KQBMod] Getting deposit info
[KQBMod] Blue deposits
new Vector2 (-19.73867, 9.520034)
new Vector2 (-18.03412, 9.520034)
new Vector2 (-20.59094, 8.951859)
new Vector2 (-18.8864, 8.951859)
new Vector2 (-17.18185, 8.951859)
new Vector2 (-19.73867, 8.383683)
new Vector2 (-18.03412, 8.383683)
new Vector2 (-20.59094, 7.815508)
new Vector2 (-18.8864, 7.815508)
new Vector2 (-17.18185, 7.815508)
new Vector2 (-19.73867, 7.247334)
new Vector2 (-18.03412, 7.247334)
[KQBMod] Gold deposits
new Vector2 (18.0336, 9.520034)
new Vector2 (19.73815, 9.520034)
new Vector2 (17.18133, 8.951859)
new Vector2 (18.88587, 8.951859)
new Vector2 (20.59042, 8.951859)
new Vector2 (18.0336, 8.383683)
new Vector2 (19.73815, 8.383683)
new Vector2 (17.18133, 7.815508)
new Vector2 (18.88587, 7.815508)
new Vector2 (20.59042, 7.815508)
new Vector2 (18.0336, 7.247334)
new Vector2 (19.73815, 7.247334)
[KQBMod] Trying to escape the 3 Ways


    The spire

    [KQBMod] Getting deposit info
[KQBMod] Blue deposits
new Vector2 (-3.750234, 11.47456)
new Vector2 (-2.386598, 11.47456)
new Vector2 (-1.022961, 11.47456)
new Vector2 (-4.613863, 10.11094)
new Vector2 (-3.022954, 10.11094)
new Vector2 (-1.432045, 10.11094)
new Vector2 (-5.454764, 8.747315)
new Vector2 (-3.636582, 8.747315)
new Vector2 (-1.818401, 8.747315)
new Vector2 (-6.307029, 7.383696)
new Vector2 (-4.261575, 7.383696)
new Vector2 (-2.21612, 7.383696)
[KQBMod] Gold deposits
new Vector2 (1.022437, 11.47456)
new Vector2 (2.386073, 11.47456)
new Vector2 (3.74971, 11.47456)
new Vector2 (1.420156, 10.11094)
new Vector2 (3.011065, 10.11094)
new Vector2 (4.601974, 10.11094)
new Vector2 (1.817876, 8.747315)
new Vector2 (3.636058, 8.747315)
new Vector2 (5.45424, 8.747315)
new Vector2 (2.22696, 7.383696)
new Vector2 (4.272414, 7.383696)
new Vector2 (6.317869, 7.383696)

    Split

    [KQBMod] Getting deposit info
[KQBMod] Blue deposits
[KQBMod] Deposit id 65590 pos (-20.56821, 11.2473)
[KQBMod] Deposit id 65591 pos (-19.65912, 11.2473)
[KQBMod] Deposit id 65592 pos (-18.75003, 11.2473)
[KQBMod] Deposit id 65593 pos (-17.84094, 11.2473)
[KQBMod] Deposit id 65594 pos (-20.11367, 10.33821)
[KQBMod] Deposit id 65595 pos (-19.20458, 10.33821)
[KQBMod] Deposit id 65596 pos (-18.29549, 10.33821)
[KQBMod] Deposit id 65602 pos (-21.02276, -10.11608)
[KQBMod] Deposit id 65603 pos (-20.11367, -10.11608)
[KQBMod] Deposit id 65604 pos (-19.20458, -10.11608)
[KQBMod] Deposit id 65605 pos (-18.29549, -10.11608)
[KQBMod] Deposit id 65606 pos (-17.3864, -10.11608)
[KQBMod] Gold deposits
[KQBMod] Deposit id 65583 pos (17.84042, 11.2473)
[KQBMod] Deposit id 65584 pos (18.74951, 11.2473)
[KQBMod] Deposit id 65585 pos (19.6586, 11.2473)
[KQBMod] Deposit id 65586 pos (20.56769, 11.2473)
[KQBMod] Deposit id 65587 pos (18.29496, 10.33821)
[KQBMod] Deposit id 65588 pos (19.20405, 10.33821)
[KQBMod] Deposit id 65589 pos (20.11314, 10.33821)
[KQBMod] Deposit id 65597 pos (17.38587, -10.11608)
[KQBMod] Deposit id 65598 pos (18.29496, -10.11608)
[KQBMod] Deposit id 65599 pos (19.20405, -10.11608)
[KQBMod] Deposit id 65600 pos (20.11314, -10.11608)
[KQBMod] Deposit id 65601 pos (21.02223, -10.11608)

 */

}
