﻿
namespace BooruDatasetTagManager
{
    partial class Form_settings
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
            label1 = new System.Windows.Forms.Label();
            comboBox1 = new System.Windows.Forms.ComboBox();
            LabelPreviewImageSize = new System.Windows.Forms.Label();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            BtnSave = new System.Windows.Forms.Button();
            BtnCancel = new System.Windows.Forms.Button();
            AutoSortCheckBox = new System.Windows.Forms.CheckBox();
            CheckAskChange = new System.Windows.Forms.CheckBox();
            textBox2 = new System.Windows.Forms.TextBox();
            LabelSeparatorSave = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            LabelSeparatorLoad = new System.Windows.Forms.Label();
            comboAutocompSort = new System.Windows.Forms.ComboBox();
            LabelAutocompSort = new System.Windows.Forms.Label();
            comboAutocompMode = new System.Windows.Forms.ComboBox();
            LabelAutocompMode = new System.Windows.Forms.Label();
            LabelChars = new System.Windows.Forms.Label();
            LabelAutocompAfter = new System.Windows.Forms.Label();
            numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            LabelLanguage = new System.Windows.Forms.Label();
            comboBoxLanguage = new System.Windows.Forms.ComboBox();
            BtnAutocompFontChange = new System.Windows.Forms.Button();
            numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            label14 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            BtnGridviewFontChange = new System.Windows.Forms.Button();
            LabelTagHeight = new System.Windows.Forms.Label();
            LabelAutocompFont = new System.Windows.Forms.Label();
            LabelTagFont = new System.Windows.Forms.Label();
            checkBox1 = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            comboBox2 = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            dataGridViewHotkeys = new System.Windows.Forms.DataGridView();
            CmdId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Hotkey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SettingFrame = new Manina.Windows.Forms.TabControl();
            tabGeneral = new Manina.Windows.Forms.Tab();
            tabUI = new Manina.Windows.Forms.Tab();
            label4 = new System.Windows.Forms.Label();
            comboBoxColorScheme = new System.Windows.Forms.ComboBox();
            tabTranslations = new Manina.Windows.Forms.Tab();
            tabHotkeys = new Manina.Windows.Forms.Tab();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewHotkeys).BeginInit();
            SettingFrame.SuspendLayout();
            tabGeneral.SuspendLayout();
            tabUI.SuspendLayout();
            tabTranslations.SuspendLayout();
            tabHotkeys.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 16);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 15);
            label1.TabIndex = 0;
            label1.Text = "Translation language";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(141, 13);
            comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(263, 23);
            comboBox1.TabIndex = 1;
            // 
            // LabelPreviewImageSize
            // 
            LabelPreviewImageSize.AutoSize = true;
            LabelPreviewImageSize.Location = new System.Drawing.Point(10, 10);
            LabelPreviewImageSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelPreviewImageSize.Name = "LabelPreviewImageSize";
            LabelPreviewImageSize.Size = new System.Drawing.Size(106, 15);
            LabelPreviewImageSize.TabIndex = 2;
            LabelPreviewImageSize.Text = "Preview image size";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(140, 11);
            numericUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown1.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(140, 23);
            numericUpDown1.TabIndex = 3;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // BtnSave
            // 
            BtnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnSave.Location = new System.Drawing.Point(7, 445);
            BtnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new System.Drawing.Size(88, 27);
            BtnSave.TabIndex = 4;
            BtnSave.Text = "Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BtnCancel.Location = new System.Drawing.Point(101, 445);
            BtnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(88, 27);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // AutoSortCheckBox
            // 
            AutoSortCheckBox.AutoSize = true;
            AutoSortCheckBox.Location = new System.Drawing.Point(15, 248);
            AutoSortCheckBox.Name = "AutoSortCheckBox";
            AutoSortCheckBox.Size = new System.Drawing.Size(215, 19);
            AutoSortCheckBox.TabIndex = 10;
            AutoSortCheckBox.Text = "Auto sort when changing selections";
            AutoSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // CheckAskChange
            // 
            CheckAskChange.AutoSize = true;
            CheckAskChange.Location = new System.Drawing.Point(15, 218);
            CheckAskChange.Name = "CheckAskChange";
            CheckAskChange.Size = new System.Drawing.Size(288, 19);
            CheckAskChange.TabIndex = 9;
            CheckAskChange.Text = "Prompt to save changes when changing selection";
            CheckAskChange.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(140, 135);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(100, 23);
            textBox2.TabIndex = 8;
            // 
            // LabelSeparatorSave
            // 
            LabelSeparatorSave.AutoSize = true;
            LabelSeparatorSave.Location = new System.Drawing.Point(10, 138);
            LabelSeparatorSave.Name = "LabelSeparatorSave";
            LabelSeparatorSave.Size = new System.Drawing.Size(100, 15);
            LabelSeparatorSave.TabIndex = 7;
            LabelSeparatorSave.Text = "Separator on save";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(140, 103);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(100, 23);
            textBox1.TabIndex = 8;
            // 
            // LabelSeparatorLoad
            // 
            LabelSeparatorLoad.AutoSize = true;
            LabelSeparatorLoad.Location = new System.Drawing.Point(10, 106);
            LabelSeparatorLoad.Name = "LabelSeparatorLoad";
            LabelSeparatorLoad.Size = new System.Drawing.Size(100, 15);
            LabelSeparatorLoad.TabIndex = 7;
            LabelSeparatorLoad.Text = "Separator on load";
            // 
            // comboAutocompSort
            // 
            comboAutocompSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboAutocompSort.FormattingEnabled = true;
            comboAutocompSort.Location = new System.Drawing.Point(140, 71);
            comboAutocompSort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboAutocompSort.Name = "comboAutocompSort";
            comboAutocompSort.Size = new System.Drawing.Size(272, 23);
            comboAutocompSort.TabIndex = 5;
            // 
            // LabelAutocompSort
            // 
            LabelAutocompSort.AutoSize = true;
            LabelAutocompSort.Location = new System.Drawing.Point(10, 74);
            LabelAutocompSort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelAutocompSort.Name = "LabelAutocompSort";
            LabelAutocompSort.Size = new System.Drawing.Size(106, 15);
            LabelAutocompSort.TabIndex = 4;
            LabelAutocompSort.Text = "Autocomplete sort";
            // 
            // comboAutocompMode
            // 
            comboAutocompMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboAutocompMode.FormattingEnabled = true;
            comboAutocompMode.Location = new System.Drawing.Point(140, 40);
            comboAutocompMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboAutocompMode.Name = "comboAutocompMode";
            comboAutocompMode.Size = new System.Drawing.Size(272, 23);
            comboAutocompMode.TabIndex = 5;
            // 
            // LabelAutocompMode
            // 
            LabelAutocompMode.AutoSize = true;
            LabelAutocompMode.Location = new System.Drawing.Point(10, 43);
            LabelAutocompMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelAutocompMode.Name = "LabelAutocompMode";
            LabelAutocompMode.Size = new System.Drawing.Size(117, 15);
            LabelAutocompMode.TabIndex = 4;
            LabelAutocompMode.Text = "Autocomplete mode";
            // 
            // LabelChars
            // 
            LabelChars.AutoSize = true;
            LabelChars.Location = new System.Drawing.Point(228, 165);
            LabelChars.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelChars.Name = "LabelChars";
            LabelChars.Size = new System.Drawing.Size(35, 15);
            LabelChars.TabIndex = 2;
            LabelChars.Text = "chars";
            // 
            // LabelAutocompAfter
            // 
            LabelAutocompAfter.AutoSize = true;
            LabelAutocompAfter.Location = new System.Drawing.Point(10, 166);
            LabelAutocompAfter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelAutocompAfter.Name = "LabelAutocompAfter";
            LabelAutocompAfter.Size = new System.Drawing.Size(140, 15);
            LabelAutocompAfter.TabIndex = 2;
            LabelAutocompAfter.Text = "Show autocomplete after";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new System.Drawing.Point(172, 163);
            numericUpDown2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown2.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new System.Drawing.Size(48, 23);
            numericUpDown2.TabIndex = 3;
            numericUpDown2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // LabelLanguage
            // 
            LabelLanguage.AutoSize = true;
            LabelLanguage.Location = new System.Drawing.Point(9, 117);
            LabelLanguage.Name = "LabelLanguage";
            LabelLanguage.Size = new System.Drawing.Size(59, 15);
            LabelLanguage.TabIndex = 6;
            LabelLanguage.Text = "Language";
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Items.AddRange(new object[] { "zh-CN", "en-US" });
            comboBoxLanguage.Location = new System.Drawing.Point(185, 113);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new System.Drawing.Size(121, 23);
            comboBoxLanguage.TabIndex = 5;
            // 
            // BtnAutocompFontChange
            // 
            BtnAutocompFontChange.Location = new System.Drawing.Point(362, 81);
            BtnAutocompFontChange.Name = "BtnAutocompFontChange";
            BtnAutocompFontChange.Size = new System.Drawing.Size(75, 23);
            BtnAutocompFontChange.TabIndex = 4;
            BtnAutocompFontChange.Text = "Change...";
            BtnAutocompFontChange.UseVisualStyleBackColor = true;
            BtnAutocompFontChange.Click += BtnAutocompFontChange_Click;
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new System.Drawing.Point(185, 47);
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new System.Drawing.Size(62, 23);
            numericUpDown3.TabIndex = 3;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(185, 81);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(12, 15);
            label14.TabIndex = 2;
            label14.Text = "-";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(185, 17);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(12, 15);
            label11.TabIndex = 2;
            label11.Text = "-";
            // 
            // BtnGridviewFontChange
            // 
            BtnGridviewFontChange.Location = new System.Drawing.Point(362, 17);
            BtnGridviewFontChange.Name = "BtnGridviewFontChange";
            BtnGridviewFontChange.Size = new System.Drawing.Size(75, 23);
            BtnGridviewFontChange.TabIndex = 1;
            BtnGridviewFontChange.Text = "Change...";
            BtnGridviewFontChange.UseVisualStyleBackColor = true;
            BtnGridviewFontChange.Click += BtnGridviewFontChange_Click;
            // 
            // LabelTagHeight
            // 
            LabelTagHeight.AutoSize = true;
            LabelTagHeight.Location = new System.Drawing.Point(9, 49);
            LabelTagHeight.Name = "LabelTagHeight";
            LabelTagHeight.Size = new System.Drawing.Size(138, 15);
            LabelTagHeight.TabIndex = 0;
            LabelTagHeight.Text = "Tags gridview row height";
            // 
            // LabelAutocompFont
            // 
            LabelAutocompFont.AutoSize = true;
            LabelAutocompFont.Location = new System.Drawing.Point(9, 81);
            LabelAutocompFont.Name = "LabelAutocompFont";
            LabelAutocompFont.Size = new System.Drawing.Size(108, 15);
            LabelAutocompFont.TabIndex = 0;
            LabelAutocompFont.Text = "Autocomplete font";
            // 
            // LabelTagFont
            // 
            LabelTagFont.AutoSize = true;
            LabelTagFont.Location = new System.Drawing.Point(9, 17);
            LabelTagFont.Name = "LabelTagFont";
            LabelTagFont.Size = new System.Drawing.Size(103, 15);
            LabelTagFont.TabIndex = 0;
            LabelTagFont.Text = "Tags gridview font";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(13, 75);
            checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(286, 19);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Load only manual translations into autocomplete";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 47);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(103, 15);
            label3.TabIndex = 0;
            label3.Text = "Translation service";
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new System.Drawing.Point(141, 44);
            comboBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new System.Drawing.Size(263, 23);
            comboBox2.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            label2.Location = new System.Drawing.Point(0, -15);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(286, 15);
            label2.TabIndex = 1;
            label2.Text = "Select row with shortcut and press shortcut you need";
            // 
            // dataGridViewHotkeys
            // 
            dataGridViewHotkeys.AllowUserToAddRows = false;
            dataGridViewHotkeys.AllowUserToDeleteRows = false;
            dataGridViewHotkeys.AllowUserToResizeColumns = false;
            dataGridViewHotkeys.AllowUserToResizeRows = false;
            dataGridViewHotkeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewHotkeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { CmdId, Command, Hotkey });
            dataGridViewHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewHotkeys.Location = new System.Drawing.Point(0, 0);
            dataGridViewHotkeys.MultiSelect = false;
            dataGridViewHotkeys.Name = "dataGridViewHotkeys";
            dataGridViewHotkeys.ReadOnly = true;
            dataGridViewHotkeys.RowHeadersVisible = false;
            dataGridViewHotkeys.RowTemplate.Height = 25;
            dataGridViewHotkeys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewHotkeys.Size = new System.Drawing.Size(0, 0);
            dataGridViewHotkeys.TabIndex = 0;
            dataGridViewHotkeys.KeyDown += dataGridView1_KeyDown;
            // 
            // CmdId
            // 
            CmdId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            CmdId.HeaderText = "Id";
            CmdId.Name = "CmdId";
            CmdId.ReadOnly = true;
            CmdId.Visible = false;
            // 
            // Command
            // 
            Command.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Command.HeaderText = "Command";
            Command.Name = "Command";
            Command.ReadOnly = true;
            // 
            // Hotkey
            // 
            Hotkey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Hotkey.HeaderText = "Hotkey";
            Hotkey.Name = "Hotkey";
            Hotkey.ReadOnly = true;
            // 
            // SettingFrame
            // 
            SettingFrame.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SettingFrame.Controls.Add(tabGeneral);
            SettingFrame.Controls.Add(tabUI);
            SettingFrame.Controls.Add(tabTranslations);
            SettingFrame.Controls.Add(tabHotkeys);
            SettingFrame.Location = new System.Drawing.Point(2, 1);
            SettingFrame.Name = "SettingFrame";
            SettingFrame.SelectedIndex = 0;
            SettingFrame.Size = new System.Drawing.Size(565, 427);
            SettingFrame.TabIndex = 6;
            SettingFrame.Tabs.Add(tabGeneral);
            SettingFrame.Tabs.Add(tabUI);
            SettingFrame.Tabs.Add(tabTranslations);
            SettingFrame.Tabs.Add(tabHotkeys);
            // 
            // tabGeneral
            // 
            tabGeneral.Controls.Add(AutoSortCheckBox);
            tabGeneral.Controls.Add(LabelPreviewImageSize);
            tabGeneral.Controls.Add(CheckAskChange);
            tabGeneral.Controls.Add(numericUpDown1);
            tabGeneral.Controls.Add(textBox2);
            tabGeneral.Controls.Add(numericUpDown2);
            tabGeneral.Controls.Add(LabelSeparatorSave);
            tabGeneral.Controls.Add(LabelAutocompAfter);
            tabGeneral.Controls.Add(textBox1);
            tabGeneral.Controls.Add(LabelChars);
            tabGeneral.Controls.Add(LabelSeparatorLoad);
            tabGeneral.Controls.Add(LabelAutocompMode);
            tabGeneral.Controls.Add(comboAutocompSort);
            tabGeneral.Controls.Add(comboAutocompMode);
            tabGeneral.Controls.Add(LabelAutocompSort);
            tabGeneral.Location = new System.Drawing.Point(1, 23);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Size = new System.Drawing.Size(563, 403);
            tabGeneral.Text = "General";
            // 
            // tabUI
            // 
            tabUI.Controls.Add(label4);
            tabUI.Controls.Add(LabelLanguage);
            tabUI.Controls.Add(LabelTagFont);
            tabUI.Controls.Add(comboBoxColorScheme);
            tabUI.Controls.Add(comboBoxLanguage);
            tabUI.Controls.Add(LabelAutocompFont);
            tabUI.Controls.Add(BtnAutocompFontChange);
            tabUI.Controls.Add(LabelTagHeight);
            tabUI.Controls.Add(numericUpDown3);
            tabUI.Controls.Add(BtnGridviewFontChange);
            tabUI.Controls.Add(label14);
            tabUI.Controls.Add(label11);
            tabUI.Location = new System.Drawing.Point(0, 0);
            tabUI.Name = "tabUI";
            tabUI.Size = new System.Drawing.Size(0, 0);
            tabUI.Text = "UI";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(9, 146);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(80, 15);
            label4.TabIndex = 6;
            label4.Text = "Color scheme";
            // 
            // comboBoxColorScheme
            // 
            comboBoxColorScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxColorScheme.FormattingEnabled = true;
            comboBoxColorScheme.Location = new System.Drawing.Point(185, 142);
            comboBoxColorScheme.Name = "comboBoxColorScheme";
            comboBoxColorScheme.Size = new System.Drawing.Size(121, 23);
            comboBoxColorScheme.TabIndex = 5;
            // 
            // tabTranslations
            // 
            tabTranslations.Controls.Add(checkBox1);
            tabTranslations.Controls.Add(label1);
            tabTranslations.Controls.Add(label3);
            tabTranslations.Controls.Add(comboBox1);
            tabTranslations.Controls.Add(comboBox2);
            tabTranslations.Location = new System.Drawing.Point(0, 0);
            tabTranslations.Name = "tabTranslations";
            tabTranslations.Size = new System.Drawing.Size(0, 0);
            tabTranslations.Text = "Translations";
            // 
            // tabHotkeys
            // 
            tabHotkeys.Controls.Add(dataGridViewHotkeys);
            tabHotkeys.Controls.Add(label2);
            tabHotkeys.Location = new System.Drawing.Point(0, 0);
            tabHotkeys.Name = "tabHotkeys";
            tabHotkeys.Size = new System.Drawing.Size(0, 0);
            tabHotkeys.Text = "Hotkeys";
            // 
            // Form_settings
            // 
            AcceptButton = BtnSave;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = BtnCancel;
            ClientSize = new System.Drawing.Size(570, 479);
            Controls.Add(SettingFrame);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSave);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form_settings";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Settings";
            Load += Form_settings_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewHotkeys).EndInit();
            SettingFrame.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            tabGeneral.PerformLayout();
            tabUI.ResumeLayout(false);
            tabUI.PerformLayout();
            tabTranslations.ResumeLayout(false);
            tabTranslations.PerformLayout();
            tabHotkeys.ResumeLayout(false);
            tabHotkeys.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label LabelPreviewImageSize;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboAutocompMode;
        private System.Windows.Forms.Label LabelAutocompMode;
        private System.Windows.Forms.ComboBox comboAutocompSort;
        private System.Windows.Forms.Label LabelAutocompSort;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label LabelSeparatorSave;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label LabelSeparatorLoad;
        private System.Windows.Forms.Label LabelChars;
        private System.Windows.Forms.Label LabelAutocompAfter;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox CheckAskChange;
        private System.Windows.Forms.Label LabelTagFont;
        private System.Windows.Forms.Button BtnGridviewFontChange;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label LabelTagHeight;
        private System.Windows.Forms.Button BtnAutocompFontChange;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label LabelAutocompFont;
        private System.Windows.Forms.CheckBox AutoSortCheckBox;
        private System.Windows.Forms.Label LabelLanguage;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.DataGridView dataGridViewHotkeys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn CmdId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hotkey;
        private Manina.Windows.Forms.TabControl SettingFrame;
        private Manina.Windows.Forms.Tab tabGeneral;
        private Manina.Windows.Forms.Tab tabUI;
        private Manina.Windows.Forms.Tab tabTranslations;
        private Manina.Windows.Forms.Tab tabHotkeys;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxColorScheme;
    }
}