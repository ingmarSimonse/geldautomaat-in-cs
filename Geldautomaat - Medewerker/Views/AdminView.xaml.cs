using Geldautomaat.classes;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Geldautomaat___Medewerker.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        DataSet dsAdmin;
        Account account = new Account();

        public AdminView()
        {
            InitializeComponent();

            btnSearch.Click += BtnSearch_Click;
            btnLogOut.Click += BtnLogOut_Click;

            dsAdmin = account.GetAllAccounts();
            dsAdmin.Tables[0].Rows.Add();
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["name"] = "Nieuw account";
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["admin"] = false;
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["blocked"] = false;
            dgUsers.DataContext = dsAdmin;
        }

        private void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = "%" + txbSearch.Text + "%";
            dsAdmin = account.GetSearchAccounts(searchQuery);
            dsAdmin.Tables[0].Rows.Add();
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["name"] = "Nieuw account";
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["admin"] = false;
            dsAdmin.Tables[0].Rows[dsAdmin.Tables[0].Rows.Count - 1]["blocked"] = false;
            dgUsers.DataContext = dsAdmin;
        }

        private void BtnResetPin_Click(object sender, RoutedEventArgs e)
        {
            // get cellinfo id
            DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[0]);
            TextBlock cellContent = (TextBlock)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            int accountID = 0;
            int.TryParse(cellContent.Text.ToString(), out accountID);

            account.ResetPin(accountID);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // get cellinfo name
            var dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[1]);
            TextBlock cellContent = (TextBlock)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            string name = cellContent.Text.ToString();

            // get cellinfo id
            dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[0]);
            cellContent = (TextBlock)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            int accountID = 0;
            int.TryParse(cellContent.Text.ToString(), out accountID);

            // get cellinfo adress
            dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[2]);
            cellContent = (TextBlock)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            string adress = cellContent.Text.ToString();

            // get cellinfo blocked
            dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[4]);
            CheckBox cbContent = (CheckBox)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            bool isBlocked = (bool)cbContent.IsChecked;

            // get cellinfo admin
            dataGridCellInfo = new DataGridCellInfo(
                dgUsers.Items[dgUsers.SelectedIndex], dgUsers.Columns[5]);
            cbContent = (CheckBox)dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            bool isAdmin = (bool)cbContent.IsChecked;

            // check if new account
            if (accountID == 0)
            {
                // new account
                account.Create(name, adress, isBlocked, isAdmin);
            }
            else
            {
                // update account
                account.Update(name, adress, accountID, isBlocked, isAdmin);
            }

            // refresh widow
            AdminView adminView = new AdminView();
            adminView.Show();
            this.Close();
        }
    }
}
