using System.Drawing.Drawing2D;
using System.Timers;
using System.Xml;
using System.Xml.Linq;

namespace AlarmClock;

public partial class FrmClock : Form
{
    public List<Reminder> GetReminderList
    {
        get => reminders; set => reminders = value;
    }

    public Point MsgLocation
    {
        get => msgLocation; set => msgLocation = value;
    }

    private Point centerPoint;
    private readonly string appName = Application.ProductName ?? "AlarmClock";
    private static readonly string appPath = Application.ExecutablePath;
    private readonly string xmlPath;
    private string windowPosition = string.Empty;
    private string messagePosition = string.Empty;
    private bool clickThrough = true;
    private bool showCurrentDate = true;
    private int inflateSize = 100;
    private int listSelection = 0; // für die ListView-Position
    private int currentSecond = -1;
    private List<Reminder> reminders = [];
    private readonly System.Timers.Timer systemTimer = new();
    private readonly Dictionary<string, DateTime> nextExecDatetimeDict = [];
    private bool execFlag = false; // Action-Abfrage im Timer ist nicht aktiv
    private bool foo = false; // für den Hotkey-Handler 
    private bool bar = false; // für den Hotkey-Handler 
    private AlarmList? frmAlarmList;
    private Point msgLocation = new(0, 0); // new Point(Screen.PrimaryScreen.WorkingArea.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2);

    public FrmClock()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        InitializeComponent();
        centerPoint = new Point((ClientSize.Width / 2) + 5, (ClientSize.Height / 2) + 1);
        xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, appName + ".xml");
        if (!Utilities.IsInnoSetupValid(Path.GetDirectoryName(appPath)!)) { xmlPath = Path.ChangeExtension(appPath, ".xml"); } // portable
        systemTimer.Interval = 200; // Millisekunden
        systemTimer.AutoReset = true; // Timer tickt alle 100 ms
        systemTimer.Elapsed += Timer_Tick;
    }

    private void Timer_Tick(object? source, ElapsedEventArgs e)
    {
        BeginInvoke(new Action(() =>
        {
            if (clicktroughToolStripMenuItem.Checked)
            {
                Opacity = new Rectangle(Bounds.X - inflateSize, Bounds.Y - inflateSize, Bounds.Width + (inflateSize * 2), Bounds.Height + (inflateSize * 2)).
                    Contains(Cursor.Position) ? 0.05 : 1.0;
            }
        }));
        var jetzt = DateTime.Now;
        if (jetzt.Second != currentSecond)
        {
            currentSecond = DateTime.Now.Second;
            if (execFlag)
            {
                for (var i = 0; i < nextExecDatetimeDict.Count; i++)
                {
                    var eintrag = nextExecDatetimeDict.ElementAt(i);
                    if (eintrag.Value.AddTicks(-eintrag.Value.Ticks % TimeSpan.TicksPerSecond) == jetzt.AddTicks(-jetzt.Ticks % TimeSpan.TicksPerSecond))
                    {
                        _ = BeginInvoke(new Action<int>(index => Utilities.ShowReminder(this, msgLocation, reminders[index], index)), i); // Übergabe des aktuellen Werts von i als Argument
                        nextExecDatetimeDict[eintrag.Key] = GetSingleExecTime(i); //UpdateNextExecDatetimeDict();
                        UpdateNotifyIconText();
                    }
                }
            }
            BeginInvoke(new Action(Invalidate));
        }
    }

    private void FrmClock_Load(object sender, EventArgs e)
    {
        foo = NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID1, (uint)(NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win | NativeMethods.Modifiers.Shift), (uint)Keys.J);
        bar = NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID0, (uint)(NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win), (uint)Keys.J);
        if (!foo && !bar) { Utilities.ErrorMsgTaskDlg(IntPtr.Zero, "Strg+Win+J konnte nicht registriert werden.", "Wahrscheinlich wird die Tastenkombination\nbereits von einer anderen App benutzt."); }
        else { notifyIcon.Text = appName + "\nStrg+Win+J"; }

        if (File.Exists(xmlPath))
        {
            try
            {
                var xDocument = XDocument.Load(xmlPath);
                if (xDocument == null || xDocument.Root == null) { File.Delete(xmlPath); }
                else if (xDocument != null && xDocument.Root != null)
                {
                    foreach (var element in xDocument.Root.Descendants("Configuration"))
                    {
                        clicktroughToolStripMenuItem.Checked = clickThrough = bool.TryParse(element.Element("ClickThrough")?.Value, out var st) ? st : clickThrough;
                        showCurrentDateToolStripMenuItem.Checked = bool.TryParse(element.Element("ShowCurrentDate")?.Value, out var sd) ? sd : showCurrentDate;
                        windowPosition = element.Element("WindowPosition")?.Value ?? string.Empty;
                        messagePosition = element.Element("MessagePosition")?.Value ?? string.Empty;
                        inflateSize = int.TryParse(element.Element("InflateSize")?.Value, out var i) ? i : inflateSize;
                        listSelection = int.TryParse(element.Element("ListSelection")?.Value, out var s) ? s : 0;
                    }

                    foreach (var element in xDocument.Root.Descendants("Reminders"))
                    {
                        for (var i = 0; i < 9; i++)
                        {
                            if (element.Descendants("Reminder" + i).Any())
                            {
                                var test = element.Element("Reminder" + i);
                                if (test != null) // Ensure the XElement is not null before accessing its elements.
                                {
                                    var check = bool.TryParse(test.Element("Check")?.Value, out var c) && c;
                                    var name = test.Element("Name")?.Value ?? string.Empty;
                                    var dateString = test.Element("Date")?.Value ?? string.Empty;
                                    var repeat = int.TryParse(test.Element("Repeat")?.Value, out var r) ? r : 0;
                                    var action = int.TryParse(test.Element("Action")?.Value, out var a) ? a : 0;
                                    var progPath = test.Element("Path")?.Value ?? string.Empty;
                                    var askBefore = bool.TryParse(test.Element("AskBefore")?.Value, out var ab) && ab;
                                    var hibernate = bool.TryParse(test.Element("Hibernate")?.Value, out var h) && h;
                                    var reminder = new Reminder(check, name, dateString, repeat, action, progPath, askBefore, hibernate);
                                    reminders.Add(reminder);
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Utilities.StartFile(xmlPath);
                Utilities.ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message);
            }
        }
        else { Directory.CreateDirectory(Path.GetDirectoryName(xmlPath)!); }

        var screen = Screen.PrimaryScreen!.WorkingArea;

        if (!string.IsNullOrEmpty(windowPosition))
        {
            var coords = windowPosition.Split(',');
            var width = Width; // 132
            var height = Height; // 126
            var xPos = int.Parse(coords[0]);
            var yPos = int.Parse(coords[1]);
            xPos = xPos > screen.Width ? screen.Width - width : xPos;
            xPos = xPos < 0 ? 0 : xPos;
            yPos = yPos + height > screen.Height ? screen.Height - height : yPos;
            yPos = yPos < 0 ? 0 : yPos;
            //width = xPos + width > screen.Width ? screen.Width - xPos : width;
            //height = yPos + height > screen.Height ? screen.Height - yPos : height;
            Location = new Point(xPos, yPos);
            //Size = new Size(width, height);
        }
        if (!string.IsNullOrEmpty(messagePosition))
        {
            var coords = messagePosition.Split(',');
            var width = 350; //  int.Parse(coords[2]);
            var height = 150; // int.Parse(coords[3]);
            var xPos = int.Parse(coords[0]);
            var yPos = int.Parse(coords[1]);
            xPos = xPos > screen.Width ? screen.Width - width : xPos;
            xPos = xPos < 0 ? 0 : xPos;
            yPos = yPos + height > screen.Height ? screen.Height - height : yPos;
            yPos = yPos < 0 ? 0 : yPos;
            //width = xPos + width > screen.Width ? screen.Width - xPos : width;
            //height = yPos + height > screen.Height ? screen.Height - yPos : height;
            msgLocation = new Point(xPos, yPos);
            //Size = new Size(width, height);
        }
        MakeFormTransparent(clickThrough);
        UpdateNextExecDatetimeDict();
        systemTimer.Start();
        execFlag = true; // Timer prüft
    }

    private void FrmClock_MouseEnter(object sender, EventArgs e) => Cursor = Cursors.Hand;
    private void FrmClock_MouseLeave(object sender, EventArgs e) => Cursor = Cursors.Default;

    private void FrmClock_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Clicks != 1) { return; } // MouseDoubleClick event not firing after add MouseDown event
        NativeMethods.ReleaseCapture();
        _ = NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
    }

    private void FrmClock_FormClosing(object sender, FormClosingEventArgs e)
    {
        systemTimer.Stop();
        systemTimer.Dispose();
        if (foo) { foo = NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ID1); }
        if (bar) { bar = NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ID0); }
        SaveConfiguration();
    }

    private void SaveConfiguration()
    {
        var point = Location; // Fensterposition speichern
        var size = Size;
        if (WindowState != FormWindowState.Normal)
        {
            point = RestoreBounds.Location;
            size = RestoreBounds.Size;
        }
        Hide(); // täuscht Schnelligkeit beim Beenden vor

        if (!File.Exists(xmlPath)) { Directory.CreateDirectory(Path.GetDirectoryName(xmlPath)!); }
        var doc = new XDocument();
        XElement root = new(appName);

        XElement configuration = new("Configuration");
        configuration.Add(new XElement("WindowPosition", string.Join(",", point.X, point.Y))); //, size.Width, size.Height)));
        configuration.Add(new XElement("MessagePosition", string.Join(",", msgLocation.X, msgLocation.Y))); //, size.Width, size.Height)));    
        configuration.Add(new XElement("ClickThrough", clicktroughToolStripMenuItem.Checked.ToString()));
        configuration.Add(new XElement("ShowCurrentDate", showCurrentDate.ToString()));
        configuration.Add(new XElement("InflateSize", inflateSize.ToString()));
        configuration.Add(new XElement("ListSelection", listSelection.ToString()));
        root.Add(configuration);

        XElement remindersElement = new("Reminders");
        if (reminders != null)
        {
            for (var i = 0; i < reminders.Count; i++)
            {
                var r = reminders[i];
                XElement el = new("Reminder" + i);
                el.Add(new XElement("Check", r.Check));
                el.Add(new XElement("Name", r.Name));
                el.Add(new XElement("Date", r.DateString));
                el.Add(new XElement("Repeat", r.Repeat));
                el.Add(new XElement("Action", r.Action));
                el.Add(new XElement("Path", r.ProgPath));
                el.Add(new XElement("AskBefore", r.AskBefore));
                el.Add(new XElement("Hibernate", r.Hibernate));
                remindersElement.Add(el);
            }
        }
        root.Add(remindersElement);
        doc.Add(root);
        doc.Save(xmlPath);
    }

    private void MakeFormTransparent(bool transparent)
    {
        var extendedStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE); // Fensterattribute abrufen
        if (transparent) { clicktroughToolStripMenuItem.Checked = NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, extendedStyle | NativeMethods.WS_EX_TRANSPARENT) > 0; }
        else { clicktroughToolStripMenuItem.Checked = NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, extendedStyle & ~NativeMethods.WS_EX_TRANSPARENT) == 0; }
    }

    private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        clicktroughToolStripMenuItem.Checked = (NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE) & NativeMethods.WS_EX_TRANSPARENT) != 0;
        showHideToolStripMenuItem.Checked = Visible;
        autoStartToolStripMenuItem.Checked = Utilities.IsAutoStartEnabled(appName, appPath);
    }

    private void ClicktroughToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (clicktroughToolStripMenuItem.Checked)
        {
            MakeFormTransparent(false);
            clicktroughToolStripMenuItem.Checked = false;
        }
        else
        {
            MakeFormTransparent(true);
            clicktroughToolStripMenuItem.Checked = true;
        }
    }

    private void UpdateNextExecDatetimeDict()
    {
        nextExecDatetimeDict.Clear();
        for (var i = 0; i < reminders.Count; i++) { nextExecDatetimeDict.Add(reminders[i].Name, GetSingleExecTime(i)); }
        UpdateNotifyIconText();
    }

    private void UpdateNotifyIconText()
    {
        var tempExecDatetimeDict = nextExecDatetimeDict.Where(static x => x.Value > DateTime.Now); // IEnumerable
        if (tempExecDatetimeDict.Any()) // check if the collection has elements
        {
            var text = string.Join("\n", tempExecDatetimeDict.OrderBy(static y => y.Value).Take(2).Select(static z => z.Key +
            (z.Value.Date == DateTime.Now.Date ?
            " heute um " + z.Value.ToString("HH:mm:ss") :
            z.Value.Date == DateTime.Now.Date.AddDays(1) ?
            " morgen um " + z.Value.ToString("HH:mm:ss") :
            " am " + z.Value.ToString("d.M.yy 'um' HH:mm:ss")) + " Uhr")) + (tempExecDatetimeDict.Count() > 2 ? "\n…" : "");
            notifyIcon.Text = text.Length > 127 ? text[..127] : text; // Ensure text length is within bounds (less than 128 characters) 
        }
        else { notifyIcon.Text = appName + (foo && bar ? "\n(Shift+)Strg+Win+J" : ""); }
    }

    private DateTime GetSingleExecTime(int index)
    {
        var reminder = reminders[index];
        var date = new DateTime(2000, 01, 01, 00, 00, 00); // don't use DateTime.MinVaue as a default value
        try
        {
            if (reminder.Check)
            {
                if (reminder.Repeat == 0 && reminder.Datetime > DateTime.Now) { date = reminder.Datetime; } // einmalig   
                else if (reminder.Repeat == 1) { date = Utilities.GetNextDate_1(reminder); } // stündlich  
                else if (reminder.Repeat == 2) { date = Utilities.GetNextDate_2(reminder); } // täglich  
                else if (reminder.Repeat == 3) { date = Utilities.GetNextDate_3(reminder); } // wöchentlich  
                else if (reminder.Repeat == 4) { date = Utilities.GetNextDate_4(reminder); } // monatlich   
                else if (reminder.Repeat == 5) { date = Utilities.GetNextDate_5(reminder); } // alle 3 Monate  
                else if (reminder.Repeat == 6) { date = Utilities.GetNextDate_6(reminder); } // jährlich  
            }
        }
        catch (Exception ex) { Utilities.ErrorMsgTaskDlg(IntPtr.Zero, ex.GetType().ToString(), ex.Message); }
        return date;
    }

    private void ShowCurrentDateToolStripMenuItem_Click(object sender, EventArgs e) => showCurrentDate = showCurrentDateToolStripMenuItem.Checked = !showCurrentDateToolStripMenuItem.Checked;

    private void ShowHideToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Visible) { Hide(); }
        else { Show(); }
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

    private void AutoStartToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (autoStartToolStripMenuItem.Checked)
        {
            Utilities.RemoveAutoStart(appName);
            autoStartToolStripMenuItem.Checked = false;
        }
        else
        {
            Utilities.SetAutoStart(appName, appPath);
            autoStartToolStripMenuItem.Checked = true;
        }
    }

    private void AlarmListeAnzeigenToolStripMenuItem_Click(object sender, EventArgs e)
    {
        execFlag = false; // Timer anhalten
        frmAlarmList = new AlarmList(reminders, listSelection, xmlPath);
        frmAlarmList.FormClosed += (sender, e) =>
        {
            reminders = frmAlarmList.GetReminders();
            listSelection = frmAlarmList.GetSelectedIndex();
            UpdateNextExecDatetimeDict();
            execFlag = true; // Timer wieder starten
        };
        frmAlarmList.Show(); // nicht-modal öffnen und beim Schließen Werte oder Variablen übernehmen
    }

    private async void Minutes90ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes90ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(90));
        Utilities.ShowReminderForm(msgLocation, 90);
        minutes90ToolStripMenuItem.Checked = false;
    }

    private async void Minutes60ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes60ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(60));
        Utilities.ShowReminderForm(msgLocation, 60);
        minutes60ToolStripMenuItem.Checked = false;
    }

    private async void Minutes45ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes45ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(45));
        Utilities.ShowReminderForm(msgLocation, 45);
        minutes45ToolStripMenuItem.Checked = false;
    }

    private async void Minutes30ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes30ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(30));
        Utilities.ShowReminderForm(msgLocation, 30);
        minutes30ToolStripMenuItem.Checked = false;
    }

    private async void Minutes20ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes20ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(20));
        Utilities.ShowReminderForm(msgLocation, 20);
        minutes20ToolStripMenuItem.Checked = false;
    }

    private async void Minutes15ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes15ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(15));
        Utilities.ShowReminderForm(msgLocation, 15);
        minutes15ToolStripMenuItem.Checked = false;
    }

    private async void Minutes10ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes10ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(10));
        Utilities.ShowReminderForm(msgLocation, 10);
        minutes10ToolStripMenuItem.Checked = false;
    }

    private async void Minutes5ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes5ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(5));
        Utilities.ShowReminderForm(msgLocation, 5);
        minutes5ToolStripMenuItem.Checked = false;
    }

    private async void Minutes3ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes3ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(3));
        Utilities.ShowReminderForm(msgLocation, 3);
        minutes3ToolStripMenuItem.Checked = false;
    }

    private async void Minutes2ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        minutes2ToolStripMenuItem.Checked = true;
        await Task.Delay(TimeSpan.FromMinutes(2));
        Utilities.ShowReminderForm(msgLocation, 2);
        minutes2ToolStripMenuItem.Checked = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        var radius = Math.Min(centerPoint.X, centerPoint.Y) - 4; // Radius des Ziffernblatts  
        g.FillEllipse(Brushes.FloralWhite, centerPoint.X - radius + 1, centerPoint.Y - radius + 1, 2 * radius - 2, 2 * radius - 2);
        g.FillEllipse(Brushes.Black, centerPoint.X - 3, centerPoint.Y - 3, 6, 6);
        using var pen = new Pen(Color.DarkSeaGreen, 2);
        g.DrawEllipse(pen, centerPoint.X - radius, centerPoint.Y - radius, 2 * radius, 2 * radius); // äußerer Kreis  
        pen.Color = Color.DimGray;
        pen.Width = 0.5f;
        g.DrawEllipse(pen, centerPoint.X - radius + 2, centerPoint.Y - radius + 2, 2 * radius - 4, 2 * radius - 4); // innerer äußerer  

        var now = DateTime.Now; // Aktuelle Uhrzeit
        pen.Color = Color.DimGray;
        pen.Width = 2f;
        for (var i = 0; i < 12; i++) // Zeichne die Striche  
        {
            var angle = i * Math.PI / 6; // Winkel in Radiant  
            var startX = (int)Math.Round(centerPoint.X + ((radius - 8) * Math.Sin(angle)));
            var endX = (int)Math.Round(centerPoint.X + (radius * Math.Sin(angle)));
            var startY = (int)Math.Round(centerPoint.Y - ((radius - 8) * Math.Cos(angle)));
            var endY = (int)Math.Round(centerPoint.Y - (radius * Math.Cos(angle)));
            g.DrawLine(pen, startX, startY, endX, endY);
        }
        var hour = now.Hour % 12;
        var minute = now.Minute;
        var sekunde = now.Second;
        var secTuplePoint = Utilities.CalculateSecondPoint(sekunde, 55, centerPoint);
        var minEndPoint = Utilities.CalculateMinutePoint(minute, 55, centerPoint);
        var hourEndPoint = Utilities.CalculateHourPoint(hour, minute, 40, centerPoint);
        if (showCurrentDate)
        {
            var rectWidth = 26; // Breite des Rectangle
            var rectX = centerPoint.X - rectWidth / 2;
            var rectY = 32;
            var (qRight, qObliq) = Utilities.GetCoveredQuadrants(centerPoint, minEndPoint, hourEndPoint); // Arrays mit 4 bool-Werten (für jedes Viertel)
            if (!qRight[0] && !qRight[1]) // Zeiger in unterer Hälfte
            {
                rectX = centerPoint.X - rectWidth / 2;
                rectY = 32;
            }
            else if (!qRight[2] && !qRight[3]) // Zeiger in oberer Hälfte
            {
                rectX = centerPoint.X - rectWidth / 2;
                rectY = 76;
            }
            else if (!qRight[1] && !qRight[2]) // Zeiger in linker Hälfte
            {
                rectX = centerPoint.X + 10;
                rectY = centerPoint.Y - 6;
            }
            else if (!qRight[0] && !qRight[3]) // Zeiger in rechter Hälfte
            {
                rectX = centerPoint.X - 38;
                rectY = centerPoint.Y - 6;
            }
            else if (!qObliq[0]) // oben
            {
                rectX = centerPoint.X - rectWidth / 2;
                rectY = 32;
            }
            else if (!qObliq[2]) // unten
            {
                rectX = centerPoint.X - rectWidth / 2;
                rectY = 76;
            }
            else if (!qObliq[3]) // rechts
            {
                rectX = centerPoint.X + 10;
                rectY = centerPoint.Y - 6;
            }
            else if (!qObliq[1]) // links
            {
                rectX = centerPoint.X - 38;
                rectY = centerPoint.Y - 6;
            }
            var rect = new Rectangle(rectX, rectY, rectWidth, 17);
            g.FillRectangle(Brushes.White, rect);
            pen.Width = 0.5f;
            pen.Color = Color.GreenYellow;
            g.DrawRectangle(pen, rect);
            g.DrawString(now.Day.ToString(), new Font(Font.FontFamily, 12, FontStyle.Bold), Brushes.DarkSeaGreen, rect, new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            });
        }
        pen.Color = Color.Black;
        pen.Width = 3f;
        g.DrawLine(pen, centerPoint, hourEndPoint);
        pen.Color = Color.Black;
        pen.Width = 2f;
        g.DrawLine(pen, centerPoint, minEndPoint);
        pen.Color = Color.Crimson;
        pen.Width = 1.5f;
        g.DrawLine(pen, secTuplePoint.Item1, secTuplePoint.Item2);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_HOTKEY)
        {
            if (m.WParam == NativeMethods.HOTKEY_ID0)
            {
                if (frmAlarmList != null && frmAlarmList.Visible && frmAlarmList == ActiveForm) { frmAlarmList.Close(); }
                else
                {
                    if (frmAlarmList != null && frmAlarmList.Visible) { frmAlarmList.Activate(); }
                    else { AlarmListeAnzeigenToolStripMenuItem_Click(alarmListeAnzeigenToolStripMenuItem, EventArgs.Empty); } // Pass a valid non-null object instead of null  
                }
                //foreach (Form openForm in Application.OpenForms)
                //{
                //    if (openForm is AlarmList && openForm.Visible && openForm == ActiveForm)
                //    {
                //        openForm.Close();
                //        return;
                //    }
                //}
                //AlarmListeAnzeigenToolStripMenuItem_Click(alarmListeAnzeigenToolStripMenuItem, EventArgs.Empty); // Pass a valid non-null object instead of null  
            }
            else if (m.WParam == NativeMethods.HOTKEY_ID1)
            {
                if (contextMenuStrip.Visible)
                {
                    eggTimerToolStripMenuItem.HideDropDown();
                    contextMenuStrip.Hide();
                }
                else
                {
                    var primaryScreen = Screen.PrimaryScreen;
                    if (primaryScreen != null)
                    {
                        contextMenuStrip.Show(primaryScreen.Bounds.Width / 2 - (contextMenuStrip.Width / 2), primaryScreen.Bounds.Height / 2 - (contextMenuStrip.Height / 2));
                        eggTimerToolStripMenuItem.ShowDropDown();
                    }
                }
            }
        }
        else { base.WndProc(ref m); }
    }

    private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (frmAlarmList != null && frmAlarmList.Visible) { frmAlarmList.Activate(); }
            else { AlarmListeAnzeigenToolStripMenuItem_Click(alarmListeAnzeigenToolStripMenuItem, e); }
        }
    }

}

