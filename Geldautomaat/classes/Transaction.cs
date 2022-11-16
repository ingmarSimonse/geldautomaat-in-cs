using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Geldautomaat.classes
{
    public class Transaction
    {
        private int _ID;
        private int _accountID;
        private int _amount;
        private DateTime _date;

        public Transaction()
        {

        }

        public Transaction(int accountID, int amount)
        {
            _accountID = accountID;
            _amount = amount;
        }

        public int ID { get { return _ID; } }
        public int accountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }
        public int amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public DateTime date
        {
            get { return _date; }
            set { _date = value; }
        }

        SQL sql = new SQL();

        public string DoTransaction()
        {
            int transactionCount = 0;
            string SQL = string.Format("SELECT COUNT(ID) FROM transaction " +
                "WHERE date >= NOW() - INTERVAL 1 DAY AND accountID = {0} " +
                "AND amount < 0", accountID);
            int.TryParse(sql.GetDataSet(SQL).Tables[0].Rows[0]["COUNT(ID)"].ToString(), out transactionCount);

            // max 3 withdraw per day
            if (transactionCount >= 3 && amount < 0)
            {
                return "Je kan maximaal 3 keer per dag geld opnemen";
            }

            int balance = 0;
            string SQL1 = string.Format("SELECT balance FROM account WHERE ID = {0}", accountID);
            int.TryParse(sql.GetDataSet(SQL1).Tables[0].Rows[0]["balance"].ToString(), out balance);


            if (amount < -500)
            {
                // cant withdraw more than 500
                return "Je kan niet meer dan €500,- per keer opnemen";
            }
            else if (balance + amount < 0)
            {
                // amount to withdraw is bigger than balance
                return "Je kan niet meer dan je balans opnemen";
            }
            else
            {
                balance = balance + amount;
                // insert transaction in DB
                string SQL2 = string.Format("INSERT INTO transaction (accountID, amount)" +
                    "VALUES ({0}, {1})", accountID, amount);
                sql.ExecuteNonQuery(SQL2);
                // update balance
                string SQL3 = string.Format("UPDATE account SET balance = {0}" +
                    " WHERE ID = {1}", balance, accountID);
                sql.ExecuteNonQuery(SQL3);
            }

            return "";
        }

        public DataSet LastTransactions(int accID)
        {
            int transactionAmount = 3;

            string SQL = string.Format("SELECT amount, date FROM transaction WHERE accountID = {0} " +
                    "ORDER BY date DESC LIMIT {1}", accID, transactionAmount);

            return sql.GetDataSet(SQL);
        }
    }
}
