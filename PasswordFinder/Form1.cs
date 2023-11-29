using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateProfileList();
        }

        private void UpdateProfileList()
        {
            // netsh komutunu çalıştır ve çıktısını oku
            string output = RunNetshCommand("wlan show profiles");

            // Profil isimlerini çıktıdan al ve combobox'a ekle
            string[] profiles = GetProfileNames(output);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(profiles);
        }

        private string RunNetshCommand(string arguments)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string[] GetProfileNames(string netshOutput)
        {
            // netsh çıktısından profil isimlerini ayıkla
            string[] lines = netshOutput.Split('\n');
            System.Collections.Generic.List<string> profiles = new System.Collections.Generic.List<string>();

            foreach (string line in lines)
            {
                if (line.Contains("All User Profile"))
                {
                    // "All User Profile" içeren satırdan profil adını çıkar
                    int startIndex = line.IndexOf(":") + 1;
                    string profileName = line.Substring(startIndex).Trim();
                    profiles.Add(profileName);
                }
            }

            return profiles.ToArray();
        }

    
        private void button1_Click(object sender, EventArgs e)
        {
            // "Key'i Göster" butonuna tıklandığında seçilen profille ilgili key'i al ve TextBox'a yazdır
            string selectedProfile = comboBox1.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedProfile))
            {
                // netsh komutunu çalıştır ve çıktısını oku
                string commandOutput = RunNetshCommand($"wlan show profiles name=\"{selectedProfile}\" key=clear");

                // "Key Content" değerini al
                string keyContent = GetKeyContent(commandOutput);

                // TextBox'a yazdır
                textBox1.Text = keyContent;
            }
        }

        private string GetKeyContent(string netshOutput)
        {
            // netsh çıktısından "Key Content" değerini ayıkla
            string[] lines = netshOutput.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains("Key Content"))
                {
                    // "Key Content" içeren satırdan değeri çıkar
                    int startIndex = line.IndexOf(":") + 1;
                    string keyContent = line.Substring(startIndex).Trim();
                    return keyContent;
                }
            }

            return "Key Content not found.";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

