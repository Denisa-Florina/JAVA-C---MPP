using System.ComponentModel;

namespace proiect.view;

partial class Login
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        Log = new System.Windows.Forms.Button();
        username = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        password = new System.Windows.Forms.TextBox();
        SuspendLayout();
        // 
        // Log
        // 
        Log.BackColor = System.Drawing.Color.LightBlue;
        Log.Location = new System.Drawing.Point(285, 207);
        Log.Name = "Log";
        Log.Size = new System.Drawing.Size(172, 46);
        Log.TabIndex = 0;
        Log.Text = "Login";
        Log.UseVisualStyleBackColor = false;
        Log.Click += Log_Click;
        // 
        // username
        // 
        username.Location = new System.Drawing.Point(232, 79);
        username.Name = "username";
        username.Size = new System.Drawing.Size(372, 27);
        username.TabIndex = 2;
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("Segoe Script", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
        label1.Location = new System.Drawing.Point(102, 79);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(124, 28);
        label1.TabIndex = 3;
        label1.Text = "Username:";
        // 
        // label2
        // 
        label2.Font = new System.Drawing.Font("Segoe Script", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
        label2.Location = new System.Drawing.Point(102, 135);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(94, 25);
        label2.TabIndex = 4;
        label2.Text = "Password:";
        // 
        // password
        // 
        password.Location = new System.Drawing.Point(233, 135);
        password.Name = "password";
        password.PasswordChar = '*';
        password.Size = new System.Drawing.Size(371, 27);
        password.TabIndex = 5;
        // 
        // Login
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.WhiteSmoke;
        ClientSize = new System.Drawing.Size(695, 332);
        Controls.Add(password);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(username);
        Controls.Add(Log);
        Text = "Login";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox password;

    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.TextBox username;

    private System.Windows.Forms.Button Log;

    #endregion
}