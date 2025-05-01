namespace AlarmClock;
public partial class MessageForm : Form
{
    public bool CheckBoxChecked() => checkBox.Checked;
    public Point MessageLocation() => Location;
    public Font HeadingFont() => lblHeading.Font;


    public MessageForm(Point? location, string heading, string text, string repeat, bool check)
    {
        InitializeComponent();
        if (location != null) { Location = location.Value; } else { StartPosition = FormStartPosition.CenterScreen; }
        lblHeading.Text = heading;
        lblText.Text = text;
        checkBox.Checked = check;
        if (!string.IsNullOrEmpty(repeat))
        {
            pictureBox.Image = Properties.Resources.Clock_32x;
            if (!repeat.Equals(Utilities.repeatList[0])) { checkBox.Visible = true; }
        }
        button.Focus();
    }

    private void Button_Click(object sender, EventArgs e) => Close();

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData is Keys.Enter or Keys.Escape)
        {
            Close();
            return true; // Mark the key as handled
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
