using Microsoft.Win32;
using System.IO;

namespace RimeLibrarian.Tool
{
    internal static class Files
    {
        public static void AutoLoad()
            => TryCatch.Do("自动载入词库", () =>
            {
                string location = Path.GetFullPath(@"..\xkjd6.cizu.dict.yaml");
                if (LoadLib(location))
                {
                    MsgB.OkInfo($"已自动载入程序上级目录中的词库，如需修改请手动重载。", "提示");
                    return;
                }

                location = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime\xkjd6.cizu.dict.yaml";
                if (LoadLib(location))
                {
                    MsgB.OkInfo($"已自动载入Rime默认目录中的词库，如需修改请手动重载。", "提示");
                    return;
                }

                MsgB.OkWarn("未能自动载入Rime键道词库。\n请手动选择词库。", "提示");
                while (Dict.IsEmpty)
                    _ = LoadLib();

                MsgB.OkInfo($"已载入指定位置的词库，如需修改请手动重载。", "提示");
            });

        public static bool LoadLib()
        {
            var location = SetLibLocation();
            return location.Length > 0
                   && LoadLib(location);
        }

        public static bool LoadLib(string wordLocation)
        {
            string directory = Path.GetDirectoryName(wordLocation)
                ?? throw new ArgumentException("找不到父文件夹。");
            string filename = Path.GetFileName(wordLocation);
            string newFilename = filename.Replace("cizu", "danzi");
            string danLocation = Path.Combine(directory, newFilename);

            if (File.Exists(wordLocation) && File.Exists(danLocation))
            {
                TryCatch.Do("载入词库", () =>
                {
                    Dict.Load(wordLocation);
                    Dan.Load(danLocation);
                }, out bool success);
                return success;
            }
            return false;
        }

        public static string SetLibLocation()
        {
            OpenFileDialog ofd = new()
            {
                DefaultExt = ".yaml",
                InitialDirectory = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime",
                Filter = "Rime键道词库 (.yaml)|*.yaml",
                Title = "请选取xkjd6.cizu.dict.yaml文件"
            };
            return ofd.ShowDialog() == true
                ? ofd.FileName
                : string.Empty;
        }

        public static void SaveAnother()
        {
            SaveFileDialog sfd = new()
            {
                DefaultExt = ".txt",
                FileName = $"xkjd6.cizu.dict({DateTime.Now:yyyy-MM-dd-HH-mm-ss}).txt",
                Filter = "无法存储的词库 (.txt)|*.txt",
                Title = "将修改后的词库另存于"
            };
            while (sfd.ShowDialog() != true)
                continue;
            Dict.Save(sfd.FileName);
        }
    }
}
