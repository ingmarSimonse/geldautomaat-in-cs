using Geldautomaat.classes;
using Geldautomaat___Medewerker.Views;
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

namespace Geldautomaat___Medewerker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Account acc = new Account();

        public MainWindow()
        {
            InitializeComponent();

            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            int ID;
            int.TryParse(txbUserName.Text, out ID);

            if (acc.VerifyAccount(pwbPassword.Password, ID))
            {
                // open admin window
                AdminView adminView = new AdminView();
                adminView.Show();
                this.Close();
            }
        }
    }
}
