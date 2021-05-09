using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ClientWPF
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

        private void Register_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFieldsFilled())
            {
                MessageBox.Show("Please, fill all input fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (IsFieldsFilledCorrectly())
            {
                Chat window = new Chat(IPAddress_textBox.Text, Port_textBox.Text, Name_textBox.Text);
                window.Show();
                Close();
            }
        }

        private bool IsFieldsFilledCorrectly()
        {
            string value = IPAddress_textBox.Text;
            bool isValid = IPAddress.TryParse(value, out _);
            if (!isValid)
            {
                MessageBox.Show("Error with IP", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IPAddress_textBox.Text = "";
                return false;
            }

            value = Port_textBox.Text;
            isValid = int.TryParse(value, out _);
            if (!isValid)
            {
                MessageBox.Show("Error with port", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Port_textBox.Text = "";
                return false;
            }

            return true;
        }

        private bool IsFieldsFilled()
        {
            if (IPAddress_textBox.Text == "" || Port_textBox.Text == "" || Name_textBox.Text == "")
            {
                return false;
            }

            return true;
        }

        private void Port_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = new string
                (
                    textBox.Text
                           .Where
                            (ch =>
                                ch == '0' || ch == '1' || ch == '2' || ch == '3' ||
                                ch == '4' || ch == '5' || ch == '6' || ch == '7' ||
                                ch == '8' || ch == '9'
                            )
                           .ToArray()
                );
                textBox.SelectionStart = e.Changes.First().Offset + 1;
                textBox.SelectionLength = 0;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Register_btn_Click(sender, e);
            }
        }
    }
}
