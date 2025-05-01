namespace AlarmClock;

partial class AlarmEditor
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmEditor));
        gbName = new GroupBox();
        tbName = new TextBox();
        gbTime = new GroupBox();
        cbRepeat = new ComboBox();
        dtpDate = new DateTimePicker();
        dtpTime = new DateTimePicker();
        gbAction = new GroupBox();
        btnProgram = new Button();
        tbProgram = new TextBox();
        rbtnProgram = new RadioButton();
        rbtnShutdown = new RadioButton();
        rbtnMessage = new RadioButton();
        rbtnSound = new RadioButton();
        ckbHibernate = new CheckBox();
        ckbAskBefore = new CheckBox();
        btnOK = new Button();
        btnCancel = new Button();
        gbOption = new GroupBox();
        btnNow = new Button();
        openFileDialog = new OpenFileDialog();
        gbName.SuspendLayout();
        gbTime.SuspendLayout();
        gbAction.SuspendLayout();
        gbOption.SuspendLayout();
        SuspendLayout();
        // 
        // gbName
        // 
        gbName.Controls.Add(tbName);
        gbName.Location = new Point(12, 12);
        gbName.Name = "gbName";
        gbName.Size = new Size(309, 60);
        gbName.TabIndex = 0;
        gbName.TabStop = false;
        gbName.Text = "Name";
        // 
        // tbName
        // 
        tbName.Location = new Point(6, 24);
        tbName.MaxLength = 50;
        tbName.Name = "tbName";
        tbName.Size = new Size(297, 25);
        tbName.TabIndex = 0;
        // 
        // gbTime
        // 
        gbTime.Controls.Add(cbRepeat);
        gbTime.Controls.Add(dtpDate);
        gbTime.Controls.Add(dtpTime);
        gbTime.Location = new Point(12, 78);
        gbTime.Name = "gbTime";
        gbTime.Size = new Size(309, 60);
        gbTime.TabIndex = 1;
        gbTime.TabStop = false;
        gbTime.Text = "Zeit";
        // 
        // cbRepeat
        // 
        cbRepeat.FormattingEnabled = true;
        cbRepeat.Location = new Point(188, 24);
        cbRepeat.Name = "cbRepeat";
        cbRepeat.Size = new Size(115, 25);
        cbRepeat.TabIndex = 2;
        cbRepeat.Text = "Einmal";
        // 
        // dtpDate
        // 
        dtpDate.Format = DateTimePickerFormat.Short;
        dtpDate.Location = new Point(91, 24);
        dtpDate.Name = "dtpDate";
        dtpDate.Size = new Size(91, 25);
        dtpDate.TabIndex = 1;
        // 
        // dtpTime
        // 
        dtpTime.Format = DateTimePickerFormat.Time;
        dtpTime.Location = new Point(6, 24);
        dtpTime.Name = "dtpTime";
        dtpTime.ShowUpDown = true;
        dtpTime.Size = new Size(79, 25);
        dtpTime.TabIndex = 0;
        // 
        // gbAction
        // 
        gbAction.Controls.Add(btnProgram);
        gbAction.Controls.Add(tbProgram);
        gbAction.Controls.Add(rbtnProgram);
        gbAction.Controls.Add(rbtnShutdown);
        gbAction.Controls.Add(rbtnMessage);
        gbAction.Controls.Add(rbtnSound);
        gbAction.Location = new Point(12, 144);
        gbAction.Name = "gbAction";
        gbAction.Size = new Size(309, 88);
        gbAction.TabIndex = 2;
        gbAction.TabStop = false;
        gbAction.Text = "Aktion";
        // 
        // btnProgram
        // 
        btnProgram.Enabled = false;
        btnProgram.Location = new Point(272, 53);
        btnProgram.Name = "btnProgram";
        btnProgram.Size = new Size(31, 27);
        btnProgram.TabIndex = 5;
        btnProgram.Text = "…";
        btnProgram.UseVisualStyleBackColor = true;
        btnProgram.Click += BtnProgram_Click;
        // 
        // tbProgram
        // 
        tbProgram.Enabled = false;
        tbProgram.Location = new Point(107, 53);
        tbProgram.MaxLength = 256;
        tbProgram.Name = "tbProgram";
        tbProgram.Size = new Size(159, 25);
        tbProgram.TabIndex = 4;
        // 
        // rbtnProgram
        // 
        rbtnProgram.AutoSize = true;
        rbtnProgram.Location = new Point(6, 53);
        rbtnProgram.Name = "rbtnProgram";
        rbtnProgram.Size = new Size(95, 23);
        rbtnProgram.TabIndex = 3;
        rbtnProgram.Text = "Programm:";
        rbtnProgram.UseVisualStyleBackColor = true;
        rbtnProgram.CheckedChanged += RbtnProgram_CheckedChanged;
        // 
        // rbtnShutdown
        // 
        rbtnShutdown.AutoSize = true;
        rbtnShutdown.Location = new Point(181, 24);
        rbtnShutdown.Name = "rbtnShutdown";
        rbtnShutdown.Size = new Size(121, 23);
        rbtnShutdown.TabIndex = 2;
        rbtnShutdown.Text = "Herunterfahren";
        rbtnShutdown.UseVisualStyleBackColor = true;
        rbtnShutdown.CheckedChanged += RbtnShutdown_CheckedChanged;
        // 
        // rbtnMessage
        // 
        rbtnMessage.AutoSize = true;
        rbtnMessage.Checked = true;
        rbtnMessage.Location = new Point(85, 24);
        rbtnMessage.Name = "rbtnMessage";
        rbtnMessage.Size = new Size(85, 23);
        rbtnMessage.TabIndex = 1;
        rbtnMessage.TabStop = true;
        rbtnMessage.Text = "Nachricht";
        rbtnMessage.UseVisualStyleBackColor = true;
        // 
        // rbtnSound
        // 
        rbtnSound.AutoSize = true;
        rbtnSound.Location = new Point(6, 24);
        rbtnSound.Name = "rbtnSound";
        rbtnSound.Size = new Size(66, 23);
        rbtnSound.TabIndex = 0;
        rbtnSound.Text = "Sound";
        rbtnSound.UseVisualStyleBackColor = true;
        // 
        // ckbHibernate
        // 
        ckbHibernate.AutoSize = true;
        ckbHibernate.Enabled = false;
        ckbHibernate.Location = new Point(6, 53);
        ckbHibernate.Name = "ckbHibernate";
        ckbHibernate.Size = new Size(284, 23);
        ckbHibernate.TabIndex = 7;
        ckbHibernate.Text = "Ruhezustand statt Herunterfahren wählen";
        ckbHibernate.UseVisualStyleBackColor = true;
        // 
        // ckbAskBefore
        // 
        ckbAskBefore.AutoSize = true;
        ckbAskBefore.Enabled = false;
        ckbAskBefore.Location = new Point(6, 24);
        ckbAskBefore.Name = "ckbAskBefore";
        ckbAskBefore.Size = new Size(294, 23);
        ckbAskBefore.TabIndex = 6;
        ckbAskBefore.Text = "Vor Ausführung fragen/Abbrechen zulassen";
        ckbAskBefore.UseVisualStyleBackColor = true;
        // 
        // btnOK
        // 
        btnOK.DialogResult = DialogResult.OK;
        btnOK.Location = new Point(103, 326);
        btnOK.Name = "btnOK";
        btnOK.Size = new Size(105, 27);
        btnOK.TabIndex = 3;
        btnOK.Text = "Speichern";
        btnOK.UseVisualStyleBackColor = true;
        btnOK.Click += BtnOK_Click;
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(214, 326);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(107, 27);
        btnCancel.TabIndex = 4;
        btnCancel.Text = "Abbrechen";
        btnCancel.UseVisualStyleBackColor = true;
        // 
        // gbOption
        // 
        gbOption.Controls.Add(ckbHibernate);
        gbOption.Controls.Add(ckbAskBefore);
        gbOption.Location = new Point(12, 238);
        gbOption.Name = "gbOption";
        gbOption.Size = new Size(309, 82);
        gbOption.TabIndex = 5;
        gbOption.TabStop = false;
        gbOption.Text = "Optionen";
        // 
        // btnNow
        // 
        btnNow.Location = new Point(12, 326);
        btnNow.Name = "btnNow";
        btnNow.Size = new Size(60, 27);
        btnNow.TabIndex = 6;
        btnNow.Text = "Jetzt";
        btnNow.UseVisualStyleBackColor = true;
        btnNow.Click += BtnNow_Click;
        // 
        // openFileDialog
        // 
        openFileDialog.DefaultExt = "exe";
        openFileDialog.Filter = "Programme (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
        // 
        // AlarmEditor
        // 
        AcceptButton = btnOK;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(332, 365);
        Controls.Add(btnNow);
        Controls.Add(gbOption);
        Controls.Add(btnCancel);
        Controls.Add(btnOK);
        Controls.Add(gbAction);
        Controls.Add(gbTime);
        Controls.Add(gbName);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AlarmEditor";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "AlarmEditor";
        FormClosing += AlarmEditor_FormClosing;
        gbName.ResumeLayout(false);
        gbName.PerformLayout();
        gbTime.ResumeLayout(false);
        gbAction.ResumeLayout(false);
        gbAction.PerformLayout();
        gbOption.ResumeLayout(false);
        gbOption.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox gbName;
    private TextBox tbName;
    private GroupBox gbTime;
    private DateTimePicker dtpTime;
    private ComboBox cbRepeat;
    private DateTimePicker dtpDate;
    private GroupBox gbAction;
    private Button btnProgram;
    private TextBox tbProgram;
    private RadioButton rbtnProgram;
    private RadioButton rbtnShutdown;
    private RadioButton rbtnMessage;
    private RadioButton rbtnSound;
    private Button btnOK;
    private Button btnCancel;
    private CheckBox ckbAskBefore;
    private CheckBox ckbHibernate;
    private GroupBox gbOption;
    private Button btnNow;
    private OpenFileDialog openFileDialog;
}