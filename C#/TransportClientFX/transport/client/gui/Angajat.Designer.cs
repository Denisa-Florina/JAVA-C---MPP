using System.ComponentModel;

namespace proiect.view;

partial class Angajat
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
        listView1 = new System.Windows.Forms.ListView();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        textBox1 = new System.Windows.Forms.TextBox();
        dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
        Search = new System.Windows.Forms.Button();
        label3 = new System.Windows.Forms.Label();
        label4 = new System.Windows.Forms.Label();
        Passagers = new System.Windows.Forms.TextBox();
        Buy = new System.Windows.Forms.Button();
        seats = new System.Windows.Forms.TextBox();
        SuspendLayout();
        // 
        // listView1
        // 
        listView1.Location = new System.Drawing.Point(29, 32);
        listView1.Name = "listView1";
        listView1.Size = new System.Drawing.Size(528, 353);
        listView1.TabIndex = 0;
        listView1.UseCompatibleStateImageBehavior = false;
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(628, 41);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(101, 32);
        label1.TabIndex = 2;
        label1.Text = "Destination:";
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(648, 88);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(53, 28);
        label2.TabIndex = 3;
        label2.Text = "Date:";
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(752, 41);
        textBox1.Name = "textBox1";
        textBox1.Size = new System.Drawing.Size(237, 27);
        textBox1.TabIndex = 4;
        // 
        // dateTimePicker1
        // 
        dateTimePicker1.Location = new System.Drawing.Point(751, 89);
        dateTimePicker1.Name = "dateTimePicker1";
        dateTimePicker1.Size = new System.Drawing.Size(238, 27);
        dateTimePicker1.TabIndex = 5;
        // 
        // Search
        // 
        Search.Location = new System.Drawing.Point(808, 149);
        Search.Name = "Search";
        Search.Size = new System.Drawing.Size(134, 35);
        Search.TabIndex = 6;
        Search.Text = "Search";
        Search.UseVisualStyleBackColor = true;
        Search.Click += Search_Click;
        // 
        // label3
        // 
        label3.Location = new System.Drawing.Point(648, 241);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(65, 28);
        label3.TabIndex = 7;
        label3.Text = "Seats:";
        // 
        // label4
        // 
        label4.Location = new System.Drawing.Point(638, 300);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(91, 28);
        label4.TabIndex = 9;
        label4.Text = "Passagers:";
        // 
        // Passagers
        // 
        Passagers.Location = new System.Drawing.Point(768, 301);
        Passagers.Name = "Passagers";
        Passagers.Size = new System.Drawing.Size(231, 27);
        Passagers.TabIndex = 10;
        // 
        // Buy
        // 
        Buy.Location = new System.Drawing.Point(811, 367);
        Buy.Name = "Buy";
        Buy.Size = new System.Drawing.Size(131, 36);
        Buy.TabIndex = 11;
        Buy.Text = "Buy";
        Buy.UseVisualStyleBackColor = true;
        Buy.Click += Buy_Click;
        // 
        // seats
        // 
        seats.Location = new System.Drawing.Point(766, 242);
        seats.Name = "seats";
        seats.Size = new System.Drawing.Size(233, 27);
        seats.TabIndex = 12;
        // 
        // Angajat
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1073, 450);
        Controls.Add(seats);
        Controls.Add(Buy);
        Controls.Add(Passagers);
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(Search);
        Controls.Add(dateTimePicker1);
        Controls.Add(textBox1);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(listView1);
        Text = "Angajat";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.TextBox seats;

    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox Passagers;
    private System.Windows.Forms.Button Buy;

    private System.Windows.Forms.Label label3;

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.Button Search;

    private System.Windows.Forms.ListView listView1;

    #endregion
}