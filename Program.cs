namespace AlarmClock;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        using Mutex singleMutex = new(true, "{4bd6ab17-76f7-334d-9130-a61c16bcbc7f}", out var isNewInstance);
        if (isNewInstance)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmClock());
        }
        else { MessageBox.Show("AlarmClock wird bereits ausgeführt!", "AlarmClock"); } // make the currently running instance jump on top of all the other windows
    }
}