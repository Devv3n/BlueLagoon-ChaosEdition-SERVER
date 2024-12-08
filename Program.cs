// "2-Dimensional Representation Of A 3-Dimensional Cross-Section Of A 4-Dimensional Cube"
//      +___________+
//     /:\         ,:\
//    / : \       , : \
//   /  :  \     ,  :  \
//  /   :   +-----------+
// +....:../:...+   :  /|
// |\   +./.:...`...+ / |
// | \ ,`/  :   :` ,`/  |
// |  \ /`. :   : ` /`  |
// | , +-----------+  ` |
// |,  |   `+...:,.|...`+
// +...|...,'...+  |   /
//  \  |  ,     `  |  /
//   \ | ,       ` | /
//    \|,         `|/   mn, 7/97
//     +___________+

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    internal static class Program {
        public static Server form = new Server();
        public static int gameStatus = 0; //0-noServer 1-noMap 2/3-gamingStage

        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //"yeah yeah no one cares"
            //- DevvEn

            GameHandler.noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            ApplicationConfiguration.Initialize();

            Application.Run(form);

            Logging.CloseLog();
        }
    }

    // file handling!!
    public static class Logging {
        static StreamWriter sw = new StreamWriter("Log.txt");

        // Logging functions
        public static void Log(string? message) {
            sw?.WriteLine($"<{CurrentTime()}> {message}");
        }
        public static void LogException(string? message) {
            sw?.WriteLine($"[{CurrentTime()}] {message}");
        }
        static string CurrentTime() => DateTime.Now.ToString("HH:mm:ss");

        public static void CloseLog() {
            sw?.WriteLine($"|{CurrentTime()}| Goodbye!");
            sw?.Close();
        }
    }
}