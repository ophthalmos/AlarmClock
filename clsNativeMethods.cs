using System.Runtime.InteropServices;

namespace AlarmClock;
internal class NativeMethods
{
    public const int WM_NCLBUTTONDOWN = 161;
    public const int HT_CAPTION = 2;
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TRANSPARENT = 32;
    internal const int HWND_BROADCAST = 65535;
    internal const int WM_HOTKEY = 0x312;
    internal const int HOTKEY_ID0 = 0x0311;
    internal const int HOTKEY_ID1 = 0x0310;
    internal const int SW_SHOWNOACTIVATE = 4;
    internal const int SW_SHOWNA = 8;
    internal const int SW_SHOWDEFAULT = 10;
    private const int WS_EX_TOPMOST = 0x00000008;

    internal enum Modifiers : uint
    {
        Control = 0x0002, Shift = 0x0004, Win = 0x0008
    }

    public static bool IsTopMost(IntPtr hWnd) => (GetWindowLong(hWnd, GWL_EXSTYLE) & WS_EX_TOPMOST) != 0;

    [DllImport("user32.dll")]
    internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("powrprof.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

}
