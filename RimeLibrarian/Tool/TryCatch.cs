namespace RimeLibrarian.Tool
{
    /// <summary>
    /// 为简化代码，将修改时的TryCatch块封装成一个类
    /// </summary>
    internal static class TryCatch
    {
        public static void Do(string name, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MsgB.OkErr($"{name}错误：\n{ex.Message}", "错误");
            }
        }

        public static void Do(string name, Action action, out bool success)
        {
            try
            {
                action();
                success = true;
            }
            catch (Exception ex)
            {
                MsgB.OkErr($"{name}错误：\n{ex.Message}", "错误");
                success = false;
            }
        }

        public static void Do(string name, Action action, Action errorAction)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MsgB.OkErr($"{name}错误：\n{ex.Message}", "错误");
                errorAction();
            }
        }
    }
}
