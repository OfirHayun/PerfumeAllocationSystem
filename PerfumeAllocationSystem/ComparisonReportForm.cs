using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem
{
    public partial class ComparisonReportForm : Form
    {
        private List<StoreRequirement> _optimizedResults;
        private List<StoreRequirement> _simpleResults;
        private decimal _optimizedProfit;
        private decimal _simpleProfit;

        public ComparisonReportForm(List<StoreRequirement> optimizedResults, decimal optimizedProfit,
                                   List<StoreRequirement> simpleResults, decimal simpleProfit)
        {
            InitializeComponent();
            _optimizedResults = optimizedResults;
            _simpleResults = simpleResults;
            _optimizedProfit = optimizedProfit;
            _simpleProfit = simpleProfit;

            SetupUI();
            PopulateReport();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ComparisonReportForm
            // 
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "ComparisonReportForm";
            this.Text = "Perfume Allocation Algorithm Comparison";
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Set form properties
            this.Text = "Perfume Allocation Algorithm Comparison";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main table layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White
            };

            // Row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Footer

            this.Controls.Add(mainLayout);

            // Header panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            // Add titles to header
            Label lblTitle = new Label
            {
                Text = "Algorithm Effectiveness Comparison",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 40
            };
            headerPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Backtracking Algorithm vs. Simple Allocation",
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 25,
                Top = 40
            };
            headerPanel.Controls.Add(lblSubtitle);

            // Add the legend
            Panel legendPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            CreateLegendItem(legendPanel, "My Backtracking Algorithm", Color.FromArgb(46, 139, 87), 20);
            CreateLegendItem(legendPanel, "Simple Algorithm", Color.FromArgb(80, 80, 120), 300);

            headerPanel.Controls.Add(legendPanel);
            mainLayout.Controls.Add(headerPanel, 0, 0);

            // Content panel - split into two sections
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };

            // Column styles
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Stats table
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Chart

            mainLayout.Controls.Add(contentLayout, 0, 1);

            // Stats table panel
            Panel statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Create table for metrics
            TableLayoutPanel metricsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            // Set column styles
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Set row styles - making them equal
            for (int i = 0; i < 6; i++)
            {
                metricsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));
            }

            // Add header row
            Label lblMetricHeader = CreateHeaderLabel("Performance Metrics");
            Label lblOptimizedHeader = CreateHeaderLabel("My Algorithm");
            Label lblSimpleHeader = CreateHeaderLabel("Simple Algorithm");

            metricsTable.Controls.Add(lblMetricHeader, 0, 0);
            metricsTable.Controls.Add(lblOptimizedHeader, 1, 0);
            metricsTable.Controls.Add(lblSimpleHeader, 2, 0);

            statsPanel.Controls.Add(metricsTable);
            contentLayout.Controls.Add(statsPanel, 0, 0);

            // Chart panel
            Panel chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            Panel graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            graphPanel.Paint += (s, e) => DrawComparisonChart(e.Graphics, graphPanel);
            chartPanel.Controls.Add(graphPanel);

            contentLayout.Controls.Add(chartPanel, 1, 0);

            // Footer with close button
            Panel footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 100)
            };

            Button btnClose = new Button
            {
                Text = "Close Report",
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(40, 40, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point((this.ClientSize.Width - 150) / 2, 7)
            };
            btnClose.Click += (s, e) => this.Close();
            footerPanel.Controls.Add(btnClose);

            mainLayout.Controls.Add(footerPanel, 0, 2);

            // Store the metrics table for later population
            Tag = metricsTable;
        }

        private void CreateLegendItem(Panel panel, string text, Color color, int xPosition)
        {
            Panel colorBox = new Panel
            {
                BackColor = color,
                Size = new Size(16, 16),
                Location = new Point(xPosition, 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(colorBox);

            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(xPosition + 20, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White
            };
            panel.Controls.Add(label);
        }

        private Label CreateHeaderLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(60, 60, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
        }

        private void PopulateReport()
        {
            TableLayoutPanel metricsTable = (TableLayoutPanel)Tag;

            // Calculate metrics
            decimal optimizedRevenue = _optimizedProfit / 0.3m; // 30% profit margin
            decimal simpleRevenue = _simpleProfit / 0.3m;

            double profitImprovement = CalculatePercentImprovement((double)_optimizedProfit, (double)_simpleProfit);

            int optimizedPerfumeCount = GetTotalPerfumeCount(_optimizedResults);
            int simplePerfumeCount = GetTotalPerfumeCount(_simpleResults);

            double perfumeCountImprovement = CalculatePercentImprovement(optimizedPerfumeCount, simplePerfumeCount);

            double optimizedAvgSatisfaction = GetAverageSatisfaction(_optimizedResults);
            double simpleAvgSatisfaction = GetAverageSatisfaction(_simpleResults);

            double satisfactionImprovement = CalculatePercentImprovement(optimizedAvgSatisfaction, simpleAvgSatisfaction);

            int optimizedFullyAllocated = CountFullyAllocatedStores(_optimizedResults);
            int simpleFullyAllocated = CountFullyAllocatedStores(_simpleResults);

            int optimizedHighSatisfaction = CountStoresAboveThreshold(_optimizedResults, 70);
            int simpleHighSatisfaction = CountStoresAboveThreshold(_simpleResults, 70);

            // Add the key metrics rows
            AddMetricRow(metricsTable, 1, "Total Profit",
                          $"{_optimizedProfit:C}",
                          $"{_simpleProfit:C}",
                          profitImprovement);

            AddMetricRow(metricsTable, 2, "Total Perfumes Allocated",
                          $"{optimizedPerfumeCount}",
                          $"{simplePerfumeCount}",
                          perfumeCountImprovement);

            AddMetricRow(metricsTable, 3, "Average Satisfaction",
                          $"{optimizedAvgSatisfaction:F2}%",
                          $"{simpleAvgSatisfaction:F2}%",
                          satisfactionImprovement);

            AddMetricRow(metricsTable, 4, "Fully Allocated Stores",
                          $"{optimizedFullyAllocated} of {_optimizedResults.Count}",
                          $"{simpleFullyAllocated} of {_simpleResults.Count}",
                          CalculatePercentImprovement(optimizedFullyAllocated, simpleFullyAllocated));

            AddMetricRow(metricsTable, 5, "Stores With 70%+ Satisfaction",
                          $"{optimizedHighSatisfaction} of {_optimizedResults.Count}",
                          $"{simpleHighSatisfaction} of {_simpleResults.Count}",
                          CalculatePercentImprovement(optimizedHighSatisfaction, simpleHighSatisfaction));
        }

        private void AddMetricRow(TableLayoutPanel table, int row, string metricName, string optimizedValue, string simpleValue, double improvement)
        {
            // Create metric label
            Label metricLabel = new Label
            {
                Text = metricName,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Font = new Font("Segoe UI", 10F)
            };

            // Create optimized value label with improvement 
            string improvementStr = improvement > 0 ? $" (+{improvement:F1}%)" : "";
            Label optimizedLabel = new Label
            {
                Text = optimizedValue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = improvement > 0 ? Color.FromArgb(46, 139, 87) : Color.Black
            };

            // Create simple value label
            Label simpleLabel = new Label
            {
                Text = simpleValue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F)
            };

            // Add improvement indicator if positive
            if (improvement > 0)
            {
                Label improvementLabel = new Label
                {
                    Text = improvementStr,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(46, 139, 87),
                    Location = new Point(optimizedLabel.Width - 50, optimizedLabel.Height / 2 - 10)
                };
                optimizedLabel.Controls.Add(improvementLabel);
            }

            table.Controls.Add(metricLabel, 0, row);
            table.Controls.Add(optimizedLabel, 1, row);
            table.Controls.Add(simpleLabel, 2, row);
        }

        private void DrawComparisonChart(Graphics g, Panel panel)
        {
            // Set up dimensions
            int margin = 40;
            int barWidth = 30;
            int spacing = 15;
            int chartHeight = panel.Height - 2 * margin;
            int barBottom = panel.Height - margin;

            // Draw axes
            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, margin, barBottom, panel.Width - margin, barBottom); // X-axis
            g.DrawLine(axisPen, margin, margin, margin, barBottom); // Y-axis

            // Add axis labels
            g.DrawString("Metrics", new Font("Arial", 10, FontStyle.Bold), Brushes.Black,
                         panel.Width / 2 - 25, panel.Height - 20);

            // Calculate metrics
            double optimizedProfit = (double)_optimizedProfit;
            double simpleProfit = (double)_simpleProfit;

            int optimizedPerfumeCount = GetTotalPerfumeCount(_optimizedResults);
            int simplePerfumeCount = GetTotalPerfumeCount(_simpleResults);

            double optimizedSatisfaction = GetAverageSatisfaction(_optimizedResults);
            double simpleSatisfaction = GetAverageSatisfaction(_simpleResults);

            // Normalize values (to make all fit on same chart)
            double maxProfit = Math.Max(optimizedProfit, simpleProfit);
            double normOptProfit = optimizedProfit / maxProfit;
            double normSimProfit = simpleProfit / maxProfit;

            double maxCount = Math.Max(optimizedPerfumeCount, simplePerfumeCount);
            double normOptCount = optimizedPerfumeCount / maxCount;
            double normSimCount = simplePerfumeCount / maxCount;

            // Satisfaction is already 0-100
            double normOptSat = optimizedSatisfaction / 100;
            double normSimSat = simpleSatisfaction / 100;

            // Calculate bar positions
            int profitX = margin + spacing;
            int countX = profitX + 2 * barWidth + 2 * spacing;
            int satX = countX + 2 * barWidth + 2 * spacing;

            // Draw bars and labels
            DrawBarPair(g, "Profit", profitX, barBottom, barWidth, chartHeight,
                      normOptProfit, normSimProfit,
                      $"${_optimizedProfit:N0}", $"${_simpleProfit:N0}");

            DrawBarPair(g, "Perfumes", countX, barBottom, barWidth, chartHeight,
                      normOptCount, normSimCount,
                      optimizedPerfumeCount.ToString(), simplePerfumeCount.ToString());

            DrawBarPair(g, "Satisfaction", satX, barBottom, barWidth, chartHeight,
                      normOptSat, normSimSat,
                      $"{optimizedSatisfaction:F1}%", $"{simpleSatisfaction:F1}%");
        }

        private void DrawBarPair(Graphics g, string label, int x, int baseY, int width, int maxHeight,
                               double optValue, double simValue, string optText, string simText)
        {
            // Calculate bar heights (minimum 10 pixels for visibility)
            int optHeight = Math.Max(10, (int)(optValue * maxHeight * 0.9)); // 0.9 to leave room for labels
            int simHeight = Math.Max(10, (int)(simValue * maxHeight * 0.9));

            // Draw the optimized algorithm bar
            Rectangle optBar = new Rectangle(x, baseY - optHeight, width, optHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(46, 139, 87)), optBar);
            g.DrawRectangle(new Pen(Color.Black), optBar);

            // Draw the simple algorithm bar
            Rectangle simBar = new Rectangle(x + width + 5, baseY - simHeight, width, simHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, 80, 120)), simBar);
            g.DrawRectangle(new Pen(Color.Black), simBar);

            // Add value labels above bars - make them smaller
            g.DrawString(optText, new Font("Arial", 8, FontStyle.Bold), Brushes.Black,
                        x, baseY - optHeight - 15);

            g.DrawString(simText, new Font("Arial", 8), Brushes.Black,
                        x + width + 5, baseY - simHeight - 15);

            // Add x-axis label
            g.DrawString(label, new Font("Arial", 9, FontStyle.Bold), Brushes.Black,
                        x + width - 10, baseY + 5);
        }

        private int GetTotalPerfumeCount(List<StoreRequirement> results)
        {
            int total = 0;
            foreach (var store in results)
            {
                total += store.AllocatedPerfumes.Count;
            }
            return total;
        }

        private double GetAverageSatisfaction(List<StoreRequirement> results)
        {
            if (results.Count == 0) return 0;

            double total = 0;
            foreach (var store in results)
            {
                total += store.SatisfactionPercentage;
            }
            return total / results.Count;
        }

        private int CountFullyAllocatedStores(List<StoreRequirement> results)
        {
            int count = 0;
            foreach (var store in results)
            {
                if (store.RemainingQuantity == 0)
                {
                    count++;
                }
            }
            return count;
        }

        private int CountStoresAboveThreshold(List<StoreRequirement> results, double threshold)
        {
            int count = 0;
            foreach (var store in results)
            {
                if (store.SatisfactionPercentage >= threshold)
                {
                    count++;
                }
            }
            return count;
        }

        private double CalculatePercentImprovement(double optimized, double simple)
        {
            if (simple == 0) return 100; // If simple is 0, show 100% improvement
            return ((optimized - simple) / simple) * 100;
        }
    }
}