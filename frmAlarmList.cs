namespace AlarmClock;
public partial class AlarmList : Form
{

    public List<Reminder> GetReminders() => reminders;
    public int GetSelectedIndex() => listView.SelectedIndices.Count > 0 ? listView.SelectedIndices[0] : -1;
    public ListView GetListView() => listView;

    private readonly List<Reminder> reminders;
    private bool checkLVDoubleClick = false;
    private bool ignoreCheckChange = false;
    private readonly string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlarmClock.xml"); // Path to the XML file    

    public AlarmList(List<Reminder> listReminder, int listSelection, string xmlFile)
    {
        InitializeComponent();
        for (var i = 0; i < listReminder.Count; i++)
        {
            var reminder = listReminder[i];
            var item = listView.Items[i];
            item.Checked = (reminder.Repeat != 0 || reminder.Datetime >= DateTime.Now) && reminder.Check; // Update the checked state of the item
            item.SubItems[0].Text = reminder.Name;
            item.SubItems[1].Text = reminder.Datetime.ToString("HH:mm:ss");
            item.SubItems[2].Text = Utilities.repeatList[reminder.Repeat] ?? Utilities.repeatList[0];
            listView.Items[i] = item; // Update the ListView with the modified item   
        }
        listView.InsertionMark.Color = SystemColors.HotTrack;
        if (listSelection >= 0 && listSelection < Utilities.GetIndexOfFirstEmptyRow(listView)) { listView.Items[listSelection].Selected = true; }
        reminders = listReminder;
        xmlPath = xmlFile;
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        var row = Utilities.GetIndexOfFirstEmptyRow(listView);
        if (row == -1)
        {
            Utilities.ErrorMsgTaskDlg(Handle, "Ups!", "Die Liste ist voll. Bitte löschen Sie einen Eintrag, bevor Sie einen neuen hinzufügen.");
            return;
        }
        using var frm = new AlarmEditor(null, [.. reminders.Select(reminder => reminder.Name)]);
        if (frm.ShowDialog() == DialogResult.OK)
        {
            var reminder = frm.GetReminder();
            if (reminder == null)
            {
                Utilities.ErrorMsgTaskDlg(Handle, "Ups!", "Fehler beim Erstellen des Erinnerungsobjekts.");
                return;
            }
            reminders.Add(reminder);
            var item = listView.Items[row];
            item.Checked = (reminder.Repeat != 0 || reminder.Datetime >= DateTime.Now) && reminder.Check;
            item.SubItems[0].Text = reminder.Name;
            item.SubItems[1].Text = reminder.Datetime.ToString("HH:mm:ss");
            item.SubItems[2].Text = Utilities.repeatList[reminder.Repeat] ?? Utilities.repeatList[0];
            listView.Items[row] = item; // Update the ListView with the modified item   
            listView.Items[row].Selected = true; // Select the newly added item 
        }
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        var rowIndex = ListViewSelectedIndex();
        if (rowIndex == -1) { return; }
        using var frm = new AlarmEditor(reminders[rowIndex], [.. reminders.Where((reminder, index) => index != rowIndex).Select(reminder => reminder.Name)]);
        if (frm.ShowDialog() == DialogResult.OK)
        {
            var reminder = frm.GetReminder();
            if (reminder == null)
            {
                Utilities.ErrorMsgTaskDlg(Handle, "Ups!", "Fehler beim Erstellen des Erinnerungsobjekts.");
                return;
            }
            reminders[rowIndex] = reminder;
            var item = listView.SelectedItems[0];
            item.Checked = (reminder.Repeat != 0 || reminder.Datetime >= DateTime.Now) && reminder.Check;
            item.SubItems[0].Text = reminder.Name;
            item.SubItems[1].Text = reminder.Datetime.ToString("HH:mm:ss");
            item.SubItems[2].Text = Utilities.repeatList[reminder.Repeat]; // ?? Utilities.repeatList[0];
            listView.Items[rowIndex] = item; // Update the ListView with the modified item     
        }
    }

    private void BtnDel_Click(object sender, EventArgs e)
    {
        var rowIndex = ListViewSelectedIndex();
        if (rowIndex == -1) { return; }
        if (Utilities.TaskDialogYesNo(Handle, "Löschen", listView.Items[rowIndex].SubItems[0].Text, "Möchten Sie den Eintrag wirklich löschen?"))
        {
            listView.Items.RemoveAt(rowIndex);
            ignoreCheckChange = true; // Ignore the ItemChecked event 
            listView.Items.Add(new ListViewItem(["", "", ""]) { Checked = false }); // Hauptinhalt und zwei SubItems
            reminders.RemoveAt(rowIndex);
        }
    }

    private void BtnTest_Click(object sender, EventArgs e)
    {
        var rowIndex = ListViewSelectedIndex();
        if (rowIndex == -1) { return; }
        var rm = reminders[rowIndex];
        if (rm.Action == 2 && !rm.AskBefore) { Utilities.ErrorMsgTaskDlg(Handle, "Herunterfahren ohne Nachfrage", "Die Aufgabe wird nicht ausgeführt."); }
        else { Utilities.ShowReminder(this, null, reminders[rowIndex], rowIndex); }
    }

    private void BtnProp_Click(object sender, EventArgs e)
    {
        var rowIndex = ListViewSelectedIndex();
        if (rowIndex == -1) { return; }
        var rm = reminders[rowIndex];
        var footnote = rm.Repeat == 0 && rm.Datetime < DateTime.Now ? "Der Termin liegt in der Vergangenheit!" : "";
        var execution = rm.Repeat == 0 ? rm.Datetime :
            rm.Repeat == 1 ? Utilities.GetNextDate_1(rm) :
            rm.Repeat == 2 ? Utilities.GetNextDate_2(rm) :
            rm.Repeat == 3 ? Utilities.GetNextDate_3(rm) :
            rm.Repeat == 4 ? Utilities.GetNextDate_4(rm) :
            rm.Repeat == 5 ? Utilities.GetNextDate_5(rm) :
            rm.Repeat == 6 ? (DateTime?)Utilities.GetNextDate_6(rm) :
            null;
        if (footnote.Length > 0)
        {
            var item = listView.SelectedItems[0];
            item.Checked = false;
        }
        var page = new TaskDialogPage
        {
            Caption = Application.ProductName,
            Heading = rm.Name,
            Text = "Uhrzeit: " + rm.Datetime.ToLongTimeString() + "\n" +
            "Datum: " + rm.Datetime.ToLongDateString() + "\n" +
            "Wiederholung: " + Utilities.repeatList[rm.Repeat] + "\n" +
            "Aktion: " + Utilities.actionList[rm.Action] + "\n" +
            (string.IsNullOrWhiteSpace(rm.ProgPath) ? "" : "Programm: " + rm.ProgPath + "\n") +
            "Optionen:" + "\n" +
             (rm.AskBefore ? "• Vorabfrage\n" : "") +
             (rm.Hibernate ? "• Ruhezustand\n" : "") +
             (execution != null ? "\n" + "Nächste Ausführung am " + execution.Value.ToString("dddd d.M.yy") + " um " + execution.Value.ToLongTimeString() + " Uhr." : ""),
            Footnote = footnote,
            AllowCancel = true,
            SizeToContent = true
        };
        TaskDialog.ShowDialog(Handle, page);
    }

    private void BtnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void AlarmList_Shown(object sender, EventArgs e)
    {
        BringToFront();
        Activate();
    }

    private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (!string.IsNullOrEmpty(listView.SelectedItems[0].SubItems[0].Text)) { BtnEdit_Click(sender, e); }
        else
        {
            listView.SelectedItems[0].Checked = false;
            listView.Items[Utilities.GetIndexOfFirstEmptyRow(listView)].Selected = true;
            BtnAdd_Click(sender, e);
        }
    }

    private void ListView_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (checkLVDoubleClick)
        {
            e.NewValue = e.CurrentValue;
            checkLVDoubleClick = false;
        }
    }

    private void ListView_MouseDown(object sender, MouseEventArgs e)
    {
        if ((e.Button == MouseButtons.Left) && (e.Clicks > 1)) { checkLVDoubleClick = true; }
    }

    private void ListView_KeyDown(object sender, KeyEventArgs e) => checkLVDoubleClick = false;

    private int ListViewSelectedIndex()
    {
        if (listView.SelectedItems.Count == 0 || string.IsNullOrEmpty(listView.SelectedItems[0].SubItems[0].Text))
        {
            Utilities.ErrorMsgTaskDlg(Handle, "Ups!", "Bitte wählen Sie einen Eintrag aus.");
            return -1;
        }
        return listView.Items.IndexOf(listView.SelectedItems[0]);
    }

    private void AlarmList_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Utilities.HelpMsgTaskDlg(Handle, Icon!);
        listView.Focus();
    }

    private void AlarmList_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
        hlpevent.Handled = true;
        Utilities.HelpMsgTaskDlg(Handle, Icon!);
        listView.Focus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Add | Keys.Control:
            case Keys.Oemplus | Keys.Control:
                BtnAdd_Click(null!, null!);
                return true;
            case Keys.F2:
                BtnEdit_Click(null!, null!);
                return true;
            case Keys.Enter | Keys.Control:
                BtnTest_Click(null!, null!);
                return true;
            case Keys.Enter:
                if (listView.Focused && listView.SelectedItems.Count > 0)
                {
                    BtnEdit_Click(null!, null!);
                    return true;
                }
                return false;
            case Keys.Subtract | Keys.Control:
            case Keys.OemMinus | Keys.Control:
            case Keys.Delete | Keys.Control:
                BtnDel_Click(null!, null!);
                return true;
            case Keys.Enter | Keys.Alt:
                BtnProp_Click(null!, null!);
                return true;
            case Keys.F2 | Keys.Control | Keys.Shift: { Utilities.StartFile(xmlPath); return true; }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void AlarmList_FormClosing(object sender, FormClosingEventArgs e) => DialogResult = DialogResult.OK;

    private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        if (!ignoreCheckChange) { reminders[e.Item.Index].Check = e.Item.Checked; }
        ignoreCheckChange = false; // Reset the flag to allow future check changes  
    }

    private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Item is ListViewItem item && !string.IsNullOrWhiteSpace(item.Text)) { listView.DoDragDrop(item, DragDropEffects.Move); }
    }

    private void ListView_DragEnter(object sender, DragEventArgs e) => e.Effect = e.AllowedEffect;

    private void ListView_DragOver(object sender, DragEventArgs e)
    {
        //Point targetPoint = listView.PointToClient(new Point(e.X, e.Y)); // Retrieve the client coordinates of the mouse pointer.
        //int targetIndex = listView.InsertionMark.NearestIndex(targetPoint);  // Retrieve the index of the item closest to the mouse pointer.
        //if (targetIndex > -1) // Confirm that the mouse pointer is not over the dragged item.
        //{
        //    Rectangle itemBounds = listView.GetItemRect(targetIndex);
        //    if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2)) { listView.InsertionMark.AppearsAfterItem = true; }
        //    else { listView.InsertionMark.AppearsAfterItem = false; }
        //}
        //listView.InsertionMark.Index = targetIndex; // If the mouse is over the dragged item, the targetIndex value is -1 and the insertion mark disappears.

        if (e.Data?.GetDataPresent(typeof(ListViewItem)) == true) // Added null check for e.Data
        {
            e.Effect = DragDropEffects.Move;
            var point = listView.PointToClient(new Point(e.X, e.Y));  // Ermitteln der Einfügeposition
            var targetIndex = listView.InsertionMark.NearestIndex(point);
            var targetItem = listView.Items[targetIndex]; // Ermitteln des Ziel-Items
            //var targetItem = listView.GetItemAt(point.X, point.Y);
            if (targetItem != null)
            {
                if (!string.IsNullOrWhiteSpace(targetItem.Text)) // Leere Zeilen dürfen nicht übersprungen werden
                {
                    listView.InsertionMark.Index = targetItem.Index;
                    listView.InsertionMark.AppearsAfterItem = point.Y > (targetItem.Bounds.Top + targetItem.Bounds.Height / 2);
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    listView.InsertionMark.Index = -1; // Kein Indikator anzeigen
                }
            }
            //else
            //{
            //    //e.Effect = DragDropEffects.None;
            //    listView.InsertionMark.Index = -1; 
            //}
        }
        //else
        //{
        //    e.Effect = DragDropEffects.None;
        //    listView.InsertionMark.Index = -1;
        //}
    }

    private void ListView_DragLeave(object sender, EventArgs e) => listView.InsertionMark.Index = -1; // Removes the insertion mark when the mouse leaves the control.

    private void ListView_DragDrop(object sender, DragEventArgs e)
    {
        //var point = listView.PointToClient(new Point(e.X, e.Y));
        //var targetItem = listView.GetItemAt(point.X, point.Y);

        var targetIndex = listView.InsertionMark.Index;
        //if (targetIndex == -1) { return; }  // If the insertion mark is not visible, exit the method.
        //if (listView.InsertionMark.AppearsAfterItem) { targetIndex++; } // If the insertion mark is to the right of the item with the corresponding index, increment the target index.
        var targetItem = listView.Items[targetIndex];
        if (targetItem != null && !string.IsNullOrWhiteSpace(targetItem.Text))
        {
            if (e.Data?.GetData(typeof(ListViewItem)) is ListViewItem draggedItem) // Null check added  
            {
                //var targetIndex = targetItem.Index;
                if (draggedItem == targetItem)
                {
                    Console.Beep();
                    listView.InsertionMark.Index = -1;
                    return;
                }
                var movedElement = reminders[draggedItem.Index];
                reminders.RemoveAt(draggedItem.Index);
                reminders.Insert(targetIndex, movedElement);
                listView.Items.Remove(draggedItem);
                listView.Items.Insert(targetIndex, draggedItem);
            }
        }
    }

}
