// ReSharper disable IdentifierTypo

#pragma warning disable CS0414 // Field is assigned but its value is never used
namespace Tanuden.Announce;

internal abstract class AudioMappings
{
    internal static readonly string UrlPrefix = "https://announce.r2.tanuden.tanu.ch/";

    internal abstract class StationName
    {
        internal static readonly string Tatehama = "tatehama.mp3";
        internal static readonly string Komano = "komano.mp3";
        internal static readonly string Kawarazaki = "kawarazaki.mp3";
        internal static readonly string Kaigankoen = "kaigankoen.mp3";
        internal static readonly string Nijigahama = "nijigahama.mp3";
        internal static readonly string Hagomorobashi = "hagoromobashi.mp3";
        internal static readonly string Araigawa = "araigawa.mp3";
        internal static readonly string Shinnozaki = "shinnozaki.mp3";
        internal static readonly string Enohara = "enohara.mp3";
        internal static readonly string Daidoji = "daidoji.mp3";

        internal static readonly string Akayamacho = "akayamacho.mp3";
        internal static readonly string Tamagawaonsen = "tamagawaonsen.mp3";
        internal static readonly string Nada = "nada.mp3";
        internal static readonly string Toi = "toi.mp3";
        internal static readonly string Shinoji = "shinoji.mp3";
        internal static readonly string Oji = "oji.mp3";
        internal static readonly string Otebashi = "otebashi.mp3";
    }

    internal class StationNameArrival
    {
        internal static readonly string Tatehama = "arr_tatehama.mp3";
        internal static readonly string Komano = "arr_komano.mp3";
        internal static readonly string Kawarazaki = "arr_kawarazaki.mp3";
        internal static readonly string Kaigankoen = "arr_kaigankoen.mp3";
        internal static readonly string Nijigahama = "arr_nijigahama.mp3";
        internal static readonly string Hagoromo = "arr_hagoromobashi.mp3";
        internal static readonly string Araigawa = "arr_araigawa.mp3";
        internal static readonly string Shinnozaki = "arr_shinnozaki.mp3";
        internal static readonly string Enohara = "arr_enohara.mp3";
        internal static readonly string Daidoji = "arr_daidoji.mp3";

        // In future: add more stations in future releases
    }

    internal abstract class PlatformNumber
    {
        internal static readonly string Platform1 = "platform-1.mp3";
        internal static readonly string Platform2 = "platform-2.mp3";
        internal static readonly string Platform3 = "platform-3.mp3";
        internal static readonly string Platform4 = "platform-4.mp3";
        internal static readonly string Platform5 = "platform-5.mp3";
        internal static readonly string Platform6 = "platform-6.mp3";
    }

    internal abstract class TrainClass
    {
        internal static readonly string Local = "shubetsu_local.mp3";
        internal static readonly string SemiExpress = "shubetsu_semi-express.mp3";
        internal static readonly string Express = "shubetsu_express.mp3";
        internal static readonly string RapidExpress = "shubetsu_rapid-express.mp3";
        internal static readonly string LimitedExpress = "shubetsu_limited-express.mp3";
    }

    internal abstract class Sentence
    {
        internal static readonly string ForgotBelongings = "wasuremonochui.mp3";
        internal static readonly string MindTheGap = "ashimotochui.mp3";
        internal static readonly string ThankYouForRiding = "joshaarigato.mp3";
        internal static readonly string ThankYouForRidingFinal = "shuten-joshaarigato.mp3";
        internal static readonly string Terminates = "terminates.mp3";

        internal static readonly string LastStop = "last-stop.mp3";
        internal static readonly string Returning = "returning.mp3";

        internal static readonly string NextStop = "next-station.mp3";
        internal static readonly string WillStopAt = "will-stop-at.mp3";

        internal static readonly string BoundFor = "bound-for.mp3";

        internal static readonly string JieiRNorikae = "jiei-norikae.mp3";

        internal static readonly string Desu = "to-be.mp3";
    }
}