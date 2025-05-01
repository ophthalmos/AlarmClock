namespace AlarmClock;

partial class AlarmList
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
        var listViewItem1 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem2 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem3 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem4 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem5 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem6 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem7 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem8 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem9 = new ListViewItem(new string[] { "", "", "", "" }, -1);
        var listViewItem10 = new ListViewItem(new string[] { "", "", "" }, -1);
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmList));
        listView = new ListView();
        chName = new ColumnHeader();
        chTime = new ColumnHeader();
        chExec = new ColumnHeader();
        btnAdd = new Button();
        btnEdit = new Button();
        btnDel = new Button();
        btnTest = new Button();
        btnProp = new Button();
        btnClose = new Button();
        SuspendLayout();
        // 
        // listView
        // 
        listView.AllowDrop = true;
        listView.CheckBoxes = true;
        listView.Columns.AddRange(new ColumnHeader[] { chName, chTime, chExec });
        listView.Font = new Font("Segoe UI", 10F);
        listView.FullRowSelect = true;
        listView.GridLines = true;
        listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        listViewItem1.StateImageIndex = 0;
        listViewItem2.StateImageIndex = 0;
        listViewItem3.StateImageIndex = 0;
        listViewItem4.StateImageIndex = 0;
        listViewItem5.StateImageIndex = 0;
        listViewItem6.StateImageIndex = 0;
        listViewItem7.StateImageIndex = 0;
        listViewItem8.StateImageIndex = 0;
        listViewItem9.StateImageIndex = 0;
        listViewItem10.StateImageIndex = 0;
        listView.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3, listViewItem4, listViewItem5, listViewItem6, listViewItem7, listViewItem8, listViewItem9, listViewItem10 });
        listView.Location = new Point(12, 12);
        listView.MultiSelect = false;
        listView.Name = "listView";
        listView.Size = new Size(325, 238);
        listView.TabIndex = 0;
        listView.UseCompatibleStateImageBehavior = false;
        listView.View = View.Details;
        listView.ItemCheck += ListView_ItemCheck;
        listView.ItemChecked += ListView_ItemChecked;
        listView.ItemDrag += ListView_ItemDrag;
        listView.DragDrop += ListView_DragDrop;
        listView.DragEnter += ListView_DragEnter;
        listView.DragOver += ListView_DragOver;
        listView.DragLeave += ListView_DragLeave;
        listView.KeyDown += ListView_KeyDown;
        listView.MouseDoubleClick += ListView_MouseDoubleClick;
        listView.MouseDown += ListView_MouseDown;
        // 
        // chName
        // 
        chName.Text = "Name";
        chName.Width = 160;
        // 
        // chTime
        // 
        chTime.Text = "Uhrzeit";
        chTime.Width = 67;
        // 
        // chExec
        // 
        chExec.Text = "Ausführung";
        chExec.Width = 93;
        // 
        // btnAdd
        // 
        btnAdd.Image = Properties.Resources.Add_16x;
        btnAdd.ImageAlign = ContentAlignment.MiddleLeft;
        btnAdd.Location = new Point(343, 12);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(105, 32);
        btnAdd.TabIndex = 1;
        btnAdd.Text = "Hinzufügen";
        btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += BtnAdd_Click;
        // 
        // btnEdit
        // 
        btnEdit.Image = Properties.Resources.Edit_16x;
        btnEdit.ImageAlign = ContentAlignment.MiddleLeft;
        btnEdit.Location = new Point(343, 50);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(105, 32);
        btnEdit.TabIndex = 2;
        btnEdit.Text = "Bearbeiten";
        btnEdit.TextAlign = ContentAlignment.MiddleRight;
        btnEdit.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += BtnEdit_Click;
        // 
        // btnDel
        // 
        btnDel.Image = Properties.Resources.Remove_16x;
        btnDel.ImageAlign = ContentAlignment.MiddleLeft;
        btnDel.Location = new Point(343, 88);
        btnDel.Name = "btnDel";
        btnDel.Size = new Size(105, 32);
        btnDel.TabIndex = 3;
        btnDel.Text = "Löschen";
        btnDel.TextAlign = ContentAlignment.MiddleRight;
        btnDel.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnDel.UseVisualStyleBackColor = true;
        btnDel.Click += BtnDel_Click;
        // 
        // btnTest
        // 
        btnTest.Image = Properties.Resources.Run_16x;
        btnTest.ImageAlign = ContentAlignment.MiddleLeft;
        btnTest.Location = new Point(343, 126);
        btnTest.Name = "btnTest";
        btnTest.Size = new Size(105, 32);
        btnTest.TabIndex = 4;
        btnTest.Text = "Testen";
        btnTest.TextAlign = ContentAlignment.MiddleRight;
        btnTest.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnTest.UseVisualStyleBackColor = true;
        btnTest.Click += BtnTest_Click;
        // 
        // btnProp
        // 
        btnProp.Image = Properties.Resources.Info_16x;
        btnProp.ImageAlign = ContentAlignment.MiddleLeft;
        btnProp.Location = new Point(343, 164);
        btnProp.Name = "btnProp";
        btnProp.Size = new Size(105, 32);
        btnProp.TabIndex = 5;
        btnProp.Text = "Information";
        btnProp.TextAlign = ContentAlignment.MiddleRight;
        btnProp.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnProp.UseVisualStyleBackColor = true;
        btnProp.Click += BtnProp_Click;
        // 
        // btnClose
        // 
        btnClose.Location = new Point(343, 218);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(105, 32);
        btnClose.TabIndex = 6;
        btnClose.Text = "Schließen";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += BtnClose_Click;
        // 
        // AlarmList
        // 
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnClose;
        ClientSize = new Size(460, 262);
        Controls.Add(btnClose);
        Controls.Add(btnProp);
        Controls.Add(btnTest);
        Controls.Add(btnDel);
        Controls.Add(btnEdit);
        Controls.Add(btnAdd);
        Controls.Add(listView);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        HelpButton = true;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AlarmList";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Alarm-Liste";
        HelpButtonClicked += AlarmList_HelpButtonClicked;
        FormClosing += AlarmList_FormClosing;
        Shown += AlarmList_Shown;
        HelpRequested += AlarmList_HelpRequested;
        ResumeLayout(false);
    }

    #endregion

    private ListView listView;
    private ColumnHeader chName;
    private ColumnHeader chTime;
    private ColumnHeader chExec;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDel;
    private Button btnTest;
    private Button btnProp;
    private Button btnClose;
}