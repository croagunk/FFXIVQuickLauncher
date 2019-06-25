using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace XIVLauncher
{
    public static class Util
    {
        public static void ShowError(string message, string caption, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageBox.Show($"{message}\n\n{callerName} L{callerLineNumber}", caption, MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        
        /// <summary> Gets the git hash value from the assembly
        /// or null if it cannot be found. </summary>
        public static string GetGitHash()
        {
            var asm = typeof(Util).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
        }

        public static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        public static bool IsValidFFXIVPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;

            return Directory.Exists(Path.Combine(path, "game")) && Directory.Exists(Path.Combine(path, "boot"));
        }

        private static readonly string[] PathsToTry = 
        {
            "C:\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn",
            "C:\\Program Files (x86)\\Steam\\steamapps\\common\\FINAL FANTASY XIV Online",
            "C:\\Program Files (x86)\\Steam\\steamapps\\common\\FINAL FANTASY XIV - A Realm Reborn",
            "C:\\Program Files (x86)\\FINAL FANTASY XIV - A Realm Reborn"
        };

        public static string TryGamePaths()
        {
            foreach (var path in PathsToTry)
            {
                if (Directory.Exists(path) && IsValidFFXIVPath(path))
                    return path;
            }

            return null;
        }

        public static bool IsWindowsDarkModeEnabled()
        {
            try
            {
                return (int) Registry.GetValue(
                           "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                           "AppsUseLightTheme", 0x1) == 0x0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string GetLangCode(this ClientLanguage language)
        {
            switch (language)
            {
                case ClientLanguage.Japanese:
                    return "ja";

                case ClientLanguage.English:
                    return "en-gb";

                case ClientLanguage.German:
                    return "de";

                case ClientLanguage.French:
                    return "fr";

                default:
                    return "en-gb";
            }
        }

        public static int GetUnixMillis() => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static string GenerateFrontierReferer()
        {
            var langCode = Settings.GetLanguage().GetLangCode();  
            var formattedTime = DateTime.UtcNow.ToString("yyyy-MM-dd-HH");

            return $"https://frontier.ffxiv.com/version_4_0_win/index.html?rc_lang={langCode}&time={formattedTime}";
        }

        public static Color ColorFromArgb(int argb)
        {
            return Color.FromArgb((byte) (argb >> 24), (byte) (argb >> 16), (byte) (argb >> 8), (byte) argb);
        }

        public static int ColorToArgb(Color color) => (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;

        public static SolidColorBrush SolidColorBrushFromArgb(int argb) => new SolidColorBrush(ColorFromArgb(argb));
    }
}