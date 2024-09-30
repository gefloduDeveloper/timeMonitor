Console.WriteLine("Listening for window changes. Press Ctrl+C to exit.");

WindowTracker.StartListeningForWindowChanges();

while (true)
{
    // Keep the app alive to listen for events
    Thread.Sleep(100);
}

WindowTracker.StopListening();