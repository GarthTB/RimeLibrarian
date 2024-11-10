using System.Text;
using System.Windows;

namespace RimeLibrarian.Tool
{
    internal static class Help
    {
        private static readonly string version = "0.4.4";

        public static void Show()
        {
            var sb = new StringBuilder();
            var msg = sb.AppendLine("功能：")
                        .AppendLine("加词：在词库中添加一个条目（有该词则词变红，有多码则码变红）")
                        .AppendLine("删除：在词库中删除一个条目")
                        .AppendLine("截短：将当前选中的词放在当前搜索的编码上，原本在这码上的词自动加长到剩下最短的空码上")
                        .AppendLine("应用修改：在列表中手动修改完成后，点击一下，将改动保存")
                        .AppendLine("日志：记录所有的改动，以便查错和回溯")
                        .AppendLine("\n注意：")
                        .AppendLine("依赖官方Rime键道的词库分类法，需要一个cizu.yaml和一个danzi.yaml在相同目录中才能工作")
                        .AppendLine("仅在启动时加载一次词组和单字，关闭时写入，请确保妥善关闭")
                        .AppendLine("自动编码严格按照官方键道6的编码规则")
                        .AppendLine("词组保存时会按照编码升序来重排，可能破坏原有顺序")
                        .AppendLine("不支持某些生僻字（char类型无法容纳的，具体有哪些我也不清楚）")
                        .AppendLine("\n快捷键：")
                        .AppendLine("主页F1：帮助")
                        .AppendLine("日志页F1：保存日志")
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
