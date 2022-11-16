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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Geldautomaat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int currentWindow;
        // 0 = login_rekening   |   1 = login_pincode
        // 2 = options          |   3 = balance
        // 4 = transactions     |   5 = withdraw
        // 6 = deposit          |   7 = admin
        public string pinInputString = "";
        public int accountID;
        Account acc = new Account();

        public MainWindow()
        {
            InitializeComponent();

            // make font work
            Style = (Style)FindResource(typeof(Window));

            currentWindow = 0;

            // pin machine buttons
            btn0.Click += Btn0_Click;
            btn1.Click += Btn1_Click;
            btn2.Click += Btn2_Click;
            btn3.Click += Btn3_Click;
            btn4.Click += Btn4_Click;
            btn5.Click += Btn5_Click;
            btn6.Click += Btn6_Click;
            btn7.Click += Btn7_Click;
            btn8.Click += Btn8_Click;
            btn9.Click += Btn9_Click;
            btnBack.Click += BtnBack_Click;
            btnOk.Click += BtnOk_Click;

            btnPinBack.Click += BtnLogOut_Click;

            // options buttons
            btnOptionBalance.Click += BtnOptionBalance_Click;
            btnOptionDeposit.Click += BtnOptionDeposit_Click;
            btnOptionTransactions.Click += BtnOptionTransactions_Click;
            btnOptionWithdraw.Click += BtnOptionWithdraw_Click;
            btnLogOut.Click += BtnLogOut_Click;

            // balance button
            btnBalanceBack.Click += BtnBalanceBack_Click;

            // transactions button
            btnTransactionsBack.Click += BtnTransactionsBack_Click;
        }

        // transactions button
        private void BtnTransactionsBack_Click(object sender, RoutedEventArgs e)
        {
            grdLastTransactions.Visibility = Visibility.Hidden;
            grdOptions.Visibility = Visibility.Visible;
            currentWindow = 2;
        }

        // balance button
        private void BtnBalanceBack_Click(object sender, RoutedEventArgs e)
        {
            grdOptions.Visibility = Visibility.Visible;
            grdBalance.Visibility = Visibility.Hidden;
            currentWindow = 2;
        }

        // options buttons
        private void BtnOptionWithdraw_Click(object sender, RoutedEventArgs e)
        {
            grdOptions.Visibility = Visibility.Hidden;
            grdLogin.Visibility = Visibility.Visible;
            lblTitle.Content = "Voer een bedrag in";
            lblPlaceholderLogin.Content = "";
            currentWindow = 5;
        }

        private void BtnOptionTransactions_Click(object sender, RoutedEventArgs e)
        {
            currentWindow = 4;
            grdLastTransactions.Visibility = Visibility.Visible;
            grdOptions.Visibility = Visibility.Hidden;
            Transaction transaction = new Transaction();
            // get last 3 transactions
            DataTable lastTransactions = transaction.LastTransactions(accountID).Tables[0];
            if (lastTransactions.Rows.Count == 0)
            {
                // no transactions where made
                lblTransaction1.Content = "U heeft nog geen transacties gemaakt";
            } else
            {
                // get last three transactions
                if (lastTransactions.Rows.Count > 0)
                {
                    string dateStr = lastTransactions.Rows[0]["date"].ToString();
                    dateStr = dateStr.Remove(dateStr.Length - 3);
                    lblTransaction1.Content = dateStr + " | €" + lastTransactions.Rows[0]["amount"] + ",-";
                }
                if (lastTransactions.Rows.Count > 1)
                {
                    string dateStr = lastTransactions.Rows[1]["date"].ToString();
                    dateStr = dateStr.Remove(dateStr.Length - 3);
                    lblTransaction2.Content = dateStr + " | €" + lastTransactions.Rows[1]["amount"] + ",-";
                }
                if (lastTransactions.Rows.Count > 2)
                {
                    string dateStr = lastTransactions.Rows[2]["date"].ToString();
                    dateStr = dateStr.Remove(dateStr.Length - 3);
                    lblTransaction3.Content = dateStr + " | €" + lastTransactions.Rows[2]["amount"] + ",-";
                }
            }
        }

        private void BtnOptionDeposit_Click(object sender, RoutedEventArgs e)
        {
            grdOptions.Visibility = Visibility.Hidden;
            grdLogin.Visibility = Visibility.Visible;
            lblTitle.Content = "Voer een bedrag in";
            lblPlaceholderLogin.Content = "";
            currentWindow = 6;
        }

        private void BtnOptionBalance_Click(object sender, RoutedEventArgs e)
        {
            currentWindow = 3;
            grdOptions.Visibility = Visibility.Hidden;
            grdBalance.Visibility = Visibility.Visible;
            lblBalance.Content = "€" + acc.GetBalance(accountID) + ",-";
        }

        private void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        // alert
        private void Alert(string message)
        {
            lblAlert.Content = message;
        }

        // pin buttons
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // check if window is pin or acc number
            if (currentWindow == 0)
            {
                // check if account exists
                accountID = int.Parse(pinInputString);
                if (acc.AccountExists(accountID))
                {
                    btnPinBack.Visibility = Visibility.Visible;
                    grdPinBack.Visibility = Visibility.Visible;
                    currentWindow = 1;
                    lblTitle.Content = "Voer uw pincode in";
                    lblPlaceholderLogin.Content = "";
                    Alert("");
                    pinInputString = "";
                }
                else
                {
                    Alert("Rekeningnummer niet gevonden");
                }
            }
            else if (currentWindow == 1)
            {
                // if pin correct go to options screen
                if (acc.VerifyPin(pinInputString, accountID))
                {
                    currentWindow = 2;

                    btnPinBack.Visibility = Visibility.Hidden;
                    grdPinBack.Visibility = Visibility.Hidden;

                    // show options window
                    grdLogin.Visibility = Visibility.Hidden;
                    grdOptions.Visibility = Visibility.Visible;
                    pinInputString = "";
                } else
                {
                    // pincode incorrect
                    currentWindow = 0;
                    lblTitle.Content = "Voer uw rekeningnummer in";
                    lblPlaceholderLogin.Content = "";
                    Alert("Pincode incorrect");
                    pinInputString = "";
                    btnPinBack.Visibility = Visibility.Hidden;
                    grdPinBack.Visibility = Visibility.Hidden;
                }
            } else if (currentWindow == 5)
            {
                // withdraw
                if (pinInputString != "" && int.Parse(pinInputString) != 0)
                {
                    int withdrawAmount = 0;
                    int.TryParse("-" + pinInputString, out withdrawAmount);
                    Transaction transaction = new Transaction(accountID, withdrawAmount);
                    string transactionDone = transaction.DoTransaction();
                    Alert(transactionDone);
                }

                grdLogin.Visibility = Visibility.Hidden;
                grdOptions.Visibility= Visibility.Visible;
                pinInputString = "";
            }
            else
            {
                // deposit
                if (pinInputString != "" && int.Parse(pinInputString) != 0)
                {
                    int withdrawAmount = 0;
                    int.TryParse(pinInputString, out withdrawAmount);
                    Transaction transaction = new Transaction(accountID, withdrawAmount);
                    string transactionDone = transaction.DoTransaction();
                    Alert(transactionDone);
                }
                grdLogin.Visibility = Visibility.Hidden;
                grdOptions.Visibility = Visibility.Visible;
                pinInputString = "";
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (pinInputString.Length > 0)
            {
                pinInputString = pinInputString.Remove(pinInputString.Length - 1);
            }

            displayPin();
        }

        private void Btn9_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "9";
            displayPin();
        }

        private void Btn8_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "8";
            displayPin();
        }

        private void Btn7_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "7";
            displayPin();
        }

        private void Btn6_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "6";
            displayPin();
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "5";
            displayPin();
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "4";
            displayPin();
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "3";
            displayPin();
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "2";
            displayPin();
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "1";
            displayPin();
        }

        private void Btn0_Click(object sender, RoutedEventArgs e)
        {
            pinInputString += "0";
            displayPin();
        }

        private void displayPin()
        {
            // display account number or pin in lbl
            if (currentWindow == 1)
            {
                string pinString = "";
                for (int i = 0; i < pinInputString.Length; i++)
                {
                    pinString += "*";
                }

                lblPlaceholderLogin.Content = pinString;
            } else
            {
                lblPlaceholderLogin.Content = pinInputString;
            }
        }
    }
}
