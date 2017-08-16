namespace Long_Connection
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
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_fasong = new System.Windows.Forms.Button();
            this.textBox_send = new System.Windows.Forms.TextBox();
            this.listBox_chat = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(63, 313);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 23);
            this.button_start.TabIndex = 4;
            this.button_start.Text = "启动";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(160, 313);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(75, 23);
            this.button_stop.TabIndex = 5;
            this.button_stop.Text = "停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_fasong
            // 
            this.button_fasong.Location = new System.Drawing.Point(236, 269);
            this.button_fasong.Name = "button_fasong";
            this.button_fasong.Size = new System.Drawing.Size(75, 23);
            this.button_fasong.TabIndex = 6;
            this.button_fasong.Text = "发送";
            this.button_fasong.UseVisualStyleBackColor = true;
            this.button_fasong.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox_send
            // 
            this.textBox_send.Location = new System.Drawing.Point(12, 269);
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.Size = new System.Drawing.Size(208, 21);
            this.textBox_send.TabIndex = 7;
            // 
            // listBox_chat
            // 
            this.listBox_chat.FormattingEnabled = true;
            this.listBox_chat.ItemHeight = 12;
            this.listBox_chat.Location = new System.Drawing.Point(12, 12);
            this.listBox_chat.Name = "listBox_chat";
            this.listBox_chat.Size = new System.Drawing.Size(299, 220);
            this.listBox_chat.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 348);
            this.Controls.Add(this.listBox_chat);
            this.Controls.Add(this.textBox_send);
            this.Controls.Add(this.button_fasong);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Name = "Form1";
            this.Text = "服务端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_fasong;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.ListBox listBox_chat;
    }
}

