namespace ChatClient
{
    partial class ChatSessionForm
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
            this.ChatTextBox = new System.Windows.Forms.TextBox();
            this.OnlineLabel = new System.Windows.Forms.Label();
            this.OnlineTextBox = new System.Windows.Forms.TextBox();
            this.MessageBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ChatTextBox
            // 
            this.ChatTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ChatTextBox.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChatTextBox.Location = new System.Drawing.Point(176, 25);
            this.ChatTextBox.MaxLength = 999999;
            this.ChatTextBox.Multiline = true;
            this.ChatTextBox.Name = "ChatTextBox";
            this.ChatTextBox.ReadOnly = true;
            this.ChatTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChatTextBox.Size = new System.Drawing.Size(612, 375);
            this.ChatTextBox.TabIndex = 0;
            this.ChatTextBox.TextChanged += new System.EventHandler(this.ChatTextBox_TextChanged);
            // 
            // OnlineLabel
            // 
            this.OnlineLabel.AutoSize = true;
            this.OnlineLabel.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OnlineLabel.Location = new System.Drawing.Point(12, 5);
            this.OnlineLabel.Name = "OnlineLabel";
            this.OnlineLabel.Size = new System.Drawing.Size(99, 17);
            this.OnlineLabel.TabIndex = 1;
            this.OnlineLabel.Text = "Users online:";
            // 
            // OnlineTextBox
            // 
            this.OnlineTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.OnlineTextBox.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OnlineTextBox.Location = new System.Drawing.Point(15, 25);
            this.OnlineTextBox.MaxLength = 999999;
            this.OnlineTextBox.Multiline = true;
            this.OnlineTextBox.Name = "OnlineTextBox";
            this.OnlineTextBox.ReadOnly = true;
            this.OnlineTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OnlineTextBox.Size = new System.Drawing.Size(155, 375);
            this.OnlineTextBox.TabIndex = 2;
            // 
            // MessageBox
            // 
            this.MessageBox.AllowDrop = true;
            this.MessageBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MessageBox.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MessageBox.ForeColor = System.Drawing.Color.Black;
            this.MessageBox.Location = new System.Drawing.Point(15, 412);
            this.MessageBox.Name = "MessageBox";
            this.MessageBox.Size = new System.Drawing.Size(773, 26);
            this.MessageBox.TabIndex = 4;
            this.MessageBox.Enter += new System.EventHandler(this.MessageBox_Enter);
            this.MessageBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MessageBox_KeyPress);
            this.MessageBox.Leave += new System.EventHandler(this.MessageBox_Leave);
            // 
            // ChatSessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MessageBox);
            this.Controls.Add(this.OnlineTextBox);
            this.Controls.Add(this.OnlineLabel);
            this.Controls.Add(this.ChatTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ChatSessionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chat Session [TCP]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatSessionForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox ChatTextBox;
        private System.Windows.Forms.Label OnlineLabel;
        private System.Windows.Forms.TextBox OnlineTextBox;
        private System.Windows.Forms.TextBox MessageBox;
    }
}