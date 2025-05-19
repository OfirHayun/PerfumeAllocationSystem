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

        // Initializes the comparison report with optimized and simple allocation results
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

        // Initializes form components
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "ComparisonReportForm";
            this.Text = "Perfume Allocation Algorithm Comparison";
            this.ResumeLayout(false);
        }

        // Sets up the form UI with all panels and components
        private void SetupUI()
        {
            this.Text = "Perfume Allocation Algorithm Comparison";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            this.Controls.Add(mainLayout);

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 100)
            };

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

            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            mainLayout.Controls.Add(contentLayout, 0, 1);

            Panel statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            TableLayoutPanel metricsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            for (int i = 0; i < 6; i++)
            {
                metricsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));
            }

            Label lblMetricHeader = CreateHeaderLabel("Performance Metrics");
            Label lblOptimizedHeader = CreateHeaderLabel("My Algorithm");
            Label lblSimpleHeader = CreateHeaderLabel("Simple Algorithm");

            metricsTable.Controls.Add(lblMetricHeader, 0, 0);
            metricsTable.Controls.Add(lblOptimizedHeader, 1, 0);
            metricsTable.Controls.Add(lblSimpleHeader, 2, 0);

            statsPanel.Controls.Add(metricsTable);
            contentLayout.Controls.Add(statsPanel, 0, 0);

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

            Tag = metricsTable;
        }

        // Creates a legend item with a color box and label
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

        // Creates a header label with consistent styling
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

        // Populates the report with metrics and comparison data
        private void PopulateReport()
        {
            TableLayoutPanel metricsTable = (TableLayoutPanel)Tag;

            decimal optimizedRevenue = _optimizedProfit / 0.3m;
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

        // Adds a metric row to the comparison table with improvement indicators
        private void AddMetricRow(TableLayoutPanel table, int row, string metricName, string optimizedValue, string simpleValue, double improvement)
        {
            Label metricLabel = new Label
            {
                Text = metricName,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Font = new Font("Segoe UI", 10F)
            };

            string improvementStr = improvement > 0 ? $" (+{improvement:F1}%)" : "";
            Label optimizedLabel = new Label
            {
                Text = optimizedValue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = improvement > 0 ? Color.FromArgb(46, 139, 87) : Color.Black
            };

            Label simpleLabel = new Label
            {
                Text = simpleValue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F)
            };

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

        // Draws the comparison chart with metrics visualization
        private void DrawComparisonChart(Graphics g, Panel panel)
        {
            int margin = 40;
            int barWidth = 30;
            int spacing = 15;
            int chartHeight = panel.Height - 2 * margin;
            int barBottom = panel.Height - margin;

            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, margin, barBottom, panel.Width - margin, barBottom);
            g.DrawLine(axisPen, margin, margin, margin, barBottom);

            g.DrawString("Metrics", new Font("Arial", 10, FontStyle.Bold), Brushes.Black,
                         panel.Width / 2 - 25, panel.Height - 20);

            double optimizedProfit = (double)_optimizedProfit;
            double simpleProfit = (double)_simpleProfit;

            int optimizedPerfumeCount = GetTotalPerfumeCount(_optimizedResults);
            int simplePerfumeCount = GetTotalPerfumeCount(_simpleResults);

            double optimizedSatisfaction = GetAverageSatisfaction(_optimizedResults);
            double simpleSatisfaction = GetAverageSatisfaction(_simpleResults);

            double maxProfit = Math.Max(optimizedProfit, simpleProfit);
            double normOptProfit = optimizedProfit / maxProfit;
            double normSimProfit = simpleProfit / maxProfit;

            double maxCount = Math.Max(optimizedPerfumeCount, simplePerfumeCount);
            double normOptCount = optimizedPerfumeCount / maxCount;
            double normSimCount = simplePerfumeCount / maxCount;

            double normOptSat = optimizedSatisfaction / 100;
            double normSimSat = simpleSatisfaction / 100;

            int profitX = margin + spacing;
            int countX = profitX + 2 * barWidth + 2 * spacing;
            int satX = countX + 2 * barWidth + 2 * spacing;

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

        // Draws a pair of bars for a metric in the chart
        private void DrawBarPair(Graphics g, string label, int x, int baseY, int width, int maxHeight,
                               double optValue, double simValue, string optText, string simText)
        {
            int optHeight = Math.Max(10, (int)(optValue * maxHeight * 0.9));
            int simHeight = Math.Max(10, (int)(simValue * maxHeight * 0.9));

            Rectangle optBar = new Rectangle(x, baseY - optHeight, width, optHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(46, 139, 87)), optBar);
            g.DrawRectangle(new Pen(Color.Black), optBar);

            Rectangle simBar = new Rectangle(x + width + 5, baseY - simHeight, width, simHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, 80, 120)), simBar);
            g.DrawRectangle(new Pen(Color.Black), simBar);

            g.DrawString(optText, new Font("Arial", 8, FontStyle.Bold), Brushes.Black,
                        x, baseY - optHeight - 15);

            g.DrawString(simText, new Font("Arial", 8), Brushes.Black,
                        x + width + 5, baseY - simHeight - 15);

            g.DrawString(label, new Font("Arial", 9, FontStyle.Bold), Brushes.Black,
                        x + width - 10, baseY + 5);
        }

        // Counts total perfumes allocated across all stores
        private int GetTotalPerfumeCount(List<StoreRequirement> results)
        {
            int total = 0;
            foreach (var store in results)
            {
                total += store.AllocatedPerfumes.Count;
            }
            return total;
        }

        // Calculates average satisfaction across all stores
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

        // Counts stores that received all requested perfumes
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

        // Counts stores with satisfaction above a given threshold
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

        // Calculates percentage improvement between optimized and simple values
        private double CalculatePercentImprovement(double optimized, double simple)
        {
            if (simple == 0) return 100;
            return ((optimized - simple) / simple) * 100;
        }
    }
}