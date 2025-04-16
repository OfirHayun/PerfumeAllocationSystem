using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PerfumeAllocationSystem.Core;
using PerfumeAllocationSystem.Models;
using PerfumeAllocationSystem.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

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
        private const string DEFAULT_CSV_PATH = "fragrances.csv";

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();

            // Auto-load the CSV data on startup
            LoadDefaultCsvData();
        }

        private void InitializeUI()
        {
            // Set modern font
            Font = new Font("Segoe UI", 9F);

            // Set tab control style
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += tabControl1_DrawItem;

            // Style tabs
            tabPerfumes.BackColor = Color.FromArgb(245, 245, 245);
            tabStores.BackColor = Color.FromArgb(245, 245, 245);
            tabAllocation.BackColor = Color.FromArgb(245, 245, 245);

            // Style buttons
            ApplyButtonStyle(btnAddStore);
            ApplyButtonStyle(btnGenerateRandomStore);
            ApplyButtonStyle(btnLoadPerfumes);
            ApplyButtonStyle(btnRunAllocation);
            ApplyButtonStyle(btnSaveResults);
            ApplyButtonStyle(btnClearStores);
            ApplyButtonStyle(btnReset);

            // Add label for random store message
            lblRandomStoreMsg = new Label
            {
                AutoSize = true,
                ForeColor = Color.Green,
                Location = new Point(124, 500), // Changed position to be more visible
                Name = "lblRandomStoreMsg",
                Size = new Size(236, 40),
                Visible = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), // Made font bold
                TextAlign = ContentAlignment.MiddleCenter
            };
            groupBox1.Controls.Add(lblRandomStoreMsg);

            // Add timer for hiding message
            timerHideMsg = new Timer
            {
                Interval = 5000
            };
            timerHideMsg.Tick += timerHideMsg_Tick;

            // Set up DataGridView for Perfumes
            dgvPerfumes.AutoGenerateColumns = true;
            StyleDataGridView(dgvPerfumes);

            // Set up DataGridView for Store Requirements
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

            // Set up DataGridView for Allocation Results
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

            // Initialize comboboxes
            cboGender.Items.AddRange(new string[] { "Any", "Male", "Female", "Unisex" });
            cboGender.SelectedIndex = 0;

            // Populate main accord types (from your dataset)
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

            // Use method references instead of lambda expressions
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
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 100, 160);
            dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 100);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This event handler customizes the appearance of tab headers
            Graphics g = e.Graphics;
            TabPage tp = tabControl1.TabPages[e.Index];
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Create a background rectangle
            Rectangle rect = tabControl1.GetTabRect(e.Index);

            // Choose colors based on whether the tab is selected
            Brush textBrush, backBrush;
            if (tabControl1.SelectedIndex == e.Index)
            {
                textBrush = new SolidBrush(Color.White);
                backBrush = new SolidBrush(Color.FromArgb(60, 60, 100));
            }
            else
            {
                textBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
                backBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            }

            // Draw the custom tab
            g.FillRectangle(backBrush, rect);
            g.DrawString(tp.Text, new Font("Segoe UI", 9F), textBrush, rect, sf);

            // Clean up resources
            textBrush.Dispose();
            backBrush.Dispose();
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
                // Check if the default CSV file exists
                if (File.Exists(DEFAULT_CSV_PATH))
                {
                    _perfumes = _dataService.LoadPerfumesFromCsv(DEFAULT_CSV_PATH);
                    dgvPerfumes.DataSource = null;
                    dgvPerfumes.DataSource = _perfumes;

                    // Create allocation engine with loaded perfumes
                    _allocationEngine = new AllocationEngine(_perfumes);

                    // Show summary
                    lblPerfumesSummary.Text = $"Loaded {_perfumes.Count} perfumes";

                    // Enable add store button
                    btnAddStore.Enabled = true;
                    btnGenerateRandomStore.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"Default fragrance data file not found! Please place '{DEFAULT_CSV_PATH}' in the application folder or load a CSV file manually.",
                        "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading default fragrance data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadPerfumes_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                openFileDialog.Title = "Select Perfume Dataset";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _perfumes = _dataService.LoadPerfumesFromCsv(openFileDialog.FileName);
                    dgvPerfumes.DataSource = null;
                    dgvPerfumes.DataSource = _perfumes;

                    // Create allocation engine with loaded perfumes
                    _allocationEngine = new AllocationEngine(_perfumes);

                    // Show summary
                    lblPerfumesSummary.Text = $"Loaded {_perfumes.Count} perfumes";

                    // Enable add store button
                    btnAddStore.Enabled = true;
                    btnGenerateRandomStore.Enabled = true;
                }
            }
        }

        private void btnAddStore_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show("Please enter a store name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtBudget.Text, out decimal budget) || budget <= 0)
            {
                MessageBox.Show("Please enter a valid budget", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtMaxPrice.Text, out decimal maxPrice) || maxPrice <= 0)
            {
                MessageBox.Show("Please enter a valid maximum price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtMinLongevity.Text, out int minLongevity) || minLongevity < 0 || minLongevity > 10)
            {
                MessageBox.Show("Please enter a valid minimum longevity (0-10)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtMinProjection.Text, out int minProjection) || minProjection < 0 || minProjection > 10)
            {
                MessageBox.Show("Please enter a valid minimum projection (0-10)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create store requirement object
            StoreRequirement store = new StoreRequirement
            {
                StoreName = txtStoreName.Text,
                Budget = budget,
                QuantityNeeded = quantity,
                Gender = cboGender.SelectedItem.ToString() == "Any" ? "" : cboGender.SelectedItem.ToString(),
                PreferredAccord = cboAccord.SelectedItem.ToString() == "Any" ? "" : cboAccord.SelectedItem.ToString(),
                PreferredTopNotes = txtTopNotes.Text,
                PreferredMiddleNotes = txtMiddleNotes.Text,
                PreferredBaseNotes = txtBaseNotes.Text,
                MinLongevity = minLongevity,
                MinProjection = minProjection,
                MaxPrice = maxPrice
            };

            // Add to list
            _storeRequirements.Add(store);

            // Refresh grid
            UpdateStoreRequirementsGrid();

            // Clear inputs
            ClearStoreInputs();
        }

        private void btnGenerateRandomStore_Click(object sender, EventArgs e)
        {
            if (_perfumes.Count == 0)
            {
                MessageBox.Show("Please load perfume data first!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate a random store name
            string storeName = $"Store_{_random.Next(1, 1000)}";

            // Budget between $500 and $5000
            decimal budget = _random.Next(500, 5001);

            // Quantity between 5 and 20
            int quantity = _random.Next(5, 21);

            // Random gender preference
            string[] genders = { "Any", "Male", "Female", "Unisex" };
            string gender = genders[_random.Next(genders.Length)];

            // Random accord preference
            string[] accords = {
                "Any", "Aromatic", "Woody", "Fresh", "Sweet", "Floral", "Citrus",
                "Oriental", "Fruity", "Spicy", "Gourmand", "Leather", "Tobacco"
            };
            string accord = accords[_random.Next(accords.Length)];

            // Random notes (select from existing perfumes)
            string topNote = "";
            string middleNote = "";
            string baseNote = "";

            if (_perfumes.Count > 0)
            {
                // Pick random perfumes to extract notes from
                var randomPerfume1 = _perfumes[_random.Next(_perfumes.Count)];
                var randomPerfume2 = _perfumes[_random.Next(_perfumes.Count)];

                if (!string.IsNullOrEmpty(randomPerfume1.TopNotes))
                {
                    string[] notes = randomPerfume1.TopNotes.Split(',');
                    if (notes.Length > 0)
                        topNote = notes[0].Trim();
                }

                if (!string.IsNullOrEmpty(randomPerfume2.MiddleNotes))
                {
                    string[] notes = randomPerfume2.MiddleNotes.Split(',');
                    if (notes.Length > 0)
                        middleNote = notes[0].Trim();
                }

                // Get a base note from yet another random perfume
                var randomPerfume3 = _perfumes[_random.Next(_perfumes.Count)];
                if (!string.IsNullOrEmpty(randomPerfume3.BaseNotes))
                {
                    string[] notes = randomPerfume3.BaseNotes.Split(',');
                    if (notes.Length > 0)
                        baseNote = notes[0].Trim();
                }
            }

            // Random minimum longevity and projection
            int minLongevity = _random.Next(11); // 0-10
            int minProjection = _random.Next(11); // 0-10

            // Random max price (ensure it's at least 50 to give some options)
            decimal maxPrice = _random.Next(50, 401); // $50-$400

            // Just populate the form fields, don't add to the list
            txtStoreName.Text = storeName;
            txtBudget.Text = budget.ToString();
            txtQuantity.Text = quantity.ToString();
            cboGender.SelectedItem = gender;
            cboAccord.SelectedItem = accord;
            txtTopNotes.Text = topNote;
            txtMiddleNotes.Text = middleNote;
            txtBaseNotes.Text = baseNote;
            txtMinLongevity.Text = minLongevity.ToString();
            txtMinProjection.Text = minProjection.ToString();
            txtMaxPrice.Text = maxPrice.ToString();

            // Show a message to notify user to click Add Store
            lblRandomStoreMsg.Visible = true;
            lblRandomStoreMsg.Text = "Random values generated! Click 'Add Store' to add this store.";

            // Set a timer to hide the message after 5 seconds
            timerHideMsg.Start();
        }

        private void UpdateStoreRequirementsGrid()
        {
            dgvStores.DataSource = null;
            dgvStores.DataSource = _storeRequirements;

            // Enable run allocation button
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
            txtMinLongevity.Text = "0";
            txtMinProjection.Text = "0";
            txtMaxPrice.Text = "";
        }

        private void btnRunAllocation_Click(object sender, EventArgs e)
        {
            if (_allocationEngine == null)
            {
                MessageBox.Show("Please load perfumes first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_storeRequirements.Count == 0)
            {
                MessageBox.Show("Please add at least one store", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Run allocation
            List<StoreRequirement> results = _allocationEngine.AllocatePerfumes(_storeRequirements);

            // Display results in the standard results tab
            dgvResults.DataSource = null;
            dgvResults.DataSource = results;

            // Show profit
            decimal profit = _allocationEngine.GetTotalProfit();
            lblTotalProfit.Text = $"Total Profit: {profit:C}";

            // Enable save results button
            btnSaveResults.Enabled = true;

            // Show allocation details in the standard tab
            ShowAllocationDetails(results);

            // Create a new results tab with better UI
            CreateResultsTab(results, profit);
        }

        private void ShowAllocationDetails(List<StoreRequirement> results)
        {
            // Clear existing panels
            pnlAllocationDetails.Controls.Clear();

            int yPos = 10;

            foreach (var store in results)
            {
                // Create a group box for each store
                GroupBox groupBox = new GroupBox
                {
                    Text = store.StoreName,
                    Width = pnlAllocationDetails.Width - 20,
                    Height = 200,
                    Location = new Point(10, yPos)
                };

                // Add satisfaction label
                Label lblSatisfaction = new Label
                {
                    Text = $"Satisfaction: {store.SatisfactionPercentage:F2}%",
                    Location = new Point(10, 20),
                    AutoSize = true
                };
                groupBox.Controls.Add(lblSatisfaction);

                // Add budget label
                Label lblBudget = new Label
                {
                    Text = $"Budget: {store.Budget:C} | Spent: {store.TotalSpent:C} | Remaining: {store.RemainingBudget:C}",
                    Location = new Point(10, 40),
                    AutoSize = true
                };
                groupBox.Controls.Add(lblBudget);

                // Add quantity label
                Label lblQuantity = new Label
                {
                    Text = $"Requested: {store.QuantityNeeded} | Allocated: {store.AllocatedPerfumes.Count} | Remaining: {store.RemainingQuantity}",
                    Location = new Point(10, 60),
                    AutoSize = true
                };
                groupBox.Controls.Add(lblQuantity);

                // Add a list of allocated perfumes
                Label lblPerfumes = new Label
                {
                    Text = "Allocated Perfumes:",
                    Location = new Point(10, 80),
                    AutoSize = true
                };
                groupBox.Controls.Add(lblPerfumes);

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

                storeCard.Controls.Add(lstPerfumes);

                // Add card to table layout
                storeCardsPanel.Controls.Add(storeCard, col, row);

                // Move to next column or row
                col++;
                if (col >= 3)
                {
                    col = 0;
                    row++;
                }
            }

            // Add store cards panel to main panel
            mainPanel.Controls.Add(storeCardsPanel);

            // Add main panel to tab
            tabResult.Controls.Add(mainPanel);

            // Add tab to tab control
            tabControl1.TabPages.Add(tabResult);

            // Select the new tab
            tabControl1.SelectedTab = tabResult;
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
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Allocation Results";
                saveFileDialog.FileName = "PerfumeAllocationResults.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (_dataService.SaveAllocationResultsToCsv(
                        (List<StoreRequirement>)dgvResults.DataSource, saveFileDialog.FileName))
                    {
                        MessageBox.Show("Results saved successfully", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnClearStores_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all stores?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _storeRequirements.Clear();
                dgvStores.DataSource = null;
                dgvStores.DataSource = _storeRequirements;
                btnRunAllocation.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset everything?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Clear everything
                _perfumes.Clear();
                _storeRequirements.Clear();

                dgvPerfumes.DataSource = null;
                dgvStores.DataSource = null;
                dgvResults.DataSource = null;

                pnlAllocationDetails.Controls.Clear();

                lblPerfumesSummary.Text = "";
                lblTotalProfit.Text = "Total Profit: $0.00";

                btnAddStore.Enabled = false;
                btnGenerateRandomStore.Enabled = false;
                btnRunAllocation.Enabled = false;
                btnSaveResults.Enabled = false;

                _allocationEngine = null;

                // Remove any dynamically added results tabs
                for (int i = tabControl1.TabPages.Count - 1; i >= 3; i--)
                {
                    tabControl1.TabPages.RemoveAt(i);
                }

                // Reload the default CSV
                LoadDefaultCsvData();
            }
        }
    }
}
ume.AveragePrice})");
                }
                
                groupBox.Controls.Add(lstPerfumes);

// Add to panel
pnlAllocationDetails.Controls.Add(groupBox);

// Increment Y position for next group box
yPos += groupBox.Height + 10;
            }
        }

        private void CreateResultsTab(List<StoreRequirement> results, decimal totalProfit)
{
    // Create a new tab
    TabPage tabResult = new TabPage($"Results {DateTime.Now.ToString("HH:mm:ss")}");

    // Create the main panel
    Panel mainPanel = new Panel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        BackColor = Color.FromArgb(245, 245, 245)
    };

    // Add a header panel
    Panel headerPanel = new Panel
    {
        Dock = DockStyle.Top,
        Height = 80,
        BackColor = Color.FromArgb(60, 60, 100)
    };

    // Add title label
    Label lblTitle = new Label
    {
        Text = "Allocation Results",
        Font = new Font("Segoe UI", 16, FontStyle.Bold),
        ForeColor = Color.White,
        AutoSize = true,
        Location = new Point(20, 15)
    };

    // Add timestamp
    Label lblTimestamp = new Label
    {
        Text = $"Generated on {DateTime.Now.ToString("MMMM d, yyyy")} at {DateTime.Now.ToString("h:mm tt")}",
        Font = new Font("Segoe UI", 9),
        ForeColor = Color.FromArgb(220, 220, 220),
        AutoSize = true,
        Location = new Point(20, 45)
    };

    // Add profit info
    Label lblProfit = new Label
    {
        Text = $"Total Profit: {totalProfit:C}",
        Font = new Font("Segoe UI", 12, FontStyle.Bold),
        ForeColor = Color.White,
        AutoSize = true,
        Location = new Point(mainPanel.Width - 200, 25),
        Anchor = AnchorStyles.Top | AnchorStyles.Right
    };

    // Add summary label
    Label lblSummary = new Label
    {
        Text = $"Allocated perfumes for {results.Count} stores",
        Font = new Font("Segoe UI", 10),
        ForeColor = Color.FromArgb(220, 220, 220),
        AutoSize = true,
        Location = new Point(mainPanel.Width - 200, 50),
        Anchor = AnchorStyles.Top | AnchorStyles.Right
    };

    // Add controls to header
    headerPanel.Controls.Add(lblTitle);
    headerPanel.Controls.Add(lblTimestamp);
    headerPanel.Controls.Add(lblProfit);
    headerPanel.Controls.Add(lblSummary);

    // Add header to main panel
    mainPanel.Controls.Add(headerPanel);

    // Create content panel for store cards
    TableLayoutPanel storeCardsPanel = new TableLayoutPanel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        Padding = new Padding(20),
        ColumnCount = 3, // Display 3 cards per row
        RowCount = (results.Count / 3) + 1, // Calculate number of rows needed
        CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
        BackColor = Color.FromArgb(245, 245, 245)
    };

    // Set column widths
    for (int i = 0; i < 3; i++)
    {
        storeCardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
    }

    // Set row heights automatically
    for (int i = 0; i < storeCardsPanel.RowCount; i++)
    {
        storeCardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    }

    // Add store cards
    int col = 0;
    int row = 0;

    foreach (var store in results)
    {
        // Create card panel
        Panel storeCard = new Panel
        {
            Width = 300,
            Height = 300,
            Margin = new Padding(5),
            BackColor = Color.White,
            BorderStyle = BorderStyle.None,
            Dock = DockStyle.Fill
        };

        // Add store name header
        Panel storeHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = Color.FromArgb(60, 60, 100)
        };

        Label lblStoreName = new Label
        {
            Text = store.StoreName,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(10, 10)
        };
        storeHeader.Controls.Add(lblStoreName);
        storeCard.Controls.Add(storeHeader);

        // Add satisfaction panel
        Panel satisfactionPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 30,
            BackColor = GetSatisfactionColor(store.SatisfactionPercentage),
            Top = storeHeader.Bottom
        };

        Label lblSatisfaction = new Label
        {
            Text = $"Satisfaction: {store.SatisfactionPercentage:F2}%",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(10, 5)
        };
        satisfactionPanel.Controls.Add(lblSatisfaction);
        storeCard.Controls.Add(satisfactionPanel);

        // Add budget info
        Label lblBudgetInfo = new Label
        {
            Text = $"Budget: {store.Budget:C}\nSpent: {store.TotalSpent:C}\nRemaining: {store.RemainingBudget:C}",
            Font = new Font("Segoe UI", 9),
            Location = new Point(10, storeHeader.Height + satisfactionPanel.Height + 10),
            Size = new Size(280, 50)
        };
        storeCard.Controls.Add(lblBudgetInfo);

        // Add allocation info
        Label lblAllocationInfo = new Label
        {
            Text = $"Requested: {store.QuantityNeeded}\nAllocated: {store.AllocatedPerfumes.Count}\nRemaining: {store.RemainingQuantity}",
            Font = new Font("Segoe UI", 9),
            Location = new Point(10, lblBudgetInfo.Bottom + 5),
            Size = new Size(280, 50)
        };
        storeCard.Controls.Add(lblAllocationInfo);

        // Add perfumes list
        Label lblPerfumesList = new Label
        {
            Text = "Allocated Perfumes:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Location = new Point(10, lblAllocationInfo.Bottom + 5),
            AutoSize = true
        };
        storeCard.Controls.Add(lblPerfumesList);

        ListBox lstPerfumes = new ListBox
        {
            Location = new Point(10, lblPerfumesList.Bottom + 5),
            Size = new Size(280, 95),
            Font = new Font("Segoe UI", 8),
            BorderStyle = BorderStyle.FixedSingle
        };

        foreach (var perfume in store.AllocatedPerfumes)
        {
            lstPerfumes.Items.Add($"{perfume.Brand} - {perfume.Name} (${perf