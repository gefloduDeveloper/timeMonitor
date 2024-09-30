using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

class WindowTracker
{
    // Import user32.dll to interact with the Windows API
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWinEventHook(
        uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
        uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private const uint EVENT_SYSTEM_FOREGROUND = 3; // Event constant for foreground window changes
    private const uint WINEVENT_OUTOFCONTEXT = 0;  // Do not inject into the address space of the process
    private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;  // Event when a window is restored
    private const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;  // Event when a window is minimized



    // Delegate for handling window events
    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    private static WinEventDelegate _winEventDelegate;  // Store delegate to prevent garbage collection
    private static IntPtr _hookID;

    public static void StartListeningForWindowChanges()
    {
        _winEventDelegate = new WinEventDelegate(WinEventProc);  // Define the event handler
        _hookID = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

        if (_hookID == IntPtr.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            Console.WriteLine($"Failed to set hook. Error: {error}");
            throw new System.ComponentModel.Win32Exception(error);
        }
        else
        {
            Console.WriteLine("Successfully set the window change hook.");
        }
    }

    public static void StopListening()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWinEvent(_hookID);
        }
    }

    private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        Console.WriteLine("WinEventProc triggered.");

        string activeWindow = GetActiveWindowTitle();
        if (!string.IsNullOrEmpty(activeWindow))
        {
            Console.WriteLine($"Active window: {activeWindow}");
            TimeTracker.OnWindowChanged(activeWindow);
        }
    }

    public static string GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder Buff = new StringBuilder(nChars);
        IntPtr handle = GetForegroundWindow();

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return null;
    }
}
