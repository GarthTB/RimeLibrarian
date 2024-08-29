using System.Text;
using System.Windows;

namespace RimeLibrarian.Tool
{
    internal static class Help
    {
        private static readonly string version = "0.4.0";

        public static void Show()
        {
            var sb = new StringBuilder();
            var msg = sb.AppendLine("功能：")
                        .AppendLine("加词：在词库中添加一行。")
                        .AppendLine("删除：在词库中删除一行。")
                        .AppendLine("截短：将当前选中的词放在输入的编码上，原本的词移至剩下最短的空码上。")
                        .AppendLine("应用修改：在列表中修改完成后点一下此按钮。")
                        .AppendLine("日志：记录所有的改动，以便查错和回溯。")
                        .AppendLine("\n注意：")
                        .AppendLine("本工具自动编码严格依照官方键道6编码规则。")
                        .AppendLine("仅在启动时加载一次词组和单字，关闭时写入，请确保妥善关闭。")
                        .AppendLine("在软件外同时另行编辑，则会在程序关闭时覆盖，有数据损失风险。")
                        .AppendLine("词组载入后会按编码重排，可能破坏原有顺序。")
                        .AppendLine("生僻字（双字节字符）可能出错。")
                        .AppendLine("\n快捷键：")
                        .AppendLine("主页按F1跳出帮助。")
                        .AppendLine("日志页按F1保存日志。")
                        .AppendLine($"\n词器清单版v{version}，一个用于维护Rime星空键道6输入法词库的Windows工具。")
                        .AppendLine("© Garth 2024")
                        .AppendLine("\n是否复制仓库地址？")
                        .ToString();
            if (MsgB.YNInfo(msg, "帮助"))
            {
                Clipboard.SetText("https://github.com/GarthTB/RimeLibrarian");
                MsgB.OkInfo("仓库地址已复制到剪贴板。", "提示");
            }
        }
    }
}
