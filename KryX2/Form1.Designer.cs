namespace KryX2
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
            this.button1 = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.labelConnectedForecolor = new System.Windows.Forms.Label();
            this.labelConnectedBackground = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 38);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chatBox
            // 
            this.chatBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(102)))), ((int)(((byte)(129)))));
            this.chatBox.Location = new System.Drawing.Point(13, 58);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(224, 263);
            this.chatBox.TabIndex = 1;
            this.chatBox.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(121, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 38);
            this.button2.TabIndex = 2;
            this.button2.Text = "Disconnect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelConnectedForecolor
            // 
            this.labelConnectedForecolor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(102)))), ((int)(((byte)(129)))));
            this.labelConnectedForecolor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelConnectedForecolor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConnectedForecolor.ForeColor = System.Drawing.Color.White;
            this.labelConnectedForecolor.Location = new System.Drawing.Point(12, 324);
            this.labelConnectedForecolor.Name = "labelConnectedForecolor";
            this.labelConnectedForecolor.Size = new System.Drawing.Size(45, 21);
            this.labelConnectedForecolor.TabIndex = 19;
            this.labelConnectedForecolor.Text = "0%";
            this.labelConnectedForecolor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelConnectedBackground
            // 
            this.labelConnectedBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.labelConnectedBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelConnectedBackground.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConnectedBackground.ForeColor = System.Drawing.Color.Silver;
            this.labelConnectedBackground.Location = new System.Drawing.Point(12, 324);
            this.labelConnectedBackground.Name = "labelConnectedBackground";
            this.labelConnectedBackground.Size = new System.Drawing.Size(225, 21);
            this.labelConnectedBackground.TabIndex = 20;
            this.labelConnectedBackground.Text = "Connected Bots";
            this.labelConnectedBackground.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 356);
            this.Controls.Add(this.labelConnectedForecolor);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelConnectedBackground);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox chatBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelConnectedForecolor;
        private System.Windows.Forms.Label labelConnectedBackground;
    }
}

