namespace AlarmClock;

partial class FrmClock
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmClock));
        notifyIcon = new NotifyIcon(components);
        contextMenuStrip = new ContextMenuStrip(components);
        autoStartToolStripMenuItem = new ToolStripMenuItem();
        showHideToolStripMenuItem = new ToolStripMenuItem();
        showCurrentDateToolStripMenuItem = new ToolStripMenuItem();
        clicktroughToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        eggTimerToolStripMenuItem = new ToolStripMenuItem();
        minutes2ToolStripMenuItem = new ToolStripMenuItem();
        minutes3ToolStripMenuItem = new ToolStripMenuItem();
        minutes5ToolStripMenuItem = new ToolStripMenuItem();
        minutes10ToolStripMenuItem = new ToolStripMenuItem();
        minutes15ToolStripMenuItem = new ToolStripMenuItem();
        minutes20ToolStripMenuItem = new ToolStripMenuItem();
        minutes30ToolStripMenuItem = new ToolStripMenuItem();
        minutes45ToolStripMenuItem = new ToolStripMenuItem();
        minutes60ToolStripMenuItem = new ToolStripMenuItem();
        minutes90ToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator3 = new ToolStripSeparator();
        alarmListeAnzeigenToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        showlargerToolStripMenuItem = new ToolStripMenuItem();
        contextMenuStrip.SuspendLayout();
        SuspendLayout();
        // 
        // notifyIcon
        // 
        notifyIcon.ContextMenuStrip = contextMenuStrip;
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "AlarmClock";
        notifyIcon.Visible = true;
        notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        // 
        // contextMenuStrip
        // 
        contextMenuStrip.Items.AddRange(new ToolStripItem[] { autoStartToolStripMenuItem, showHideToolStripMenuItem, showlargerToolStripMenuItem, showCurrentDateToolStripMenuItem, clicktroughToolStripMenuItem, toolStripSeparator1, eggTimerToolStripMenuItem, toolStripSeparator3, alarmListeAnzeigenToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
        contextMenuStrip.Name = "contextMenuStrip";
        contextMenuStrip.Size = new Size(192, 220);
        contextMenuStrip.Opening += ContextMenuStrip_Opening;
        // 
        // autoStartToolStripMenuItem
        // 
        autoStartToolStripMenuItem.Name = "autoStartToolStripMenuItem";
        autoStartToolStripMenuItem.Size = new Size(191, 22);
        autoStartToolStripMenuItem.Text = "&AutoStart";
        autoStartToolStripMenuItem.Click += AutoStartToolStripMenuItem_Click;
        // 
        // showHideToolStripMenuItem
        // 
        showHideToolStripMenuItem.Font = new Font("Segoe UI", 9F);
        showHideToolStripMenuItem.Name = "showHideToolStripMenuItem";
        showHideToolStripMenuItem.Size = new Size(191, 22);
        showHideToolStripMenuItem.Text = "Uhr &sichtbar";
        showHideToolStripMenuItem.Click += ShowHideToolStripMenuItem_Click;
        // 
        // showCurrentDateToolStripMenuItem
        // 
        showCurrentDateToolStripMenuItem.Font = new Font("Segoe UI", 9F);
        showCurrentDateToolStripMenuItem.Name = "showCurrentDateToolStripMenuItem";
        showCurrentDateToolStripMenuItem.Size = new Size(191, 22);
        showCurrentDateToolStripMenuItem.Text = "&Datum anzeigen";
        showCurrentDateToolStripMenuItem.Click += ShowCurrentDateToolStripMenuItem_Click;
        // 
        // clicktroughToolStripMenuItem
        // 
        clicktroughToolStripMenuItem.Name = "clicktroughToolStripMenuItem";
        clicktroughToolStripMenuItem.Size = new Size(191, 22);
        clicktroughToolStripMenuItem.Text = "Uhr &hindurchklickbar";
        clicktroughToolStripMenuItem.Click += ClicktroughToolStripMenuItem_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(188, 6);
        // 
        // eggTimerToolStripMenuItem
        // 
        eggTimerToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { minutes2ToolStripMenuItem, minutes3ToolStripMenuItem, minutes5ToolStripMenuItem, minutes10ToolStripMenuItem, minutes15ToolStripMenuItem, minutes20ToolStripMenuItem, minutes30ToolStripMenuItem, minutes45ToolStripMenuItem, minutes60ToolStripMenuItem, minutes90ToolStripMenuItem });
        eggTimerToolStripMenuItem.Image = Properties.Resources.Reminder_16x;
        eggTimerToolStripMenuItem.Name = "eggTimerToolStripMenuItem";
        eggTimerToolStripMenuItem.Size = new Size(191, 22);
        eggTimerToolStripMenuItem.Text = "&Erinnerungen";
        // 
        // minutes2ToolStripMenuItem
        // 
        minutes2ToolStripMenuItem.Name = "minutes2ToolStripMenuItem";
        minutes2ToolStripMenuItem.Size = new Size(134, 22);
        minutes2ToolStripMenuItem.Text = "2 Minuten";
        minutes2ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes3ToolStripMenuItem
        // 
        minutes3ToolStripMenuItem.Name = "minutes3ToolStripMenuItem";
        minutes3ToolStripMenuItem.Size = new Size(134, 22);
        minutes3ToolStripMenuItem.Text = "3 Minuten";
        minutes3ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes5ToolStripMenuItem
        // 
        minutes5ToolStripMenuItem.Name = "minutes5ToolStripMenuItem";
        minutes5ToolStripMenuItem.Size = new Size(134, 22);
        minutes5ToolStripMenuItem.Text = "5 Minuten";
        minutes5ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes10ToolStripMenuItem
        // 
        minutes10ToolStripMenuItem.Name = "minutes10ToolStripMenuItem";
        minutes10ToolStripMenuItem.Size = new Size(134, 22);
        minutes10ToolStripMenuItem.Text = "10 Minuten";
        minutes10ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes15ToolStripMenuItem
        // 
        minutes15ToolStripMenuItem.Name = "minutes15ToolStripMenuItem";
        minutes15ToolStripMenuItem.Size = new Size(134, 22);
        minutes15ToolStripMenuItem.Text = "15 Minuten";
        minutes15ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes20ToolStripMenuItem
        // 
        minutes20ToolStripMenuItem.Name = "minutes20ToolStripMenuItem";
        minutes20ToolStripMenuItem.Size = new Size(134, 22);
        minutes20ToolStripMenuItem.Text = "20 Minuten";
        minutes20ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes30ToolStripMenuItem
        // 
        minutes30ToolStripMenuItem.Name = "minutes30ToolStripMenuItem";
        minutes30ToolStripMenuItem.Size = new Size(134, 22);
        minutes30ToolStripMenuItem.Text = "30 Minuten";
        minutes30ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes45ToolStripMenuItem
        // 
        minutes45ToolStripMenuItem.Name = "minutes45ToolStripMenuItem";
        minutes45ToolStripMenuItem.Size = new Size(134, 22);
        minutes45ToolStripMenuItem.Text = "45 Minuten";
        minutes45ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes60ToolStripMenuItem
        // 
        minutes60ToolStripMenuItem.Name = "minutes60ToolStripMenuItem";
        minutes60ToolStripMenuItem.Size = new Size(134, 22);
        minutes60ToolStripMenuItem.Text = "60 Minuten";
        minutes60ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // minutes90ToolStripMenuItem
        // 
        minutes90ToolStripMenuItem.Name = "minutes90ToolStripMenuItem";
        minutes90ToolStripMenuItem.Size = new Size(134, 22);
        minutes90ToolStripMenuItem.Text = "90 Minuten";
        minutes90ToolStripMenuItem.Click += MinutesToolStripMenuItem_Click;
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(188, 6);
        // 
        // alarmListeAnzeigenToolStripMenuItem
        // 
        alarmListeAnzeigenToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        alarmListeAnzeigenToolStripMenuItem.Image = Properties.Resources.List_16x;
        alarmListeAnzeigenToolStripMenuItem.Name = "alarmListeAnzeigenToolStripMenuItem";
        alarmListeAnzeigenToolStripMenuItem.Size = new Size(191, 22);
        alarmListeAnzeigenToolStripMenuItem.Text = "Alarm-&Liste anzeigen";
        alarmListeAnzeigenToolStripMenuItem.Click += AlarmListeAnzeigenToolStripMenuItem_Click;
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(188, 6);
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Image = Properties.Resources.SaveClose_16x;
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(191, 22);
        exitToolStripMenuItem.Text = "AlarmClock &beenden";
        exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
        // 
        // showlargerToolStripMenuItem
        // 
        showlargerToolStripMenuItem.Name = "showlargerToolStripMenuItem";
        showlargerToolStripMenuItem.Size = new Size(191, 22);
        showlargerToolStripMenuItem.Text = "Uhr vergrößern";
        showlargerToolStripMenuItem.Click += ShowlargerToolStripMenuItem_Click;
        // 
        // FrmClock
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.Gray;
        BackgroundImageLayout = ImageLayout.Stretch;
        ClientSize = new Size(132, 126);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "FrmClock";
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.Manual;
        Text = "AlarmClock";
        TopMost = true;
        TransparencyKey = Color.Gray;
        FormClosing += FrmClock_FormClosing;
        Load += FrmClock_Load;
        MouseDown += FrmClock_MouseDown;
        MouseEnter += FrmClock_MouseEnter;
        MouseLeave += FrmClock_MouseLeave;
        contextMenuStrip.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenuStrip;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem clicktroughToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem showHideToolStripMenuItem;
    private ToolStripMenuItem autoStartToolStripMenuItem;
    private ToolStripMenuItem alarmListeAnzeigenToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem eggTimerToolStripMenuItem;
    private ToolStripMenuItem minutes5ToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem minutes90ToolStripMenuItem;
    private ToolStripMenuItem minutes60ToolStripMenuItem;
    private ToolStripMenuItem minutes45ToolStripMenuItem;
    private ToolStripMenuItem minutes30ToolStripMenuItem;
    private ToolStripMenuItem minutes20ToolStripMenuItem;
    private ToolStripMenuItem minutes15ToolStripMenuItem;
    private ToolStripMenuItem minutes10ToolStripMenuItem;
    private ToolStripMenuItem minutes3ToolStripMenuItem;
    private ToolStripMenuItem minutes2ToolStripMenuItem;
    private ToolStripMenuItem showCurrentDateToolStripMenuItem;
    private ToolStripMenuItem showlargerToolStripMenuItem;
}
