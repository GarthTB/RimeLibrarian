using System.Windows;

namespace RimeLibrarian
{
    internal static class Help
    {
        public static void Show()
        {
            Clipboard.SetText("https://github.com/GarthTB/RimeLibrarian");
            MessageBox.Show("功能：\n"
                            + "加词：在词库中添加一行。\n"
                            + "删除：在词库中删除一行。\n"
                            + "置顶：将当前选中的词放在输入的编码上，原本的词移至剩下最短的空码上。\n"
                            + "修改：在左边直接修改当前选中的条目。\n"
                            + "确认：修改好了点一下。\n"
                            + "日志：记录所有的改动，以便查错和回溯。\n"
                            + "\n注意：\n"
                            + "1. 本工具自动编码严格依照官方键道6编码规则。\n"
                            + "2. 仅在启动时加载一次词组和单字，关闭时写入，请勿同时另行编辑。\n"
                            + "3. 词组载入后会按编码重排，可能破坏原有顺序。\n"
                            + "4. 生僻字（双字节字符）可能出错。\n"
                            + "\n词器清单版v0.1，一个用于维护Rime星空键道6输入法词库的Windows工具。\n"
                            + "源码链接已复制到剪贴板。\n"
                            + "©️ 2024 Garth", "帮助", MessageBoxButton.OK);
        }
    }
}
