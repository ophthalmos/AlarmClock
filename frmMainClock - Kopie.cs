using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Xml;
using System.Xml.Linq;

namespace AlarmClock;

public partial class FrmClock : Form
{
    public List<Reminder> GetReminderList() => reminders;
    public void SetMsgLocation(Point value) => msgLocation = value;

    private Point centerPoint;
    private readonly string appName = Application.ProductName ?? "AlarmClock";
    private static readonly string appPath = Application.ExecutablePath;
    private readonly string xmlPath;
    private string windowPosition = string.Empty;
    private string messagePosition = string.Empty;
    private bool clickThrough = true;
    private bool showLarger = false;
    private readonly int regularWdith;
    private readonly int regularHeight;
    private bool showCurrentDate = true;
    private int inflateSize = 200;
    private readonly double scaleFactor = 1.20;
    private int listSelection = 0; // für die ListView-Position
    private string hotkeyLetter = "H"; // für den Hotkey-Handler    
    private int letzteSekunde = -1;
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
        regularWdith = Width;
        regularHeight = Height;
        //centerPoint = new Point((ClientSize.Width / 2) + 5, (ClientSize.Height / 2) + 1);
        xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, appName + ".xml");
        if (!Utilities.IsInnoSetupValid(Path.GetDirectoryName(appPath)!)) { xmlPath = Path.ChangeExtension(appPath, ".xml"); } // portable
        minutes90ToolStripMenuItem.Tag = 90;
        minutes60ToolStripMenuItem.Tag = 60;
        minutes45ToolStripMenuItem.Tag = 45;
        minutes30ToolStripMenuItem.Tag = 30;
        minutes20ToolStripMenuItem.Tag = 20;
        minutes15ToolStripMenuItem.Tag = 15;
        minutes10ToolStripMenuItem.Tag = 10;
        minutes5ToolStripMenuItem.Tag = 5;
        minutes3ToolStripMenuItem.Tag = 3;
        minutes2ToolStripMenuItem.Tag = 2;
        systemTimer.Interval = 200; // Millisekunden
        systemTimer.AutoReset = true; // Timer tickt alle 100 ms
        systemTimer.Elapsed += Timer_Tick;
    }

    private void Timer_Tick(object? source, ElapsedEventArgs e) // System.Timers.Timer wird im Thread des Thread-Pools ausgeführt, nicht im UI-Thread
    {
        BeginInvoke(() =>  // if (InvokeRequired) erübrigt sich weil der Timer in einem anderen Thread läuft // .NET9: InvokeAsync
        {
            if (clicktroughToolStripMenuItem.Checked)
            {
                Opacity = new Rectangle(Bounds.X - inflateSize, Bounds.Y - inflateSize, Bounds.Width + (inflateSize * 2), Bounds.Height + (inflateSize * 2)).Contains(Cursor.Position) ? 0.05 : 1.0;
            }
            var jetzt = DateTime.Now;
            if (jetzt.Second != letzteSekunde)
            {
                letzteSekunde = DateTime.Now.Second;
                if (execFlag) // Action-Abfrage im Timer
                {
                    for (var i = 0; i < nextExecDatetimeDict.Count; i++)
                    {
                        var eintrag = nextExecDatetimeDict.ElementAt(i);
                        Debug.WriteLine(i + ": " + eintrag.Value.ToString());
                        if (eintrag.Value.AddTicks(-eintrag.Value.Ticks % TimeSpan.TicksPerSecond) == jetzt.AddTicks(-jetzt.Ticks % TimeSpan.TicksPerSecond))
                        {
                            Utilities.ShowReminder(this, msgLocation, reminders[i], i); // InvalidOperation!
                            nextExecDatetimeDict[eintrag.Key] = GetSingleExecTime(i); //UpdateNextExecDatetimeDict();
                            UpdateNotifyIconText();
                        }
                    }
                }
                if (Visible && !IsDisposed && !Disposing) { Invalidate(); } // OnPaint wird aufgerufen
                NativeMethods.SetWindowPos(Handle, 0, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
            }
        });
    }

    private void FrmClock_Load(object sender, EventArgs e)
    {
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
                        showlargerToolStripMenuItem.Checked = showLarger = bool.TryParse(element.Element("ShowLarger")?.Value, out var sl) ? sl : showLarger;
                        showCurrentDateToolStripMenuItem.Checked = bool.TryParse(element.Element("ShowCurrentDate")?.Value, out var sd) ? sd : showCurrentDate;
                        windowPosition = element.Element("WindowPosition")?.Value ?? string.Empty;
                        messagePosition = element.Element("MessagePosition")?.Value ?? string.Empty;
                        inflateSize = int.TryParse(element.Element("InflateSize")?.Value, out var i) ? i : inflateSize;
                        listSelection = int.TryParse(element.Element("ListSelection")?.Value, out var s) ? s : 0;
                        hotkeyLetter = element.Element("HotkeyLetter")?.Value ?? hotkeyLetter;
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

        Keys key;
        if (Enum.TryParse(hotkeyLetter, out Keys qxz)) { key = qxz; } // TryParse ensures valid conversion
        else
        {
            key = Keys.J;  // Defaultwert
            hotkeyLetter = "J";
        }
        foo = NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID1, (uint)(NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win | NativeMethods.Modifiers.Shift), (uint)key);
        bar = NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID0, (uint)(NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win), (uint)key);
        if (!foo && !bar) { Utilities.ErrorMsgTaskDlg(IntPtr.Zero, "Strg+Win+" + hotkeyLetter + " konnte nicht registriert werden.", "Wahrscheinlich wird die Tastenkombination\nbereits von einer anderen App benutzt."); }
        else
        {
            alarmListeAnzeigenToolStripMenuItem.ShortcutKeyDisplayString = "Strg+Win+" + hotkeyLetter;
            eggTimerToolStripMenuItem.Text += "  (Shift+Strg+Win+" + hotkeyLetter + ")";
        }

        var screen = Screen.PrimaryScreen!.WorkingArea;

        if (showLarger) { Size = new Size((int)(regularWdith * scaleFactor), (int)(regularHeight * scaleFactor)); }
        centerPoint = new Point((ClientSize.Width / 2) + 5, (ClientSize.Height / 2) + 1);

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
            Location = new Point(xPos, yPos);
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
        if (reminders != null) { UpdateNextExecDatetimeDict(); }
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
        var point = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location; // Fensterposition speichern 
        Hide(); // täuscht Schnelligkeit beim Beenden vor

        if (!File.Exists(xmlPath)) { Directory.CreateDirectory(Path.GetDirectoryName(xmlPath)!); }
        var doc = new XDocument();
        XElement root = new(appName);

        XElement configuration = new("Configuration");
        configuration.Add(new XElement("WindowPosition", string.Join(",", point.X, point.Y))); //, size.Width, size.Height)));
        configuration.Add(new XElement("MessagePosition", string.Join(",", msgLocation.X, msgLocation.Y))); //, size.Width, size.Height)));    
        configuration.Add(new XElement("ClickThrough", clicktroughToolStripMenuItem.Checked.ToString().ToLower()));
        configuration.Add(new XElement("ShowLarger", showlargerToolStripMenuItem.Checked.ToString().ToLower()));
        configuration.Add(new XElement("ShowCurrentDate", showCurrentDate.ToString().ToLower()));
        configuration.Add(new XElement("InflateSize", inflateSize.ToString()));
        configuration.Add(new XElement("ListSelection", listSelection.ToString()));
        configuration.Add(new XElement("HotkeyLetter", hotkeyLetter));
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
        else { notifyIcon.Text = appName; }
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

    private void ShowlargerToolStripMenuItem_Click(object sender, EventArgs e)
    {
        showLarger = showlargerToolStripMenuItem.Checked = !showlargerToolStripMenuItem.Checked;
        var newScaleFactor = showLarger ? scaleFactor : 1.0;
        Size = new Size((int)(regularWdith * newScaleFactor), (int)(regularHeight * newScaleFactor));
        centerPoint = new Point((ClientSize.Width / 2) + 5, (ClientSize.Height / 2) + 1);
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

    private async void MinutesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
        if (menuItem.Tag is int minutes)
        {
            menuItem.Checked = true;
            await Task.Delay(TimeSpan.FromMinutes(minutes));
            Utilities.ShowReminderForm(msgLocation, minutes);
            menuItem.Checked = false;
        }
    }

    protected override CreateParams CreateParams  // damit das Fenster nicht in der Alt-Tab-Liste erscheint
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        var radius = Math.Min(centerPoint.X, centerPoint.Y) - 4; // Radius des Ziffernblatts  
        using (var brush = new LinearGradientBrush(new Rectangle(centerPoint.X - radius, centerPoint.Y - radius, radius * 2, radius * 2), Color.White, Color.FromArgb(233, 238, 240), 45f)) // Lichtfarbe, Schattenfarbe, Winkel: 45° = von links oben nach rechts unten
        {
            g.FillEllipse(brush, centerPoint.X - radius + 1, centerPoint.Y - radius + 1, 2 * radius - 2, 2 * radius - 2);
        }
        g.FillEllipse(Brushes.Black, centerPoint.X - 3, centerPoint.Y - 3, 6, 6);
        using var pen = new Pen(Color.DarkGray, 2); // DarkSeaGreen
        g.DrawEllipse(pen, centerPoint.X - radius, centerPoint.Y - radius, 2 * radius, 2 * radius); // äußerer Kreis  
        pen.Color = Color.DimGray;
        pen.Width = 0.5f;
        g.DrawEllipse(pen, centerPoint.X - radius + 2, centerPoint.Y - radius + 2, 2 * radius - 4, 2 * radius - 4); // innerer äußerer  
        var now = DateTime.Now; // Aktuelle Uhrzeit
        pen.Color = Color.DarkGray;
        pen.Width = 2f;
        for (var i = 0; i < 60; i++)
        {
            if (i % 5 == 0) { continue; } // Überspringe die Stundenpositionen, damit sie nicht doppelt gezeichnet werden
            var angle = i * Math.PI / 30; // 360° / 60 = 6° = π/30 Radiant
            var startX = (int)Math.Round(centerPoint.X + ((radius - 6) * Math.Sin(angle)));
            var endX = (int)Math.Round(centerPoint.X + ((radius - 3) * Math.Sin(angle)));
            var startY = (int)Math.Round(centerPoint.Y - ((radius - 6) * Math.Cos(angle)));
            var endY = (int)Math.Round(centerPoint.Y - ((radius - 3) * Math.Cos(angle)));
            g.DrawLine(pen, startX, startY, endX, endY);
        }
        pen.Color = Color.DimGray;
        for (var i = 0; i < 12; i++) // Zeichne die Striche  
        {
            var angle = i * Math.PI / 6; // Winkel in Radiant  
            var startX = (int)Math.Round(centerPoint.X + ((radius - 11) * Math.Sin(angle)));
            var endX = (int)Math.Round(centerPoint.X + ((radius - 3) * Math.Sin(angle)));
            var startY = (int)Math.Round(centerPoint.Y - ((radius - 11) * Math.Cos(angle)));
            var endY = (int)Math.Round(centerPoint.Y - ((radius - 3) * Math.Cos(angle)));
            g.DrawLine(pen, startX, startY, endX, endY);
        }
        var hour = now.Hour % 12;
        var minute = now.Minute;
        var sekunde = now.Second;

        var secTuplePoint = Utilities.CalculateSecondPoint(sekunde, (int)(0.9 * radius), centerPoint);
        var minEndPoint = Utilities.CalculateMinutePoint(minute, (int)(0.9 * radius), centerPoint);
        var hourEndPoint = Utilities.CalculateHourPoint(hour, minute, (int)(0.7 * radius), centerPoint);

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
                rectY = centerPoint.Y + (showLarger ? 22 : 14);
            }
            else if (!qRight[1] && !qRight[2]) // Zeiger in linker Hälfte
            {
                rectX = centerPoint.X + (showLarger ? 18 : 10);
                rectY = centerPoint.Y - 8;
            }
            else if (!qRight[0] && !qRight[3]) // Zeiger in rechter Hälfte
            {
                rectX = centerPoint.X - 38;
                rectY = centerPoint.Y - 6;
            }
            else if (!qObliq[0]) // oben
            {
                rectX = centerPoint.X - rectWidth / 2;
                rectY = showLarger ? 40 : 32;
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
            using (var brush = new LinearGradientBrush(rect, Color.LightSlateGray, Color.SlateGray, LinearGradientMode.ForwardDiagonal)) //Color.MediumAquamarine, Color.DarkCyan
            {
                g.DrawRectangle(new Pen(brush, 1), rect); // Zeichne den Rahmen des Rechtecks
            }
            rect.Y += 1; // Verschiebe das Rechteck um 1 Pixel nach unten
            rect.X += 1;
            rect.Width -= 2;
            rect.Height -= 2;
            using (var brush = new LinearGradientBrush(rect, Color.Silver, Color.White, LinearGradientMode.ForwardDiagonal)) // innerer Schatten
            {
                g.DrawRectangle(new Pen(brush, 1), rect); // Zeichne den Rahmen des Rechtecks
            }
            rect.Y += 1; // Verschiebe das Rechteck um 1 Pixel nach unten
            g.DrawString(now.Day.ToString(), new Font(Font.FontFamily, 12, FontStyle.Regular), Brushes.DarkSlateGray, rect, new StringFormat
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
                BeginInvoke(() => // .NET9: InvokeAsync
                {
                    if (frmAlarmList != null && frmAlarmList.Visible && frmAlarmList == ActiveForm) { frmAlarmList.Close(); }
                    else
                    {
                        if (frmAlarmList != null && frmAlarmList.Visible) { frmAlarmList.Activate(); }
                        else { AlarmListeAnzeigenToolStripMenuItem_Click(alarmListeAnzeigenToolStripMenuItem, EventArgs.Empty); } // Pass a valid non-null object instead of null  
                    }
                });
            }
            else if (m.WParam == NativeMethods.HOTKEY_ID1)
            {
                BeginInvoke(() => // .NET9: InvokeAsync
                {
                    if (contextMenuStrip.Visible)
                    {
                        eggTimerToolStripMenuItem.HideDropDown();
                        contextMenuStrip.Close();
                    }
                    else
                    {
                        var primaryScreen = Screen.PrimaryScreen;
                        if (primaryScreen != null)
                        {
                            contextMenuStrip.Show(primaryScreen.Bounds.Width / 2 - (contextMenuStrip.Width / 2), primaryScreen.Bounds.Height / 2 - (contextMenuStrip.Height / 2));
                            contextMenuStrip.BringToFront();
                            eggTimerToolStripMenuItem.ShowDropDown();
                        }
                    }
                });
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

