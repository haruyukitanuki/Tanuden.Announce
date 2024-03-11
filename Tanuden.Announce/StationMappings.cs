// ReSharper disable StringLiteralTypo

namespace Tanuden.Announce;

internal class StationData
{
    public int Id { get; set; }
    public string JapaneseName { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsInterchangeWithJieiR { get; set; }
    public bool IsCurvedPlatform { get; set; }
}

internal abstract class StationMappings
{
    internal static readonly List<StationData> StationDatabase = new()
    {
        new StationData
        {
            Id = 1,
            JapaneseName = "館浜",
            Name = "Tatehama",
            IsInterchangeWithJieiR = true
        },
        new StationData
        {
            Id = 2,
            JapaneseName = "駒野",
            Name = "Komano",
            IsCurvedPlatform = true
        },
        new StationData
        {
            Id = 3,
            JapaneseName = "河原崎",
            Name = "Kawarazaki"
        },
        new StationData
        {
            Id = 4,
            JapaneseName = "海岸公園",
            Name = "Kaigankoen",
            IsCurvedPlatform = true
        },
        new StationData
        {
            Id = 5,
            JapaneseName = "虹ケ浜",
            Name = "Nijigahama"
        },
        new StationData
        {
            Id = 6,
            JapaneseName = "津崎",
            Name = "Tsuzaki",
            IsCurvedPlatform = true
        },
        new StationData
        {
            Id = 7,
            JapaneseName = "浜園",
            Name = "Hamazono",
            IsInterchangeWithJieiR = true,
            IsCurvedPlatform = true
        },
        new StationData
        {
            Id = 8,
            JapaneseName = "羽衣橋",
            Name = "Hagoromobashi"
        },
        new StationData
        {
            Id = 9,
            JapaneseName = "新井川",
            Name = "Araigawa"
        },
        new StationData
        {
            Id = 10,
            JapaneseName = "新野崎",
            Name = "Shinnozaki"
        },
        new StationData
        {
            Id = 11,
            JapaneseName = "江ノ原",
            Name = "Enohara"
        },
        new StationData
        {
            Id = 12,
            JapaneseName = "大道寺",
            Name = "Daidoji"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "赤山町",
            Name = "Akayamacho"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "玉川温泉",
            Name = "Tamagawaonsen"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "灘",
            Name = "Nada"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "東井",
            Name = "Toi"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "新大路",
            Name = "Shinoji"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "大路",
            Name = "Oji"
        },
        new StationData
        {
            Id = 0,
            JapaneseName = "大手橋",
            Name = "Otebashi"
        }
    };

    internal static string? GetEnglishStationName(string japaneseName)
    {
        return StationDatabase.Find(station => station.JapaneseName == japaneseName)!.Name;
    }

    internal static StationData? GetStationByJapaneseName(string japaneseName)
    {
        return StationDatabase.Find(station => station.JapaneseName == japaneseName);
    }
}