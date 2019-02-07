namespace VizorNEW
{
    partial class Form2
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.bExit = new System.Windows.Forms.Button();
            this.bShowNewEvents = new System.Windows.Forms.Button();
            this.bShowAllEvents = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Пыщьььь";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(12, 97);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(652, 273);
            this.textBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 39);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(155, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Добавить ключ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(266, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(180, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Включить перехват событий";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // bExit
            // 
            this.bExit.Location = new System.Drawing.Point(589, 23);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(75, 23);
            this.bExit.TabIndex = 4;
            this.bExit.Text = "Выход";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // bShowNewEvents
            // 
            this.bShowNewEvents.Location = new System.Drawing.Point(266, 38);
            this.bShowNewEvents.Name = "bShowNewEvents";
            this.bShowNewEvents.Size = new System.Drawing.Size(180, 23);
            this.bShowNewEvents.TabIndex = 5;
            this.bShowNewEvents.Text = "Показать новые события";
            this.bShowNewEvents.UseVisualStyleBackColor = true;
            this.bShowNewEvents.Click += new System.EventHandler(this.bShowNewEvents_Click);
            // 
            // bShowAllEvents
            // 
            this.bShowAllEvents.Location = new System.Drawing.Point(266, 67);
            this.bShowAllEvents.Name = "bShowAllEvents";
            this.bShowAllEvents.Size = new System.Drawing.Size(180, 23);
            this.bShowAllEvents.TabIndex = 6;
            this.bShowAllEvents.Text = "Показать все события";
            this.bShowAllEvents.UseVisualStyleBackColor = true;
            this.bShowAllEvents.Click += new System.EventHandler(this.bShowAllEvents_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 382);
            this.Controls.Add(this.bShowAllEvents);
            this.Controls.Add(this.bShowNewEvents);
            this.Controls.Add(this.bExit);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.Button bShowNewEvents;
        private System.Windows.Forms.Button bShowAllEvents;
    }
}