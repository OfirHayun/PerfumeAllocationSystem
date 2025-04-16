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
            this.btnLoadPerfumes = new System.Windows.Forms.Button();
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
            this.btnSaveResults = new System.Windows.Forms.Button();
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
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1060, 607);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPerfumes
            // 
            this.tabPerfumes.Controls.Add(this.lblPerfumesSummary);
            this.tabPerfumes.Controls.Add(this.btnLoadPerfumes);
            this.tabPerfumes.Controls.Add(this.dgvPerfumes);
            this.tabPerfumes.Location = new System.Drawing.Point(4, 22);
            this.tabPerfumes.Name = "tabPerfumes";
            this.tabPerfumes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPerfumes.Size = new System.Drawing.Size(1052, 581);
            this.tabPerfumes.TabIndex = 0;
            this.tabPerfumes.Text = "Perfume Inventory";
            this.tabPerfumes.UseVisualStyleBackColor = true;
            // 
            // lblPerfumesSummary
            // 
            this.lblPerfumesSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPerfumesSummary.AutoSize = true;
            this.lblPerfumesSummary.Location = new System.Drawing.Point(6, 559);
            this.lblPerfumesSummary.Name = "lblPerfumesSummary";
            this.lblPerfumesSummary.Size = new System.Drawing.Size(0, 13);
            this.lblPerfumesSummary.TabIndex = 2;
            // 
            // btnLoadPerfumes
            // 
            this.btnLoadPerfumes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadPerfumes.Location = new System.Drawing.Point(940, 552);
            this.btnLoadPerfumes.Name = "btnLoadPerfumes";
            this.btnLoadPerfumes.Size = new System.Drawing.Size(106, 23);
            this.btnLoadPerfumes.TabIndex = 1;
            this.btnLoadPerfumes.Text = "Load CSV";
            this.btnLoadPerfumes.UseVisualStyleBackColor = true;
            this.btnLoadPerfumes.Click += new System.EventHandler(this.btnLoadPerfumes_Click);
            // 
            // dgvPerfumes
            // 
            this.dgvPerfumes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPerfumes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPerfumes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerfumes.Location = new System.Drawing.Point(6, 6);
            this.dgvPerfumes.Name = "dgvPerfumes";
            this.dgvPerfumes.Size = new System.Drawing.Size(1040, 540);
            this.dgvPerfumes.TabIndex = 0;
            // 
            // tabStores
            // 
            this.tabStores.Controls.Add(this.btnClearStores);
            this.tabStores.Controls.Add(this.dgvStores);
            this.tabStores.Controls.Add(this.groupBox1);
            this.tabStores.Location = new System.Drawing.Point(4, 22);
            this.tabStores.Name = "tabStores";
            this.tabStores.Padding = new System.Windows.Forms.Padding(3);
            this.tabStores.Size = new System.Drawing.Size(1052, 581);
            this.tabStores.TabIndex = 1;
            this.tabStores.Text = "Store Requirements";
            this.tabStores.UseVisualStyleBackColor = true;
            // 
            // btnClearStores
            // 
            this.btnClearStores.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearStores.Location = new System.Drawing.Point(940, 552);
            this.btnClearStores.Name = "btnClearStores";
            this.btnClearStores.Size = new System.Drawing.Size(106, 23);
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
            this.dgvStores.Location = new System.Drawing.Point(378, 6);
            this.dgvStores.Name = "dgvStores";
            this.dgvStores.Size = new System.Drawing.Size(668, 540);
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
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(366, 569);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Store";
            // 
            // btnGenerateRandomStore
            // 
            this.btnGenerateRandomStore.Enabled = false;
            this.btnGenerateRandomStore.Location = new System.Drawing.Point(124, 443);
            this.btnGenerateRandomStore.Name = "btnGenerateRandomStore";
            this.btnGenerateRandomStore.Size = new System.Drawing.Size(155, 23);
            this.btnGenerateRandomStore.TabIndex = 23;
            this.btnGenerateRandomStore.Text = "Generate Random Store";
            this.btnGenerateRandomStore.UseVisualStyleBackColor = true;
            this.btnGenerateRandomStore.Click += new System.EventHandler(this.btnGenerateRandomStore_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 373);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Max Price";
            // 
            // txtMaxPrice
            // 
            this.txtMaxPrice.Location = new System.Drawing.Point(124, 370);
            this.txtMaxPrice.Name = "txtMaxPrice";
            this.txtMaxPrice.Size = new System.Drawing.Size(236, 20);
            this.txtMaxPrice.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 347);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Min Projection (0-10)";
            // 
            // txtMinProjection
            // 
            this.txtMinProjection.Location = new System.Drawing.Point(124, 344);
            this.txtMinProjection.Name = "txtMinProjection";
            this.txtMinProjection.Size = new System.Drawing.Size(236, 20);
            this.txtMinProjection.TabIndex = 19;
            this.txtMinProjection.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 321);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Min Longevity (0-10)";
            // 
            // txtMinLongevity
            // 
            this.txtMinLongevity.Location = new System.Drawing.Point(124, 318);
            this.txtMinLongevity.Name = "txtMinLongevity";
            this.txtMinLongevity.Size = new System.Drawing.Size(236, 20);
            this.txtMinLongevity.TabIndex = 17;
            this.txtMinLongevity.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 295);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Preferred Base Notes";
            // 
            // txtBaseNotes
            // 
            this.txtBaseNotes.Location = new System.Drawing.Point(124, 292);
            this.txtBaseNotes.Name = "txtBaseNotes";
            this.txtBaseNotes.Size = new System.Drawing.Size(236, 20);
            this.txtBaseNotes.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 269);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Preferred Middle Notes";
            // 
            // txtMiddleNotes
            // 
            this.txtMiddleNotes.Location = new System.Drawing.Point(124, 266);
            this.txtMiddleNotes.Name = "txtMiddleNotes";
            this.txtMiddleNotes.Size = new System.Drawing.Size(236, 20);
            this.txtMiddleNotes.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 243);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Preferred Top Notes";
            // 
            // txtTopNotes
            // 
            this.txtTopNotes.Location = new System.Drawing.Point(124, 240);
            this.txtTopNotes.Name = "txtTopNotes";
            this.txtTopNotes.Size = new System.Drawing.Size(236, 20);
            this.txtTopNotes.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Preferred Accord";
            // 
            // cboAccord
            // 
            this.cboAccord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccord.FormattingEnabled = true;
            this.cboAccord.Location = new System.Drawing.Point(124, 213);
            this.cboAccord.Name = "cboAccord";
            this.cboAccord.Size = new System.Drawing.Size(236, 21);
            this.cboAccord.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 189);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Gender";
            // 
            // cboGender
            // 
            this.cboGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGender.FormattingEnabled = true;
            this.cboGender.Location = new System.Drawing.Point(124, 186);
            this.cboGender.Name = "cboGender";
            this.cboGender.Size = new System.Drawing.Size(236, 21);
            this.cboGender.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Quantity Needed";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(124, 160);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(236, 20);
            this.txtQuantity.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Budget";
            // 
            // txtBudget
            // 
            this.txtBudget.Location = new System.Drawing.Point(124, 134);
            this.txtBudget.Name = "txtBudget";
            this.txtBudget.Size = new System.Drawing.Size(236, 20);
            this.txtBudget.TabIndex = 3;
            // 
            // lblStoreName
            // 
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Location = new System.Drawing.Point(11, 111);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(63, 13);
            this.lblStoreName.TabIndex = 2;
            this.lblStoreName.Text = "Store Name";
            // 
            // txtStoreName
            // 
            this.txtStoreName.Location = new System.Drawing.Point(124, 108);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(236, 20);
            this.txtStoreName.TabIndex = 1;
            // 
            // btnAddStore
            // 
            this.btnAddStore.Enabled = false;
            this.btnAddStore.Location = new System.Drawing.Point(124, 414);
            this.btnAddStore.Name = "btnAddStore";
            this.btnAddStore.Size = new System.Drawing.Size(122, 23);
            this.btnAddStore.TabIndex = 0;
            this.btnAddStore.Text = "Add Store";
            this.btnAddStore.UseVisualStyleBackColor = true;
            this.btnAddStore.Click += new System.EventHandler(this.btnAddStore_Click);
            // 
            // tabAllocation
            // 
            this.tabAllocation.Controls.Add(this.lblTotalProfit);
            this.tabAllocation.Controls.Add(this.pnlAllocationDetails);
            this.tabAllocation.Controls.Add(this.btnSaveResults);
            this.tabAllocation.Controls.Add(this.dgvResults);
            this.tabAllocation.Controls.Add(this.btnRunAllocation);
            this.tabAllocation.Location = new System.Drawing.Point(4, 22);
            this.tabAllocation.Name = "tabAllocation";
            this.tabAllocation.Size = new System.Drawing.Size(1052, 581);
            this.tabAllocation.TabIndex = 2;
            this.tabAllocation.Text = "Allocation Results";
            this.tabAllocation.UseVisualStyleBackColor = true;
            // 
            // lblTotalProfit
            // 
            this.lblTotalProfit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalProfit.AutoSize = true;
            this.lblTotalProfit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalProfit.Location = new System.Drawing.Point(8, 555);
            this.lblTotalProfit.Name = "lblTotalProfit";
            this.lblTotalProfit.Size = new System.Drawing.Size(134, 16);
            this.lblTotalProfit.TabIndex = 4;
            this.lblTotalProfit.Text = "Total Profit: $0.00";
            // 
            // pnlAllocationDetails
            // 
            this.pnlAllocationDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAllocationDetails.AutoScroll = true;
            this.pnlAllocationDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAllocationDetails.Location = new System.Drawing.Point(8, 361);
            this.pnlAllocationDetails.Name = "pnlAllocationDetails";
            this.pnlAllocationDetails.Size = new System.Drawing.Size(1036, 191);
            this.pnlAllocationDetails.TabIndex = 3;
            // 
            // btnSaveResults
            // 
            this.btnSaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveResults.Enabled = false;
            this.btnSaveResults.Location = new System.Drawing.Point(947, 552);
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.Size = new System.Drawing.Size(97, 23);
            this.btnSaveResults.TabIndex = 2;
            this.btnSaveResults.Text = "Save Results";
            this.btnSaveResults.UseVisualStyleBackColor = true;
            this.btnSaveResults.Click += new System.EventHandler(this.btnSaveResults_Click);
            // 
            // dgvResults
            // 
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(8, 41);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(1036, 314);
            this.dgvResults.TabIndex = 1;
            // 
            // btnRunAllocation
            // 
            this.btnRunAllocation.Enabled = false;
            this.btnRunAllocation.Location = new System.Drawing.Point(8, 12);
            this.btnRunAllocation.Name = "btnRunAllocation";
            this.btnRunAllocation.Size = new System.Drawing.Size(110, 23);
            this.btnRunAllocation.TabIndex = 0;
            this.btnRunAllocation.Text = "Run Allocation";
            this.btnRunAllocation.UseVisualStyleBackColor = true;
            this.btnRunAllocation.Click += new System.EventHandler(this.btnRunAllocation_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(970, 625);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(102, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset Everything";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 661);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tabControl1);
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
        private System.Windows.Forms.Button btnLoadPerfumes;
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
        private System.Windows.Forms.Button btnSaveResults;
        private System.Windows.Forms.Panel pnlAllocationDetails;
        private System.Windows.Forms.Label lblTotalProfit;
        private System.Windows.Forms.Label lblPerfumesSummary;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGenerateRandomStore;
    }
}