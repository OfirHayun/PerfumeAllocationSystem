using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
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
        private SimpleAllocationEngine _simpleAllocationEngine; // Add this field
        private List<StoreRequirement> _lastOptimizedResults; // To store the last optimized results
        private Button _compareAlgorithmsButton; // Store the button reference
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
            txtMinLongevity.Text = "1";
            txtMinProjection.Text = "1";

            // Add validators to prevent entering zero or invalid values
            txtMinLongevity.KeyPress += NumericTextBox_KeyPress;
            txtMinProjection.KeyPress += NumericTextBox_KeyPress;

            // Add double-click event to view store requirements
            dgvStores.CellDoubleClick += dgvStores_CellDoubleClick;

            // Create the compare algorithms button (but don't add it yet)
            CreateCompareAlgorithmsButton();
        }

        private void CreateCompareAlgorithmsButton()
        {
            // Create button with color scheme matching existing application buttons
            _compareAlgorithmsButton = new Button
            {
                Text = "Compare Algorithms",
                Width = 200,
                Height = 35,
                // Match the same purple color used in other buttons
                BackColor = Color.FromArgb(60, 60, 100),  // Dark purple to match your app
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false,
                Name = "btnCompareAlgorithms",
                Cursor = Cursors.Hand
            };

            // Remove the default button border for a cleaner look
            _compareAlgorithmsButton.FlatAppearance.BorderSize = 0;

            // Add hover effect to match other buttons
            _compareAlgorithmsButton.MouseEnter += (s, e) => {
                if (_compareAlgorithmsButton.Enabled)
                    _compareAlgorithmsButton.BackColor = Color.FromArgb(80, 80, 120); // Match your hover color
            };

            _compareAlgorithmsButton.MouseLeave += (s, e) => {
                if (_compareAlgorithmsButton.Enabled)
                    _compareAlgorithmsButton.BackColor = Color.FromArgb(60, 60, 100); // Back to original purple
            };

            _compareAlgorithmsButton.Click += btnCompareAlgorithms_Click;
        }
        private void btnCompareAlgorithms_Click(object sender, EventArgs e)
        {
            if (_lastOptimizedResults == null || _lastOptimizedResults.Count == 0)
            {
                MessageBox.Show("Please run the allocation process first", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;

            try
            {
                // Create a simple allocation engine if it doesn't exist
                if (_simpleAllocationEngine == null && _perfumes.Count > 0)
                {
                    _simpleAllocationEngine = new SimpleAllocationEngine(_perfumes);
                }

                // Get a fresh copy of the store requirements (to ensure we start fresh)
                List<StoreRequirement> freshStoreRequirements = _storeRequirements.Select(s => s.Clone()).ToList();

                // Run the simple allocation
                List<StoreRequirement> simpleResults = _simpleAllocationEngine.AllocatePerfumes(freshStoreRequirements);
                decimal simpleProfit = _simpleAllocationEngine.GetTotalProfit();

                // Get the profit from the optimized algorithm
                decimal optimizedProfit = _allocationEngine.GetTotalProfit();

                // Show the comparison report
                ComparisonReportForm reportForm = new ComparisonReportForm(
                    _lastOptimizedResults, optimizedProfit,
                    simpleResults, simpleProfit);

                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating comparison: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Add this method to view store requirements
        private void ShowStoreRequirements(StoreRequirement store)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Requirements for {store.StoreName}:");
            sb.AppendLine($"Budget: {store.Budget:C}");
            sb.AppendLine($"Quantity Needed: {store.QuantityNeeded}");
            sb.AppendLine($"Maximum Price per Perfume: {store.MaxPrice:C}");
            sb.AppendLine();

            sb.AppendLine("Perfume Preferences:");
            sb.AppendLine($"Gender: {(string.IsNullOrEmpty(store.Gender) ? "Any" : store.Gender)}");
            sb.AppendLine($"Preferred Accord: {(string.IsNullOrEmpty(store.PreferredAccord) ? "Any" : store.PreferredAccord)}");

            if (!string.IsNullOrEmpty(store.PreferredTopNotes))
                sb.AppendLine($"Preferred Top Notes: {store.PreferredTopNotes}");

            if (!string.IsNullOrEmpty(store.PreferredMiddleNotes))
                sb.AppendLine($"Preferred Middle Notes: {store.PreferredMiddleNotes}");

            if (!string.IsNullOrEmpty(store.PreferredBaseNotes))
                sb.AppendLine($"Preferred Base Notes: {store.PreferredBaseNotes}");

            sb.AppendLine($"Minimum Longevity: {store.MinLongevity}/10");
            sb.AppendLine($"Minimum Projection: {store.MinProjection}/10");

            MessageBox.Show(sb.ToString(), $"{store.StoreName} Requirements",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Add this method to create a button to view requirements
        private void AddViewRequirementsButton(Control container, StoreRequirement store, Point location, Size size)
        {
            Button btnViewRequirements = new Button
            {
                Text = "View Requirements",
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(80, 80, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnViewRequirements.MouseEnter += (s, e) => btnViewRequirements.BackColor = Color.FromArgb(100, 100, 140);
            btnViewRequirements.MouseLeave += (s, e) => btnViewRequirements.BackColor = Color.FromArgb(80, 80, 120);
            btnViewRequirements.Click += (s, e) => ShowStoreRequirements(store);

            container.Controls.Add(btnViewRequirements);
        }

        private void dgvStores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _storeRequirements.Count)
            {
                ShowStoreRequirements(_storeRequirements[e.RowIndex]);
            }
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
            ApplyButtonStyle(btnRunAllocation);
            ApplyButtonStyle(btnClearStores);
            ApplyButtonStyle(btnReset);
            // Removed btnLoadPerfumes and btnSaveResults references
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

                    // Create a small test dataset to allow the application to function
                    // even if the CSV file is missing
                    CreateDefaultPerfumeData();
                }

                // Ensure the allocation engine is always created if we have perfumes
                if (_allocationEngine == null && _perfumes.Count > 0)
                {
                    _allocationEngine = new AllocationEngine(_perfumes);
                    lblPerfumesSummary.Text = $"Loaded {_perfumes.Count} perfumes";
                    btnAddStore.Enabled = true;
                    btnGenerateRandomStore.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ShowLoadErrorMessage(ex);
                // Create minimal perfume data to allow the app to function
                CreateDefaultPerfumeData();
            }
        }

        // This method creates a minimal set of test perfumes if the CSV file is missing
        private void CreateDefaultPerfumeData()
        {
            _perfumes = new List<Perfume>();

            // Add a few sample perfumes
            _perfumes.Add(new Perfume
            {
                Name = "Sample Perfume 1",
                Brand = "Test Brand",
                Gender = "Male",
                TopNotes = "Citrus, Bergamot",
                MiddleNotes = "Lavender, Rosemary",
                BaseNotes = "Amber, Musk",
                MainAccord = "Aromatic",
                Longevity = 7,
                Projection = 6,
                AveragePrice = 85.0m,
                Stock = 10
            });

            _perfumes.Add(new Perfume
            {
                Name = "Sample Perfume 2",
                Brand = "Test Brand",
                Gender = "Female",
                TopNotes = "Rose, Violet",
                MiddleNotes = "Jasmine, Ylang-ylang",
                BaseNotes = "Vanilla, Sandalwood",
                MainAccord = "Floral",
                Longevity = 6,
                Projection = 5,
                AveragePrice = 95.0m,
                Stock = 8
            });

            _perfumes.Add(new Perfume
            {
                Name = "Sample Perfume 3",
                Brand = "Test Brand",
                Gender = "Unisex",
                TopNotes = "Cedar, Pine",
                MiddleNotes = "Patchouli, Vetiver",
                BaseNotes = "Leather, Tobacco",
                MainAccord = "Woody",
                Longevity = 8,
                Projection = 7,
                AveragePrice = 110.0m,
                Stock = 5
            });

            // Update the data grid and create an allocation engine
            dgvPerfumes.DataSource = null;
            dgvPerfumes.DataSource = _perfumes;
            _allocationEngine = new AllocationEngine(_perfumes);

            lblPerfumesSummary.Text = $"Using {_perfumes.Count} sample perfumes (CSV file not found)";
            btnAddStore.Enabled = true;
            btnGenerateRandomStore.Enabled = true;
        }

        private void LoadCsvFile(string filePath)
        {
            _perfumes = _dataService.LoadPerfumesFromCsv(filePath);
            dgvPerfumes.DataSource = null;
            dgvPerfumes.DataSource = _perfumes;

            // Make sure we always create the allocation engine
            _allocationEngine = new AllocationEngine(_perfumes);
            lblPerfumesSummary.Text = $"Loaded {_perfumes.Count} perfumes";

            btnAddStore.Enabled = true;
            btnGenerateRandomStore.Enabled = true;
        }

        private void ShowDefaultFileNotFoundMessage()
        {
            MessageBox.Show($"Default fragrance data file not found! Please place '{DEFAULT_CSV_PATH}' in the application folder.",
                "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowLoadErrorMessage(Exception ex)
        {
            MessageBox.Show($"Error loading default fragrance data: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Removed the btnLoadPerfumes_Click method entirely

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

            // Add minimum price check - important new validation!
            if (maxPrice < GetMinimumPerfumePrice())
            {
                MessageBox.Show($"Maximum price is too low. Minimum perfume price is ${GetMinimumPerfumePrice()}",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidateLongevityProjection())
                return false;

            // Check if the criteria are too restrictive
            if (CheckIfCriteriaTooRestrictive())
            {
                var result = MessageBox.Show("The criteria you've entered are very restrictive. You might want to relax some requirements.\n" +
                    "Do you want to continue anyway?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return false;
            }

            return true;
        }
        private decimal GetMinimumPerfumePrice()
        {
            if (_perfumes.Count == 0) return 0;
            return _perfumes.Min(p => p.AveragePrice);
        }

        private decimal GetAveragePerfumePrice()
        {
            if (_perfumes.Count == 0) return 100m; // Default fallback
            return _perfumes.Average(p => p.AveragePrice);
        }

        private decimal GetMaximumPerfumePrice()
        {
            if (_perfumes.Count == 0) return 500m; // Default fallback
            return _perfumes.Max(p => p.AveragePrice);
        }

        private bool CheckIfCriteriaTooRestrictive()
        {
            // Check if there are at least some perfumes that match the basic criteria
            decimal maxPrice = decimal.Parse(txtMaxPrice.Text);
            int minLongevity = int.Parse(txtMinLongevity.Text);
            int minProjection = int.Parse(txtMinProjection.Text);
            string gender = GetSelectedGender();
            string accord = GetSelectedAccord();

            int matchCount = _perfumes.Count(p =>
                p.AveragePrice <= maxPrice &&
                p.Stock > 0 &&
                (string.IsNullOrEmpty(gender) || p.Gender == gender || p.Gender == "Unisex") &&
                (string.IsNullOrEmpty(accord) || p.MainAccord == accord) &&
                p.Longevity >= minLongevity &&
                p.Projection >= minProjection
            );

            return matchCount < 5; // If less than 5 perfumes match, criteria might be too restrictive
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
            GenerateImprovedQualitySettings();
        }

        private void GenerateBasicStoreInfo()
        {
            txtStoreName.Text = $"Store_{_random.Next(1, 1000)}";

            // Increase budget range for better allocation possibilities
            txtBudget.Text = _random.Next(800, 8001).ToString();

            // Slightly lower quantity requests to balance with budget
            txtQuantity.Text = _random.Next(3, 15).ToString();

            // Higher chance of selecting "Any" gender for more flexibility
            string[] genders = { "Any", "Any", "Male", "Female", "Unisex" };
            cboGender.SelectedItem = genders[_random.Next(genders.Length)];

            // Higher chance of selecting "Any" accord for more flexibility
            string[] accords = {
                "Any", "Any", "Any", // Added more "Any" options to increase probability
                "Aromatic", "Woody", "Fresh", "Sweet", "Floral", "Citrus",
                "Oriental", "Fruity", "Spicy"
            };
            cboAccord.SelectedItem = accords[_random.Next(accords.Length)];
        }

        private void GenerateRandomNotes()
        {
            // 40% chance of having no specific notes requirement
            if (_random.Next(100) < 40)
            {
                txtTopNotes.Text = "";
                txtMiddleNotes.Text = "";
                txtBaseNotes.Text = "";
                return;
            }

            string topNote = "";
            string middleNote = "";
            string baseNote = "";

            if (_perfumes.Count > 0)
            {
                // Get the most common notes to increase match probability
                ExtractCommonNotesFromPerfumes(ref topNote, ref middleNote, ref baseNote);
            }

            // 50% chance for each note to be used (allows for partial notes specification)
            txtTopNotes.Text = _random.Next(100) < 50 ? topNote : "";
            txtMiddleNotes.Text = _random.Next(100) < 50 ? middleNote : "";
            txtBaseNotes.Text = _random.Next(100) < 50 ? baseNote : "";
        }

        private void ExtractCommonNotesFromPerfumes(ref string topNote, ref string middleNote, ref string baseNote)
        {
            // Sample multiple perfumes to find common notes
            var sampleSize = Math.Min(10, _perfumes.Count);
            var samplePerfumes = new List<Perfume>();

            // Take a random sample of perfumes
            for (int i = 0; i < sampleSize; i++)
            {
                samplePerfumes.Add(_perfumes[_random.Next(_perfumes.Count)]);
            }

            // Extract and count top notes
            var topNotes = new Dictionary<string, int>();
            var middleNotes = new Dictionary<string, int>();
            var baseNotes = new Dictionary<string, int>();

            foreach (var perfume in samplePerfumes)
            {
                ProcessNotes(perfume.TopNotes, topNotes);
                ProcessNotes(perfume.MiddleNotes, middleNotes);
                ProcessNotes(perfume.BaseNotes, baseNotes);
            }

            // Select most common notes
            topNote = GetMostCommonNote(topNotes);
            middleNote = GetMostCommonNote(middleNotes);
            baseNote = GetMostCommonNote(baseNotes);
        }

        private void ProcessNotes(string notesList, Dictionary<string, int> noteCount)
        {
            if (!string.IsNullOrEmpty(notesList))
            {
                string[] notes = notesList.Split(',');
                foreach (var note in notes)
                {
                    string trimmedNote = note.Trim();
                    if (!string.IsNullOrEmpty(trimmedNote))
                    {
                        if (noteCount.ContainsKey(trimmedNote))
                            noteCount[trimmedNote]++;
                        else
                            noteCount[trimmedNote] = 1;
                    }
                }
            }
        }

        private string GetMostCommonNote(Dictionary<string, int> noteCount)
        {
            if (noteCount.Count == 0)
                return "";

            return noteCount.OrderByDescending(x => x.Value).First().Key;
        }

        private void GenerateImprovedQualitySettings()
        {
            // Lower minimum requirements for better match probability but with some variation
            txtMinLongevity.Text = _random.Next(1, 8).ToString();
            txtMinProjection.Text = _random.Next(1, 8).ToString();

            // Get price ranges
            decimal minPrice = GetMinimumPerfumePrice();
            decimal avgPrice = GetAveragePerfumePrice();
            decimal maxPrice = GetMaximumPerfumePrice();

            // Add randomization factor (between 60% and 95% of max price)
            decimal randomFactor = (decimal)(_random.NextDouble() * 0.35 + 0.60);
            decimal targetPrice = maxPrice * randomFactor;

            // Ensure it's at least 20% above min price
            targetPrice = Math.Max(targetPrice, minPrice * 1.2m);

            // Round to nearest $10
            targetPrice = Math.Round(targetPrice / 10) * 10;

            txtMaxPrice.Text = targetPrice.ToString();
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
            // Additional check to ensure the allocation engine is initialized
            if (_allocationEngine == null && _perfumes.Count > 0)
            {
                _allocationEngine = new AllocationEngine(_perfumes);
            }

            // Verify again that we have an allocation engine
            if (_allocationEngine == null)
            {
                MessageBox.Show("Cannot run allocation - perfume data could not be loaded. Please check that the CSV file exists.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Before allocation, check if stores have any matching perfumes
            var problematicStores = new List<string>();

            foreach (var store in _storeRequirements)
            {
                int matchingPerfumes = CountMatchingPerfumes(store);
                if (matchingPerfumes == 0)
                {
                    problematicStores.Add(store.StoreName);
                }
            }

            if (problematicStores.Any())
            {
                string names = string.Join(", ", problematicStores);
                var result = MessageBox.Show($"Warning: The following stores have no matching perfumes: {names}\n" +
                    "Consider relaxing their requirements. Do you want to continue anyway?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return;
            }

            List<StoreRequirement> results = _allocationEngine.AllocatePerfumes(_storeRequirements);

            // Store the results for comparison
            _lastOptimizedResults = results;

            // Enable the compare button now that we have results
            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Enabled = true;
            }

            // Check for stores with 0% satisfaction after allocation
            var zeroSatisfactionStores = results.Where(s => s.SatisfactionPercentage == 0).ToList();
            if (zeroSatisfactionStores.Any())
            {
                string names = string.Join(", ", zeroSatisfactionStores.Select(s => s.StoreName));
                MessageBox.Show($"The following stores received 0% satisfaction: {names}\n" +
                    "This is likely due to very restrictive criteria or insufficient budget.",
                    "Allocation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            DisplayResults(results);
            EnableResultsFeatures();
            decimal profit = _allocationEngine.GetTotalProfit();
            ShowAllocationDetails(results);
            CreateResultsTab(results, profit);
            DisplayTotalProfitSummary(profit);
        }

        private int CountMatchingPerfumes(StoreRequirement store)
        {
            return _perfumes.Count(p =>
                p.AveragePrice <= store.MaxPrice &&
                p.Stock > 0 &&
                (string.IsNullOrEmpty(store.Gender) || p.Gender == store.Gender || p.Gender == "Unisex") &&
                p.Longevity >= store.MinLongevity &&
                p.Projection >= store.MinProjection
            );
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
            // Removed btnSaveResults.Enabled = true;
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

            // Add view requirements button
            AddViewRequirementsButton(groupBox, store, new Point(groupBox.Width - 150, 20), new Size(130, 25));

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

        private void DisplayTotalProfitSummary(decimal profit)
        {
            // Remove any existing profit panels first
            Panel panelToRemove = null;

            // First, find the panel (if it exists)
            foreach (Control ctrl in tabAllocation.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "profitSummaryPanel")
                {
                    panelToRemove = (Panel)ctrl;
                }
            }

            // Then, remove it if found
            if (panelToRemove != null)
            {
                tabAllocation.Controls.Remove(panelToRemove);
                panelToRemove.Dispose();
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

            // Add the compare algorithms button to the summary panel
            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Location = new Point(800, 15); // Position in the yellow area
                _compareAlgorithmsButton.Enabled = true;
                profitPanel.Controls.Add(_compareAlgorithmsButton);
            }

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

            // Add the compare algorithms button to this tab's header panel
            Button tabCompareButton = new Button
            {
                Text = "Compare Algorithms",
                Width = 200,
                Height = 35,
                BackColor = Color.FromArgb(60, 60, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Name = "tabCompareButton",
                Location = new Point(800, 15) // Position on the right side
            };

            tabCompareButton.Click += btnCompareAlgorithms_Click;
            ApplyButtonStyle(tabCompareButton);
            headerPanel.Controls.Add(tabCompareButton);

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
                    Size = new Size(storePanel.Width - 15, 205), // Shorter to make room for buttons
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

                // Add View Requirements button
                AddViewRequirementsButton(storePanel, store, new Point(5, 325), new Size(storePanel.Width - 15, 30));

                // Add Expand button for stores with satisfaction under 70%
                if (store.SatisfactionPercentage < 70.0)
                {
                    Button btnExpand = new Button
                    {
                        Text = "Explain Low Satisfaction",
                        Location = new Point(5, 360), // Position below the view requirements button
                        Size = new Size(storePanel.Width - 15, 30),
                        BackColor = Color.FromArgb(180, 180, 180),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Tag = GetSatisfactionExplanation(store), // Store explanation in tag
                        Cursor = Cursors.Hand
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

            // Check if actually allocated anything
            if (store.AllocatedPerfumes.Count == 0)
            {
                explanation += "• NO PERFUMES WERE ALLOCATED\n";
                explanation += "  This could be due to:\n";
                explanation += "  - No perfumes match your criteria\n";
                explanation += "  - Budget is insufficient\n";
                explanation += "  - All matching perfumes are out of stock\n\n";

                int affordablePerfumes = _perfumes.Count(p => p.AveragePrice <= store.MaxPrice);
                explanation += $"• Affordable perfumes (within ${store.MaxPrice}): {affordablePerfumes}\n";

                int stockedAndAffordable = _perfumes.Count(p => p.AveragePrice <= store.MaxPrice && p.Stock > 0);
                explanation += $"• In stock and affordable: {stockedAndAffordable}\n\n";
            }
            else
            {
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
                    int matchingGender = store.AllocatedPerfumes.Count(p => p.Gender == store.Gender || p.Gender == "Unisex");
                    explanation += $"  - Gender match: {matchingGender}/{store.AllocatedPerfumes.Count} perfumes\n";
                }

                // Check accord preference
                if (!string.IsNullOrEmpty(store.PreferredAccord))
                {
                    int matchingAccord = store.AllocatedPerfumes.Count(p => p.MainAccord == store.PreferredAccord);
                    explanation += $"  - Accord match: {matchingAccord}/{store.AllocatedPerfumes.Count} perfumes\n";
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
            }

            explanation += "\nPossible solutions for next time:\n";
            explanation += "• Stock more perfumes matching these preferences\n";
            explanation += "• Suggest alternative perfumes with similar profiles\n";
            explanation += "• Consider adjusting the store's expectations or budget\n";
            explanation += "• Try increasing the maximum price limit\n";
            explanation += "• Reduce the minimum requirements for longevity and projection";

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

        // Removed the btnSaveResults_Click method entirely

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

        // Enhanced PerformFullReset method to properly clear ALL allocation data
        private void PerformFullReset()
        {
            // Clear data collections
            ClearCollections();

            // Clear data grids
            ClearDataGrids();

            // Clear panels and results
            ClearAllocationResults();

            // Reset labels
            ResetLabels();

            // Disable buttons
            DisableButtons();

            // Remove dynamic tabs
            RemoveDynamicTabs();

            // Reset the last optimized results
            _lastOptimizedResults = null;

            // Clear all allocation engines
            _allocationEngine = null;
            _simpleAllocationEngine = null;

            // Ensure we have a new DataService
            _dataService = new DataService();

            // Switch to the first tab
            tabControl1.SelectedIndex = 0;
        }
        // New method to thoroughly clear all allocation results
        private void ClearAllocationResults()
        {
            // Clear the details panel
            pnlAllocationDetails.Controls.Clear();

            // Remove profit summary panel if it exists
            Panel panelToRemove = null;

            // First, find the panel (if it exists)
            foreach (Control ctrl in tabAllocation.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "profitSummaryPanel")
                {
                    panelToRemove = (Panel)ctrl;
                }
            }

            // Then, remove it if found
            if (panelToRemove != null)
            {
                tabAllocation.Controls.Remove(panelToRemove);
                panelToRemove.Dispose();
            }

            // Clear any data binding
            dgvResults.DataSource = null;

            // Reset profit
            lblTotalProfit.Text = "Total Profit: $0.00";
        }

        // Modified btnReset_Click method to ensure all UI is updated after reset
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ConfirmReset())
            {
                PerformFullReset();
                LoadDefaultCsvData();

                // Force refresh the UI
                Application.DoEvents();

                // Focus on the first tab and update display
                tabControl1.SelectedIndex = 0;
                UpdateDisplay();
            }
        }

        // New method to update the display after major changes
        private void UpdateDisplay()
        {
            // Clear and refresh data grids
            dgvPerfumes.Refresh();
            dgvStores.Refresh();
            dgvResults.Refresh();

            // Update total profit display
            lblTotalProfit.Text = "Total Profit: $0.00";

            // Force redraw of the allocation panel
            pnlAllocationDetails.Invalidate();
            pnlAllocationDetails.Update();

            // Update tab control display
            tabControl1.Invalidate();
            tabControl1.Update();
        }

        private bool ConfirmReset()
        {
            return MessageBox.Show("Are you sure you want to reset everything?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
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

            // Disable the main compare algorithms button if it exists
            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Enabled = false;
            }

            // Disable any compare buttons on result panels
            foreach (TabPage tab in tabControl1.TabPages)
            {
                foreach (Control ctrl in tab.Controls)
                {
                    if (ctrl is Panel)
                    {
                        foreach (Control panelCtrl in ctrl.Controls)
                        {
                            if (panelCtrl is Button && (panelCtrl.Name == "btnCompareAlgorithms" || panelCtrl.Name == "tabCompareButton"))
                            {
                                panelCtrl.Enabled = false;
                            }
                        }
                    }
                }
            }
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