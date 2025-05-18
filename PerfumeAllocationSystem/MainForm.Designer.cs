using System;
using System.Windows.Forms;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem
{
    partial class MainForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPerfumes = new System.Windows.Forms.TabPage();
            this.lblPerfumesSummary = new System.Windows.Forms.Label();
            this.dgvPerfumes = new System.Windows.Forms.DataGridView();
            this.tabStores = new System.Windows.Forms.TabPage();
            this.btnClearStores = new System.Windows.Forms.Button();
            this.dgvStores = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGenerateRandomStore = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtMaxPrice = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtMinProjection = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMinLongevity = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBaseNotes = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMiddleNotes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTopNotes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboAccord = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboGender = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBudget = new System.Windows.Forms.TextBox();
            this.lblStoreName = new System.Windows.Forms.Label();
            this.txtStoreName = new System.Windows.Forms.TextBox();
            this.btnAddStore = new System.Windows.Forms.Button();
            this.tabAllocation = new System.Windows.Forms.TabPage();
            this.lblTotalProfit = new System.Windows.Forms.Label();
            this.pnlAllocationDetails = new System.Windows.Forms.Panel();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.btnRunAllocation = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPerfumes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfumes)).BeginInit();
            this.tabStores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStores)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabAllocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPerfumes);
            this.tabControl1.Controls.Add(this.tabStores);
            this.tabControl1.Controls.Add(this.tabAllocation);
            this.tabControl1.Location = new System.Drawing.Point(16, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1413, 747);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPerfumes
            // 
            this.tabPerfumes.Controls.Add(this.lblPerfumesSummary);
            this.tabPerfumes.Controls.Add(this.dgvPerfumes);
            this.tabPerfumes.Location = new System.Drawing.Point(4, 25);
            this.tabPerfumes.Margin = new System.Windows.Forms.Padding(4);
            this.tabPerfumes.Name = "tabPerfumes";
            this.tabPerfumes.Padding = new System.Windows.Forms.Padding(4);
            this.tabPerfumes.Size = new System.Drawing.Size(1405, 718);
            this.tabPerfumes.TabIndex = 0;
            this.tabPerfumes.Text = "Perfume Inventory";
            this.tabPerfumes.UseVisualStyleBackColor = true;
            // 
            // lblPerfumesSummary
            // 
            this.lblPerfumesSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPerfumesSummary.AutoSize = true;
            this.lblPerfumesSummary.Location = new System.Drawing.Point(8, 688);
            this.lblPerfumesSummary.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPerfumesSummary.Name = "lblPerfumesSummary";
            this.lblPerfumesSummary.Size = new System.Drawing.Size(0, 16);
            this.lblPerfumesSummary.TabIndex = 2;
            // 
            // dgvPerfumes
            // 
            this.dgvPerfumes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPerfumes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPerfumes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerfumes.Location = new System.Drawing.Point(8, 7);
            this.dgvPerfumes.Margin = new System.Windows.Forms.Padding(4);
            this.dgvPerfumes.Name = "dgvPerfumes";
            this.dgvPerfumes.RowHeadersWidth = 51;
            this.dgvPerfumes.Size = new System.Drawing.Size(1387, 665);
            this.dgvPerfumes.TabIndex = 0;
            this.dgvPerfumes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPerfumes_CellContentClick);
            // 
            // tabStores
            // 
            this.tabStores.Controls.Add(this.btnClearStores);
            this.tabStores.Controls.Add(this.dgvStores);
            this.tabStores.Controls.Add(this.groupBox1);
            this.tabStores.Location = new System.Drawing.Point(4, 25);
            this.tabStores.Margin = new System.Windows.Forms.Padding(4);
            this.tabStores.Name = "tabStores";
            this.tabStores.Padding = new System.Windows.Forms.Padding(4);
            this.tabStores.Size = new System.Drawing.Size(1405, 718);
            this.tabStores.TabIndex = 1;
            this.tabStores.Text = "Store Requirements";
            this.tabStores.UseVisualStyleBackColor = true;
            // 
            // btnClearStores
            // 
            this.btnClearStores.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearStores.Location = new System.Drawing.Point(1253, 679);
            this.btnClearStores.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearStores.Name = "btnClearStores";
            this.btnClearStores.Size = new System.Drawing.Size(141, 28);
            this.btnClearStores.TabIndex = 3;
            this.btnClearStores.Text = "Clear All Stores";
            this.btnClearStores.UseVisualStyleBackColor = true;
            this.btnClearStores.Click += new System.EventHandler(this.btnClearStores_Click);
            // 
            // dgvStores
            // 
            this.dgvStores.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStores.Location = new System.Drawing.Point(504, 7);
            this.dgvStores.Margin = new System.Windows.Forms.Padding(4);
            this.dgvStores.Name = "dgvStores";
            this.dgvStores.RowHeadersWidth = 51;
            this.dgvStores.Size = new System.Drawing.Size(891, 665);
            this.dgvStores.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.btnGenerateRandomStore);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtMaxPrice);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtMinProjection);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtMinLongevity);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtBaseNotes);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtMiddleNotes);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtTopNotes);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboAccord);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboGender);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtQuantity);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtBudget);
            this.groupBox1.Controls.Add(this.lblStoreName);
            this.groupBox1.Controls.Add(this.txtStoreName);
            this.groupBox1.Controls.Add(this.btnAddStore);
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(488, 700);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Store";
            // 
            // btnGenerateRandomStore
            // 
            this.btnGenerateRandomStore.Enabled = false;
            this.btnGenerateRandomStore.Location = new System.Drawing.Point(165, 545);
            this.btnGenerateRandomStore.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateRandomStore.Name = "btnGenerateRandomStore";
            this.btnGenerateRandomStore.Size = new System.Drawing.Size(207, 28);
            this.btnGenerateRandomStore.TabIndex = 23;
            this.btnGenerateRandomStore.Text = "Generate Random Store";
            this.btnGenerateRandomStore.UseVisualStyleBackColor = true;
            this.btnGenerateRandomStore.Click += new System.EventHandler(this.btnGenerateRandomStore_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 459);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 16);
            this.label10.TabIndex = 22;
            this.label10.Text = "Max Price";
            // 
            // txtMaxPrice
            // 
            this.txtMaxPrice.Location = new System.Drawing.Point(165, 455);
            this.txtMaxPrice.Margin = new System.Windows.Forms.Padding(4);
            this.txtMaxPrice.Name = "txtMaxPrice";
            this.txtMaxPrice.Size = new System.Drawing.Size(313, 22);
            this.txtMaxPrice.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 427);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(127, 16);
            this.label9.TabIndex = 20;
            this.label9.Text = "Min Projection (0-10)";
            // 
            // txtMinProjection
            // 
            this.txtMinProjection.Location = new System.Drawing.Point(165, 423);
            this.txtMinProjection.Margin = new System.Windows.Forms.Padding(4);
            this.txtMinProjection.Name = "txtMinProjection";
            this.txtMinProjection.Size = new System.Drawing.Size(313, 22);
            this.txtMinProjection.TabIndex = 19;
            this.txtMinProjection.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 395);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 16);
            this.label8.TabIndex = 18;
            this.label8.Text = "Min Longevity (0-10)";
            // 
            // txtMinLongevity
            // 
            this.txtMinLongevity.Location = new System.Drawing.Point(165, 391);
            this.txtMinLongevity.Margin = new System.Windows.Forms.Padding(4);
            this.txtMinLongevity.Name = "txtMinLongevity";
            this.txtMinLongevity.Size = new System.Drawing.Size(313, 22);
            this.txtMinLongevity.TabIndex = 17;
            this.txtMinLongevity.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 363);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 16);
            this.label7.TabIndex = 16;
            this.label7.Text = "Preferred Base Notes";
            // 
            // txtBaseNotes
            // 
            this.txtBaseNotes.Location = new System.Drawing.Point(165, 359);
            this.txtBaseNotes.Margin = new System.Windows.Forms.Padding(4);
            this.txtBaseNotes.Name = "txtBaseNotes";
            this.txtBaseNotes.Size = new System.Drawing.Size(313, 22);
            this.txtBaseNotes.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 331);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "Preferred Middle Notes";
            // 
            // txtMiddleNotes
            // 
            this.txtMiddleNotes.Location = new System.Drawing.Point(165, 327);
            this.txtMiddleNotes.Margin = new System.Windows.Forms.Padding(4);
            this.txtMiddleNotes.Name = "txtMiddleNotes";
            this.txtMiddleNotes.Size = new System.Drawing.Size(313, 22);
            this.txtMiddleNotes.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 299);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "Preferred Top Notes";
            // 
            // txtTopNotes
            // 
            this.txtTopNotes.Location = new System.Drawing.Point(165, 295);
            this.txtTopNotes.Margin = new System.Windows.Forms.Padding(4);
            this.txtTopNotes.Name = "txtTopNotes";
            this.txtTopNotes.Size = new System.Drawing.Size(313, 22);
            this.txtTopNotes.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 266);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Preferred Accord";
            // 
            // cboAccord
            // 
            this.cboAccord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccord.FormattingEnabled = true;
            this.cboAccord.Location = new System.Drawing.Point(165, 262);
            this.cboAccord.Margin = new System.Windows.Forms.Padding(4);
            this.cboAccord.Name = "cboAccord";
            this.cboAccord.Size = new System.Drawing.Size(313, 24);
            this.cboAccord.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 233);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Gender";
            // 
            // cboGender
            // 
            this.cboGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGender.FormattingEnabled = true;
            this.cboGender.Location = new System.Drawing.Point(165, 229);
            this.cboGender.Margin = new System.Windows.Forms.Padding(4);
            this.cboGender.Name = "cboGender";
            this.cboGender.Size = new System.Drawing.Size(313, 24);
            this.cboGender.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 201);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Quantity Needed";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(165, 197);
            this.txtQuantity.Margin = new System.Windows.Forms.Padding(4);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(313, 22);
            this.txtQuantity.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 169);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Budget";
            // 
            // txtBudget
            // 
            this.txtBudget.Location = new System.Drawing.Point(165, 165);
            this.txtBudget.Margin = new System.Windows.Forms.Padding(4);
            this.txtBudget.Name = "txtBudget";
            this.txtBudget.Size = new System.Drawing.Size(313, 22);
            this.txtBudget.TabIndex = 3;
            // 
            // lblStoreName
            // 
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Location = new System.Drawing.Point(15, 137);
            this.lblStoreName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(79, 16);
            this.lblStoreName.TabIndex = 2;
            this.lblStoreName.Text = "Store Name";
            // 
            // txtStoreName
            // 
            this.txtStoreName.Location = new System.Drawing.Point(165, 133);
            this.txtStoreName.Margin = new System.Windows.Forms.Padding(4);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(313, 22);
            this.txtStoreName.TabIndex = 1;
            // 
            // btnAddStore
            // 
            this.btnAddStore.Enabled = false;
            this.btnAddStore.Location = new System.Drawing.Point(165, 510);
            this.btnAddStore.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddStore.Name = "btnAddStore";
            this.btnAddStore.Size = new System.Drawing.Size(163, 28);
            this.btnAddStore.TabIndex = 0;
            this.btnAddStore.Text = "Add Store";
            this.btnAddStore.UseVisualStyleBackColor = true;
            this.btnAddStore.Click += new System.EventHandler(this.btnAddStore_Click);
            // 
            // tabAllocation
            // 
            this.tabAllocation.Controls.Add(this.lblTotalProfit);
            this.tabAllocation.Controls.Add(this.pnlAllocationDetails);
            this.tabAllocation.Controls.Add(this.dgvResults);
            this.tabAllocation.Controls.Add(this.btnRunAllocation);
            this.tabAllocation.Location = new System.Drawing.Point(4, 25);
            this.tabAllocation.Margin = new System.Windows.Forms.Padding(4);
            this.tabAllocation.Name = "tabAllocation";
            this.tabAllocation.Size = new System.Drawing.Size(1405, 718);
            this.tabAllocation.TabIndex = 2;
            this.tabAllocation.Text = "Allocation Results";
            this.tabAllocation.UseVisualStyleBackColor = true;
            // 
            // lblTotalProfit
            // 
            this.lblTotalProfit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalProfit.AutoSize = true;
            this.lblTotalProfit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalProfit.Location = new System.Drawing.Point(11, 683);
            this.lblTotalProfit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalProfit.Name = "lblTotalProfit";
            this.lblTotalProfit.Size = new System.Drawing.Size(160, 20);
            this.lblTotalProfit.TabIndex = 4;
            this.lblTotalProfit.Text = "Total Profit: $0.00";
            // 
            // pnlAllocationDetails
            // 
            this.pnlAllocationDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAllocationDetails.AutoScroll = true;
            this.pnlAllocationDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAllocationDetails.Location = new System.Drawing.Point(11, 444);
            this.pnlAllocationDetails.Margin = new System.Windows.Forms.Padding(4);
            this.pnlAllocationDetails.Name = "pnlAllocationDetails";
            this.pnlAllocationDetails.Size = new System.Drawing.Size(1381, 235);
            this.pnlAllocationDetails.TabIndex = 3;
            // 
            // dgvResults
            // 
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(11, 50);
            this.dgvResults.Margin = new System.Windows.Forms.Padding(4);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 51;
            this.dgvResults.Size = new System.Drawing.Size(1381, 386);
            this.dgvResults.TabIndex = 1;
            // 
            // btnRunAllocation
            // 
            this.btnRunAllocation.Enabled = false;
            this.btnRunAllocation.Location = new System.Drawing.Point(11, 15);
            this.btnRunAllocation.Margin = new System.Windows.Forms.Padding(4);
            this.btnRunAllocation.Name = "btnRunAllocation";
            this.btnRunAllocation.Size = new System.Drawing.Size(147, 28);
            this.btnRunAllocation.TabIndex = 0;
            this.btnRunAllocation.Text = "Run Allocation";
            this.btnRunAllocation.UseVisualStyleBackColor = true;
            this.btnRunAllocation.Click += new System.EventHandler(this.btnRunAllocation_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(1293, 769);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(136, 28);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset Everything";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1445, 814);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Perfume Allocation System";
            this.tabControl1.ResumeLayout(false);
            this.tabPerfumes.ResumeLayout(false);
            this.tabPerfumes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfumes)).EndInit();
            this.tabStores.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStores)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabAllocation.ResumeLayout(false);
            this.tabAllocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPerfumes;
        private System.Windows.Forms.DataGridView dgvPerfumes;
        private System.Windows.Forms.TabPage tabStores;
        private System.Windows.Forms.TabPage tabAllocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblStoreName;
        private System.Windows.Forms.TextBox txtStoreName;
        private System.Windows.Forms.Button btnAddStore;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBudget;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboGender;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAccord;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBaseNotes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMiddleNotes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTopNotes;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtMaxPrice;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtMinProjection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMinLongevity;
        private System.Windows.Forms.DataGridView dgvStores;
        private System.Windows.Forms.Button btnClearStores;
        private System.Windows.Forms.Button btnRunAllocation;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Panel pnlAllocationDetails;
        private System.Windows.Forms.Label lblTotalProfit;
        private System.Windows.Forms.Label lblPerfumesSummary;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGenerateRandomStore;
    }
}