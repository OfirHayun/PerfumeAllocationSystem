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
        private SimpleAllocationEngine _simpleAllocationEngine;
        private List<StoreRequirement> _lastOptimizedResults;
        private Button _compareAlgorithmsButton;
        private DataService _dataService = new DataService();
        private Random _random = new Random();
        private Timer timerHideMsg;
        private Label lblRandomStoreMsg;

        private const string DEFAULT_CSV_PATH = "fragrances-database.csv";

        // Initializes the form and loads default data
        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
            LoadDefaultCsvData();
        }

        // Sets up all UI components and their initial configurations
        private void InitializeUI()
        {
            InitializeFormStyle();
            SetupTabControl();
            InitializeLabelsAndTimers();
            SetupDataGridViews();
            InitializeComboBoxes();

            txtMinLongevity.Text = "1";
            txtMinProjection.Text = "1";

            txtMinLongevity.KeyPress += NumericTextBox_KeyPress;
            txtMinProjection.KeyPress += NumericTextBox_KeyPress;

            dgvStores.CellDoubleClick += dgvStores_CellDoubleClick;

            CreateCompareAlgorithmsButton();
        }

        // Creates the compare algorithms button with styling
        private void CreateCompareAlgorithmsButton()
        {
            _compareAlgorithmsButton = new Button
            {
                Text = "Compare Algorithms",
                Width = 200,
                Height = 35,
                BackColor = Color.FromArgb(60, 60, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false,
                Name = "btnCompareAlgorithms",
                Cursor = Cursors.Hand
            };

            _compareAlgorithmsButton.FlatAppearance.BorderSize = 0;

            _compareAlgorithmsButton.MouseEnter += (s, e) => {
                if (_compareAlgorithmsButton.Enabled)
                    _compareAlgorithmsButton.BackColor = Color.FromArgb(80, 80, 120);
            };

            _compareAlgorithmsButton.MouseLeave += (s, e) => {
                if (_compareAlgorithmsButton.Enabled)
                    _compareAlgorithmsButton.BackColor = Color.FromArgb(60, 60, 100);
            };

            _compareAlgorithmsButton.Click += btnCompareAlgorithms_Click;
        }

        // Handles click event for comparing allocation algorithms
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
                if (_simpleAllocationEngine == null && _perfumes.Count > 0)
                {
                    _simpleAllocationEngine = new SimpleAllocationEngine(_perfumes);
                }

                List<StoreRequirement> freshStoreRequirements = _storeRequirements.Select(s => s.Clone()).ToList();

                List<StoreRequirement> simpleResults = _simpleAllocationEngine.AllocatePerfumes(freshStoreRequirements);
                decimal simpleProfit = _simpleAllocationEngine.GetTotalProfit();

                decimal optimizedProfit = _allocationEngine.GetTotalProfit();

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

        // Displays detailed store requirements in a message box
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

        // Adds a view requirements button to a container
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

        // Handles double-click event on store data grid to show requirements
        private void dgvStores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _storeRequirements.Count)
            {
                ShowStoreRequirements(_storeRequirements[e.RowIndex]);
            }
        }

        // Sets the form's default font
        private void InitializeFormStyle()
        {
            Font = new Font("Segoe UI", 9F);
        }

        // Configures tab control appearance and colors
        private void SetupTabControl()
        {
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += tabControl1_DrawItem;

            tabPerfumes.BackColor = Color.FromArgb(245, 245, 245);
            tabStores.BackColor = Color.FromArgb(245, 245, 245);
            tabAllocation.BackColor = Color.FromArgb(245, 245, 245);
        }

        // Initializes labels, buttons, and timer components
        private void InitializeLabelsAndTimers()
        {
            StyleAllButtons();
            AddRandomStoreMessageLabel();
            ConfigureMessageTimer();
        }

        // Applies consistent styling to all buttons
        private void StyleAllButtons()
        {
            ApplyButtonStyle(btnAddStore);
            ApplyButtonStyle(btnGenerateRandomStore);
            ApplyButtonStyle(btnRunAllocation);
            ApplyButtonStyle(btnClearStores);
            ApplyButtonStyle(btnReset);
        }

        // Creates and positions the random store message label
        private void AddRandomStoreMessageLabel()
        {
            int labelY = btnGenerateRandomStore.Bottom + 20;

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

        // Sets up the timer for hiding temporary messages
        private void ConfigureMessageTimer()
        {
            timerHideMsg = new Timer { Interval = 5000 };
            timerHideMsg.Tick += timerHideMsg_Tick;
        }

        // Initializes all data grid view components
        private void SetupDataGridViews()
        {
            SetupPerfumesGrid();
            SetupStoresGrid();
            SetupResultsGrid();
        }

        // Configures the perfumes data grid view
        private void SetupPerfumesGrid()
        {
            dgvPerfumes.AutoGenerateColumns = true;
            StyleDataGridView(dgvPerfumes);
        }

        // Configures the stores data grid view with custom columns
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

        // Configures the results data grid view with custom columns
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

        // Initializes dropdown combo boxes with their options
        private void InitializeComboBoxes()
        {
            SetupGenderComboBox();
            SetupAccordComboBox();
        }

        // Populates the gender combo box with options
        private void SetupGenderComboBox()
        {
            cboGender.Items.AddRange(new string[] { "Any", "Male", "Female", "Unisex" });
            cboGender.SelectedIndex = 0;
        }

        // Populates the accord combo box with fragrance categories
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

        // Applies consistent styling to a button
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

        // Handles mouse enter event for button hover effect
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(80, 80, 120);
            }
        }

        // Handles mouse leave event for button hover effect
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(60, 60, 100);
            }
        }

        // Applies consistent styling to data grid views
        private void StyleDataGridView(DataGridView dgv)
        {
            SetDataGridViewBasicStyle(dgv);
            SetDataGridViewHeaderStyle(dgv);
        }

        // Sets basic visual styling for data grid view
        private void SetDataGridViewBasicStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 100, 160);
            dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgv.BackgroundColor = Color.White;
        }

        // Styles the header row of data grid view
        private void SetDataGridViewHeaderStyle(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 100);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        // Handles custom drawing of tab control items
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawTabItem(e);
        }

        // Draws individual tab items with custom colors
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

        // Determines colors for active and inactive tabs
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

        // Hides temporary message labels when timer expires
        private void timerHideMsg_Tick(object sender, EventArgs e)
        {
            lblRandomStoreMsg.Visible = false;
            timerHideMsg.Stop();
        }

        // Loads default CSV data or creates sample data if file not found
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
                    CreateDefaultPerfumeData();
                }

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
                CreateDefaultPerfumeData();
            }
        }

        // Creates sample perfume data when CSV file is unavailable
        private void CreateDefaultPerfumeData()
        {
            _perfumes = new List<Perfume>();

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

            dgvPerfumes.DataSource = null;
            dgvPerfumes.DataSource = _perfumes;
            _allocationEngine = new AllocationEngine(_perfumes);

            lblPerfumesSummary.Text = $"Using {_perfumes.Count} sample perfumes (CSV file not found)";
            btnAddStore.Enabled = true;
            btnGenerateRandomStore.Enabled = true;
        }

        // Loads perfume data from CSV file and initializes allocation engine
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

        // Shows warning message when default CSV file is not found
        private void ShowDefaultFileNotFoundMessage()
        {
            MessageBox.Show($"Default fragrance data file not found! Please place '{DEFAULT_CSV_PATH}' in the application folder.",
                "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Shows error message when loading CSV data fails
        private void ShowLoadErrorMessage(Exception ex)
        {
            MessageBox.Show($"Error loading default fragrance data: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Handles add store button click to create new store requirement
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

        // Validates all store input fields for completeness and correctness
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

            if (maxPrice < GetMinimumPerfumePrice())
            {
                MessageBox.Show($"Maximum price is too low. Minimum perfume price is ${GetMinimumPerfumePrice()}",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidateLongevityProjection())
                return false;

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

        // Gets the minimum price from available perfumes
        private decimal GetMinimumPerfumePrice()
        {
            if (_perfumes.Count == 0) return 0;
            return _perfumes.Min(p => p.AveragePrice);
        }

        // Gets the average price from available perfumes
        private decimal GetAveragePerfumePrice()
        {
            if (_perfumes.Count == 0) return 100m;
            return _perfumes.Average(p => p.AveragePrice);
        }

        // Gets the maximum price from available perfumes
        private decimal GetMaximumPerfumePrice()
        {
            if (_perfumes.Count == 0) return 500m;
            return _perfumes.Max(p => p.AveragePrice);
        }

        // Checks if store criteria are too restrictive by counting matching perfumes
        private bool CheckIfCriteriaTooRestrictive()
        {
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

            return matchCount < 5;
        }

        // Validates numeric input for decimal values
        private bool ValidateNumericInput(string input, string fieldName, out decimal result)
        {
            if (!decimal.TryParse(input, out result) || result <= 0)
            {
                MessageBox.Show($"Please enter a valid {fieldName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // Validates numeric input for integer values
        private bool ValidateNumericInput(string input, string fieldName, out int result)
        {
            if (!int.TryParse(input, out result) || result <= 0)
            {
                MessageBox.Show($"Please enter a valid {fieldName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // Validates longevity and projection values are within acceptable range
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

        // Creates a store requirement object from form inputs
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

        // Gets selected gender or empty string if "Any" is selected
        private string GetSelectedGender()
        {
            return cboGender.SelectedItem.ToString() == "Any" ? "" : cboGender.SelectedItem.ToString();
        }

        // Gets selected accord or empty string if "Any" is selected
        private string GetSelectedAccord()
        {
            return cboAccord.SelectedItem.ToString() == "Any" ? "" : cboAccord.SelectedItem.ToString();
        }

        // Handles generate random store button click
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

        // Generates random data for all store requirement fields
        private void GenerateRandomStoreData()
        {
            GenerateBasicStoreInfo();
            GenerateRandomNotes();
            GenerateImprovedQualitySettings();
        }

        // Generates basic store information like name, budget, and quantity
        private void GenerateBasicStoreInfo()
        {
            txtStoreName.Text = $"Store_{_random.Next(1, 1000)}";

            txtBudget.Text = _random.Next(800, 8001).ToString();

            txtQuantity.Text = _random.Next(3, 15).ToString();

            string[] genders = { "Any", "Any", "Male", "Female", "Unisex" };
            cboGender.SelectedItem = genders[_random.Next(genders.Length)];

            string[] accords = {
                "Any", "Any", "Any",
                "Aromatic", "Woody", "Fresh", "Sweet", "Floral", "Citrus",
                "Oriental", "Fruity", "Spicy"
            };
            cboAccord.SelectedItem = accords[_random.Next(accords.Length)];
        }

        // Generates random perfume note preferences based on existing perfumes
        private void GenerateRandomNotes()
        {
            if (_random.Next(100) < 30)
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
                ExtractCommonNotesFromPerfumes(ref topNote, ref middleNote, ref baseNote);
            }

            txtTopNotes.Text = _random.Next(100) < 50 ? topNote : "";
            txtMiddleNotes.Text = _random.Next(100) < 50 ? middleNote : "";
            txtBaseNotes.Text = _random.Next(100) < 50 ? baseNote : "";
        }

        // Extracts common notes from sample perfumes to improve matching probability
        private void ExtractCommonNotesFromPerfumes(ref string topNote, ref string middleNote, ref string baseNote)
        {
            var sampleSize = Math.Min(10, _perfumes.Count);
            var samplePerfumes = new List<Perfume>();

            for (int i = 0; i < sampleSize; i++)
            {
                samplePerfumes.Add(_perfumes[_random.Next(_perfumes.Count)]);
            }

            var topNotes = new Dictionary<string, int>();
            var middleNotes = new Dictionary<string, int>();
            var baseNotes = new Dictionary<string, int>();

            foreach (var perfume in samplePerfumes)
            {
                ProcessNotes(perfume.TopNotes, topNotes);
                ProcessNotes(perfume.MiddleNotes, middleNotes);
                ProcessNotes(perfume.BaseNotes, baseNotes);
            }

            topNote = GetMostCommonNote(topNotes);
            middleNote = GetMostCommonNote(middleNotes);
            baseNote = GetMostCommonNote(baseNotes);
        }

        // Processes note string and counts occurrences in dictionary
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

        // Returns the most frequently occurring note from the dictionary
        private string GetMostCommonNote(Dictionary<string, int> noteCount)
        {
            if (noteCount.Count == 0)
                return "";

            return noteCount.OrderByDescending(x => x.Value).First().Key;
        }

        // Generates random quality settings with realistic price constraints
        private void GenerateImprovedQualitySettings()
        {
            txtMinLongevity.Text = _random.Next(1, 8).ToString();
            txtMinProjection.Text = _random.Next(1, 8).ToString();

            decimal minPrice = GetMinimumPerfumePrice();
            decimal avgPrice = GetAveragePerfumePrice();
            decimal maxPrice = GetMaximumPerfumePrice();

            decimal randomFactor = (decimal)(_random.NextDouble() * 0.35 + 0.60);
            decimal targetPrice = maxPrice * randomFactor;

            targetPrice = Math.Max(targetPrice, minPrice * 1.2m);

            targetPrice = Math.Round(targetPrice / 10) * 10;

            txtMaxPrice.Text = targetPrice.ToString();
        }

        // Shows temporary message confirming random generation
        private void ShowRandomGenerationMessage()
        {
            lblRandomStoreMsg.Visible = true;
            lblRandomStoreMsg.Text = "Random values generated! Click 'Add Store' to add this store.";
            timerHideMsg.Start();
        }

        // Updates the store requirements data grid with current data
        private void UpdateStoreRequirementsGrid()
        {
            dgvStores.DataSource = null;
            dgvStores.DataSource = _storeRequirements;
            btnRunAllocation.Enabled = _storeRequirements.Count > 0;
        }

        // Clears all store input fields and resets to default values
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
            txtMinLongevity.Text = "1";
            txtMinProjection.Text = "1";
            txtMaxPrice.Text = "";
        }

        // Handles run allocation button click to execute perfume allocation
        private void btnRunAllocation_Click(object sender, EventArgs e)
        {
            if (!ValidateAllocationRequirements())
                return;

            RunAllocationProcess();
        }

        // Validates that allocation can be run with current data
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

        // Executes the complete allocation process and displays results
        private void RunAllocationProcess()
        {
            if (_allocationEngine == null && _perfumes.Count > 0)
            {
                _allocationEngine = new AllocationEngine(_perfumes);
            }

            if (_allocationEngine == null)
            {
                MessageBox.Show("Cannot run allocation - perfume data could not be loaded. Please check that the CSV file exists.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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

            _lastOptimizedResults = results;

            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Enabled = true;
            }

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

        // Counts perfumes that match a store's requirements
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

        // Displays allocation results in the main results grid
        private void DisplayResults(List<StoreRequirement> results)
        {
            dgvResults.DataSource = null;
            dgvResults.DataSource = results;

            decimal profit = _allocationEngine.GetTotalProfit();
            lblTotalProfit.Text = $"Total Profit: {profit:C}";
        }

        // Enables UI features that become available after allocation
        private void EnableResultsFeatures()
        {
        }

        // Shows detailed allocation information for each store
        private void ShowAllocationDetails(List<StoreRequirement> results)
        {
            pnlAllocationDetails.Controls.Clear();
            int yPos = 10;

            foreach (var store in results)
            {
                AddStoreDetailPanel(store, ref yPos);
            }
        }

        // Adds a detail panel for a single store's allocation results
        private void AddStoreDetailPanel(StoreRequirement store, ref int yPos)
        {
            GroupBox groupBox = CreateStoreGroupBox(store, yPos);
            AddStoreMetricsLabels(store, groupBox);
            AddPerfumesList(store, groupBox);

            AddViewRequirementsButton(groupBox, store, new Point(groupBox.Width - 150, 20), new Size(130, 25));

            pnlAllocationDetails.Controls.Add(groupBox);
            yPos += groupBox.Height + 10;
        }

        // Creates a group box container for store details
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

        // Adds metric labels showing store satisfaction and budget information
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

        // Adds a list box showing allocated perfumes for the store
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

        // Handles key press events for numeric text boxes to allow only valid input
        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '0' && (sender as TextBox).Text.Length == 0)
            {
                e.Handled = true;
            }
        }

        // Displays profit summary panel with allocation statistics
        private void DisplayTotalProfitSummary(decimal profit)
        {
            Panel panelToRemove = null;

            foreach (Control ctrl in tabAllocation.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "profitSummaryPanel")
                {
                    panelToRemove = (Panel)ctrl;
                }
            }

            if (panelToRemove != null)
            {
                tabAllocation.Controls.Remove(panelToRemove);
                panelToRemove.Dispose();
            }

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

            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Location = new Point(800, 15);
                _compareAlgorithmsButton.Enabled = true;
                profitPanel.Controls.Add(_compareAlgorithmsButton);
            }

            profitPanel.Controls.Add(lblProfitTitle);
            profitPanel.Controls.Add(lblProfitValue);
            profitPanel.Controls.Add(lblStoreCount);
            profitPanel.Controls.Add(lblAverageSatisfaction);

            tabAllocation.Controls.Add(profitPanel);
            profitPanel.BringToFront();
        }

        // Creates a new tab page with detailed allocation results
        private void CreateResultsTab(List<StoreRequirement> results, decimal totalProfit)
        {
            TabPage tabResult = new TabPage($"Results {DateTime.Now.ToString("HH:mm:ss")}");

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            Label lblTitle = new Label
            {
                Text = "Allocation Results Summary",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            headerPanel.Controls.Add(lblTitle);

            int totalItemsRequested = results.Sum(s => s.QuantityNeeded);
            int totalItemsAllocated = results.Sum(s => s.AllocatedPerfumes.Count);
            decimal totalBudget = results.Sum(s => s.Budget);
            decimal totalSpent = results.Sum(s => s.TotalSpent);
            double avgSatisfaction = results.Average(s => s.SatisfactionPercentage);
            int storesAboveTarget = results.Count(s => s.SatisfactionPercentage >= 70.0);

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

            headerPanel.Controls.Add(statsTable);

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
                Location = new Point(800, 15)
            };

            tabCompareButton.Click += btnCompareAlgorithms_Click;
            ApplyButtonStyle(tabCompareButton);
            headerPanel.Controls.Add(tabCompareButton);

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            TableLayoutPanel storeGrid = new TableLayoutPanel
            {
                ColumnCount = 4,
                Dock = DockStyle.Top,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Padding = new Padding(5)
            };

            for (int i = 0; i < storeGrid.ColumnCount; i++)
            {
                storeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            int col = 0;
            int row = 0;

            foreach (var store in results)
            {
                Panel storePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    Padding = new Padding(5),
                    Height = 400
                };

                Label lblStoreName = new Label
                {
                    Text = $"Store name: {store.StoreName}",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(5, 5)
                };
                storePanel.Controls.Add(lblStoreName);

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

                Label lblRequestInfo = new Label
                {
                    Text = $"Requested: {store.QuantityNeeded}   Allocated: {store.AllocatedPerfumes.Count}   Remaining: {store.RemainingQuantity}",
                    Font = new Font("Segoe UI", 8),
                    AutoSize = true,
                    Location = new Point(5, 55)
                };
                storePanel.Controls.Add(lblRequestInfo);

                Label lblBudgetInfo = new Label
                {
                    Text = $"Budget: {store.Budget:C}   Spent: {store.TotalSpent:C}   Remaining: {store.RemainingBudget:C}",
                    Font = new Font("Segoe UI", 8),
                    AutoSize = true,
                    Location = new Point(5, 75)
                };
                storePanel.Controls.Add(lblBudgetInfo);

                Label lblPerfumeHeader = new Label
                {
                    Text = "List of the perfumes:",
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    Location = new Point(5, 95)
                };
                storePanel.Controls.Add(lblPerfumeHeader);

                ListBox lstPerfumes = new ListBox
                {
                    Location = new Point(5, 115),
                    Size = new Size(storePanel.Width - 15, 205),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 8.5F),
                    IntegralHeight = false
                };

                foreach (var perfume in store.AllocatedPerfumes)
                {
                    lstPerfumes.Items.Add($"{perfume.Brand} - {perfume.Name} (${perfume.AveragePrice})");
                }

                storePanel.Controls.Add(lstPerfumes);

                AddViewRequirementsButton(storePanel, store, new Point(5, 325), new Size(storePanel.Width - 15, 30));

                if (store.SatisfactionPercentage < 70.0)
                {
                    Button btnExpand = new Button
                    {
                        Text = "Explain Low Satisfaction",
                        Location = new Point(5, 360),
                        Size = new Size(storePanel.Width - 15, 30),
                        BackColor = Color.FromArgb(180, 180, 180),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Tag = GetSatisfactionExplanation(store),
                        Cursor = Cursors.Hand
                    };

                    btnExpand.Click += (sender, e) =>
                    {
                        Button button = (Button)sender;
                        string explanation = button.Tag.ToString();
                        MessageBox.Show(explanation, $"Satisfaction Analysis for {store.StoreName}",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };

                    storePanel.Controls.Add(btnExpand);
                }

                storeGrid.Controls.Add(storePanel, col, row);

                col++;
                if (col >= storeGrid.ColumnCount)
                {
                    col = 0;
                    row++;
                    if (row >= storeGrid.RowCount && row < (results.Count / storeGrid.ColumnCount) + 1)
                    {
                        storeGrid.RowCount++;
                    }
                }
            }

            mainPanel.Controls.Add(storeGrid);
            tabResult.Controls.Add(mainPanel);
            tabResult.Controls.Add(headerPanel);

            tabControl1.TabPages.Add(tabResult);
            tabControl1.SelectedTab = tabResult;
        }

        // Adds a statistic label to the table layout panel
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

        // Generates detailed explanation for stores with low satisfaction scores
        private string GetSatisfactionExplanation(StoreRequirement store)
        {
            string explanation = $"Satisfaction Analysis for {store.StoreName}:\n\n";

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
                if (store.AllocatedPerfumes.Count < store.QuantityNeeded)
                {
                    explanation += $"• Insufficient quantity: Only allocated {store.AllocatedPerfumes.Count} of {store.QuantityNeeded} requested perfumes.\n\n";
                }

                if (store.RemainingBudget < store.MaxPrice)
                {
                    explanation += $"• Budget constraints: Remaining budget (${store.RemainingBudget}) is insufficient for additional perfumes.\n\n";
                }

                explanation += "• Preference matching issues:\n";

                if (!string.IsNullOrEmpty(store.Gender))
                {
                    int matchingGender = store.AllocatedPerfumes.Count(p => p.Gender == store.Gender || p.Gender == "Unisex");
                    explanation += $"  - Gender match: {matchingGender}/{store.AllocatedPerfumes.Count} perfumes\n";
                }

                if (!string.IsNullOrEmpty(store.PreferredAccord))
                {
                    int matchingAccord = store.AllocatedPerfumes.Count(p => p.MainAccord == store.PreferredAccord);
                    explanation += $"  - Accord match: {matchingAccord}/{store.AllocatedPerfumes.Count} perfumes\n";
                }

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

        // Returns color based on satisfaction percentage for visual feedback
        private Color GetSatisfactionColor(double satisfaction)
        {
            if (satisfaction >= 90) return Color.FromArgb(46, 139, 87);
            if (satisfaction >= 80) return Color.FromArgb(60, 179, 113);
            if (satisfaction >= 70) return Color.FromArgb(46, 204, 113);
            if (satisfaction >= 60) return Color.FromArgb(241, 196, 15);
            if (satisfaction >= 50) return Color.FromArgb(243, 156, 18);
            return Color.FromArgb(231, 76, 60);
        }

        // Handles clear stores button click with confirmation
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

        // Shows confirmation dialog for clearing all stores
        private bool ConfirmClearStores()
        {
            return MessageBox.Show("Are you sure you want to clear all stores?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // Performs complete reset of all application data and UI state
        private void PerformFullReset()
        {
            ClearCollections();
            ClearDataGrids();
            ClearAllocationResults();
            ResetLabels();
            DisableButtons();
            RemoveDynamicTabs();

            _lastOptimizedResults = null;
            _allocationEngine = null;
            _simpleAllocationEngine = null;
            _dataService = new DataService();

            tabControl1.SelectedIndex = 0;
        }

        // Clears allocation results from UI panels and displays
        private void ClearAllocationResults()
        {
            pnlAllocationDetails.Controls.Clear();

            Panel panelToRemove = null;

            foreach (Control ctrl in tabAllocation.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "profitSummaryPanel")
                {
                    panelToRemove = (Panel)ctrl;
                }
            }

            if (panelToRemove != null)
            {
                tabAllocation.Controls.Remove(panelToRemove);
                panelToRemove.Dispose();
            }

            dgvResults.DataSource = null;
            lblTotalProfit.Text = "Total Profit: $0.00";
        }

        // Handles reset button click with confirmation and updates display
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ConfirmReset())
            {
                PerformFullReset();
                LoadDefaultCsvData();

                Application.DoEvents();

                tabControl1.SelectedIndex = 0;
                UpdateDisplay();
            }
        }

        // Refreshes all UI displays after reset operation
        private void UpdateDisplay()
        {
            dgvPerfumes.Refresh();
            dgvStores.Refresh();
            dgvResults.Refresh();

            lblTotalProfit.Text = "Total Profit: $0.00";

            pnlAllocationDetails.Invalidate();
            pnlAllocationDetails.Update();

            tabControl1.Invalidate();
            tabControl1.Update();
        }

        // Shows confirmation dialog for reset operation
        private bool ConfirmReset()
        {
            return MessageBox.Show("Are you sure you want to reset everything?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // Clears all data collections and allocation engine
        private void ClearCollections()
        {
            _perfumes.Clear();
            _storeRequirements.Clear();
            _allocationEngine = null;
        }

        // Clears data from all data grid views
        private void ClearDataGrids()
        {
            dgvPerfumes.DataSource = null;
            dgvStores.DataSource = null;
            dgvResults.DataSource = null;
        }

        // Clears all detail panels from allocation display
        private void ClearDetailPanels()
        {
            pnlAllocationDetails.Controls.Clear();
        }

        // Resets all informational labels to default states
        private void ResetLabels()
        {
            lblPerfumesSummary.Text = "";
            lblTotalProfit.Text = "Total Profit: $0.00";
        }

        // Disables buttons that require data to function
        private void DisableButtons()
        {
            btnAddStore.Enabled = false;
            btnGenerateRandomStore.Enabled = false;
            btnRunAllocation.Enabled = false;

            if (_compareAlgorithmsButton != null)
            {
                _compareAlgorithmsButton.Enabled = false;
            }

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

        // Removes dynamically created result tabs from tab control
        private void RemoveDynamicTabs()
        {
            for (int i = tabControl1.TabPages.Count - 1; i >= 3; i--)
            {
                tabControl1.TabPages.RemoveAt(i);
            }
        }

        // Empty handler for data grid view cell content click events
        private void dgvPerfumes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}