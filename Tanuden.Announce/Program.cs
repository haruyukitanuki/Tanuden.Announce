using System.Reflection;
using System.Text;
using Tanuden.Common;
using TrainCrew;
using Station = Tanuden.Common.Station;

// Main function
// ReSharper disable once CheckNamespace
namespace Tanuden.Announce;

// ReSharper disable once InconsistentNaming
internal static class Program
{
    private static bool DataSendLoop;
    private static Thread? _dataThread;

    private static OpenTetsuData _overallState = new();
    private static GameState? _trainCrewGameState;

    private static int? _currentStationIndex = 0;

    // This is set to true because we want to skip the first station usually.
    private static bool _announcementPlayed = true;

    private static void DataThread()
    {
        while (DataSendLoop)
        {
            try
            {
                var trainCrewTrainState = TrainCrewInput.GetTrainState();
                TrainCrewInput.RequestData(DataRequest.Signal);
                if (trainCrewTrainState.stationList.Count == 0)
                {
                    Console.WriteLine("ダイヤを選択してください。");

                    TrainCrewInput.RequestStaData();

                    Thread.Sleep(1000);
                    Console.Clear();
                    continue;
                }

                ;

                _overallState = Utils.FromTrainCrew(trainCrewTrainState, TrainCrewInput.signals);
                _trainCrewGameState = TrainCrewInput.gameState;

                // Continue if not in game
                // if (!Utils.IsInGame(TrainCrewGameState.gameScreen))
                // {
                //     Console.WriteLine("ゲーム中ではありません。");
                //     Thread.Sleep(1000);
                //     Console.Clear();
                //     continue;
                // };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            Console.WriteLine("次の駅距離: " + Math.Round((double)_overallState.NextStation?.Distance!, 2) + "m");
            Console.WriteLine(
                $"次は{_overallState.NextStation!.Name} ({(_overallState.NextStation.IsStopping ? "停" : "通")}) です。");

            var boundFor = _overallState.Diagram.BoundFor;
            var serviceType = _overallState.Diagram.ServiceType;
            var direction = _overallState.Diagram.Direction;

            var stations = _overallState.Diagram!.Stations;
            var passengerStations = stations.Where(x => x.StopType == (int?)StopType.StopForPassenger).ToList();
            var currentIndexInPassengerStations =
                passengerStations.FindIndex(x => x.Index == _overallState.NextStation.Index);

            var stateStation = stations![(int)_overallState.NextStation!.Index!];
            var stateStationInternalData = StationMappings.GetStationByJapaneseName(stateStation.Name!)!;
            var isLastStation = currentIndexInPassengerStations + 1 == passengerStations.Count;

            // Override isLastStation if the departure time is present and if the arrival and departure timings are within .5min
            if (stateStation.Timings.Departure != null && stateStation.Timings.Arrival != null)
            {
                var arrival = stateStation.Timings.Arrival;
                var departure = stateStation.Timings.Departure;
                if (departure!.Value.Subtract(arrival!.Value).TotalSeconds < 30) isLastStation = false;
            }

            // Get the station after the next
            Station nextNextStation = null!;
            try
            {
                nextNextStation = passengerStations[(int)_overallState.NextStation.Index + 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                // ignored
            }


            // If the current station index is different from the state station index and the station is a passenger stop
            if (_currentStationIndex != stateStation.Index && stateStation.StopType == (int?)StopType.StopForPassenger)
            {
                _currentStationIndex = stateStation.Index;
                _announcementPlayed = false;
            }

            // Play announcement when 5 meters away from the station and speed is less than 2km/h
            if (Math.Abs((float)_overallState.NextStation!.Distance!) < 5 && !_overallState.TrainState!.Lamps!.Pilot &&
                !_announcementPlayed)
            {
                _announcementPlayed = true;

                if (!isLastStation)
                    StandardAnnounce();
                else
                    LastStationAnnounce();
            }

            Console.WriteLine();
            Console.WriteLine("終了するにはCtrl+Cを押してください。");

            Thread.Sleep(500);
            Console.Clear();

            continue;

            void StandardAnnounce()
            {
                Utils.AudioPlayer(new List<string>
                {
                    $"arr_{stateStationInternalData.Name.ToLower()}.mp3"
                });

                // If the platform is curved
                if (stateStationInternalData.IsCurvedPlatform)
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.MindTheGap,
                        AudioMappings.Sentence.ThankYouForRiding
                    });
                else
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.ThankYouForRiding
                    });

                Thread.Sleep(100);

                // If there is a transfer
                if (stateStationInternalData.IsInterchangeWithJieiR)
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.JieiRNorikae
                    });

                Thread.Sleep(1000);

                // Get platform number
                var platformNumber = Utils.GetPlatformNumber(stateStation.PositionName!);

                var platformNumberAudio = platformNumber switch
                {
                    1 => AudioMappings.PlatformNumber.Platform1,
                    2 => AudioMappings.PlatformNumber.Platform2,
                    3 => AudioMappings.PlatformNumber.Platform3,
                    4 => AudioMappings.PlatformNumber.Platform4,
                    5 => AudioMappings.PlatformNumber.Platform5,
                    6 => AudioMappings.PlatformNumber.Platform6,
                    _ => direction == "inbound"
                        ? AudioMappings.PlatformNumber.Platform1
                        : AudioMappings.PlatformNumber.Platform2
                };

                Utils.AudioPlayer(new List<string>
                {
                    // If direction is 'inbound', play 'platform-1.mp3', outbound, play 'platform-2.mp3'
                    platformNumberAudio,
                    $"shubetsu_{serviceType}.mp3",
                    $"{StationMappings.GetEnglishStationName(boundFor!)!.ToLower()}.mp3",
                    AudioMappings.Sentence.BoundFor
                });

                // If serviceType is not 'local', play next station is
                if (serviceType != "local")
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.NextStop,
                        $"{StationMappings.GetEnglishStationName(nextNextStation.Name!)!.ToLower()}.mp3",
                        AudioMappings.Sentence.WillStopAt
                    });
            }

            void LastStationAnnounce()
            {
                // If the platform is curved
                if (stateStationInternalData.IsCurvedPlatform)
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.MindTheGap
                    });

                Thread.Sleep(50);

                Utils.AudioPlayer(new List<string>
                {
                    AudioMappings.Sentence.ThankYouForRidingFinal,
                    $"{stateStationInternalData.Name.ToLower()}.mp3"
                });

                Thread.Sleep(50);

                Utils.AudioPlayer(new List<string>
                {
                    $"{stateStationInternalData.Name.ToLower()}.mp3",
                    AudioMappings.Sentence.LastStop,
                    AudioMappings.Sentence.ForgotBelongings
                });

                Thread.Sleep(100);

                // If there is a transfer
                if (stateStationInternalData.IsInterchangeWithJieiR)
                    Utils.AudioPlayer(new List<string>
                    {
                        AudioMappings.Sentence.JieiRNorikae
                    });
            }
        }
    }

    private static void HandleExit()
    {
        // Wait for threads to terminate
        DataSendLoop = false;
        _dataThread?.Join();

        TrainCrewInput.Dispose();

        Console.WriteLine("アプリ終了しました。運転お疲れ様でした！");
        Thread.Sleep(5000);

        Environment.Exit(0);
    }

    private static void Main()
    {
        // Set console encoding to allow Japanese characters
        Console.OutputEncoding = Encoding.Unicode;

        // Display welcome message
        Console.WriteLine("Tanuden.AnnounceはTrainCrewに到着放送が追加のアプリです。");
        Console.WriteLine("狸河電鉄作品をご利用いただきありがとうございます。");
        Console.WriteLine();
        Console.WriteLine("このアプリはオープンソースソフトウェア（OSS）です。ライセンスはGNU GPLv3でライセンスです。");
        Console.WriteLine("ソースコード： https://github.com/haruyukitanuki/Tanuden.Announce");
        Console.WriteLine("ライセンスの詳しくは： https://www.gnu.org/licenses/gpl-3.0.ja.html");
        Console.WriteLine(
            "YouTubeやTwitterの投稿する場合、Tanuden TIMS利用規約の投稿に関連規則が適用されます。ご利用の際は、利用規約を遵守してください。詳しく： https://tanuden-tims.tanu.ch");
        Console.WriteLine();
        Console.WriteLine("Copyright (c) 2024, Haruyuki Tanukiji.");

        // Print version number
        Console.WriteLine("手持ちバージョン: " + Assembly.GetExecutingAssembly().GetName().Version);

        Console.WriteLine();
        Console.WriteLine("続行するには何かキーを押してください。");
        Console.ReadKey();

        Console.Clear();

        // Initialise TrainCrew
        TrainCrewInput.Init();

        // Initalise data thread
        DataSendLoop = true;
        _dataThread = new Thread(DataThread);
        _dataThread.Start();

        // Handle when application terminates
        Console.CancelKeyPress += delegate { HandleExit(); };
    }
}