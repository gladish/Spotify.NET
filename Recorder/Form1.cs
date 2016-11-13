using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Recorder
{
  public partial class Form1 : Form
  {
    private string _username;
    private string _password;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private bool GetCredentials()
    {
      Form prompt = new Form();
      prompt.Width = 500;
      prompt.Height = 150;
      prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
      prompt.Text = "Spotify Login";
      prompt.StartPosition = FormStartPosition.CenterScreen;
      Label textLabel = new Label() { Left = 50, Top = 20, Text = "Username:" };
      TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 150 };
      Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
      confirmation.Click += (sender, e) => { prompt.Close(); };
      prompt.Controls.Add(textBox);
      prompt.Controls.Add(confirmation);
      prompt.Controls.Add(textLabel);
      prompt.AcceptButton = confirmation;
      return prompt.ShowDialog() == DialogResult.OK;
    }

    private void LoadApplicationKey()
    {
      if (string.IsNullOrEmpty(Properties.Settings.Default.ApplicationKey))
      {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.ShowDialog();

        Stream s = null;
        try
        {
          s = dialog.OpenFile();
        }
        catch (Exception) { }

        if (s != null)
        {
          using (s)
          {
            byte[] buff = new byte[256];

            List<byte> keyBytes = new List<byte>();

            int n = 0;
            while ((n = s.Read(buff, 0, buff.Length)) > 0)
            {
              for (int i = 0; i < n; ++i)
                keyBytes.Add(buff[i]);
            }

            string keyUtf8 = System.Text.Encoding.UTF8.GetString(keyBytes.ToArray());
            Properties.Settings.Default.ApplicationKey = keyUtf8;
          }

          
        }
      }
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(Properties.Settings.Default.ApplicationKey))
        LoadApplicationKey();

      if (GetCredentials())
      {
        // TODO: run search
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {

    }

    private void button2_Click_1(object sender, EventArgs e)
    {

    }
  }
}
