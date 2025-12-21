namespace YurtVeriTabani
{
    partial class AnaSayfa
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            anapanel = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            panel1 = new Panel();
            label1 = new Label();
            anapanel.SuspendLayout();
            SuspendLayout();
            // 
            // anapanel
            // 
            anapanel.BackColor = Color.FromArgb(0, 54, 123);
            anapanel.BorderRadius = 40;
            anapanel.Controls.Add(panel1);
            anapanel.Controls.Add(label1);
            anapanel.CustomizableEdges = customizableEdges1;
            anapanel.Dock = DockStyle.Fill;
            anapanel.Location = new Point(0, 0);
            anapanel.Name = "anapanel";
            anapanel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            anapanel.Size = new Size(800, 450);
            anapanel.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Location = new Point(769, 408);
            panel1.Name = "panel1";
            panel1.Size = new Size(53, 52);
            panel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(339, 200);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // AnaSayfa
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 54, 123);
            ClientSize = new Size(800, 450);
            Controls.Add(anapanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AnaSayfa";
            Text = "AnaSayfa";
            TransparencyKey = Color.FromArgb(4, 244, 4);
            anapanel.ResumeLayout(false);
            anapanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel anapanel;
        private Label label1;
        private Panel panel1;
    }
}