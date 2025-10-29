namespace AlarmClock;

partial class MessageForm
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
        panel = new Panel();
        lblText = new Label();
        lblHeading = new Label();
        pictureBox = new PictureBox();
        button = new Button();
        checkBox = new CheckBox();
        panel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
        SuspendLayout();
        // 
        // panel
        // 
        panel.BackColor = SystemColors.ControlLightLight;
        panel.Controls.Add(lblText);
        panel.Controls.Add(lblHeading);
        panel.Controls.Add(pictureBox);
        panel.Dock = DockStyle.Top;
        panel.Location = new Point(0, 0);
        panel.Margin = new Padding(0);
        panel.Name = "panel";
        panel.Size = new Size(334, 73);
        panel.TabIndex = 0;
        // 
        // lblText
        // 
        lblText.AutoSize = true;
        lblText.Location = new Point(59, 42);
        lblText.Name = "lblText";
        lblText.Size = new Size(148, 19);
        lblText.TabIndex = 2;
        lblText.Text = "Die Zeit ist abgelaufen.";
        // 
        // lblHeading
        // 
        lblHeading.AutoSize = true;
        lblHeading.Font = new Font("Segoe UI", 12F);
        lblHeading.ForeColor = Color.Navy;
        lblHeading.Location = new Point(59, 12);
        lblHeading.Name = "lblHeading";
        lblHeading.Size = new Size(87, 21);
        lblHeading.TabIndex = 1;
        lblHeading.Text = "Erinnerung";
        // 
        // pictureBox
        // 
        pictureBox.Image = Properties.Resources.Reminder_32x;
        pictureBox.Location = new Point(12, 12);
        pictureBox.Name = "pictureBox";
        pictureBox.Size = new Size(32, 32);
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        // 
        // button
        // 
        button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        button.DialogResult = DialogResult.OK;
        button.Location = new Point(238, 80);
        button.Name = "button";
        button.Size = new Size(84, 28);
        button.TabIndex = 1;
        button.Text = "OK";
        button.UseVisualStyleBackColor = true;
        button.Click += Button_Click;
        // 
        // checkBox
        // 
        checkBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        checkBox.AutoSize = true;
        checkBox.Location = new Point(12, 84);
        checkBox.Name = "checkBox";
        checkBox.Size = new Size(142, 23);
        checkBox.TabIndex = 2;
        checkBox.Text = "Alarm wiederholen";
        checkBox.UseVisualStyleBackColor = true;
        checkBox.Visible = false;
        // 
        // MessageForm
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        ClientSize = new Size(334, 120);
        Controls.Add(checkBox);
        Controls.Add(button);
        Controls.Add(panel);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(350, 150);
        Name = "MessageForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "AlarmClock";
        panel.ResumeLayout(false);
        panel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel panel;
    private Button button;
    private Label lblHeading;
    private PictureBox pictureBox;
    private Label lblText;
    private CheckBox checkBox;
}