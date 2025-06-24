namespace Safari
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            panelLakeB = new Panel();
            panelLakeA = new Panel();
            panelQueueC = new Panel();
            panelQueueB = new Panel();
            panelQueueA = new Panel();
            panelLakeC = new Panel();
            SuspendLayout();
            // 
            // panelLakeB
            // 
            panelLakeB.BackColor = Color.Transparent;
            panelLakeB.Location = new Point(616, 476);
            panelLakeB.Margin = new Padding(2);
            panelLakeB.Name = "panelLakeB";
            panelLakeB.Size = new Size(383, 126);
            panelLakeB.TabIndex = 1;
            // 
            // panelLakeA
            // 
            panelLakeA.BackColor = Color.Transparent;
            panelLakeA.Location = new Point(81, 476);
            panelLakeA.Margin = new Padding(2);
            panelLakeA.Name = "panelLakeA";
            panelLakeA.Size = new Size(297, 126);
            panelLakeA.TabIndex = 2;
            // 
            // panelQueueC
            // 
            panelQueueC.BackColor = Color.Transparent;
            panelQueueC.Location = new Point(1207, 263);
            panelQueueC.Margin = new Padding(2);
            panelQueueC.Name = "panelQueueC";
            panelQueueC.Size = new Size(392, 128);
            panelQueueC.TabIndex = 3;
            // 
            // panelQueueB
            // 
            panelQueueB.BackColor = Color.Transparent;
            panelQueueB.Location = new Point(561, 263);
            panelQueueB.Margin = new Padding(2);
            panelQueueB.Name = "panelQueueB";
            panelQueueB.Size = new Size(377, 128);
            panelQueueB.TabIndex = 4;
            // 
            // panelQueueA
            // 
            panelQueueA.BackColor = Color.Transparent;
            panelQueueA.Location = new Point(39, 263);
            panelQueueA.Margin = new Padding(2);
            panelQueueA.Name = "panelQueueA";
            panelQueueA.Size = new Size(388, 128);
            panelQueueA.TabIndex = 5;
            // 
            // panelLakeC
            // 
            panelLakeC.BackColor = Color.Transparent;
            panelLakeC.Location = new Point(1158, 476);
            panelLakeC.Margin = new Padding(2);
            panelLakeC.Name = "panelLakeC";
            panelLakeC.Size = new Size(391, 126);
            panelLakeC.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1654, 770);
            Controls.Add(panelQueueA);
            Controls.Add(panelQueueB);
            Controls.Add(panelQueueC);
            Controls.Add(panelLakeA);
            Controls.Add(panelLakeB);
            Controls.Add(panelLakeC);
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "SafariSimulation";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private Panel panelLakeB;
        private Panel panelLakeA;
        private Panel panelQueueC;
        private Panel panelQueueB;
        private Panel panelQueueA;
        private Panel panelLakeC;
    }
}