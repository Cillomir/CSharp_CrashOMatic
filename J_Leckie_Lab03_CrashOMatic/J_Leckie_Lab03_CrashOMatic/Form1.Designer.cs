
namespace J_Leckie_Lab03_CrashOMatic
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.AniTimer = new System.Windows.Forms.Timer(this.components);
            this.lbl_score = new System.Windows.Forms.Label();
            this.lbl_collisions = new System.Windows.Forms.Label();
            this.btn_Start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AniTimer
            // 
            this.AniTimer.Interval = 50;
            this.AniTimer.Tick += new System.EventHandler(this.AniTimer_Tick);
            // 
            // lbl_score
            // 
            this.lbl_score.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_score.Location = new System.Drawing.Point(12, 9);
            this.lbl_score.Name = "lbl_score";
            this.lbl_score.Size = new System.Drawing.Size(192, 32);
            this.lbl_score.TabIndex = 0;
            this.lbl_score.Text = "Score: 0";
            this.lbl_score.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_collisions
            // 
            this.lbl_collisions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_collisions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_collisions.Location = new System.Drawing.Point(212, 9);
            this.lbl_collisions.Name = "lbl_collisions";
            this.lbl_collisions.Size = new System.Drawing.Size(128, 32);
            this.lbl_collisions.TabIndex = 1;
            this.lbl_collisions.Text = "Vehicles Lost: 0";
            this.lbl_collisions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_Start
            // 
            this.btn_Start.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Start.Location = new System.Drawing.Point(112, 55);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(144, 24);
            this.btn_Start.TabIndex = 2;
            this.btn_Start.Text = "Start a New Game";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 90);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.lbl_collisions);
            this.Controls.Add(this.lbl_score);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Crash-O-Matic by Joel Leckie";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AniTimer;
        private System.Windows.Forms.Label lbl_score;
        private System.Windows.Forms.Label lbl_collisions;
        private System.Windows.Forms.Button btn_Start;
    }
}

