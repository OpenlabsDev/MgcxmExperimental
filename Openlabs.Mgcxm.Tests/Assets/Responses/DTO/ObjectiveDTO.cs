// Copr. (c) Nexus 2023. All rights reserved.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Openlabs.Mgcxm.Assets;

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class ObjectiveDTO : ObjectDTO
{
    [DataMember(Name = "type")] public int Type { get; set; }
    [DataMember(Name = "score")] public int Score { get; set; }
}

public enum ObjectiveType
{
    Default = -1,
    FirstSessionOfDay = 1,
    DailyObjective1 = 10,
    DailyObjective2,
    DailyObjective3,
    OOBE_OpenMenu = 20,
    OOBE_GoToLockerRoom,
    OOBE_GoToActivity,
    CharadesGames = 100,
    CharadesWinsPerformer,
    CharadesWinsGuesser,
    DiscGolfWins = 200,
    DiscGolfGames,
    DiscGolfHolesUnderPar,
    DodgeballWins = 300,
    DodgeballGames,
    DodgeballHits,
    PaddleballGames = 400,
    PaddleballWins,
    PaddleballScores,
    PaintballAnyModeGames = 500,
    PaintballAnyModeWins,
    PaintballAnyModeHits,
    PaintballCTFWins = 600,
    PaintballCTFGames,
    PaintballCTFHits,
    PaintballFlagCaptures,
    PaintballTeamBattleWins = 700,
    PaintballTeamBattleGames,
    PaintballTeamBattleHits,
    SoccerWins = 800,
    SoccerGames,
    SoccerGoals,
    QuestGames = 1000,
    QuestWins,
    QuestPlayerRevives,
    QuestEnemyKills,
    PaintballFreeForAllWins = 1100,
    PaintballFreeForAllGames,
    PaintballFreeForAllHits
}