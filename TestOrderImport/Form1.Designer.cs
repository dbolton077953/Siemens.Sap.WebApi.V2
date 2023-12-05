namespace TestOrderImport
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            OrderNumberTextBox = new TextBox();
            GetBtn = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            OrdersGridView = new DataGridView();
            tabPage2 = new TabPage();
            OperationsGridView = new DataGridView();
            label2 = new Label();
            EndPointDDL = new ComboBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OrdersGridView).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OperationsGridView).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 29);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 0;
            label1.Text = "Order Number:";
            // 
            // OrderNumberTextBox
            // 
            OrderNumberTextBox.Location = new Point(22, 47);
            OrderNumberTextBox.Name = "OrderNumberTextBox";
            OrderNumberTextBox.Size = new Size(138, 23);
            OrderNumberTextBox.TabIndex = 1;
            OrderNumberTextBox.Text = "900001443884";
            // 
            // GetBtn
            // 
            GetBtn.Location = new Point(197, 46);
            GetBtn.Name = "GetBtn";
            GetBtn.Size = new Size(75, 23);
            GetBtn.TabIndex = 2;
            GetBtn.Text = "Get Operations";
            GetBtn.UseVisualStyleBackColor = true;
            GetBtn.Click += GetBtn_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(23, 81);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(765, 357);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(OrdersGridView);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(757, 329);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Orders";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // OrdersGridView
            // 
            OrdersGridView.AllowUserToAddRows = false;
            OrdersGridView.AllowUserToDeleteRows = false;
            OrdersGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            OrdersGridView.Dock = DockStyle.Fill;
            OrdersGridView.Location = new Point(3, 3);
            OrdersGridView.Name = "OrdersGridView";
            OrdersGridView.ReadOnly = true;
            OrdersGridView.RowTemplate.Height = 25;
            OrdersGridView.Size = new Size(751, 323);
            OrdersGridView.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(OperationsGridView);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(757, 329);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Operations";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // OperationsGridView
            // 
            OperationsGridView.AllowUserToAddRows = false;
            OperationsGridView.AllowUserToDeleteRows = false;
            OperationsGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            OperationsGridView.Dock = DockStyle.Fill;
            OperationsGridView.Location = new Point(3, 3);
            OperationsGridView.Name = "OperationsGridView";
            OperationsGridView.ReadOnly = true;
            OperationsGridView.RowTemplate.Height = 25;
            OperationsGridView.Size = new Size(751, 323);
            OperationsGridView.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 5);
            label2.Name = "label2";
            label2.Size = new Size(58, 15);
            label2.TabIndex = 4;
            label2.Text = "EndPoint:";
            // 
            // EndPointDDL
            // 
            EndPointDDL.FormattingEnabled = true;
            EndPointDDL.Items.AddRange(new object[] { "https://stage-core.con.siemens.co.uk/Siemens.Sap.WebAPI/api/v1/Rfc/CallRFC", "https://localhost:7145/api/v1/Rfc/CallRFC" });
            EndPointDDL.Location = new Point(116, 4);
            EndPointDDL.Name = "EndPointDDL";
            EndPointDDL.Size = new Size(558, 23);
            EndPointDDL.TabIndex = 5;
            EndPointDDL.SelectedIndexChanged += EndPointDDL_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(EndPointDDL);
            Controls.Add(label2);
            Controls.Add(tabControl1);
            Controls.Add(GetBtn);
            Controls.Add(OrderNumberTextBox);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OrdersGridView).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OperationsGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox OrderNumberTextBox;
        private Button GetBtn;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private DataGridView OrdersGridView;
        private TabPage tabPage2;
        private DataGridView OperationsGridView;
        private Label label2;
        private ComboBox EndPointDDL;
    }
}