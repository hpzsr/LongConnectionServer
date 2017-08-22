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
            this.label_peopleNum = new System.Windows.Forms.Label();
            this.label_roomNum = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(376, 30);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 23);
            this.button_start.TabIndex = 4;
            this.button_start.Text = "启动";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(376, 73);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(75, 23);
            this.button_stop.TabIndex = 5;
            this.button_stop.Text = "停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_fasong
            // 
            this.button_fasong.Location = new System.Drawing.Point(275, 313);
            this.button_fasong.Name = "button_fasong";
            this.button_fasong.Size = new System.Drawing.Size(75, 23);
            this.button_fasong.TabIndex = 6;
            this.button_fasong.Text = "发送";
            this.button_fasong.UseVisualStyleBackColor = true;
            this.button_fasong.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox_send
            // 
            this.textBox_send.Location = new System.Drawing.Point(12, 313);
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.Size = new System.Drawing.Size(257, 21);
            this.textBox_send.TabIndex = 7;
            // 
            // listBox_chat
            // 
            this.listBox_chat.FormattingEnabled = true;
            this.listBox_chat.ItemHeight = 12;
            this.listBox_chat.Location = new System.Drawing.Point(12, 12);
            this.listBox_chat.Name = "listBox_chat";
            this.listBox_chat.Size = new System.Drawing.Size(338, 280);
            this.listBox_chat.TabIndex = 8;
            // 
            // label_peopleNum
            // 
            this.label_peopleNum.AutoSize = true;
            this.label_peopleNum.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_peopleNum.Location = new System.Drawing.Point(381, 162);
            this.label_peopleNum.Name = "label_peopleNum";
            this.label_peopleNum.Size = new System.Drawing.Size(56, 16);
            this.label_peopleNum.TabIndex = 9;
            this.label_peopleNum.Text = "人数:0";
            this.label_peopleNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_roomNum
            // 
            this.label_roomNum.AutoSize = true;
            this.label_roomNum.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_roomNum.Location = new System.Drawing.Point(381, 127);
            this.label_roomNum.Name = "label_roomNum";
            this.label_roomNum.Size = new System.Drawing.Size(56, 16);
            this.label_roomNum.TabIndex = 10;
            this.label_roomNum.Text = "房间:0";
            this.label_roomNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 348);
            this.Controls.Add(this.label_roomNum);
            this.Controls.Add(this.label_peopleNum);
            this.Controls.Add(this.listBox_chat);
            this.Controls.Add(this.textBox_send);
            this.Controls.Add(this.button_fasong);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "服务端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_fasong;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.ListBox listBox_chat;
        private System.Windows.Forms.Label label_peopleNum;
        private System.Windows.Forms.Label label_roomNum;
    }
}

