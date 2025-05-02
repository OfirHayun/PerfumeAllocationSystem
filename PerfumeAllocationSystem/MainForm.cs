using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using PerfumeAllocationSystem.Core;
using PerfumeAllocationSystem.Models;
using PerfumeAllocationSystem.Services;

namespace PerfumeAllocationSystem
{
    public partial class MainForm : Form
    {
        private List<Perfume> _perfumes = new List<Perfume>();
        private List<StoreRequirement> _storeRequirements = new List<StoreRequirement>();
        private AllocationEngine _allocationEngine;
        private DataService _dataService = new DataService();
        private Random _random = new Random();
        private Timer timerHideMsg;
        private Label lblRandomStoreMsg;

        // Path to the embedded CSV file
        private const string DEFAULT_CSV_PATH = "fragrances-database.csv";

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
            LoadDefaultCsvData();
        }

        private void InitializeUI()
        {
            InitializeFormStyle();
            SetupTabControl();
            InitializeLabelsAndTimers();
            SetupDataGridViews();
            InitializeComboBoxes();

            // Set default values for longevity and projection
            txtMinLongevity.Text = "1";  // Change from "0" to "1"
            txtMinProjection.Text = "1";  // Change from "0" to "1"

            // Add validators to prevent entering zero or invalid values
            txtMinLongevity.KeyPress += NumericTextBox_KeyPress;
            txtMinProjection.KeyPress += NumericTextBox_KeyPress;
        }

        private void InitializeFormStyle()
        {
            Font = new Font("Segoe UI", 9F);
        }

        private void SetupTabControl()
        {
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += tabControl1_DrawItem;

            tabPerfumes.BackColor = Color.FromArgb(245, 245, 245);
            tabStores.BackColor = Color.FromArgb(245, 245, 245);
            tabAllocation.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void InitializeLabelsAndTimers()
        {
            StyleAllButtons();
            AddRandomStoreMessageLabel();
            ConfigureMessageTimer();
        }

        private void StyleAllButtons()
        {
            ApplyButtonStyle(btnAddStore);
            ApplyButtonStyle(btnGenerateRandomStore);
            ApplyButtonStyle(btnLoadPerfumes);
            ApplyButtonStyle(btnRunAllocation);
            ApplyButtonStyle(btnSaveResults);
            ApplyButtonStyle(btnClearStores);
            ApplyButtonStyle(btnReset);
        }

        private void AddRandomStoreMessageLabel()
        {
            // Calculate position relative to the Generate Random Store button
            int labelY = btnGenerateRandomStore.Bottom + 20; // 20 pixels below the button

            lblRandomStoreMsg = new Label
            {
                AutoSize = false,
                Width = btnGenerateRandomStore.Width,
                Height = 40,
                ForeColor = Color.Green,
                Location = new Point(btnGenerateRandomStore.Left, labelY),
                Name = "lblRandomStoreMsg",
                Visible = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            groupBox1.Controls.Add(lblRandomStoreMsg);
        }

        private void ConfigureMessageTimer()
        {
            timerHideMsg = new Timer { Interval = 5000 };
            timerHideMsg.Tick += timerHideMsg_Tick;
        }

        private void SetupDataGridViews()
        {
            SetupPerfumesGrid();
            SetupStoresGrid();
            SetupResultsGrid();
        }

        private void SetupPerfumesGrid()
        {
            dgvPerfumes.AutoGenerateColumns = true;
            StyleDataGridView(dgvPerfumes);
        }

        private void SetupStoresGrid()
        {
            dgvStores.AutoGenerateColumns = false;

            dgvStores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StoreName",
                HeaderText = "Store Name",
                DataPropertyName = "StoreName"
            });

            dgvStores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Budget",
                HeaderText = "Budget",
                DataPropertyName = "Budget"
            });

            dgvStores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuantityNeeded",
                HeaderText = "Quantity Needed",
                DataPropertyName = "QuantityNeeded"
            });

            StyleDataGridView(dgvStores);
        }

        private void SetupResultsGrid()
        {
            dgvResults.AutoGenerateColumns = false;

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StoreName",
                HeaderText = "Store",
                DataPropertyName = "StoreName"
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SatisfactionPercentage",
                HeaderText = "Satisfaction %",
                DataPropertyName = "SatisfactionPercentage",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.00" }
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSpent",
                HeaderText = "Total Spent",
                DataPropertyName = "TotalSpent",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C" }
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RemainingBudget",
                HeaderText = "Remaining Budget",
                DataPropertyName = "RemainingBudget",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C" }
            });

            StyleDataGridView(dgvResults);
        }

        private void InitializeComboBoxes()
        {
            SetupGenderComboBox();
            SetupAccordComboBox();
        }

        private void SetupGenderComboBox()
        {
            cboGender.Items.AddRange(new string[] { "Any", "Male", "Female", "Unisex" });
            cboGender.SelectedIndex = 0;
        }

        private void SetupAccordComboBox()
        {
            cboAccord.Items.Add("Any");
            cboAccord.Items.AddRange(new string[] {
                "Aromatic", "Woody", "Fresh", "Sweet", "Floral", "Citrus",
                "Oriental", "Fruity", "Spicy", "Gourmand", "Leather", "Tobacco",
                "Musky", "Powdery", "Green", "Aquatic", "Oud", "Animalic"
            });
            cboAccord.SelectedIndex = 0;
        }

        private void ApplyButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            button.BackColor = Color.FromArgb(60, 60, 100);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 9F);
            button.Cursor = Cursors.Hand;

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(80, 80, 120);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(60, 60, 100);
            }
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            SetDataGridViewBasicStyle(dgv);
            SetDataGridViewHeaderStyle(dgv);
        }

        private void SetDataGridViewBasicStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 100, 160);
            dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgv.BackgroundColor = Color.White;
        }

        private void SetDataGridViewHeaderStyle(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 100);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawTabItem(e);
        }

        private void DrawTabItem(DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            TabPage tp = tabControl1.TabPages[e.Index];
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            Rectangle rect = tabControl1.GetTabRect(e.Index);
            Brush textBrush, backBrush;

            SetTabColors(e.Index, out textBrush, out backBrush);

            g.FillRectangle(backBrush, rect);
            g.DrawString(tp.Text, new Font("Segoe UI", 9F), textBrush, rect, sf);

            textBrush.Dispose();
            backBrush.Dispose();
        }

        private void SetTabColors(int tabIndex, out Brush textBrush, out Brush backBrush)
        {
            if (tabControl1.SelectedIndex == tabIndex)
            {
                textBrush = new SolidBrush(Color.White);
                backBrush = new SolidBrush(Color.FromArgb(60, 60, 100));
            }
            else
            {
                textBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
                backBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            }
        }

        private void timerHideMsg_Tick(object sender, EventArgs e)
        {
            lblRandomStoreMsg.Visible = false;
            timerHideMsg.Stop();
        }

        private void LoadDefaultCsvData()
        {
            try
            {
                if (File.Exists(DEFAULT_CSV_PATH))
                {
                    LoadCsvFile(DEFAULT_CSV_PATH);
                }
                else
                {
                    ShowDefaultFileNotFoundMessage();
                }
            }
            catch (Exception ex)
            {
                ShowLoadErrorMessage(ex);
            }
        }

        private void LoadCsvFile(string filePath)
        {
            _perfumes = _dataService.LoadPerfumesFromCsv(filePath);
            dgvPerfumes.DataSource = null;
            dgvPerfumes.DataSource = _perfumes;

            _allocationEngine = new AllocationEngine(_perfumes);
            lblPerfumesSummary.Text = $"Loaded {_perfumes.Count} perfumes";

            btnAddStore.Enabled = true;
            btnGenerateRandomStore.Enabled = true;
        }

        private void ShowDefaultFileNotFoundMessage()
        {
            MessageBox.Show($"Default fragrance data file not found! Please place '{DEFAULT_CSV_PATH}' in the application folder or load a CSV file manually.",
                "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowLoadErrorMessage(Exception ex)
        {
            MessageBox.Show($"Error loading default fragrance data: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnLoadPerfumes_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                ConfigureOpenFileDialog(openFileDialog);
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadCsvFile(openFileDialog.FileName);
                }
            }
        }

        private void ConfigureOpenFileDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Perfume Dataset";
        }

        private void btnAddStore_Click(object sender, EventArgs e)
        {
            if (!ValidateStoreInputs())
            {
                return;
            }

            StoreRequirement store = CreateStoreFromInputs();
            _storeRequirements.Add(store);
            UpdateStoreRequirementsGrid();
            ClearStoreInputs();
        }

        private bool ValidateStoreInputs()
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show("Please enter a store name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!ValidateNumericInput(txtBudget.Text, "budget", out decimal budget) || budget <= 0)
                return false;

            if (!ValidateNumericInput(txtQuantity.Text, "quantity", out int quantity) || quantity <= 0)
                return false;

            if (!ValidateNumericInput(txtMaxPrice.Text, "maximum price", out decimal maxPrice) || maxPrice <= 0)
                return false;

            if (!ValidateLongevityProjection())
                return false;

            return true;
        }

        private bool ValidateNumericInput(string input, string fieldName, out decimal result)
        {
            if (!decimal.TryParse(input, out result) || result <= 0)
            {
                MessageBox.Show($"Please enter a valid {fieldName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool ValidateNumericInput(string input, string fieldName, out int result)
        {
            if (!int.TryParse(input, out result) || result <= 0)
            {
                MessageBox.Show($"Please enter a valid {fieldName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool ValidateLongevityProjection()
        {
            if (!int.TryParse(txtMinLongevity.Text, out int minLongevity) || minLongevity <= 0 || minLongevity > 10)
            {
                MessageBox.Show("Please enter a valid minimum longevity (1-10)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(txtMinProjection.Text, out int minProjection) || minProjection <= 0 || minProjection > 10)
            {
                MessageBox.Show("Please enter a valid minimum projection (1-10)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private StoreRequirement CreateStoreFromInputs()
        {
            decimal budget = decimal.Parse(txtBudget.Text);
            int quantity = int.Parse(txtQuantity.Text);
            decimal maxPrice = decimal.Parse(txtMaxPrice.Text);
            int minLongevity = int.Parse(txtMinLongevity.Text);
            int minProjection = int.Parse(txtMinProjection.Text);

            return new StoreRequirement
            {
                StoreName = txtStoreName.Text,
                Budget = budget,
                QuantityNeeded = quantity,
                Gender = GetSelectedGender(),
                PreferredAccord = GetSelectedAccord(),
                PreferredTopNotes = txtTopNotes.Text,
                PreferredMiddleNotes = txtMiddleNotes.Text,
                PreferredBaseNotes = txtBaseNotes.Text,
                MinLongevity = minLongevity,
                MinProjection = minProjection,
                MaxPrice = maxPrice
            };
        }

        private string GetSelectedGender()
        {
            return cboGender.SelectedItem.ToString() == "Any" ? "" : cboGender.SelectedItem.ToString();
        }

        private string GetSelectedAccord()
        {
            return cboAccord.SelectedItem.ToString() == "Any" ? "" : cboAccord.SelectedItem.ToString();
        }

        private void btnGenerateRandomStore_Click(object sender, EventArgs e)
        {
            if (_perfumes.Count == 0)
            {
                MessageBox.Show("Please load perfume data first!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateRandomStoreData();
            ShowRandomGenerationMessage();
        }

        private void GenerateRandomStoreData()
        {
            GenerateBasicStoreInfo();
            GenerateRandomNotes();
            GenerateRandomQualitySettings();
        }

        private void GenerateBasicStoreInfo()
        {
            txtStoreName.Text = $"Store_{_random.Next(1, 1000)}";
            txtBudget.Text = _random.Next(500, 5001).ToString();
            txtQuantity.Text = _random.Next(5, 21).ToString();

            string[] genders = { "Any", "Male", "Female", "Unisex" };
            cboGender.SelectedItem = genders[_random.Next(genders.Length)];

            string[] accords = {
                "Any", "Aromatic", "Woody", "Fresh", "Sweet", "Floral", "Citrus",
                "Oriental", "Fruity", "Spicy", "Gourmand", "Leather", "Tobacco"
            };
            cboAccord.SelectedItem = accords[_random.Next(accords.Length)];
        }

        private void GenerateRandomNotes()
        {
            string topNote = "";
            string middleNote = "";
            string baseNote = "";

            if (_perfumes.Count > 0)
            {
                ExtractRandomNotesFromPerfumes(ref topNote, ref middleNote, ref baseNote);
            }

            txtTopNotes.Text = topNote;
            txtMiddleNotes.Text = middleNote;
            txtBaseNotes.Text = baseNote;
        }

        private void ExtractRandomNotesFromPerfumes(ref string topNote, ref string middleNote, ref string baseNote)
        {
            var randomPerfume1 = _perfumes[_random.Next(_perfumes.Count)];
            var randomPerfume2 = _perfumes[_random.Next(_perfumes.Count)];
            var randomPerfume3 = _perfumes[_random.Next(_perfumes.Count)];

            ExtractRandomNote(randomPerfume1.TopNotes, ref topNote);
            ExtractRandomNote(randomPerfume2.MiddleNotes, ref middleNote);
            ExtractRandomNote(randomPerfume3.BaseNotes, ref baseNote);
        }

        private void ExtractRandomNote(string notesList, ref string selectedNote)
        {
            if (!string.IsNullOrEmpty(notesList))
            {
                string[] notes = notesList.Split(',');
                if (notes.Length > 0)
                    selectedNote = notes[0].Trim();
            }
        }

        private void GenerateRandomQualitySettings()
        {
            txtMinLongevity.Text = _random.Next(1, 11).ToString(); // 1-10 instead of 0-10
            txtMinProjection.Text = _random.Next(1, 11).ToString(); // 1-10 instead of 0-10
            txtMaxPrice.Text = _random.Next(50, 401).ToString(); // $50-$400
        }

        private void ShowRandomGenerationMessage()
        {
            lblRandomStoreMsg.Visible = true;
            lblRandomStoreMsg.Text = "Random values generated! Click 'Add Store' to add this store.";
            timerHideMsg.Start();
        }

        private void UpdateStoreRequirementsGrid()
        {
            dgvStores.DataSource = null;
            dgvStores.DataSource = _storeRequirements;
            btnRunAllocation.Enabled = _storeRequirements.Count > 0;
        }

        private void ClearStoreInputs()
        {
            txtStoreName.Text = "";
            txtBudget.Text = "";
            txtQuantity.Text = "";
            cboGender.SelectedIndex = 0;
            cboAccord.SelectedIndex = 0;
            txtTopNotes.Text = "";
            txtMiddleNotes.Text = "";
            txtBaseNotes.Text = "";
            txtMinLongevity.Text = "1"; // Changed from "0" to "1"
            txtMinProjection.Text = "1"; // Changed from "0" to "1"
            txtMaxPrice.Text = "";
        }

        private void btnRunAllocation_Click(object sender, EventArgs e)
        {
            if (!ValidateAllocationRequirements())
                return;

            RunAllocationProcess();
        }

        private bool ValidateAllocationRequirements()
        {
            if (_allocationEngine == null)
            {
                MessageBox.Show("Please load perfumes first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (_storeRequirements.Count == 0)
            {
                MessageBox.Show("Please add at least one store", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void RunAllocationProcess()
        {
            List<StoreRequirement> results = _allocationEngine.AllocatePerfumes(_storeRequirements);
            DisplayResults(results);
            EnableResultsFeatures();
            decimal profit = _allocationEngine.GetTotalProfit();
            ShowAllocationDetails(results);
            CreateResultsTab(results, profit);

            // Add the profit summary display
            DisplayTotalProfitSummary(profit);
        }

        private void DisplayResults(List<StoreRequirement> results)
        {
            dgvResults.DataSource = null;
            dgvResults.DataSource = results;

            decimal profit = _allocationEngine.GetTotalProfit();
            lblTotalProfit.Text = $"Total Profit: {profit:C}";
        }

        private void EnableResultsFeatures()
        {
            btnSaveResults.Enabled = true;
        }

        private void ShowAllocationDetails(List<StoreRequirement> results)
        {
            pnlAllocationDetails.Controls.Clear();
            int yPos = 10;

            foreach (var store in results)
            {
                AddStoreDetailPanel(store, ref yPos);
            }
        }

        private void AddStoreDetailPanel(StoreRequirement store, ref int yPos)
        {
            GroupBox groupBox = CreateStoreGroupBox(store, yPos);
            AddStoreMetricsLabels(store, groupBox);
            AddPerfumesList(store, groupBox);
            pnlAllocationDetails.Controls.Add(groupBox);
            yPos += groupBox.Height + 10;
        }

        private GroupBox CreateStoreGroupBox(StoreRequirement store, int yPos)
        {
            return new GroupBox
            {
                Text = store.StoreName,
                Width = pnlAllocationDetails.Width - 20,
                Height = 200,
                Location = new Point(10, yPos)
            };
        }

        private void AddStoreMetricsLabels(StoreRequirement store, GroupBox groupBox)
        {
            Label lblSatisfaction = new Label
            {
                Text = $"Satisfaction: {store.SatisfactionPercentage:F2}%",
                Location = new Point(10, 20),
                AutoSize = true
            };
            groupBox.Controls.Add(lblSatisfaction);

            Label lblBudget = new Label
            {
                Text = $"Budget: {store.Budget:C} | Spent: {store.TotalSpent:C} | Remaining: {store.RemainingBudget:C}",
                Location = new Point(10, 40),
                AutoSize = true
            };
            groupBox.Controls.Add(lblBudget);

            Label lblQuantity = new Label
            {
                Text = $"Requested: {store.QuantityNeeded} | Allocated: {store.AllocatedPerfumes.Count} | Remaining: {store.RemainingQuantity}",
                Location = new Point(10, 60),
                AutoSize = true
            };
            groupBox.Controls.Add(lblQuantity);

            Label lblPerfumes = new Label
            {
                Text = "Allocated Perfumes:",
                Location = new Point(10, 80),
                AutoSize = true
            };
            groupBox.Controls.Add(lblPerfumes);
        }

        private void AddPerfumesList(StoreRequirement store, GroupBox groupBox)
        {
            ListBox lstPerfumes = new ListBox
            {
                Location = new Point(10, 100),
                Width = groupBox.Width - 40,
                Height = 80
            };

            foreach (var perfume in store.AllocatedPerfumes)
            {
                lstPerfumes.Items.Add($"{perfume.Brand} - {perfume.Name} (${perfume.AveragePrice})");
            }

            groupBox.Controls.Add(lstPerfumes);
        }

        // Helper method to handle key press events for numeric text boxes
        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            // Special handling for "0" - don't allow as first character
            if (e.KeyChar == '0' && (sender as TextBox).Text.Length == 0)
            {
                e.Handled = true;
            }
        }

        // Add this method to display a profit summary panel
        private void DisplayTotalProfitSummary(decimal profit)
        {
            // Remove any existing profit panels first
            foreach (Control ctrl in tabAllocation.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "profitSummaryPanel")
                {
                    tabAllocation.Controls.Remove(ctrl);
                    ctrl.Dispose();
                    break;
                }
            }

            // Create a profit summary panel at the top of the allocation results
            Panel profitPanel = new Panel
            {
                Name = "profitSummaryPanel",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            Label lblProfitTitle = new Label
            {
                Text = "ALLOCATION SUMMARY",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 5)
            };

            Label lblProfitValue = new Label
            {
                Text = $"Total Profit: {profit:C}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 30)
            };

            Label lblStoreCount = new Label
            {
                Text = $"Stores Served: {_storeRequirements.Count}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(300, 30)
            };

            decimal totalSatisfaction = (decimal)_storeRequirements.Average(s => s.SatisfactionPercentage);
            Label lblAverageSatisfaction = new Label
            {
                Text = $"Average Satisfaction: {totalSatisfaction:F2}%",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(500, 30)
            };

            profitPanel.Controls.Add(lblProfitTitle);
            profitPanel.Controls.Add(lblProfitValue);
            profitPanel.Controls.Add(lblStoreCount);
            profitPanel.Controls.Add(lblAverageSatisfaction);

            // Insert at the top of the panel
            tabAllocation.Controls.Add(profitPanel);
            profitPanel.BringToFront();
        }

        private void CreateResultsTab(List<StoreRequirement> results, decimal totalProfit)
        {
            // Create new tab page
            TabPage tabResult = new TabPage($"Results {DateTime.Now.ToString("HH:mm:ss")}");

            // Create a header panel with detailed profit info
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120, // Make it taller to fit more stats
                BackColor = Color.FromArgb(60, 60, 100)
            };

            // Add title
            Label lblTitle = new Label
            {
                Text = "Allocation Results Summary",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            headerPanel.Controls.Add(lblTitle);

            // Calculate additional statistics
            int totalItemsRequested = results.Sum(s => s.QuantityNeeded);
            int totalItemsAllocated = results.Sum(s => s.AllocatedPerfumes.Count);
            decimal totalBudget = results.Sum(s => s.Budget);
            decimal totalSpent = results.Sum(s => s.TotalSpent);
            double avgSatisfaction = results.Average(s => s.SatisfactionPercentage);
            int storesAboveTarget = results.Count(s => s.SatisfactionPercentage >= 70.0);

            // Create a table layout for statistics
            TableLayoutPanel statsTable = new TableLayoutPanel
            {
                Location = new Point(20, 40),
                Size = new Size(1000, 70),
                ColumnCount = 4,
                RowCount = 2,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            AddStatLabel(statsTable, "Total Spent:", $"{totalSpent:C}", 1, 0);
            AddStatLabel(statsTable, "Remaining Budget:", $"{totalBudget - totalSpent:C}", 1, 1);
            AddStatLabel(statsTable, "Items Requested:", $"{totalItemsRequested}", 2, 0);
            AddStatLabel(statsTable, "Items Allocated:", $"{totalItemsAllocated}", 2, 1);
            AddStatLabel(statsTable, "Stores Above 70%:", $"{storesAboveTarget} of {results.Count}", 3, 0);
            AddStatLabel(statsTable, "Avg. Satisfaction:", $"{avgSatisfaction:F2}%", 3, 1);

            // Add the table to the header
            headerPanel.Controls.Add(statsTable);

            // Main panel with auto scroll
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Create a table layout - 4 stores per row
            TableLayoutPanel storeGrid = new TableLayoutPanel
            {
                ColumnCount = 4,
                Dock = DockStyle.Top,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Padding = new Padding(5)
            };

            // Set column widths evenly
            for (int i = 0; i < storeGrid.ColumnCount; i++)
            {
                storeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            // Add stores to the grid
            int col = 0;
            int row = 0;

            foreach (var store in results)
            {
                // Create store panel
                Panel storePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    Padding = new Padding(5),
                    Height = 400
                };

                // Store name at the top
                Label lblStoreName = new Label
                {
                    Text = $"Store name: {store.StoreName}",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(5, 5)
                };
                storePanel.Controls.Add(lblStoreName);

                // Create satisfaction panel with colored background
                Panel satisfactionPanel = new Panel
                {
                    Location = new Point(5, 25),
                    Width = storePanel.Width - 15,
                    Height = 25,
                    BackColor = GetSatisfactionColor(store.SatisfactionPercentage),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                Label lblSatisfaction = new Label
                {
                    Text = $"Satisfaction: {store.SatisfactionPercentage:F2}%",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(5, 3)
                };
                satisfactionPanel.Controls.Add(lblSatisfaction);
                storePanel.Controls.Add(satisfactionPanel);

                // Add requested/allocated info
                Label lblRequestInfo = new Label
                {
                    Text = $"Requested: {store.QuantityNeeded}   Allocated: {store.AllocatedPerfumes.Count}   Remaining: {store.RemainingQuantity}",
                    Font = new Font("Segoe UI", 8),
                    AutoSize = true,
                    Location = new Point(5, 55)
                };
                storePanel.Controls.Add(lblRequestInfo);

                // Add budget info
                Label lblBudgetInfo = new Label
                {
                    Text = $"Budget: {store.Budget:C}   Spent: {store.TotalSpent:C}   Remaining: {store.RemainingBudget:C}",
                    Font = new Font("Segoe UI", 8),
                    AutoSize = true,
                    Location = new Point(5, 75)
                };
                storePanel.Controls.Add(lblBudgetInfo);

                // Perfume list header
                Label lblPerfumeHeader = new Label
                {
                    Text = "List of the perfumes:",
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    Location = new Point(5, 95)
                };
                storePanel.Controls.Add(lblPerfumeHeader);

                // Perfume list - slightly shorter to make room for expand button
                ListBox lstPerfumes = new ListBox
                {
                    Location = new Point(5, 115),
                    Size = new Size(storePanel.Width - 15, 235), // Shorter to make room for button
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 8.5F),
                    IntegralHeight = false
                };

                // Add perfumes to the list
                foreach (var perfume in store.AllocatedPerfumes)
                {
                    lstPerfumes.Items.Add($"{perfume.Brand} - {perfume.Name} (${perfume.AveragePrice})");
                }

                storePanel.Controls.Add(lstPerfumes);

                // Add Expand button for stores with satisfaction under 70%
                if (store.SatisfactionPercentage < 70.0)
                {
                    Button btnExpand = new Button
                    {
                        Text = "Explain Low Satisfaction",
                        Location = new Point(5, 355), // Position below the listbox
                        Size = new Size(storePanel.Width - 15, 30),
                        BackColor = Color.FromArgb(180, 180, 180),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Tag = GetSatisfactionExplanation(store) // Store explanation in tag
                    };

                    // Add click event handler
                    btnExpand.Click += (sender, e) =>
                    {
                        Button button = (Button)sender;
                        string explanation = button.Tag.ToString();
                        MessageBox.Show(explanation, $"Satisfaction Analysis for {store.StoreName}",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };

                    storePanel.Controls.Add(btnExpand);
                }

                // Add to grid
                storeGrid.Controls.Add(storePanel, col, row);

                // Move to next column or row
                col++;
                if (col >= storeGrid.ColumnCount)
                {
                    col = 0;
                    row++;
                    // Add a new row if needed
                    if (row >= storeGrid.RowCount && row < (results.Count / storeGrid.ColumnCount) + 1)
                    {
                        storeGrid.RowCount++;
                    }
                }
            }

            // Add components to tab
            mainPanel.Controls.Add(storeGrid);
            tabResult.Controls.Add(mainPanel);
            tabResult.Controls.Add(headerPanel);

            // Add tab to control
            tabControl1.TabPages.Add(tabResult);
            tabControl1.SelectedTab = tabResult;
        }

        // Helper method for the CreateResultsTab method
        private void AddStatLabel(TableLayoutPanel table, string title, string value, int col, int row, bool isHighlight = false)
        {
            Panel statPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = isHighlight ? Color.FromArgb(80, 80, 120) : Color.FromArgb(70, 70, 110)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(5, 5)
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 11, isHighlight ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(5, 25)
            };

            statPanel.Controls.Add(lblTitle);
            statPanel.Controls.Add(lblValue);
            table.Controls.Add(statPanel, col, row);
        }

        // Helper method to generate explanations for low satisfaction
        private string GetSatisfactionExplanation(StoreRequirement store)
        {
            // Base explanation
            string explanation = $"Satisfaction Analysis for {store.StoreName}:\n\n";

            // Check allocated vs requested
            if (store.AllocatedPerfumes.Count < store.QuantityNeeded)
            {
                explanation += $"• Insufficient quantity: Only allocated {store.AllocatedPerfumes.Count} of {store.QuantityNeeded} requested perfumes.\n\n";
            }

            // Check budget constraints
            if (store.RemainingBudget < store.MaxPrice)
            {
                explanation += $"• Budget constraints: Remaining budget (${store.RemainingBudget}) is insufficient for additional perfumes.\n\n";
            }

            // Check preference matches
            explanation += "• Preference matching issues:\n";

            // Check gender preference
            if (!string.IsNullOrEmpty(store.Gender))
            {
                explanation += $"  - Requested gender: {store.Gender}\n";
            }

            // Check accord preference
            if (!string.IsNullOrEmpty(store.PreferredAccord))
            {
                explanation += $"  - Requested accord: {store.PreferredAccord}\n";
            }

            // Check notes preferences
            if (!string.IsNullOrEmpty(store.PreferredTopNotes) ||
                !string.IsNullOrEmpty(store.PreferredMiddleNotes) ||
                !string.IsNullOrEmpty(store.PreferredBaseNotes))
            {
                explanation += "  - Requested notes: ";
                if (!string.IsNullOrEmpty(store.PreferredTopNotes))
                    explanation += $"Top: {store.PreferredTopNotes} ";
                if (!string.IsNullOrEmpty(store.PreferredMiddleNotes))
                    explanation += $"Middle: {store.PreferredMiddleNotes} ";
                if (!string.IsNullOrEmpty(store.PreferredBaseNotes))
                    explanation += $"Base: {store.PreferredBaseNotes}";
                explanation += "\n";
            }

            // Check quality requirements
            if (store.MinLongevity > 0 || store.MinProjection > 0)
            {
                explanation += $"  - Quality requirements: Min. Longevity {store.MinLongevity}/10, Min. Projection {store.MinProjection}/10\n";
            }

            explanation += "\nPossible solutions for next time:\n";
            explanation += "• Stock more perfumes matching these preferences\n";
            explanation += "• Suggest alternative perfumes with similar profiles\n";
            explanation += "• Consider adjusting the store's expectations or budget";

            return explanation;
        }

        private Color GetSatisfactionColor(double satisfaction)
        {
            if (satisfaction >= 90) return Color.FromArgb(46, 139, 87); // Green
            if (satisfaction >= 80) return Color.FromArgb(60, 179, 113); // Medium Sea Green
            if (satisfaction >= 70) return Color.FromArgb(46, 204, 113); // Emerald
            if (satisfaction >= 60) return Color.FromArgb(241, 196, 15); // Yellow
            if (satisfaction >= 50) return Color.FromArgb(243, 156, 18); // Orange
            return Color.FromArgb(231, 76, 60); // Red
        }

        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                ConfigureSaveFileDialog(saveFileDialog);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveResultsToFile(saveFileDialog.FileName);
                }
            }
        }

        private void ConfigureSaveFileDialog(SaveFileDialog saveFileDialog)
        {
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            saveFileDialog.Title = "Save Allocation Results";
            saveFileDialog.FileName = "PerfumeAllocationResults.csv";
        }

        private void SaveResultsToFile(string fileName)
        {
            if (_dataService.SaveAllocationResultsToCsv(
                (List<StoreRequirement>)dgvResults.DataSource, fileName))
            {
                MessageBox.Show("Results saved successfully", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClearStores_Click(object sender, EventArgs e)
        {
            if (ConfirmClearStores())
            {
                _storeRequirements.Clear();
                dgvStores.DataSource = null;
                dgvStores.DataSource = _storeRequirements;
                btnRunAllocation.Enabled = false;
            }
        }

        private bool ConfirmClearStores()
        {
            return MessageBox.Show("Are you sure you want to clear all stores?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ConfirmReset())
            {
                PerformFullReset();
                LoadDefaultCsvData();
            }
        }

        private bool ConfirmReset()
        {
            return MessageBox.Show("Are you sure you want to reset everything?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void PerformFullReset()
        {
            ClearCollections();
            ClearDataGrids();
            ClearDetailPanels();
            ResetLabels();
            DisableButtons();
            RemoveDynamicTabs();
        }

        private void ClearCollections()
        {
            _perfumes.Clear();
            _storeRequirements.Clear();
            _allocationEngine = null;
        }

        private void ClearDataGrids()
        {
            dgvPerfumes.DataSource = null;
            dgvStores.DataSource = null;
            dgvResults.DataSource = null;
        }

        private void ClearDetailPanels()
        {
            pnlAllocationDetails.Controls.Clear();
        }

        private void ResetLabels()
        {
            lblPerfumesSummary.Text = "";
            lblTotalProfit.Text = "Total Profit: $0.00";
        }

        private void DisableButtons()
        {
            btnAddStore.Enabled = false;
            btnGenerateRandomStore.Enabled = false;
            btnRunAllocation.Enabled = false;
            btnSaveResults.Enabled = false;
        }

        private void RemoveDynamicTabs()
        {
            for (int i = tabControl1.TabPages.Count - 1; i >= 3; i--)
            {
                tabControl1.TabPages.RemoveAt(i);
            }
        }

        // Add empty handler for CellContentClick if it's referenced in Designer
        private void dgvPerfumes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // This is an empty handler to fix the designer error
            // No action needed as we don't require cell click functionality
        }
    }
}