// ReSharper disable StringLiteralTypo
namespace Tanuden.Announce;

internal class StationData
{
    public int Id { get; set; }
    public string JapaneseName { get; set; } = "";
    public string Name { get; set; } = "";
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
        },
        new StationData
        {
            Id = 2,
            JapaneseName = "駒野",
            Name = "Komano",
        },
        new StationData
        {
            Id = 3,
            JapaneseName = "河原崎",
            Name = "Kawarazaki",
        },
        new StationData
        {
            Id = 4,
            JapaneseName = "海岸公園",
            Name = "Kaigankoen",
        },
        new StationData
        {
            Id = 5,
            JapaneseName = "虹ケ浜",
            Name = "Nijigahama",
        },
        new StationData
        {
            Id = 6,
            JapaneseName = "津崎",
            Name = "Tsuzaki",
        },
        new StationData
        {
            Id = 7,
            JapaneseName = "浜園",
            Name = "Hamazono",
        },
        new StationData
        {
            Id = 8,
            JapaneseName = "羽衣橋",
            Name = "Hagoromobashi",
        },
        new StationData
        {
            Id = 9,
            JapaneseName = "新井川",
            Name = "Araigawa",
        },
        new StationData
        {
            Id = 10,
            JapaneseName = "新野崎",
            Name = "Shinnozaki",
        },
        new StationData
        {
            Id = 11,
            JapaneseName = "江ノ原",
            Name = "Enohara",
        },
        new StationData
        {
            Id = 12,
            JapaneseName = "大道寺",
            Name = "Daidoji",
        }
    };
    
    internal static string? GetEnglishStationName(string japaneseName)
    {
        return StationDatabase.Find(station => station.JapaneseName == japaneseName)!.Name;
    }
}