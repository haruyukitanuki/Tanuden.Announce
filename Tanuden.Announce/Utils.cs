using System.Text.RegularExpressions;
using NAudio.Wave;
using Tanuden.Common;
using TrainCrew;
using TrainState = TrainCrew.TrainState;

namespace Tanuden.Announce;

public abstract class Utils
{
    public static bool IsInGame(GameScreen gameScreen)
    {
        // If gameScreen is MainGame, return true
        return gameScreen is GameScreen.MainGame;
    }
    
    public static void AudioPlayer(List<string> audioMapNames)
    {
        // List of streams
        var streams = new List<WaveStream>();
        
        // Load all the audio into memory first. Prevents audio from being delayed.
        foreach (var fileName in audioMapNames)
        {
            var url = AudioMappings.UrlPrefix + fileName;
            var mf = new MediaFoundationReader(url);
            
            streams.Add(mf);
        }

        foreach (var stream in streams)
        {
            using var wo = new WasapiOut();
            wo.Init(stream);
            wo.Play();
            
            while (wo.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(10);
            }
        }
    }

    public static int GetPlatformNumber(string stopName)
    {
        // Detect whether the station name contains a number and proceeds that is the word 番
        // If it does, return the number

        if (stopName.Contains('番'))
        {
            var platformNumber = stopName.Split("番")[0];
            // Delete any non-numeric characters
            platformNumber = Regex.Replace(platformNumber, "[^0-9]", "");
            
            return int.Parse(platformNumber);
        }
        
        return 0;
    }

    public static OpenTetsuData FromTrainCrew(TrainState rawTrainState, List<SignalInfo>? signals)
    {
        // Format signal data
        List<Signal> formattedSignalData = new();

        string DetermineSignalType(string name)
        {
            if (name.Contains("入換")) return "switch";
            if (name.Contains("場内")) return "stop";
            if (name.Contains("出発")) return "departure";

            return "standard";
        }

        if (signals != null)
            formattedSignalData = signals.Select(signal => new Signal
            {
                Name = signal.name,
                Type = DetermineSignalType(signal.name),
                Phase = signal.phase,
                Distance = signal.distance,
                Transponders = signal.beacons.Select(transponder => new Transponder
                {
                    Distance = transponder.distance,
                    SpeedLimit = transponder.speed,
                    Type = transponder.type
                }).ToList()
            }).ToList();

        string DetermineSpeedLimitType(float speedLimit, float? distance = null)
        {
            // Determine without distance -- Lacks accuracy
            if (distance == null) return speedLimit is 0 or 25 or 55 or 80 ? "signal" : "speedlimit";

            var type = "speedlimit";
            // For floating point comparison
            var tolerance = 1;

            formattedSignalData.ForEach(signal =>
            {
                // Find a signal that has the same distance.
                signal.Transponders!.ForEach(transponder =>
                {
                    if (Math.Abs(transponder.Distance - (float)distance) > tolerance) return;

                    type = "signal";
                });
            });

            return type;
        }

        // Find which direction towards
        // Count the number of 上　and 下
        var upCount = rawTrainState.stationList.Count(station => station.StopPosName.Contains('上'));
        var downCount = rawTrainState.stationList.Count(station => station.StopPosName.Contains('下'));

        // If 上 is more than 下, then it's bound for 上
        var direction = upCount > downCount ? "inbound" : "outbound";

        // Convert (timespan)rawTrainState.NowTime to DateTime
        // NowTime is a Timespan. Set the date to today.
        var relativeNowTime = DateTime.Today.Add(rawTrainState.NowTime);

        var classType = "";
        // Convert Class into English
        if (rawTrainState.Class.Contains("普通"))
            classType = "local";
        else if (rawTrainState.Class.Contains("準急"))
            classType = "semi-express";
        else if (rawTrainState.Class.Contains("急行"))
            classType = "express";
        else if (rawTrainState.Class.Contains("快速"))
            classType = "rapid-express";
        else if (rawTrainState.Class.Contains("特急"))
            classType = "limited-express";

        var formattedData = new OpenTetsuData
        {
            RunNumber = rawTrainState.diaName,
            CurrentTime = relativeNowTime,
            NextStation = new NextStation
            {
                Name = rawTrainState.nextStaName,
                IsStopping = rawTrainState.nextStopType is "停車" or "運転停車",
                Index = rawTrainState.nowStaIndex,
                Distance = rawTrainState.nextStaDistance
            },
            TrainState = new Common.TrainState
            {
                Consist = rawTrainState.CarStates.Count,
                Speed = rawTrainState.Speed,
                SpeedLimit = rawTrainState.speedLimit,
                SpeedLimitType = DetermineSpeedLimitType(rawTrainState.speedLimit),
                Gradient = rawTrainState.gradient,
                NextSpeedLimit = new NextSpeedLimit
                {
                    Limit = rawTrainState.nextSpeedLimit,
                    Type = DetermineSpeedLimitType(rawTrainState.nextSpeedLimit, rawTrainState.nextSpeedLimitDistance),
                    Distance = rawTrainState.nextSpeedLimitDistance
                },
                MrPressure = rawTrainState.MR_Press,

                // reformat rawData.CarStates to models.CarState
                CarStates = rawTrainState.CarStates.Select((carState, index) => new Tanuden.Common.CarState
                {
                    CarNo = index + 1,
                    IsDoorClosed = carState.DoorClose,
                    BcPressure = carState.BC_Press,
                    Amperage = carState.Ampare
                }).ToList(),

                Lamps = new Lamps
                {
                    Pilot = rawTrainState.Lamps[PanelLamp.DoorClose],
                    RegenBrake = rawTrainState.Lamps[PanelLamp.RegenerativeBrake],
                    EBrake = rawTrainState.Lamps[PanelLamp.EmagencyBrake],
                    EbTimer = rawTrainState.Lamps[PanelLamp.EB_Timer],
                    Overload = rawTrainState.Lamps[PanelLamp.Overload],

                    Ats = new LampsAts
                    {
                        InOperation = rawTrainState.Lamps[PanelLamp.ATS_Ready],
                        BrakeApplication = rawTrainState.Lamps[PanelLamp.ATS_BrakeApply],
                        Isolated = rawTrainState.Lamps[PanelLamp.ATS_Open]
                    }
                }
            },

            SignalStates = formattedSignalData,

            ControllerState = new ControllerState
            {
                Notch = rawTrainState.Pnotch - rawTrainState.Bnotch,
                Reverser = rawTrainState.Reverser
            },

            AtsState = new AtsState
            {
                ClassType = rawTrainState.ATS_Class,
                Speed = float.Parse(rawTrainState.ATS_Speed),
                State = rawTrainState.ATS_State
            },

            Diagram = new Diagram
            {
                Direction = direction,
                BoundFor = rawTrainState.BoundFor,
                ServiceType = classType, // NOTE: This differs from the OpenTetsu standard. Usually this would be in Japanese.

                // Take the last item on rawData.stations and subtract with rawData.TotalLength
                RemainingDistance = rawTrainState.stationList.Count > 0
                    ? float.Abs(rawTrainState.TotalLength - rawTrainState.stationList.Last().TotalLength)
                    : 0,

                Stations = rawTrainState.stationList.Select((station, index) => new Tanuden.Common.Station
                {
                    Name = station.Name,
                    Index = index,
                    Timings = new StationTimings
                    {
                        Arrival = DateTime.Today.Add(station.ArvTime),
                        Departure = DateTime.Today.Add(station.DepTime)
                    },
                    StopType = (int)station.stopType,
                    PositionName = station.StopPosName,
                    DistanceFromKmZero = station.TotalLength
                }).ToList()
            }
        };

        return formattedData;
    }
}