﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translator;
using static BooruDatasetTagManager.DatasetManager;

namespace BooruDatasetTagManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            tagsBuffer = new List<string>();
            previewPicBox = new PictureBox();
            previewPicBox.Name = "previewPicBox";
            allTagsFilter = new Form_filter();
            switchLanguage();
            InitHotkeyCommands();

            //test color scheme
            //Program.ColorManager.SelectScheme("Dark");
            Program.ColorManager.ChangeColorScheme(this, Program.ColorManager.SelectedScheme);
            Program.ColorManager.ChangeColorSchemeInConteiner(Controls, Program.ColorManager.SelectedScheme);
            Program.ColorManager.SchemeChanded += ColorManager_SchemeChanded;
        }

        private Form_filter allTagsFilter;
        List<string> tagsBuffer;

        private bool isAllTags = true;
        private bool isTranslate = false;
        private bool isFiltered = false;
        private bool showCount = false;

        private Form_preview fPreview;
        private bool isShowPreview = false;
        private PictureBox previewPicBox;
        private int previewRowIndex = -1;
        private FilterType filterAnd = FilterType.Or;
        private bool isLoading = false;
        private List<string> selectedFiles = new List<string>();

        private bool isCtrlOrShiftPressed = false;
        private bool needReloadTags = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " " + Application.ProductVersion;
            gridViewDS.RowTemplate.Height = Program.Settings.PreviewSize + 10;
            gridViewAllTags.RowTemplate.Height = Program.Settings.GridViewRowHeight;
            gridViewTags.RowTemplate.Height = Program.Settings.GridViewRowHeight;
            gridViewTags.DefaultCellStyle.Font = Program.Settings.GridViewFont.GetFont();
            gridViewAllTags.DefaultCellStyle.Font = Program.Settings.GridViewFont.GetFont();
            gridViewDS.DefaultCellStyle.Font = Program.Settings.GridViewFont.GetFont();
            //splitContainer2.SplitterDistance = Width / 3;
            toolStrippromptFixedLengthComboBox.SelectedIndex = 0;
#if !DEBUG
            Extensions.CheckForUpdateAsync(Application.ProductVersion);
#endif
        }

        private void ColorManager_SchemeChanded(object sender, EventArgs e)
        {
            if (Program.ColorManager.SelectedScheme != null)
            {
                Program.ColorManager.ChangeColorScheme(this, Program.ColorManager.SelectedScheme);
                Program.ColorManager.ChangeColorSchemeInConteiner(Controls, Program.ColorManager.SelectedScheme);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.DataManager != null && Program.DataManager.IsDataSetChanged())
            {
                DialogResult result = MessageBox.Show("The dataset has been changed,\ndo you want to save the changes?", "Saving changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Program.DataManager.SaveAll();
                }
            }
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() != DialogResult.OK)
                return;
            isLoading = true;
            Program.DataManager = new DatasetManager();
            if (!Program.DataManager.LoadFromFolder(openFolderDialog.Folder))
            {
                SetStatus(I18n.GetText("TipFolderWrong"));
                return;
            }

            gridViewDS.DataSource = Program.DataManager.GetDataSource();
            Program.DataManager.UpdateData();
            BindTagList();
            ApplyDataSetGridStyle();
            isLoading = false;
            gridViewDS.AutoResizeColumns();
        }

        private async Task FillTranslation(DataGridView grid)
        {
            if (grid.Columns.Contains("Translation") && grid.Columns["Translation"].Visible == false)
            {
                grid.Columns["Translation"].Visible = true;
            }
            LockEdit(true);
            SetStatus(I18n.GetText("StatusTranslating"));
            try
            {
                for (int i = 0; i < grid.RowCount; i++)
                {
                    SetStatus($"{I18n.GetText("SettingTabTranslations")} {i}/{grid.RowCount}");
                    grid["Translation", i].ReadOnly = true;
                    if (string.IsNullOrWhiteSpace((string)grid["Translation", i].Value) && !string.IsNullOrWhiteSpace((string)grid["ImageTags", i].Value))
                        grid["Translation", i].Value = await Program.TransManager.TranslateAsync((string)grid["ImageTags", i].Value);
                }
            }
            catch (Exception ex)
            {

            }
            SetStatus(I18n.GetText("StatusTranslationComplete"));
            LockEdit(false);
        }

        private void LockEdit(bool locked)
        {
            toolStripTags.Enabled = !locked;
            toolStripAllTags.Enabled = !locked;
            gridViewTags.Enabled = !locked;
            if (gridViewTags.SelectedRows.Count == 1)
                gridViewTags.AllowDrop = !locked;
            gridViewAllTags.Enabled = !locked;
            gridViewDS.Enabled = !locked;
            gridViewAutoTags.Enabled = !locked;
            toolStripAutoTags.Enabled = !locked;
        }

        private void ShowPreview(string img)
        {
            if (fPreview == null || fPreview.IsDisposed)
                fPreview = new Form_preview();
            fPreview.Show(img);
        }

        private void HidePreview()
        {
            fPreview?.Hide();
        }

        private async void LoadSelectedImageToGrid()
        {
            gridViewTags.AutoGenerateColumns = false;
            if (gridViewDS.SelectedRows.Count == 0)
                return;
            if (gridViewDS.SelectedRows.Count == 1)
            {
                gridViewTags.Tag = (string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value;
                ChageImageColumn(false);
                gridViewTags.DataSource = Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value].Tags;
                if (isShowPreview)
                {
                    ShowPreview((string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value);
                }

            }
            else
            {
                if (isShowPreview)
                {
                    HidePreview();
                }
                gridViewTags.DataSource = null;
                gridViewTags.AllowDrop = false;
                gridViewTags.Rows.Clear();
                ChageImageColumn(true);

                gridViewTags.Tag = "0";
                List<DataItem> selectedTagsList = new List<DataItem>();
                for (int i = 0; i < gridViewDS.SelectedRows.Count; i++)
                {
                    selectedTagsList.Add(Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[i].Cells["ImageFilePath"].Value]);
                }

                MultiSelectDataTable multiSelectData = new MultiSelectDataTable();
                multiSelectData.SetTranslationMode(isTranslate);
                await multiSelectData.CreateTableFromSelectedImages(selectedTagsList);
                gridViewTags.DataSource = multiSelectData;
            }

            if (Program.Settings.AutoSort)
            {
                SortPrompt();
            }

            gridViewDS.Focus();
            if (isTranslate)
            {
                var dsType = GetTagsDataSourceType();
                if (dsType == DataSourceType.Single)
                {
                    await ((EditableTagList)gridViewTags.DataSource).TranslateAllAsync();
                }
            }
            //await FillTranslation(gridViewTags);
            if (showCount)
                UpdateTagCount();
        }

        /// <summary>
        /// Add or remove Image column
        /// </summary>
        /// <param name="add"> true to add, false to remove</param>
        private void ChageImageColumn(bool add)
        {
            gridViewTags.Columns["ImageName"].Visible = add;
        }

        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = gridViewTags.DoDragDrop(
                    gridViewTags.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = gridViewTags.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = gridViewTags.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                gridViewTags.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                if (rowIndexFromMouseDown != rowIndexOfItemUnderMouseToDrop)
                {
                    int destIndex = ((EditableTagList)gridViewTags.DataSource).Move(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
                    gridViewTags.ClearSelection();
                    gridViewTags[0, destIndex].Selected = true;
                }
            }
        }

        private void BtnAddTag_Clicked(object sender, EventArgs e)
        {
            AddNewRow();
        }

        private void AddNewRow()
        {
            if (gridViewTags.DataSource == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            if (gridViewDS.SelectedRows.Count > 1)
            {
                using (Form_addTag addTag = new Form_addTag())
                {
                    if (gridViewTags.SelectedCells.Count > 0)
                    {
                        addTag.tagTextBox.Text = ((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[gridViewTags.SelectedCells[0].RowIndex]).GetTagText();
                        addTag.tagTextBox.SelectAll();
                    }
                    if (addTag.ShowDialog() == DialogResult.OK)
                    {
                        AddingType addType = (AddingType)Enum.Parse(typeof(AddingType), (string)addTag.comboBox1.SelectedItem);
                        int customIndex = (int)addTag.numericUpDown1.Value;
                        bool skipExist = addTag.checkBoxSkipExist.Checked;
                        AddTagMultiselectedMode(addTag.tagTextBox.Text, skipExist, addType, customIndex);
                    }
                    addTag.Close();
                }
            }
            else
            {
                if (gridViewTags.SelectedCells.Count == 0 || gridViewTags.RowCount == 0)
                {
                    ((EditableTagList)gridViewTags.DataSource).AddNew();
                    SetDGVSelection(gridViewTags, gridViewTags.RowCount - 1, "ImageTags");
                }
                else
                {
                    int index = GetFirstDGVSelectionIndex(gridViewTags);
                    ((EditableTagList)gridViewTags.DataSource).InsertNew(gridViewTags.SelectedCells[0].RowIndex + 1);
                    SetDGVSelection(gridViewTags, index + 1, "ImageTags");
                }
            }
        }

        private void SetDGVSelection(DataGridView dgv, int index, string column)
        {
            dgv.ClearSelection();
            dgv[column, index].Selected = true;
        }

        private int GetFirstDGVSelectionIndex(DataGridView dgv)
        {
            if (dgv.SelectedCells.Count == 0)
                return -1;
            return dgv.SelectedCells[0].RowIndex;
        }

        private void BtnTagDelete_Click(object sender, EventArgs e)
        {
            if (gridViewTags.SelectedCells.Count == 0)
                return;
            gridViewTags.Rows.RemoveAt(gridViewTags.SelectedCells[0].RowIndex);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (gridViewTags.SelectedCells.Count == 0 || gridViewTags.SelectedCells[0].RowIndex == 0)
                return;
            int curIndex = gridViewTags.SelectedCells[0].RowIndex;
            int destIndex = ((EditableTagList)gridViewTags.DataSource).Move(curIndex, curIndex - 1);
            gridViewTags.ClearSelection();
            gridViewTags["ImageTags", destIndex].Selected = true;
            if (showCount)
                UpdateTagCount();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (gridViewTags.SelectedCells.Count == 0 || gridViewTags.SelectedCells[0].RowIndex == gridViewTags.RowCount - 1)
                return;
            int curIndex = gridViewTags.SelectedCells[0].RowIndex;
            int destIndex = ((EditableTagList)gridViewTags.DataSource).Move(curIndex, curIndex + 1);
            gridViewTags["ImageTags", destIndex].Selected = true;
            if (showCount)
                UpdateTagCount();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            isAllTags = !isAllTags;
            if (isAllTags)
                toolStripLabelAllTags.Text = I18n.GetText("UILabelAllTags");
            else
                toolStripLabelAllTags.Text = I18n.GetText("UILabelCommonTags");
            BindTagList();
        }

        private void BindTagList()
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            if (isAllTags)
            {
                BingSourceToDGV(gridViewAllTags, Program.DataManager.AllTags);
            }
            else
            {
                BingSourceToDGV(gridViewAllTags, Program.DataManager.CommonTags);
            }
            gridViewAllTags.Columns["ImageTags"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private async void BingSourceToDGV(DataGridView dgv, List<TagValue> source)
        {
            var scroll = dgv.FirstDisplayedScrollingRowIndex;
            var all = GetSelectedTags();
            dgv.Rows.Clear();
            if (dgv.Columns.Count == 0)
                dgv.Columns.Add("ImageTags", "ImageTags");
            foreach (var item in source)
            {
                int row = dgv.Rows.Add(item.Tag);
                if (row == 0)
                    dgv.Rows[row].Selected = false;
                if (all.Contains(item.Tag))
                    dgv.Rows[row].Selected = true;
            }

            if (scroll >= dgv.RowCount)
            {
                scroll = dgv.Rows.Count - 1;
            }
            if (scroll != -1)
            {
                dgv.FirstDisplayedScrollingRowIndex = scroll;
            }
            if (isTranslate)
            {
                await FillTranslation(dgv);
            }

            if (showCount)
                UpdateTagCount();
        }

        private void BtnAddTagForAll_Click(object sender, EventArgs e)
        {
            AddTagToAll(false);
        }

        private async void AddTagToAll(bool filtered)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            Form_addTag addTag = new Form_addTag();
            int index = gridViewAllTags.RowCount;
            if (gridViewAllTags.SelectedCells.Count > 0)
            {
                index = gridViewAllTags.SelectedCells[0].RowIndex;
                addTag.tagTextBox.Text = (string)gridViewAllTags.Rows[index].Cells[0].Value;
                addTag.tagTextBox.SelectAll();
            }
            if (addTag.ShowDialog() == DialogResult.OK)
            {
                int customIndex = (int)addTag.numericUpDown1.Value;
                bool skipExist = addTag.checkBoxSkipExist.Checked;
                DatasetManager.AddingType addType = (DatasetManager.AddingType)Enum.Parse(typeof(DatasetManager.AddingType), (string)addTag.comboBox1.SelectedItem);
                Program.DataManager.AddTagToAll(addTag.tagTextBox.Text, skipExist, addType, customIndex, filtered);
                Program.DataManager.UpdateData();
                if (gridViewDS.SelectedRows.Count == 1)
                {
                    if (isTranslate)
                    {
                        await ((EditableTagList)gridViewTags.DataSource).TranslateAllAsync();
                    }

                    if (showCount)
                        UpdateTagCount();

                    var allIndex = IndexOfValueInGrig(gridViewAllTags, "ImageTags", addTag.tagTextBox.Text);
                    if (allIndex == -1)
                    {
                        gridViewAllTags.Rows.Insert(index, 1);
                        gridViewAllTags.Rows[index].Cells[0].Value = addTag.tagTextBox.Text;
                        if (isTranslate)
                        {
                            gridViewAllTags.Rows[index].Cells[1].Value = await Program.TransManager.TranslateAsync(addTag.tagTextBox.Text);
                        }

                        if (showCount)
                            UpdateTagCount();
                    }
                }
                else
                {
                    AddTagMultiselectedMode(addTag.tagTextBox.Text, skipExist, addType, customIndex);
                }
            }
            addTag.Close();
        }

        private int IndexOfValueInGrig(DataGridView gridView, string colName, string value)
        {
            for (int i = 0; i < gridView.RowCount; i++)
            {
                if (gridView[colName, i].Value != DBNull.Value)
                {
                    if ((string)gridView[colName, i].Value == value)
                        return i;
                }
                else if (value == null)
                    return i;
            }
            return -1;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count != 1)
            {
                MessageBox.Show("Replace does not support multiple selection. Choose one image.");
                return;
            }

            if (gridViewAllTags.SelectedCells.Count == 0)
                return;
            Form_replaceAll replaceAll = new Form_replaceAll();
            replaceAll.comboBox1.DataSource = Program.DataManager.AllTags;
            replaceAll.comboBox1.DisplayMember = "Tag";
            replaceAll.comboBox1.SelectedIndex = gridViewAllTags.SelectedCells[0].RowIndex;
            replaceAll.comboBox2.Items.AddRange(Program.DataManager.AllTags.Select(a => a.Tag).ToArray());
            if (replaceAll.ShowDialog() == DialogResult.OK)
            {
                Program.DataManager.ReplaceTagInAll(((TagValue)replaceAll.comboBox1.SelectedItem).Tag, (string)replaceAll.comboBox2.Text, true);
                Program.DataManager.UpdateData();
            }
            replaceAll.Close();
            BindTagList();
        }

        private void saveAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            Program.DataManager.SaveAll();
            Program.DataManager.UpdateDatasetHash();
            SetStatus(I18n.GetText("StatusSaved"));
        }

        private void showPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            isShowPreview = !isShowPreview;
            MenuShowPreview.Checked = isShowPreview;
            if (isShowPreview)
            {
                if (gridViewDS.SelectedRows.Count == 1)
                {
                    ShowPreview((string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value);
                }
                else
                {
                    HidePreview();
                }
            }
            else
            {
                HidePreview();
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count == 1)
            {
                tagsBuffer.Clear();
                for (int i = 0; i < gridViewTags.RowCount; i++)
                {
                    tagsBuffer.Add((string)gridViewTags["ImageTags", i].Value);
                }
                SetStatus(I18n.GetText("StatusCopied"));
            }
            else if (gridViewDS.SelectedRows.Count > 1)
            {
                MessageBox.Show(I18n.GetText("TipMultiImageCopy"));
            }
            else
            {
                MessageBox.Show(I18n.GetText("TipSelectImage"));
            }
        }

        private void SetStatus(string text)
        {
            statusLabel.Text = text;
        }

        private async void BtnPasteTag_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count == 1)
            {
                var eTagList = (EditableTagList)gridViewTags.DataSource;
                eTagList.Clear();
                eTagList.AddRange(tagsBuffer, true);
                if (isTranslate)
                    await FillTranslation(gridViewTags);
                if (showCount)
                    UpdateTagCount();
                SetStatus(I18n.GetText("StatusPasted"));
            }
            else if (gridViewDS.SelectedRows.Count > 1)
            {
                MessageBox.Show(I18n.GetText("TipMultiImagePaste"));
            }
            else
            {
                MessageBox.Show(I18n.GetText("TipSelectImage"));
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            ((EditableTagList)gridViewTags.DataSource).PrevState();
            //return;
            //LoadSelectedImageToGrid();
            //lastGridViewTagsHash = GetgridViewTagsHash();
        }

        private void BtnDeleteTagForAll_Click(object sender, EventArgs e)
        {
            RemoveTagFromAll(false);
        }

        private void RemoveTagFromAll(bool filtered)
        {

            List<KeyValuePair<int, string>> tagsToDel = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < gridViewAllTags.SelectedCells.Count; i++)
            {
                var row = gridViewAllTags.SelectedCells[i].RowIndex;
                tagsToDel.Add(new KeyValuePair<int, string>(row, (string)gridViewAllTags.Rows[row].Cells["ImageTags"].Value));
            }

            tagsToDel.Sort((a, b) => b.Key.CompareTo(a.Key));

            foreach (var item in tagsToDel)
            {
                Program.DataManager.DeleteTagFromAll(item.Value, filtered);
                if (gridViewDS.SelectedRows.Count > 1)
                    LoadSelectedImageToGrid();
                if (!Program.DataManager.AllTags.Exists(a => a.Tag == item.Value))
                    gridViewAllTags.Rows.RemoveAt(item.Key);
            }
            Program.DataManager.UpdateData();
        }

        private async void translateTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isTranslate = !isTranslate;
            MenuItemTranslateTags.Checked = isTranslate;

            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            if (isTranslate)
            {
                gridViewAllTags.Columns.Insert(1, new DataGridViewTextBoxColumn()
                {
                    Name = "Translation",
                    HeaderText = "Translation",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                gridViewTags.Columns["Translation"].Visible = true;
                await FillTranslation(gridViewAllTags);
                await FillTranslation(gridViewTags);
            }
            else
            {
                gridViewAllTags.Columns.Remove("Translation");
                gridViewTags.Columns["Translation"].Visible = false;
            }
        }

        //private int findIndex = -1;
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            SetFilter();
        }

        private HashSet<string> GetSelectedTags()
        {
            HashSet<string> findTags = new HashSet<string>();
            for (int i = 0; i < gridViewAllTags.SelectedCells.Count; i++)
            {
                int row = gridViewAllTags.SelectedCells[i].RowIndex;
                string value = (string)gridViewAllTags.Rows[row].Cells[0].Value;
                if (!findTags.Contains(value))
                    findTags.Add(value);
            }
            return findTags;
        }

        private void SaveSelectedInViewDs()
        {
            selectedFiles.Clear();
            for (int i = 0; i < gridViewDS.SelectedRows.Count; i++)
            {
                selectedFiles.Add((string)gridViewDS.SelectedRows[i].Cells["ImageFilePath"].Value);
            }
        }

        private void LoadSelectedInViewDs()
        {
            gridViewDS.ClearSelection();
            bool foundSelected = false;
            int firstDisplayed = 0;
            for (int i = 0; i < gridViewDS.RowCount; i++)
            {
                if (selectedFiles.Contains((string)gridViewDS["ImageFilePath", i].Value))
                {
                    if (firstDisplayed == 0)
                        firstDisplayed = i;
                    gridViewDS.Rows[i].Selected = true;
                    foundSelected = true;
                }
            }
            if (!foundSelected && gridViewDS.RowCount > 0)
            {
                gridViewDS.Rows[0].Selected = true;
            }
            // Will throw an exception by itself if there is nothing found due to being set to -1 internally when the list is loaded in empty. Lazy bypass of 
            try
            {
                gridViewDS.FirstDisplayedScrollingRowIndex = firstDisplayed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void SetFilter()
        {
            isLoading = true;
            if (gridViewAllTags.SelectedCells.Count > 0)
            {
                SaveSelectedInViewDs();
                if (isFiltered)
                {
                    ResetFilter();
                }

                gridViewDS.DataSource = Program.DataManager.GetDataSource(DatasetManager.OrderType.Name, filterAnd, GetSelectedTags());
                if (gridViewDS.RowCount == 0)
                    gridViewTags.Rows.Clear();
                isFiltered = true;
                LoadSelectedInViewDs();
                BtnImageExitFilter.Enabled = true;
            }
            isLoading = false;
        }

        private void ResetFilter()
        {
            isLoading = true;
            if (isFiltered)
            {
                SaveSelectedInViewDs();
                gridViewDS.DataSource = Program.DataManager.GetDataSource();
                isFiltered = false;
                BtnImageExitFilter.Enabled = false;
                LoadSelectedInViewDs();
            }
            isLoading = false;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnTagDelete.PerformClick();
            }
            else if (e.KeyCode == Keys.Insert)
            {
                BtnTagAdd.PerformClick();
            }
        }

        private void loadLossFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private async void toolStripButton15_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    var lines = PromptParser.ParsePrompt(text, Program.Settings.SeparatorOnLoad);
                    EditableTagList tagList = new EditableTagList(lines);
                    gridViewTags.DataSource = tagList;
                }
                //MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                //return;
            }
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                var lines = PromptParser.ParsePrompt(text, Program.Settings.SeparatorOnLoad);
                var tagsDSType = GetTagsDataSourceType();
                if (tagsDSType == DataSourceType.Single)
                {
                    EditableTagList etl = (EditableTagList)gridViewTags.DataSource;
                    etl.Clear();
                    etl.AddRange(lines, true);
                }
                if (isTranslate)
                    await FillTranslation(gridViewTags);

                if (showCount)
                    UpdateTagCount();
            }
        }

        private DataSourceType GetTagsDataSourceType()
        {
            if (gridViewTags.DataSource == null)
                return DataSourceType.None;
            else if (gridViewTags.DataSource.GetType() == typeof(EditableTagList))
                return DataSourceType.Single;
            else if (gridViewTags.DataSource.GetType() == typeof(MultiSelectDataTable))
                return DataSourceType.Multi;
            else
                throw new Exception("Unknown datasource type!");
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            var dts = GetTagsDataSourceType();
            if (dts == DataSourceType.Single)
            {
                Form_Edit fPrint = new Form_Edit();
                EditableTagList tagsDS = (EditableTagList)gridViewTags.DataSource;
                fPrint.textBox1.Text = tagsDS.ToString();
                fPrint.Show();
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (gridViewTags.CurrentCell.ColumnIndex == 0)
            {
                AutoCompleteTextBox autoText = e.Control as AutoCompleteTextBox;
                if (autoText != null)
                {
                    //autoText.SetParent(gridViewTags);
                    if (Program.Settings.AutocompleteMode != AutocompleteMode.Disable && autoText.Values == null)
                    {
                        autoText.SetAutocompleteMode(Program.Settings.AutocompleteMode, Program.Settings.AutocompleteSort);
                        autoText.Values = Program.TagsList.Tags;
                    }
                    //autoText.Location = new Point(10, 10);
                    //autoText.Size = new Size(25, 75);
                    //autoText.AutoCompleteMode = AutoCompleteMode.Suggest;
                    //autoText.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    //autoText.AutoCompleteCustomSource = Program.TagsList.Tags;
                }
            }
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count != 1)
            {
                MessageBox.Show("Select one image!");
                return;
            }
            if (GetTagsDataSourceType() != DataSourceType.Single)
            {
                SetStatus(I18n.GetText("TipMultiImagePaste"));
                return;
            }
            EditableTagList clonedTagList = (EditableTagList)((EditableTagList)gridViewTags.DataSource).Clone();
            switch (MessageBox.Show("Set tag list to empty images only?\nYes - only empty, No - to all images, Cancel - do nothing.", "Tag setting option", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    Program.DataManager.SetTagListToAll(clonedTagList, true);
                    break;
                case DialogResult.No:
                    Program.DataManager.SetTagListToAll(clonedTagList, false);
                    break;
                case DialogResult.Cancel:
                    return;
            }
            Program.DataManager.UpdateData();
            BindTagList();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.DataManager != null && Program.DataManager.IsDataSetChanged())
            {
                DialogResult result = MessageBox.Show("The dataset has been changed,\ndo you want to save the changes?", "Saving changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Program.DataManager.SaveAll();
                }
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;
            AddSelectedAllTagsToImageTags();
        }

        private void dataGridView3_DataSourceChanged(object sender, EventArgs e)
        {

        }

        private void ApplyDataSetGridStyle()
        {
            for (int i = 0; i < gridViewDS.ColumnCount; i++)
            {
                if (gridViewDS.Columns[i].ValueType == typeof(Image))
                {
                    ((DataGridViewImageColumn)gridViewDS.Columns[i]).ImageLayout = DataGridViewImageCellLayout.NotSet;
                    gridViewDS.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (gridViewDS.Columns[i].Name == "Loss" || gridViewDS.Columns[i].Name == "LastLoss")
                {
                    gridViewDS.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    gridViewDS.Columns[i].Visible = Program.DataManager.IsLossLoaded;
                }

            }
        }

        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            //Disabled because it causes a bug with shift or ctrl sticking
            //if (isCtrlOrShiftPressed)
            //{
            //    needReloadTags = true;
            //    return;
            //}
            needReloadTags = false;
            LoadSelectedImageToGrid();
        }


        private int GetgridViewTagsHash()
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < gridViewTags.RowCount; i++)
            {
                tags.Add((string)gridViewTags["ImageTags", i].Value);
            }
            return string.Join("|", tags).GetHashCode();
        }

        private void dataGridViewTags_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (gridViewTags.Columns["ImageName"].Visible)
                {
                    if (e.RowIndex != previewRowIndex)
                    {

                        //var dataItem = Program.DataManager.DataSet[(string)gridViewTags["Image", e.RowIndex].Value];
                        //var dataItem = (DataItem)gridViewTags["ImageTags", e.RowIndex].Tag;
                        var dataItem = (DataItem)((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[e.RowIndex]).ExtendedProperties["DataItem"];
                        previewPicBox.Size = new Size(Program.Settings.PreviewSize, Program.Settings.PreviewSize);
                        previewPicBox.Image = dataItem.Img;
                        previewPicBox.SizeMode = PictureBoxSizeMode.AutoSize;
                        previewPicBox.Location = new Point(splitContainer1.Panel2.Location.X + splitContainer2.Panel2.Location.X, PointToClient(Cursor.Position).Y);

                        if (!this.Controls.ContainsKey("previewPicBox"))
                        {
                            this.Controls.Add(previewPicBox);
                        }
                        previewPicBox.BringToFront();
                        previewRowIndex = e.RowIndex;
                    }
                }
                else
                {
                    if (this.Controls.ContainsKey("previewPicBox"))
                    {
                        this.Controls.RemoveByKey("previewPicBox");
                        previewRowIndex = -1;
                    }
                }
            }
        }

        private void dataGridViewTags_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (this.Controls.ContainsKey("previewPicBox"))
            {
                this.Controls.RemoveByKey("previewPicBox");
                previewRowIndex = -1;
            }
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            switch (filterAnd)
            {
                case FilterType.Not:
                    filterAnd = FilterType.Or;
                    BtnTagMultiModeSwitch.Image = Properties.Resources.ORIcon;
                    break;
                case FilterType.Or:
                    filterAnd = FilterType.Xor;
                    BtnTagMultiModeSwitch.Image = Properties.Resources.XORIcon;
                    break;
                case FilterType.Xor:
                    filterAnd = FilterType.And;
                    BtnTagMultiModeSwitch.Image = Properties.Resources.ANDIcon;
                    break;
                case FilterType.And:
                    filterAnd = FilterType.Not;
                    BtnTagMultiModeSwitch.Image = Properties.Resources.NOTIcon;
                    break;
                default:
                    throw new ArgumentException($"Invalid filter type: {filterAnd}");
            }
            SetFilter();
        }

        private void gridViewTags_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridViewTags.Columns["ImageTags"].Index == e.ColumnIndex && e.RowIndex != -1)
            {
                string editedValue = (string)gridViewTags[e.ColumnIndex, e.RowIndex].Value;
                if (gridViewDS.SelectedRows.Count == 1)
                {
                    for (int i = 0; i < gridViewTags.RowCount; i++)
                    {
                        if (i != e.RowIndex && (string)gridViewTags[e.ColumnIndex, i].Value == editedValue)
                        {
                            this.BeginInvoke(new MethodInvoker(() =>
                            {
                                gridViewTags.Rows.RemoveAt(e.RowIndex);
                            }));

                        }
                    }
                }
                else if (gridViewDS.SelectedRows.Count > 1)
                {
                    if (string.IsNullOrEmpty((string)gridViewTags["Image", e.RowIndex].Value))
                    {
                        MessageBox.Show("Image name must be filled!");
                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            gridViewTags.Rows.RemoveAt(e.RowIndex);
                        }));
                    }
                    else
                    {
                        //gridViewTags["Image", e.RowIndex].Tag = gridViewTags["ImageTags", e.RowIndex].Value;
                        //gridViewTags["Name", e.RowIndex].Tag = gridViewTags["ImageTags", e.RowIndex].Value;
                    }
                }
            }
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            AddSelectedAllTagsToImageTags();
        }

        private void AddTagSingleSelectedMode(string tag)
        {
            if (gridViewDS.SelectedRows.Count != 1)
            {
                SetStatus("The number of selected images is not equal to 1");
                return;
            }

            for (int i = 0; i < gridViewTags.RowCount; i++)
            {
                if ((string)gridViewTags["ImageTags", i].Value == tag)
                {
                    return;
                }
            }
            ((EditableTagList)gridViewTags.DataSource).AddTag(tag, true);
        }

        private async void AddTagMultiselectedMode(string tag, bool skipExist, AddingType addType, int pos = -1)
        {
            if (gridViewDS.SelectedRows.Count < 2)
            {
                SetStatus("The number of selected images must be greater than 1");
                return;
            }
            ((MultiSelectDataTable)gridViewTags.DataSource).AddTag(tag, skipExist, addType, pos);
        }

        private void RemoveTagFromImageTags(string tag)
        {
            if (gridViewDS.SelectedRows.Count == 0)
            {
                SetStatus("The number of selected images must be greater than 0");
                return;
            }

            for (int i = gridViewTags.RowCount - 1; i >= 0; i--)
            {
                if (gridViewDS.SelectedRows.Count == 1)
                {
                    if ((string)gridViewTags["ImageTags", i].Value == tag)
                    {
                        gridViewTags.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    var rowTag = (string)((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[i]).ExtendedProperties["TextTag"];
                    if (rowTag == tag)
                    {
                        gridViewTags.Rows.RemoveAt(i);
                    }
                }


            }
        }

        private List<string> GetSelectedTagsInAllTags()
        {
            List<string> selectedTags = new List<string>();
            for (int i = 0; i < gridViewAllTags.SelectedCells.Count; i++)
            {
                var row = gridViewAllTags.SelectedCells[i].RowIndex;
                var tag = (string)gridViewAllTags.Rows[row].Cells[0].Value;
                if (!selectedTags.Contains(tag))
                    selectedTags.Add(tag);
            }
            return selectedTags;
        }

        private async void AddSelectedAllTagsToImageTags()
        {
            if (gridViewAllTags.SelectedCells.Count == 0 || gridViewDS.SelectedRows.Count == 0)
            {
                SetStatus("Images or tags not selected!");
                return;
            }
            foreach (var item in GetSelectedTagsInAllTags())
            {
                if (gridViewDS.SelectedRows.Count == 1)
                    AddTagSingleSelectedMode(item);
                else
                    AddTagMultiselectedMode(item, true, AddingType.Down);
            }
            if (isTranslate)
                await FillTranslation(gridViewTags);

            if (showCount)
                UpdateTagCount();
        }

        private void RemoveSelectedAllTagsToImageTags()
        {
            if (gridViewAllTags.SelectedCells.Count == 0 || gridViewDS.SelectedRows.Count == 0)
            {
                SetStatus("Images or tags not selected!");
                return;
            }
            foreach (var item in GetSelectedTagsInAllTags())
            {
                RemoveTagFromImageTags(item);
            }
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            RemoveSelectedAllTagsToImageTags();
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            AddTagToAll(true);
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            RemoveTagFromAll(true);
        }

        private void gridViewDS_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.ColumnIndex != -1)
            {
                if (Enum.IsDefined(typeof(DatasetManager.OrderType), gridViewDS.Columns[e.ColumnIndex].Name))
                {
                    isLoading = true;
                    gridViewDS.DataSource = Program.DataManager.GetDataSourceWithLastFilter((DatasetManager.OrderType)Enum.Parse(typeof(DatasetManager.OrderType), gridViewDS.Columns[e.ColumnIndex].Name));
                    isLoading = false;
                }
            }
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            string searchedTag;
            if (gridViewDS.SelectedRows.Count == 1)
            {
                searchedTag = (string)gridViewTags["ImageTags", gridViewTags.CurrentCell.RowIndex].Value;
            }
            else if (gridViewDS.SelectedRows.Count > 1)
            {
                searchedTag = (string)gridViewTags.Rows[gridViewTags.CurrentCell.RowIndex].Tag;
            }
            else
                return;
            for (int i = 0; i < gridViewAllTags.RowCount; i++)
            {
                if (((string)gridViewAllTags[0, i].Value) == searchedTag)
                {
                    gridViewAllTags.ClearSelection();
                    gridViewAllTags.Rows[i].Selected = true;
                    if (i < gridViewAllTags.FirstDisplayedScrollingRowIndex || i > gridViewAllTags.FirstDisplayedScrollingRowIndex + gridViewAllTags.DisplayedRowCount(false))
                    {
                        gridViewAllTags.FirstDisplayedScrollingRowIndex = i;
                    }
                }
            }
        }



        private void gridViewTags_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gridViewDS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteImage();
            }
            if (e.Control || e.Shift)
            {
                isCtrlOrShiftPressed = true;
            }
        }

        private void DeleteImage()
        {
            if (gridViewDS.SelectedRows.Count < 1)
                return;
            if (MessageBox.Show(I18n.GetText("TipDeleteFile"), I18n.GetText("LabelDeleteFile"),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                gridViewTags.DataSource = null;

                var scroll = gridViewDS.FirstDisplayedScrollingRowIndex;
                var select = gridViewDS.SelectedRows[0].Index;
                var selects = new List<DataItem>();
                var list = gridViewDS.DataSource as List<DataItem>;
                foreach (DataGridViewRow item in gridViewDS.SelectedRows)
                {
                    selects.Add(list[item.Index]);
                    var file = (string)item.Cells["ImageFilePath"].Value;
                    var tagFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".txt");
                    try
                    {
                        File.Delete(file);
                        File.Delete(tagFile);
                        Program.DataManager.Remove(file);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                Program.DataManager.UpdateData();
                BindTagList();
                //gridViewDS.DataSource = Program.DataManager.GetDataSource();
                foreach (var item in selects)
                {
                    list.Remove(item);
                }
                gridViewDS.DataSource = null;
                gridViewDS.DataSource = list;
                if (gridViewDS.RowCount > 0)
                {
                    gridViewDS.FirstDisplayedScrollingRowIndex = scroll;
                    if (select >= gridViewDS.RowCount)
                    {
                        select = gridViewDS.RowCount - 1;
                    }
                    gridViewDS.ClearSelection();
                    gridViewDS.Rows[select].Selected = true;
                }
            }
        }

        private void gridViewDS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && gridViewDS.SelectedRows.Count > 0)
            {
                var file = (string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value;
                ShowPreview(file);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count > 0)
            {
                var file = (string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value;
                ExplorerFile(file);
            }
        }

        private void gridViewDS_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.RowIndex < gridViewDS.Rows.Count && e.Button == MouseButtons.Right)
            {
                gridViewDS.ClearSelection();
                gridViewDS.Rows[e.RowIndex].Selected = true;
                contextMenuStrip1.Show(MousePosition);
            }
        }

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        public static void ExplorerFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                IntPtr pidlList = ILCreateFromPathW(filePath);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                    }
                    catch { }
                    finally
                    {
                        ILFree(pidlList);
                    }
                }
                return;
            }

            if (Directory.Exists(filePath))
            {
                Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");
                return;
            }
            var dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir))
            {
                Process.Start(@"explorer.exe", "\"" + dir + "\"");
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DeleteImage();
        }

        private void gridView_Enter(object sender, EventArgs e)
        {
            if (sender is DataGridView grid)
                grid.BorderStyle = BorderStyle.FixedSingle;
        }

        private void gridView_Leave(object sender, EventArgs e)
        {
            if (sender is DataGridView grid)
                grid.BorderStyle = BorderStyle.Fixed3D;
        }

        private void ShowAllTagsFilter(bool show)
        {
            if (!show)
                toolStripTextBox1.TextChanged -= TextBox1_TextChanged;
            toolStripTextBox1.Clear();
            toolStripTextBox1.Visible = show;
            toolStripButton1.Visible = show;
            if (show)
            {
                toolStripTextBox1.Focus();
                toolStripTextBox1.TextChanged += TextBox1_TextChanged;
            }
            else
            {
                gridViewAllTags.Focus();
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Length > 0)
            {
                isLoading = true;
                int index = Program.DataManager.AllTags.FindIndex(a => a.Tag.StartsWith(toolStripTextBox1.Text));
                if (index != -1)
                {
                    //gridViewAllTags.ClearSelection();
                    //gridViewAllTags.Rows[index].Selected = true;
                    gridViewAllTags.CurrentCell = gridViewAllTags.Rows[index].Cells[0];
                    if (index < gridViewAllTags.FirstDisplayedScrollingRowIndex || index > gridViewAllTags.FirstDisplayedScrollingRowIndex + gridViewAllTags.DisplayedRowCount(false))
                    {
                        gridViewAllTags.FirstDisplayedScrollingRowIndex = index;
                    }
                }
                else
                {
                    toolStripTextBox1.Text = toolStripTextBox1.Text.Substring(0, toolStripTextBox1.Text.Length - 1);
                    toolStripTextBox1.SelectionStart = toolStripTextBox1.TextLength;
                }
                isLoading = false;
            }
        }

        private void gridViewAllTags_KeyPress(object sender, KeyPressEventArgs e)
        {
            ShowAllTagsFilter(true);
            toolStripTextBox1.Text = e.KeyChar.ToString();
            toolStripTextBox1.SelectionStart = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowAllTagsFilter(false);
        }

        private void gridViewAllTags_SelectionChanged(object sender, EventArgs e)
        {
            if (!isLoading)
                ShowAllTagsFilter(false);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int pos = -1;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                ShowAllTagsFilter(false);
                if (e.KeyCode == Keys.Down)
                    pos = 1;
                int index = gridViewAllTags.CurrentCell.RowIndex;
                gridViewAllTags.CurrentCell = gridViewAllTags.Rows[index + pos].Cells[0];
            }
        }

        private void gridViewDS_KeyUp(object sender, KeyEventArgs e)
        {
            if (isCtrlOrShiftPressed && !e.Control && !e.Shift)
            {
                isCtrlOrShiftPressed = false;
                if (needReloadTags)
                {
                    dataGridView3_SelectionChanged(sender, EventArgs.Empty);
                }
            }
        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            if (allTagsFilter == null || allTagsFilter.IsDisposed)
            {
                allTagsFilter = new Form_filter();
            }
            if (allTagsFilter.ShowDialog() != DialogResult.OK)
                return;
            if (isAllTags)
            {
                BingSourceToDGV(gridViewAllTags, Program.DataManager.GetFilteredAllTags(allTagsFilter.textBox1.Text));
            }
            //string filterText = 

        }

        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            BindTagList();
        }

        private void promptSortBtn_Click(object sender, EventArgs e)
        {
            SortPrompt();
        }

        private void SortPrompt()
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            var fixedLengthIndex = toolStrippromptFixedLengthComboBox.SelectedIndex;
            if (fixedLengthIndex == -1)
                return;
            if (GetTagsDataSourceType() == DataSourceType.Single)
            {
                EditableTagList eTagList = (EditableTagList)gridViewTags.DataSource;
                eTagList.Sort(fixedLengthIndex);
            }
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        public void switchLanguage()
        {
            I18n.Initialize(Program.Settings.Language);
            fileToolStripMenuItem.Text = I18n.GetText("MenuLabelFile");
            MenuSetting.Text = I18n.GetText("MenuLabelSettings");
            viewToolStripMenuItem.Text = I18n.GetText("MenuLabelView");
            toolStripLabelDataSet.Text = I18n.GetText("UILabelDataSet");
            toolStripLabelAllTags.Text = I18n.GetText("UILabelAllTags");
            toolStripLabelImageTags.Text = I18n.GetText("UILabelImageTags");
            toolStripPromptFixTipLabel.Text = I18n.GetText("UILabelFixPromptLength");
            openFolderToolStripMenuItem.Text = I18n.GetText("MenuItemLoadFolder");
            saveAllChangesToolStripMenuItem.Text = I18n.GetText("MenuItemSaveChanges");
            MenuShowPreview.Text = I18n.GetText("MenuItemShowPreview");
            MenuItemTranslateTags.Text = I18n.GetText("MenuItemTranslateTags");
            MenuHideAllTags.Text = I18n.GetText("MenuHideAllTags");
            MenuHideTags.Text = I18n.GetText("MenuHideTags");
            MenuHideDataset.Text = I18n.GetText("MenuHideDataset");

            BtnTagAddToAll.Text = I18n.GetText("BtnTagAddToAll");
            BtnTagAdd.Text = I18n.GetText("BtnTagAdd");
            BtnTagUndo.Text = I18n.GetText("BtnTagUndo");
            BtnTagRedo.Text = I18n.GetText("BtnTagRedo");
            BtnTagDelete.Text = I18n.GetText("BtnTagDelete");
            BtnTagCopy.Text = I18n.GetText("BtnTagCopy");
            BtnTagPaste.Text = I18n.GetText("BtnTagPaste");
            BtnTagSetToAll.Text = I18n.GetText("BtnTagSetToAll");
            BtnTagPasteFromClipBoard.Text = I18n.GetText("BtnTagPasteFromClipBoard");
            BtnTagShow.Text = I18n.GetText("BtnTagShow");
            BtnTagUp.Text = I18n.GetText("BtnTagUp");
            BtnTagDown.Text = I18n.GetText("BtnTagDown");
            BtnTagFindInAll.Text = I18n.GetText("BtnTagFindInAll");

            BtnTagSwitch.Text = I18n.GetText("BtnTagSwitch");
            BtnTagAddToAll.Text = I18n.GetText("BtnTagAddToAll");
            BtnTagDeleteForAll.Text = I18n.GetText("BtnTagDeleteForAll");
            BtnTagReplace.Text = I18n.GetText("BtnTagReplace");
            BtnTagAddToSelected.Text = I18n.GetText("BtnTagAddToSelected");
            BtnTagDeleteForSelected.Text = I18n.GetText("BtnTagDeleteForSelected");
            BtnTagAddToFiltered.Text = I18n.GetText("BtnTagAddToFiltered");
            BtnTagDeleteForFiltered.Text = I18n.GetText("BtnTagDeleteForFiltered");
            BtnTagMultiModeSwitch.Text = I18n.GetText("BtnTagMultiModeSwitch");
            BtnImageFilter.Text = I18n.GetText("BtnImageFilter");
            BtnImageExitFilter.Text = I18n.GetText("BtnImageExitFilter");
            BtnTagFilter.Text = I18n.GetText("BtnTagFilter");
            BtnTagExitFilter.Text = I18n.GetText("BtnTagExitFilter");
            MenuShowTagCount.Text = I18n.GetText("MenuShowCount");
            BtnMenuGenTagsWithCurrentSettings.Text = I18n.GetText("BtnMenuGenTagsWithCurrentSettings");
            BtnMenuGenTagsWithSetWindow.Text = I18n.GetText("BtnMenuGenTagsWithSetWindow");
            toolStripPromptSortBtn.Text = I18n.GetText("toolStripPromptSortBtn");


            switch (Program.Settings.Language)
            {
                case "en-US":
                    LanguageENBtn.Checked = true;
                    LanguageCNBtn.Checked = false;
                    break;
                case "zh-CN":
                    LanguageENBtn.Checked = false;
                    LanguageCNBtn.Checked = true;
                    break;
                default:
                    break;
            }
        }

        private void LanguageENBtn_Click(object sender, EventArgs e)
        {
            if (LanguageENBtn.Checked) { return; }
            Program.Settings.Language = "en-US";
            Program.Settings.SaveSettings();
            switchLanguage();
            LanguageENBtn.Checked = true;
            LanguageCNBtn.Checked = false;
        }

        private void LanguageCNBtn_Click(object sender, EventArgs e)
        {
            if (LanguageCNBtn.Checked) { return; }
            Program.Settings.Language = "zh-CN";
            Program.Settings.SaveSettings();
            switchLanguage();
            LanguageCNBtn.Checked = true;
            LanguageENBtn.Checked = false;
        }

        private void MenuShowTagCount_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            showCount = !showCount;
            MenuShowTagCount.Checked = showCount;
            var Header = "Count";
            if (showCount)
            {
                gridViewAllTags.Columns.Insert(1, new DataGridViewTextBoxColumn()
                {
                    Name = Header,
                    HeaderText = Header,
                    ReadOnly = true,
                    Width = 80,
                });

                // add count
                LockEdit(true);
                for (int i = 0; i < gridViewAllTags.RowCount; i++)
                {
                    gridViewAllTags[Header, i].ReadOnly = true;
                    gridViewAllTags[Header, i].Value = 0;
                }
                UpdateTagCount();
                LockEdit(false);
            }
            else
            {
                gridViewAllTags.Columns.Remove(Header);
            }
        }

        public void UpdateTagCount()
        {
            var dataset = Program.DataManager.DataSet;
            var Header = "Count";
            int tmpCount;
            for (int i = 0; i < gridViewAllTags.RowCount; i++)
            {
                tmpCount = 0;
                foreach (var item in dataset)
                {
                    for (int j = 0; j < item.Value.Tags.Count; ++j)
                    {
                        if (item.Value.Tags[j].Tag == gridViewAllTags["Tag", i].Value.ToString())
                        {
                            tmpCount++;
                            break;
                        }
                    }
                }
                gridViewAllTags[Header, i].Value = tmpCount;
            }
        }

        private void BtnTagRedo_Click(object sender, EventArgs e)
        {
            ((EditableTagList)gridViewTags.DataSource).NextState();
        }

        private void gridViewTags_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (gridViewTags[e.ColumnIndex, e.RowIndex].Value == DBNull.Value)
                    gridViewTags[e.ColumnIndex, e.RowIndex].Value = string.Empty;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_settings settings = new Form_settings();
            if (settings.ShowDialog() == DialogResult.OK)
            {
                SetStatus("Settings have been saved");
            }
            settings.Close();
            switchLanguage();
        }

        private async void replaceTransparentBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridViewDS.SelectedRows.Count == 0)
                return;
            Form_backgroundReplace backgroundReplace = new Form_backgroundReplace();
            if (backgroundReplace.ShowDialog() != DialogResult.OK)
                return;
            LockEdit(true);
            SetStatus("In progress, please wait...");
            bool randomColor = backgroundReplace.checkBox1.Checked;
            Color replColor = backgroundReplace.pictureBox1.BackColor;
            List<DataItem> selectedTagsList = new List<DataItem>();
            Random r = new Random();
            for (int i = 0; i < gridViewDS.SelectedRows.Count; i++)
            {
                selectedTagsList.Add(Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[i].Cells["ImageFilePath"].Value]);
            }
            await Task.Run(() =>
            {
                foreach (var item in selectedTagsList)
                {
                    Bitmap bmp = (Bitmap)Bitmap.FromFile(item.ImageFilePath);
                    System.Drawing.Imaging.ImageFormat format = bmp.RawFormat;
                    if (randomColor)
                    {
                        replColor = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                    }
                    Bitmap bmpRes = Extensions.Transparent2Color(bmp, replColor);
                    bmp.Dispose();
                    bmpRes.Save(item.ImageFilePath, format);
                    bmpRes.Dispose();
                    item.Img = Extensions.MakeThumb(item.ImageFilePath, Program.Settings.PreviewSize);
                }
            });
            LockEdit(false);
            SetStatus("Background replacement complete!");
        }

        #region HotkeysCode

        private void InitHotkeyCommands()
        {
            if (Program.Settings.Hotkeys.Commands == null)
                Program.Settings.Hotkeys.Commands = new Dictionary<string, Action>();
            var cmds = Program.Settings.Hotkeys.Commands;

            cmds["DatasetFocus"] = delegate () { DatasetFocus(); };
            cmds["TagsFocus"] = delegate () { TagsFocus(); };
            cmds["AllTagsFocus"] = delegate () { AllTagsFocus(); };
            cmds["AutoTagsFocus"] = delegate () { AutoTagsFocus(); };

            cmds["MenuItemSaveChanges"] = delegate () { saveAllChangesToolStripMenuItem.PerformClick(); };
            cmds["MenuItemShowPreview"] = delegate () { MenuShowPreview.PerformClick(); };
            cmds["MenuHideAllTags"] = delegate () { MenuHideAllTags.PerformClick(); };
            cmds["MenuHideTags"] = delegate () { MenuHideTags.PerformClick(); };
            cmds["MenuHideDataset"] = delegate () { MenuHideDataset.PerformClick(); };


            cmds["BtnTagAdd"] = delegate () { BtnTagAdd.PerformClick(); };
            cmds["BtnTagDelete"] = delegate () { BtnTagDelete.PerformClick(); };
            cmds["BtnTagUndo"] = delegate () { BtnTagUndo.PerformClick(); };
            cmds["BtnTagRedo"] = delegate () { BtnTagRedo.PerformClick(); };
            cmds["BtnTagUp"] = delegate () { BtnTagUp.PerformClick(); };
            cmds["BtnTagDown"] = delegate () { BtnTagDown.PerformClick(); };
            cmds["BtnTagFindInAll"] = delegate () { BtnTagFindInAll.PerformClick(); };
            cmds["BtnTagAddToAll"] = delegate () { BtnTagAddToAll.PerformClick(); };
            cmds["BtnTagAddToSelected"] = delegate () { BtnTagAddToSelected.PerformClick(); };
            cmds["BtnTagAddToFiltered"] = delegate () { BtnTagAddToFiltered.PerformClick(); };
            cmds["BtnTagDeleteForAll"] = delegate () { BtnTagDeleteForAll.PerformClick(); };
            cmds["BtnTagDeleteForSelected"] = delegate () { BtnTagDeleteForSelected.PerformClick(); };
            cmds["BtnTagDeleteForFiltered"] = delegate () { BtnTagDeleteForFiltered.PerformClick(); };
            cmds["BtnTagReplace"] = delegate () { BtnTagReplace.PerformClick(); };
            cmds["BtnImageFilter"] = delegate () { BtnImageFilter.PerformClick(); };
            cmds["BtnImageExitFilter"] = delegate () { BtnImageExitFilter.PerformClick(); };
            cmds["BtnTagMultiModeSwitch"] = delegate () { BtnTagMultiModeSwitch.PerformClick(); };
            cmds["BtnTagFilter"] = delegate () { BtnTagFilter.PerformClick(); };
            cmds["BtnTagExitFilter"] = delegate () { BtnTagExitFilter.PerformClick(); };
            cmds["BtnMenuGenTagsWithCurrentSettings"] = delegate () { BtnMenuGenTagsWithCurrentSettings.PerformClick(); };
            cmds["BtnMenuGenTagsWithSetWindow"] = delegate () { BtnMenuGenTagsWithSetWindow.PerformClick(); };
            cmds["toolStripPromptSortBtn"] = delegate () { toolStripPromptSortBtn.PerformClick(); };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (gridViewTags.IsCurrentCellInEditMode && keyData == Keys.Enter)
            {
                var rowIndex = gridViewTags.CurrentCell.RowIndex;
                gridViewTags.EndEdit();
                if (GetTagsDataSourceType() == DataSourceType.Single)
                {
                    var eTagList = ((EditableTagList)gridViewTags.DataSource);
                    eTagList.EndEdit(rowIndex);
                }
                else if (GetTagsDataSourceType() == DataSourceType.Multi)
                {
                    var mTagList = ((MultiSelectDataTable)gridViewTags.DataSource);
                    mTagList.EndEdit();
                }
                //((EditableTagList)gridViewTags.DataSource).
            }
            else if (gridViewTags.Focused && !gridViewTags.IsCurrentCellInEditMode && keyData == Keys.Enter)
            {
                gridViewTags.BeginEdit(true);
                return true;
            }
            var hotkey = Program.Settings.Hotkeys.Items.Find(a => a.FullKeyData == keyData);
            if (hotkey != null)
            {
                Program.Settings.Hotkeys.Commands[hotkey.Id]();
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DatasetFocus()
        {
            gridViewDS.Focus();
        }

        private void TagsFocus()
        {
            gridViewTags.Focus();
        }

        private void AllTagsFocus()
        {
            tabControl1.SelectedIndex = 0;
            gridViewAllTags.Focus();
        }

        private void AutoTagsFocus()
        {
            tabControl1.SelectedIndex = 1;
            gridViewAutoTags.Focus();
        }

        private void HideShowAllTagsWindow()
        {
            if (splitContainer2.Panel1Collapsed)
                HideShowTagsWindow();
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            MenuHideAllTags.Checked = splitContainer2.Panel2Collapsed;
        }

        private void HideShowTagsWindow()
        {
            if (splitContainer2.Panel2Collapsed)
                HideShowAllTagsWindow();
            splitContainer2.Panel1Collapsed = !splitContainer2.Panel1Collapsed;
            MenuHideTags.Checked = splitContainer2.Panel1Collapsed;
        }

        private void HideShowDataset()
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
            MenuHideDataset.Checked = splitContainer1.Panel1Collapsed;
        }

        #endregion

        private void autoTaggerSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_AutoTaggerSettings autoTaggerSettings = new Form_AutoTaggerSettings();
            autoTaggerSettings.ShowDialog();
        }

        private async void BtnAutoGetTagsDefSet_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            tabAutoTags.Select();
            LockEdit(true);
            var selectedImageData = Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value];
            var tagList = await GetTagsWithAutoTagger(selectedImageData.ImageFilePath, true);
            if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.AllWithReplacement)
                gridViewAutoTags.DataSource = tagList;
            else if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.OnlyNewWithAddition)
            {
                foreach (var item in selectedImageData.Tags.TextTags)
                {
                    tagList.RemoveAll(a => a.Tag == item);
                }
                gridViewAutoTags.DataSource = tagList;
            }

            LockEdit(false);
        }

        private async Task<List<AutoTagItem>> GetTagsWithAutoTagger(string imagePath, bool defSettings)
        {
            if (gridViewDS.SelectedRows.Count == 0)
                return new List<AutoTagItem>();
            if (!defSettings || Program.Settings.AutoTagger.InterragatorParams.Count == 0)
            {
                Form_AutoTaggerSettings autoTaggerSettings = new Form_AutoTaggerSettings();
                if (autoTaggerSettings.ShowDialog() != DialogResult.OK || Program.Settings.AutoTagger.InterragatorParams.Count == 0)
                {
                    autoTaggerSettings.Close();
                    SetStatus("Generation operation canceled");
                    return new List<AutoTagItem>();
                }
            }
            if (!Program.AutoTagger.IsConnected)
            {
                if (!await Program.AutoTagger.ConnectAsync())
                {
                    SetStatus("Unable to connect to the Interrogator service! Please start the service from the interrogator_rpc folder.");
                    return new List<AutoTagItem>();
                }
            }
            List<Image_Interrogator_Ns.NetworkInterrogationParameters> parameters = new List<Image_Interrogator_Ns.NetworkInterrogationParameters>();
            foreach (var item in Program.Settings.AutoTagger.InterragatorParams)
            {
                parameters.Add(new Image_Interrogator_Ns.NetworkInterrogationParameters() { InterrogatorNetwork = item.Key, InterrogatorThreshold = item.Value });
            }
            var listOfTags = await Program.AutoTagger.InterrogateImage(imagePath, parameters, Program.Settings.AutoTagger.SerializeVramUsage, Program.Settings.AutoTagger.SkipInternetRequests);
            SetStatus(listOfTags.Message);
            if (!listOfTags.Success)
            {
                return new List<AutoTagItem>();
            }
            List<AutoTagItem> result = listOfTags.GetTagList(Program.Settings.AutoTagger.UnionMode);
            if (Program.Settings.AutoTagger.SortMode == AutoTaggerSort.Confidence)
            {
                result.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
            }
            else if (Program.Settings.AutoTagger.SortMode == AutoTaggerSort.Alphabetical)
            {
                result.Sort((a, b) => a.Tag.CompareTo(b.Tag));
            }
            return result;
        }

        private async void generateTagsWithCurrentSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await GenerateTagsInTags(true, false);
        }

        private async Task GenerateTagsInTags(bool defSettings, bool allTags)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            LockEdit(true);
            List<DataItem> selectedTagsList = new List<DataItem>();
            if (!allTags)
            {
                for (int i = 0; i < gridViewDS.SelectedRows.Count; i++)
                {
                    selectedTagsList.Add(Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[i].Cells["ImageFilePath"].Value]);
                }
            }
            else
            {
                foreach (var item in Program.DataManager.DataSet)
                {
                    selectedTagsList.Add(item.Value);
                }
            }
            foreach (var item in selectedTagsList)
            {
                var tagList = await GetTagsWithAutoTagger(item.ImageFilePath, defSettings);
                if (!defSettings)
                    defSettings = true;
                if (tagList != null)
                {
                    if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.AllWithReplacement)
                    {
                        item.Tags.Clear();
                        item.Tags.AddRange(tagList.Select(a => a.Tag), true);
                    }
                    else if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.OnlyNewWithAddition)
                    {
                        foreach (var aTag in tagList)
                        {
                            item.Tags.AddTag(aTag.Tag, true, AddingType.Down, 0);
                        }

                    }
                }
            }
            if (selectedTagsList.Count > 1)
            {
                LoadSelectedImageToGrid();
            }
            Program.DataManager.UpdateData();
            if (gridViewAllTags.DataSource == null)
                BindTagList();
            LockEdit(false);
        }

        private async void generateTagsWithSettingsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await GenerateTagsInTags(false, false);
        }

        private async void btnAutoGetTagsOpenSet_Click(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }
            tabAutoTags.Select();
            LockEdit(true);
            var selectedImageData = Program.DataManager.DataSet[(string)gridViewDS.SelectedRows[0].Cells["ImageFilePath"].Value];
            var tagList = await GetTagsWithAutoTagger(selectedImageData.ImageFilePath, false);
            if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.AllWithReplacement)
                gridViewAutoTags.DataSource = tagList;
            else if (Program.Settings.AutoTagger.SetMode == NetworkResultSetMode.OnlyNewWithAddition)
            {
                foreach (var item in selectedImageData.Tags.TextTags)
                {
                    tagList.RemoveAll(a => a.Tag == item);
                }
                gridViewAutoTags.DataSource = tagList;
            }

            LockEdit(false);
        }

        private void MenuHideAllTags_Click(object sender, EventArgs e)
        {
            HideShowAllTagsWindow();
        }

        private void MenuHideTags_Click(object sender, EventArgs e)
        {
            HideShowTagsWindow();
        }

        private void MenuHideDataset_Click(object sender, EventArgs e)
        {
            HideShowDataset();
        }

        private void toolStripMenuItemWeight_ValueChanged(object sender, EventArgs e)
        {
            if (Program.DataManager == null)
            {
                return;
            }
            int wMod = toolStripMenuItemWeight.Value;
            float weight = 1;
            if (wMod == 0)
                weight = 1;
            else if (wMod > 0)
            {
                weight = WeightMultiplier(weight, PromptParser.round_bracket_multiplier, wMod);
            }
            else
            {
                weight = WeightMultiplier(weight, PromptParser.square_bracket_multiplier, Math.Abs(wMod));
            }
            toolStripTextBoxWeight.Text = weight.ToString();

            if (gridViewTags.SelectedCells.Count == 0)
                return;
            int rowIndex = gridViewTags.SelectedCells[0].RowIndex;
            var dsType = GetTagsDataSourceType();

            if (dsType == DataSourceType.Single)
            {
                ((EditableTagList)gridViewTags.DataSource)[rowIndex].Weight = weight;
            }
            else if (dsType == DataSourceType.Multi)
            {
                var dataItem = (DataItem)((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[rowIndex]).GetDataItem();
                dataItem.Tags[((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[rowIndex]).GetTagIndex()].Weight = weight;
            }
        }

        private float WeightMultiplier(float value, float multiplier, int count)
        {
            for (int i = 0; i < count; i++)
            {
                value *= multiplier;
            }
            return value;
        }

        private void gridViewTags_SelectionChanged(object sender, EventArgs e)
        {
            if (gridViewTags.SelectedCells.Count == 0)
                return;
            int rowIndex = gridViewTags.SelectedCells[0].RowIndex;
            var dsType = GetTagsDataSourceType();
            float weight = 1;
            if (dsType == DataSourceType.Single)
            {
                weight = ((EditableTagList)gridViewTags.DataSource)[rowIndex].Weight;
                toolStripTextBoxWeight.Text = weight.ToString();
            }
            else if (dsType == DataSourceType.Multi)
            {
                if (((MultiSelectDataTable)gridViewTags.DataSource).Rows[rowIndex].RowState == DataRowState.Detached && rowIndex < gridViewTags.RowCount)
                {
                    rowIndex++;
                }
                var dataItem = (DataItem)((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[rowIndex]).GetDataItem();
                weight = dataItem.Tags[((MultiSelectDataRow)((MultiSelectDataTable)gridViewTags.DataSource).Rows[rowIndex]).GetTagIndex()].Weight;
                toolStripTextBoxWeight.Text = weight.ToString();
            }
            else
                return;
            if (weight == 1)
                toolStripMenuItemWeight.Value = 0;
            else if (weight > 1)
            {
                int brCount = Extensions.CalcBracketsCount(weight, true);
                if (brCount != 0)
                    toolStripMenuItemWeight.Value = brCount;
                else
                    toolStripMenuItemWeight.Value = 0;
            }
            else
            {
                int brCount = Extensions.CalcBracketsCount(weight, false);
                if (brCount != 0)
                    toolStripMenuItemWeight.Value = -brCount;
                else
                    toolStripMenuItemWeight.Value = 0;
            }
        }

        private async void generateTagsWithAutoTaggerForAllImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await GenerateTagsInTags(false, true);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AddSelectedAutoTagsToImageTags();
        }

        private void AddSelectedAutoTagsToImageTags()
        {
            if (gridViewTags.DataSource == null)
            {
                MessageBox.Show(I18n.GetText("TipDatasetNoLoad"));
                return;
            }

            if (gridViewAutoTags.SelectedCells.Count == 0)
                return;
            List<string> selectedTags = new List<string>();
            for (int i = 0; i < gridViewAutoTags.SelectedCells.Count; i++)
            {
                string tag = (string)gridViewAutoTags["Tag", gridViewAutoTags.SelectedCells[i].RowIndex].Value;
                if (!selectedTags.Contains(tag))
                    selectedTags.Add(tag);
            }
            if (gridViewDS.SelectedRows.Count > 1)
            {
                foreach (var item in selectedTags)
                {
                    AddTagMultiselectedMode(item, true, AddingType.Down, 0);
                }

            }
            else
            {
                var eTagList = ((EditableTagList)gridViewTags.DataSource);
                foreach (var item in selectedTags)
                {
                    eTagList.AddTag(item, true, AddingType.Down, 0);
                }
            }
            Program.DataManager.UpdateData();
            if (showCount)
                UpdateTagCount();
        }

        private void gridViewAutoTags_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddSelectedAutoTagsToImageTags();
        }
    }
}
