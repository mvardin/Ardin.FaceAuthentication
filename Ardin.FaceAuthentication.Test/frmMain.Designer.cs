
namespace Ardin.FaceAuthentication.Test
{
    partial class frmMain
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
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTimeElapsed = new System.Windows.Forms.Label();
            this.btnAddFace = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestFaceLocation = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFace = new System.Windows.Forms.Label();
            this.lblAuthenticated = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbMain
            // 
            this.pbMain.Location = new System.Drawing.Point(0, 0);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(598, 445);
            this.pbMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMain.TabIndex = 0;
            this.pbMain.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(604, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time elapsed:";
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.AutoSize = true;
            this.lblTimeElapsed.Location = new System.Drawing.Point(683, 9);
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(13, 13);
            this.lblTimeElapsed.TabIndex = 1;
            this.lblTimeElapsed.Text = "0";
            // 
            // btnAddFace
            // 
            this.btnAddFace.Location = new System.Drawing.Point(61, 53);
            this.btnAddFace.Name = "btnAddFace";
            this.btnAddFace.Size = new System.Drawing.Size(75, 23);
            this.btnAddFace.TabIndex = 3;
            this.btnAddFace.Text = "Add Face";
            this.btnAddFace.UseVisualStyleBackColor = true;
            this.btnAddFace.Click += new System.EventHandler(this.btnAddFace_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestFaceLocation);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnAddFace);
            this.groupBox1.Location = new System.Drawing.Point(1049, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 434);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Face";
            // 
            // btnTestFaceLocation
            // 
            this.btnTestFaceLocation.Location = new System.Drawing.Point(6, 405);
            this.btnTestFaceLocation.Name = "btnTestFaceLocation";
            this.btnTestFaceLocation.Size = new System.Drawing.Size(96, 23);
            this.btnTestFaceLocation.TabIndex = 6;
            this.btnTestFaceLocation.Text = "test face location";
            this.btnTestFaceLocation.UseVisualStyleBackColor = true;
            this.btnTestFaceLocation.Click += new System.EventHandler(this.btnTestFaceLocation_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(61, 27);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(197, 20);
            this.txtName.TabIndex = 5;
            this.txtName.Text = "Ardin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(758, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "Face: ";
            // 
            // lblFace
            // 
            this.lblFace.AutoSize = true;
            this.lblFace.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFace.Location = new System.Drawing.Point(831, 52);
            this.lblFace.Name = "lblFace";
            this.lblFace.Size = new System.Drawing.Size(0, 25);
            this.lblFace.TabIndex = 1;
            // 
            // lblAuthenticated
            // 
            this.lblAuthenticated.AutoSize = true;
            this.lblAuthenticated.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthenticated.Location = new System.Drawing.Point(779, 148);
            this.lblAuthenticated.Name = "lblAuthenticated";
            this.lblAuthenticated.Size = new System.Drawing.Size(0, 25);
            this.lblAuthenticated.TabIndex = 1;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(750, 25);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(607, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(135, 420);
            this.listBox1.TabIndex = 2;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 458);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblTimeElapsed);
            this.Controls.Add(this.lblAuthenticated);
            this.Controls.Add(this.lblFace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbMain);
            this.Name = "frmMain";
            this.Text = "Face Athentication";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTimeElapsed;
        private System.Windows.Forms.Button btnAddFace;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFace;
        private System.Windows.Forms.Label lblAuthenticated;
        private System.Windows.Forms.Button btnTestFaceLocation;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox listBox1;
    }
}