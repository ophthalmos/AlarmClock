namespace AlarmClock;
public partial class AlarmEditor : Form
{
    public Reminder? GetReminder() => currReminder;

    private readonly Reminder currReminder;
    private readonly List<string> existingNames = [];

    public AlarmEditor(Reminder? reminder, List<string> listNames)
    {
        InitializeComponent();
        existingNames = listNames;
        cbRepeat.Items.AddRange([.. Utilities.repeatList]);

        if (reminder != null)
        {
            tbName.Text = reminder.Name;
            dtpTime.Value = dtpDate.Value = reminder.Datetime;
            cbRepeat.SelectedIndex = reminder.Repeat;
            SetCheckRadioButton(reminder.Action);
            tbProgram.Text = reminder.ProgPath;
            ckbAskBefore.Checked = reminder.AskBefore;
            ckbHibernate.Checked = reminder.Hibernate;
            currReminder = reminder;
        }
        else
        {
            currReminder = new Reminder(true, "", "", 0, 1, "", false, false);
            dtpDate.Value = DateTime.Now;
            cbRepeat.SelectedIndex = 0;
        }
    }

    private void BtnOK_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(tbName.Text))
        {
            DialogResult = DialogResult.Continue; // AlarmEditor_FormClosing, prvent closing form   
            Utilities.ErrorMsgTaskDlg(Handle, "Unvollstandige Eingabe", "Bitte vergeben Sie einen Namen.");
            tbName.Focus();
            return;
        }
        if (existingNames.Contains(tbName.Text.Trim()))
        {
            DialogResult = DialogResult.Continue; // AlarmEditor_FormClosing, prvent closing form   
            Utilities.ErrorMsgTaskDlg(Handle, "Alarm existiert bereits", "Bitte vergeben Sie einen anderen Namen.");
            tbName.Focus();
            return;
        }
        currReminder.Name = tbName.Text.Trim();
        currReminder.Datetime = dtpDate.Value.Date.Add(dtpTime.Value.TimeOfDay);
        currReminder.Repeat = cbRepeat.SelectedIndex;
        currReminder.Action = GetSelectedRadioButton();
        currReminder.ProgPath = tbProgram.Text;
        currReminder.AskBefore = currReminder.Action >= 2 && ckbAskBefore.Checked; // nicht bei Sound (0) und Nachricht (1)
        currReminder.Hibernate = currReminder.Action == 2 && ckbHibernate.Checked; // nur bei Herunterfahren (2)    
    }

    private int GetSelectedRadioButton()
    {
        foreach (Control control in gbAction.Controls)
        {
            if (control is RadioButton radioButton && radioButton.Checked)
            {
                return Utilities.actionList.IndexOf(radioButton.Text.TrimEnd(':'));
            }
        }
        return -1;
    }

    private void SetCheckRadioButton(int action)
    {
        action = Math.Clamp(action, 0, Utilities.actionList.Count - 1);
        foreach (Control control in gbAction.Controls)
        {
            if (control is RadioButton radioButton && radioButton.Text.StartsWith(Utilities.actionList[action])) // wg. Programm: Doppelpunkt!
            {
                radioButton.Checked = true;
                break;
            }
        }
    }

    private void RbtnProgram_CheckedChanged(object sender, EventArgs e)
    {
        tbProgram.Enabled = btnProgram.Enabled = rbtnProgram.Checked;
        ckbAskBefore.Enabled = rbtnShutdown.Checked || rbtnProgram.Checked;
    }

    private void RbtnShutdown_CheckedChanged(object sender, EventArgs e)
    {
        ckbAskBefore.Enabled = rbtnShutdown.Checked || rbtnProgram.Checked;
        ckbHibernate.Enabled = rbtnShutdown.Checked;
    }

    private void AlarmEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.Continue) { e.Cancel = true; }
    }

    private void BtnNow_Click(object sender, EventArgs e)
    {
        dtpDate.Value = dtpTime.Value = DateTime.Now;
        dtpTime.Focus();
    }

    private void BtnProgram_Click(object sender, EventArgs e)
    {
        openFileDialog.InitialDirectory = !string.IsNullOrEmpty(tbProgram.Text) ? Path.GetDirectoryName(tbProgram.Text) : null;
        openFileDialog.CheckFileExists = true;
        if (openFileDialog.ShowDialog() == DialogResult.OK) { tbProgram.Text = openFileDialog.FileName; }
    }
}

