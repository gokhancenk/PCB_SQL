using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.DirectoryServices.ActiveDirectory;

namespace PCB_SQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var dataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pcb.db");
            SQLiteConnection connection = new SQLiteConnection("Data Source=" + dataPath);

            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT description FROM list WHERE str = @strValue", connection);

            command.Parameters.AddWithValue("@strValue", PCB_code.Text);

            var description = command.ExecuteScalar() as string;

            if (!string.IsNullOrEmpty(description))
            {
                PCB_description.Text = description;
            }
            else
            {
                PCB_description.Text = "Açıklama bulunamadı!";
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            int numbersNums = 0;
            string code = "";

            while(code.Length < 4)
            {
                int charType = random.Next(2);
                if (numbersNums > 1) { charType = 0; }
                if (numbersNums == 0 && code.Length == 3 ) { charType = 1; }
                if (charType == 0)
                {
                    int index = random.Next(26);
                    code += chars.ElementAt(index);
                }
                else if (charType == 1)
                {
                    int index = random.Next(10);
                    code += numbers.ElementAt(index);
                    numbersNums++;
                }
            }

            PCB_code.Text = code;
        }
    

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PCB_code.Text))
            {
                MessageBox.Show("Lütfen kod oluşturunuz!");
                return;
            }
            if (string.IsNullOrWhiteSpace(PCB_description.Text))
            {
                MessageBox.Show("Lütfen açıklama giriniz!");
                return;
            }
            Boolean done = false;
            while(!done)
            {
                var dataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pcb.db");
                SQLiteConnection connection = new SQLiteConnection("Data Source=" + dataPath);

                connection.Open();

                SQLiteCommand command = new SQLiteCommand("INSERT INTO list (str, description) VALUES (@str, @description)", connection);

                command.Parameters.AddWithValue("@str", PCB_code.Text);
                command.Parameters.AddWithValue("@description", PCB_description.Text);

                try
                {
                    command.ExecuteNonQuery();
                    done = true;
                }
                catch 
                {
                    Create_Click(sender, e);
                }

                connection.Close();
            }
            
            
            MessageBox.Show("Kaydedildi!");
        }

       
        private void PCB_code_TextChanged(object sender, TextChangedEventArgs e)
        {
            PCB_code.Text = PCB_code.Text.ToUpper().Replace("İ", "I");
            PCB_code.SelectionStart = PCB_code.Text.Length;

        }

        //when pressed the enter after writing the pcb code, searching will start
        private void PCB_code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search_Click(sender, e);
            }
        }
    }
}
