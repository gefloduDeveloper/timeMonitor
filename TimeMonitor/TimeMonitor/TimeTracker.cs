using System;
using System.IO;

class TimeTracker
{
    private static string _currentWindow = "";
    private static DateTime _startTime;
    private const string LogFile = @"C:\Users\geflodu\source\repos\TimeMonitor\TimeMonitor\TimeMonitor\bin\Debug\net8.0\WindowUsageLog.txt";


    public static void OnWindowChanged(string newWindow)
    {
        Console.WriteLine($"Window changed to: {newWindow}");  // Add this for debugging

        // Log the previous window and time spent
        if (!string.IsNullOrEmpty(_currentWindow))
        {
            TimeSpan timeSpent = DateTime.Now - _startTime;
            LogWindowUsage(_currentWindow, timeSpent);
        }

        // Update to the new window
        _currentWindow = newWindow;
        _startTime = DateTime.Now;
    }

    private static void LogWindowUsage(string windowTitle, TimeSpan timeSpent)
    {
        string logEntry = $"{DateTime.Now}: {windowTitle} - {timeSpent.TotalSeconds} seconds";

        // Print log entry for debugging
        Console.WriteLine($"Logging: {logEntry}");

        try
        {
            // Write the log to the file
            File.AppendAllText(LogFile, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }

}
