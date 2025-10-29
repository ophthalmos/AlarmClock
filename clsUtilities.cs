using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Linq;

namespace AlarmClock;
internal static class Utilities
{
    public static List<string> repeatList = ["Einmal", "Stündlich", "Täglich", "Wöchentlich", "Monatlich", "Alle 3 Monate", "Jährlich"];
    public static List<string> actionList = ["Sound", "Nachricht", "Herunterfahren", "Programm"];
    private const string runLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private static readonly string appName = Application.ProductName ?? "AlarmClock";
    private static readonly SoundPlayer soundPlayer = new();

    internal static DateTime GetNextDate_1(Reminder reminder) // Stündlich
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else { return reminder.Datetime.AddHours(Math.Ceiling((jetzt - reminder.Datetime).TotalHours)); }
    }

    internal static DateTime GetNextDate_2(Reminder reminder) // Täglich
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else
        {
            var targetTime = reminder.Datetime.TimeOfDay; // the fraction of the day that has elapsed since midnight   
            return jetzt.Date.AddDays(jetzt.TimeOfDay > targetTime ? 1 : 0) + targetTime;
        }
    }

    internal static DateTime GetNextDate_3(Reminder reminder) // Wöchentlich
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else { return reminder.Datetime.AddDays((int)Math.Ceiling((jetzt - reminder.Datetime).TotalDays / 7) * 7); }
    }

    internal static DateTime GetNextDate_4(Reminder reminder) // Monatlich
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else { return reminder.Datetime.AddMonths(((jetzt.Year - reminder.Datetime.Year) * 12) + (jetzt.Month - reminder.Datetime.Month) + 1); }
    }

    internal static DateTime GetNextDate_5(Reminder reminder) // Alle 3 Monate  
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else
        {
            var monthsPassed = ((jetzt.Year - reminder.Datetime.Year) * 12) + (jetzt.Month - reminder.Datetime.Month);
            var intervalsPassed = (int)Math.Ceiling(monthsPassed / 3.0); // Auf nächste 3-Monats-Periode aufrunden
            return reminder.Datetime.AddMonths(intervalsPassed * 3);
        }
    }

    internal static DateTime GetNextDate_6(Reminder reminder) // Jährlich
    {
        var jetzt = DateTime.Now.AddSeconds(1);
        if (jetzt <= reminder.Datetime) { return reminder.Datetime; }
        else { return reminder.Datetime.AddYears(jetzt.Year - reminder.Datetime.Year + 1); }
    }

    internal static void ShowReminderForm(Point location, int delayTime) // ShowReminderForm wird von Minutenmenü in FrmClock aufgerufen
    {
        PlaySound("warning.wav");
        var heading = "Erinnerung (" + DateTime.Now.ToLongTimeString() + ")";
        var text = delayTime + " Minuten sind abgelaufen.";
        var frm = new MessageForm(location, heading, text, "", false);
        frm.FormClosed += (sender, e) =>
        {
            if (Application.OpenForms[0] is not null and FrmClock frmClock) { frmClock.SetMsgLocation(frm.Location); }
        };
        NativeMethods.ShowWindow(frm.Handle, NativeMethods.SW_SHOWNOACTIVATE); // frm.Show();
        NativeMethods.SetWindowPos(frm.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
    }

    private static void ShowMessageForm(Form? caller, Point? location, Reminder reminder, int remIndex) // ShowMessageForm wird nur von ShowReminder aufgerufen
    {
        PlaySound("warning.wav");
        var repeatText = repeatList[reminder.Repeat];
        var heading = reminder.Name + " (" + DateTime.Now.ToLongTimeString() + ")";
        var text = "Diese Meldung erscheint " + char.ToLower(repeatText[0]) + repeatText[1..] + ".";
        var check = reminder.Check;
        if (caller is not null and AlarmList alarmList)
        {
            using var frm = new MessageForm(location, heading, text, repeatText, check);
            frm.Width = Math.Max(frm.Width, TextRenderer.MeasureText(heading, frm.HeadingFont()).Width + 90);
            if (frm.ShowDialog() == DialogResult.OK) { alarmList.GetListView().SelectedItems[0].Checked = frm.CheckBoxChecked(); }
        }
        else
        {
            var frm = new MessageForm(location, heading, text, repeatText, check);
            frm.Width = Math.Max(frm.Width, TextRenderer.MeasureText(heading, frm.HeadingFont()).Width + 90);
            frm.FormClosed += (sender, e) =>
            {
                if (caller is not null and FrmClock frmClock) //if (Application.OpenForms[0] is not null and FrmClock frmClock)
                {
                    frmClock.GetReminderList()[remIndex].Check = frm.CheckBoxChecked();
                    frmClock.SetMsgLocation(frm.Location);
                }
            };
            NativeMethods.ShowWindow(frm.Handle, NativeMethods.SW_SHOWNOACTIVATE); // frm.Show();
            NativeMethods.SetWindowPos(frm.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
        }
    }

    internal static bool TaskDialogYesNo(IntPtr hwnd, string caption, string heading, string text)
    {
        using TaskDialogIcon questionDialogIcon = new(Properties.Resources.Question_32x);
        var page = new TaskDialogPage
        {
            Caption = caption,
            Heading = heading,
            Text = text,
            Icon = questionDialogIcon,
            Buttons = { TaskDialogButton.Yes, TaskDialogButton.No },
            AllowCancel = true,
            SizeToContent = true
        };
        return TaskDialog.ShowDialog(hwnd, page) == TaskDialogButton.Yes;
        //}
    }

    internal static void ErrorMsgTaskDlg(IntPtr hwnd, string error, string message, TaskDialogIcon? icon = null)
    {
        TaskDialog.ShowDialog(hwnd, new TaskDialogPage() { Caption = appName, SizeToContent = true, Heading = error, Text = message, Icon = icon ?? TaskDialogIcon.Error, AllowCancel = true, Buttons = { TaskDialogButton.OK } });
    }

    internal static void ShowReminder(Form? caller, Point? location, Reminder reminder, int remIndex)
    {
        var hwnd = caller?.Handle ?? IntPtr.Zero;  // hwnd wird nur von TaskDialogYesNo verwendet, nicht ShowMessageForm
        if (caller is not null and FrmClock) { hwnd = IntPtr.Zero; }  // TaskDialogYesNo zentral auf Bildschirm anzeigen
        try
        {
            switch (reminder.Action)
            {
                case 0: // Sound
                    PlaySound("reminder.wav");
                    break;
                case 1: // Nachricht
                    ShowMessageForm(caller, location, reminder, remIndex);
                    break;
                case 2: // Herunterfahren
                    if (reminder.Hibernate)
                    {
                        if (reminder.AskBefore && !TaskDialogYesNo(hwnd, appName, reminder.Name, "Möchten Sie den PC in den Ruhemodus versetzen?")) { return; }
                        else { NativeMethods.SetSuspendState(false, false, false); }
                    }
                    else
                    {
                        if (reminder.AskBefore && !TaskDialogYesNo(hwnd, appName, reminder.Name, "Möchten Sie den PC herunterfahren?")) { return; }
                        else { ShutdownPCAndExit(); }
                    }
                    break;
                case 3: // Programm 
                    if (reminder.AskBefore && !TaskDialogYesNo(hwnd, appName, reminder.Name + " ausführen?", reminder.ProgPath)) { return; }
                    else { StartFileDirProg(reminder.ProgPath); }
                    break;
            }
        }
        catch (Exception ex) { ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message); }
    }

    internal static void HelpMsgTaskDlg(nint hwnd, Icon icon)
    {
        var foot = "              © " + GetBuildDate().ToString("yyyy") + " Wilhelm Happe, Version " + Assembly.GetExecutingAssembly().GetName().Version!.ToString() + " (" + GetBuildDate().ToString("d") + ")";
        var msg = "Desktop-Analoguhr +\nErinnerungsfunktionen";
        var txt = "Die Alarme erinnern an Termine, starten zu\nfestgelegten Zeiten bestimmte Programme\noder fahren automatisch den PC herunter.";
        TaskDialog.ShowDialog(hwnd, new TaskDialogPage() { Caption = appName, Heading = msg, Text = txt, Icon = new TaskDialogIcon(icon), AllowCancel = true, Buttons = { TaskDialogButton.OK }, Footnote = foot });
    }

    private static void PlaySound(string shortFilePath)
    {
        try
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, shortFilePath);
            if (File.Exists(filePath))
            {
                soundPlayer.SoundLocation = filePath;
                soundPlayer.Load();
                soundPlayer.Play();
            }
            else { ErrorMsgTaskDlg(IntPtr.Zero, appName, "Die Sounddatei " + filePath + " wurde nicht gefunden."); }
        }
        catch (Exception ex) { ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message); }
    }

    //public static (bool[] right, bool[] obliq) GetCoveredQuadrants(Point center, Point minuteHand, Point hourHand)
    //{
    //    var minuteAngle = CalculateAngle(center, minuteHand);
    //    var hourAngle = CalculateAngle(center, hourHand);
    //    var quadUpright = new bool[4]; // Index 0: linksoben, 1: rechtsoben, 2: rechtsunten, 3: linksunten
    //    var quadOblique = new bool[4]; // Index 0: oben, 1: rechts, 2: unten, 3: links
    //    quadUpright[GetUpright(minuteAngle)] = true;
    //    quadUpright[GetUpright(hourAngle)] = true;
    //    quadOblique[GetOblique(minuteAngle)] = true;
    //    quadOblique[GetOblique(hourAngle)] = true;
    //    return (quadUpright, quadOblique);
    //}

    private static double CalculateAngle(Point center, Point end)
    {
        var angle = Math.Atan2(end.Y - center.Y, end.X - center.X) * (180 / Math.PI);
        return (angle < 0) ? angle + 360 : angle;
    }

    private static int GetOblique(double angle) => angle switch { >= 45 and < 135 => 2, >= 135 and < 225 => 3, >= 225 and < 315 => 0, _ => 1 };
    private static int GetUpright(double angle) => angle switch { >= 0 and < 90 => 2, >= 90 and < 180 => 3, >= 180 and < 270 => 0, _ => 1 };

    internal static DateTime GetBuildDate()
    { //s. <SourceRevisionId>build$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))</SourceRevisionId> in AlarmClock.csproj
        const string BuildVersionMetadataPrefix = "+build";
        var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value[(index + BuildVersionMetadataPrefix.Length)..];
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)) { return result; }
            }
        }
        return default;
    }

    public static bool IsInnoSetupValid(string assemblyLocation)
    {
        var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\AlarmClock_is1");
        if (key == null) { return false; }
        var value = (string?)key.GetValue("InstallLocation");
        //MessageBox.Show(assemblyLocation + Environment.NewLine + value);
        if (value == null) { return false; }
        else if (Debugger.IsAttached) { return true; } // run by Visual Studio
        else { return assemblyLocation.Equals(value.Trim('\\')); }
        //else { return assemblyLocation.Equals(RemoveFromEnd(value.Trim('"'), "\\unins000.exe")); } // "C:\Program Files\AlarmClock\unins000.exe"
    }

    //private static string RemoveFromEnd(string str, string toRemove) => str.EndsWith(toRemove) ? str[..^toRemove.Length] : str;


    //private static int GetStringLengthPx(string text, Font font)
    //{
    //    //return TextRenderer.MeasureText(text, font).Width;
    //    using var g = Graphics.FromHwnd(IntPtr.Zero);
    //    return (int)g.MeasureString(text, font, 0, StringFormat.GenericTypographic).Width;
    //}

    public static void SetAutoStart(string appName, string assemblyLocation)
    {
        using var key = Registry.CurrentUser.CreateSubKey(runLocation);
        key.SetValue(appName, assemblyLocation);
    }

    public static int GetIndexOfFirstEmptyRow(ListView listView)
    {
        for (var i = 0; i < listView.Items.Count; i++) { if (string.IsNullOrEmpty(listView.Items[i].Text)) { return i; } }
        return -1; // Keine leere Zeile gefunden
    }

    public static void RemoveAutoStart(string appName)
    {
        using var key = Registry.CurrentUser.CreateSubKey(runLocation);
        key.DeleteValue(appName, false);
    }

    public static bool IsAutoStartEnabled(string appName, string assemblyLocation)
    {
        using var key = Registry.CurrentUser.OpenSubKey(runLocation);
        if (key == null) { return false; }
        if (key.GetValue(appName) is not string value) { return false; }
        else if (Debugger.IsAttached) { return true; } // run by Visual Studio
        else { return value == assemblyLocation; }
    }

    public static void ShutdownPCAndExit()
    {
        var processInfo = new ProcessStartInfo { FileName = "shutdown.exe", Arguments = "-s /t 4 /c \"Windows wird heruntergefahren\"", UseShellExecute = false };
        var p = Process.Start(processInfo);
        if (p == null) { return; }
        else { Application.Exit(); }
    }

    private static void StartFileDirProg(string input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input)) { return; }
            var programPath = "";
            var arguments = "";
            if (input.StartsWith('"')) // Anführungszeichen prüfen  
            {
                var endQuoteIndex = input.IndexOf('"', 1);
                if (endQuoteIndex > 0)
                {
                    programPath = input[1..endQuoteIndex];
                    arguments = input[(endQuoteIndex + 1)..].Trim();
                }
            }
            else // Kein Anführungszeichen
            {
                var firstSpaceIndex = input.IndexOf("exe");
                if (firstSpaceIndex > 0)
                {
                    programPath = input[..(firstSpaceIndex + 3)]; // + "exe".Length 
                    arguments = input[(firstSpaceIndex + 3)..].Trim(); // startIndex cannot be larger than length of string
                }
            }
            //MessageBox.Show(programPath + Environment.NewLine + arguments);
            if (!string.IsNullOrEmpty(arguments) && File.Exists(programPath)) // Überprüfung auf eine ausführbare Datei mit Argumenten  
            {
                Process.Start(new ProcessStartInfo { FileName = programPath, Arguments = arguments, UseShellExecute = true });
            }
            else if (new Regex(@"^((http|https)://|www\.)\S+$").IsMatch(input)) // Überprüfung auf eine URL  
            {
                Process.Start(new ProcessStartInfo { FileName = input, UseShellExecute = true });
            }
            else if (File.Exists(input)) // Überprüfung, ob es sich um eine existierende Datei handelt  
            {
                Process.Start(new ProcessStartInfo { FileName = input, UseShellExecute = true });
            }
            else if (Directory.Exists(input)) // Überprüfung, ob es sich um einen Ordner handelt  
            {
                Process.Start(new ProcessStartInfo { FileName = input, UseShellExecute = true });
            }
            else
            {
                ErrorMsgTaskDlg(IntPtr.Zero, "Pfad oder Datei nicht gefunden.", "Bitte überprüfen Sie Ihre Eingabe.");
            }
        }
        catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException)
        {
            ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message);
        }
    }

    internal static void StartFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                ProcessStartInfo psi = new(filePath) { UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(filePath) };
                Process.Start(psi);
            }
        }
        catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException) { ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message); }
    }

    //public static bool EqualsWithTolerance(this DateTime dateTime1, DateTime dateTime2, TimeSpan tolerance)
    //{
    //    var difference = dateTime1 - dateTime2;
    //    if (difference < TimeSpan.Zero) { difference = -difference; }
    //    return difference <= tolerance;
    //}

    //public static bool UhrzeitenSindGleich(this DateTime zeit1, DateTime zeit2, TimeSpan toleranz) => zeit1.TimeOfDay.Subtract(zeit2.TimeOfDay).Duration() <= toleranz;

    //public static DateTime ConvertStringToDate(string dateString)
    //{
    //    var day = int.Parse(dateString[..2]);
    //    var month = int.Parse(dateString.Substring(2, 2));
    //    var year = int.Parse(dateString.Substring(4, 4));
    //    return new DateTime(year, month, day);
    //}

    public static DateTime ConvertStringToDateTime(string dateString)
    {
        if (dateString.Length != 14)
        {
            ErrorMsgTaskDlg(IntPtr.Zero, "Error", "ConvertString to short" + Environment.NewLine + dateString);
            throw new ArgumentException("Das Datumsformat muss ddMMyyyyHHmmss sein.", nameof(dateString));
        }
        var day = int.Parse(dateString[..2]);
        var month = int.Parse(dateString.Substring(2, 2));
        var year = int.Parse(dateString.Substring(4, 4));
        var hour = int.Parse(dateString.Substring(8, 2));
        var minute = int.Parse(dateString.Substring(10, 2));
        var second = int.Parse(dateString.Substring(12, 2));
        return new DateTime(year, month, day, hour, minute, second);
    }

    internal static (Point, Point) CalculateSecondPoint(int seconds, int radius, Point center)
    {
        var overlapRatio = 0.2f;
        var angle = (seconds * 6) * Math.PI / 180; // Oder (Math.PI / 30 * seconds); 6 Grad pro Sekunde
        var endX = (int)(center.X + (radius * Math.Sin(angle)));
        var endY = (int)(center.Y - (radius * Math.Cos(angle)));
        var startX = (int)(center.X - (radius * overlapRatio * Math.Sin(angle))); // Overlap in entgegengesetzte Richtung
        var startY = (int)(center.Y + (radius * overlapRatio * Math.Cos(angle)));
        return (new Point(startX, startY), new Point(endX, endY));
    }

    internal static Point CalculateMinutePoint(int minute, int radius, Point center)
    {
        var bogenmass = minute * 6 * Math.PI / 180;
        var x = center.X + (int)(radius * Math.Sin(bogenmass));
        var y = center.Y - (int)(radius * Math.Cos(bogenmass));
        return new Point(x, y);
    }

    internal static Point CalculateHourPoint(int hour, int minute, int radius, Point center)
    {
        var hourAngleRadians = (double)((hour + (minute / 60.0)) * 30) * Math.PI / 180;
        var x = center.X + (int)(radius * Math.Sin(hourAngleRadians));
        var y = center.Y - (int)(radius * Math.Cos(hourAngleRadians));
        return new Point(x, y);
    }

    public enum Quadrant
    {
        Top, Bottom, Left, Right
    }

    public static double GetAngularDifference(double angle1, double angle2) // Berechnet die kleinste absolute Winkeldifferenz zwischen zwei Winkeln in Bogenmaß.
    {
        var diff = Math.Abs(angle1 - angle2);
        return Math.Min(diff, (2 * Math.PI) - diff);  // Wähle den kürzeren Weg (z.B. ist der Abstand zwischen 350° und 10° nicht 340°, sondern 20°)
    }

    public static Quadrant FindFreestQuadrant(Point minuteEnd, Point hourEnd, Point center)
    {
        var minAngle = Math.Atan2(minuteEnd.Y - center.Y, minuteEnd.X - center.X); // Winkel der Zeiger berechnen
        var hourAngle = Math.Atan2(hourEnd.Y - center.Y, hourEnd.X - center.X);
        var placementAngles = new Dictionary<Quadrant, double> { { Quadrant.Right, 0 }, { Quadrant.Bottom, Math.PI / 2 }, { Quadrant.Left, Math.PI }, { Quadrant.Top, -Math.PI / 2 } };
        var quadrantScores = new Dictionary<Quadrant, double>(); // Score ist der *minimale* Abstand, den dieser Platz zu *irgendeinem* Zeiger hat.
        foreach (var placement in placementAngles)
        {
            var targetAngle = placement.Value;
            var distToMin = GetAngularDifference(targetAngle, minAngle);      // Finde den Abstand dieses Platzes zu beiden Zeigern
            var distToHour = GetAngularDifference(targetAngle, hourAngle);    // Finde den Abstand dieses Platzes zu beiden Zeigern
            quadrantScores[placement.Key] = Math.Min(distToMin, distToHour);  // Der "Sicherheits-Score" dieses Platzes ist der Abstand zum NÄCHSTEN Zeiger.
        }
        return (new[] { Quadrant.Right, Quadrant.Bottom, Quadrant.Left, Quadrant.Top }).OrderByDescending(q => quadrantScores[q]).First(); // Preferred order if tie   
    }
}
