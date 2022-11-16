using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Geldautomaat.classes
{
    public class Account
    {
        private int _ID;
        private string _name;
        private int _pin;
        private bool _blocked;
        private bool _admin;
        private int _saldo;


        public Account()
        {

        }

        public Account(string name, int pin, bool blocked, bool admin, int saldo)
        {
            _name = name;
            _pin = pin;
            _blocked = blocked;
            _admin = admin;
            _saldo = saldo;
        }

        public int ID { get { return _ID; } }
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int pin
        {
            get { return _pin; }
            set { _pin = value; }
        }
        public bool blocked
        {
            get { return _blocked; }
            set { _blocked = value; }
        }
        public bool admin
        {
            get { return _admin; }
            set { _admin = value; }
        }
        public int saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }

        SQL sql = new SQL();

        public bool AccountExists(int ID)
        {
            string SQL = string.Format("SELECT 1 FROM account WHERE ID = {0} AND blocked = 0", ID);

            if (sql.GetDataSet(SQL).Tables[0].Rows.Count != 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool IsAdmin(int ID)
        {
            // check if account is admin
            string SQL = string.Format("SELECT 1 FROM account WHERE ID = {0} AND admin = 1", ID);

            if (sql.GetDataSet(SQL).Tables[0].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetBalance(int ID)
        {
            int balance = 0;
            
            string SQL = string.Format("SELECT balance FROM account WHERE ID = {0}", ID);

            int.TryParse(sql.GetDataSet(SQL).Tables[0].Rows[0]["balance"].ToString(), out balance);

            return balance;
        }

        public DataSet GetAllAccounts()
        {
            string SQL = "SELECT account.ID, user.name, user.adress, account.pin, account.blocked, " +
                "account.admin, account.balance From user INNER JOIN account ON user.ID = " +
                "account.userID ORDER BY user.name ASC";

            return sql.GetDataSet(SQL);
        }

        public bool VerifyAccount(string pin, int ID)
        {
            // verify if account exists and if pin is correct
            if (AccountExists(ID))
            {
                return VerifyPin(pin, ID);
            }
            return false;
        }

        public bool VerifyPin(string pin, int ID)
        {
            string SQL = string.Format("SELECT pin FROM account WHERE ID = {0}", ID);

            DataSet dataSet = sql.GetDataSet(SQL);

            return BCryptNet.Verify(pin, dataSet.Tables[0].Rows[0]["pin"].ToString());
        }

        public void ResetPin(int ID)
        {
            Random rnd = new Random();

            // generate pin
            string pin = "";
            for (int i = 0; i < 4; i++)
            {
                int num = rnd.Next(0, 10);
                pin = pin + num;
            }
            string passwordHash = BCryptNet.HashPassword(pin);

            string SQL = string.Format("UPDATE account SET pin = '{1}' WHERE ID = {0}",
                ID, passwordHash);

            sql.ExecuteNonQuery(SQL);

            MessageBox.Show("!pincode: " + pin);
        }

        public void Update(string name, string adress, int ID, bool blocked, bool admin)
        {
            // get userID
            string SQL = string.Format("SELECT userID FROM account WHERE ID = {0}", ID);
            int userID = int.Parse(sql.GetDataSet(SQL).Tables[0].Rows[0]["userID"].ToString());

            // update account
            SQL = string.Format("UPDATE account SET blocked = {1}, admin = {2} " +
                "WHERE ID = {0}", ID, blocked, admin);

            sql.ExecuteNonQuery(SQL);

            // update user
            SQL = string.Format("UPDATE user SET name = '{0}', adress = '{1}' " +
                "WHERE ID = {2}", name, adress, userID);

            sql.ExecuteNonQuery(SQL);
        }

        public void Create(string name, string adress, bool blocked, bool admin)
        {
            Random rnd = new Random();

            // generate pin
            string pin = "";
            for (int i = 0; i < 4; i++)
            {
                int num = rnd.Next(0, 10);
                pin = pin + num;
            }
            string passwordHash = BCryptNet.HashPassword(pin);

            // check if user exists
            string SQL = string.Format("SELECT ID, adress FROM user WHERE name = '{0}'", name);
            DataSet user = sql.GetDataSet(SQL);
            int userID = 0;

            if (user.Tables[0].Rows.Count != 0)
            {
                // user exists get userID
                userID = int.Parse(user.Tables[0].Rows[0]["ID"].ToString());
                adress = user.Tables[0].Rows[0]["adress"].ToString();
            }
            else
            {
                // create user
                SQL = string.Format("INSERT INTO user (name, adress) VALUES ('{0}', '{1}')",
                    name, adress);
                sql.ExecuteNonQuery(SQL);

                // get userID
                SQL = "SELECT LAST_INSERT_ID()";
                userID = int.Parse(sql.GetDataSet(SQL).Tables[0].Rows[0]["LAST_INSERT_ID()"]
                    .ToString());
            }

            SQL = string.Format("INSERT INTO account (userID, pin) VALUES ({0}, '{1}')", 
                userID, passwordHash);

            sql.ExecuteNonQuery(SQL);

            MessageBox.Show("!pincode: " + pin);
        }

        public void Block(int ID)
        {
            string SQL = string.Format("UPDATE account SET blocked = 1 WHERE ID = {0}", ID);

            sql.ExecuteNonQuery(SQL);
        }

        public void Unblock(int ID)
        {
            string SQL = string.Format("UPDATE account SET blocked = 0 WHERE ID = {0}", ID);

            sql.ExecuteNonQuery(SQL);
        }

        public void MakeAdmin(int ID)
        {
            string SQL = string.Format("UPDATE account SET admin = 1 WHERE ID = {0}", ID);

            sql.ExecuteNonQuery(SQL);
        }

        public void UnAdmin(int ID)
        {
            string SQL = string.Format("UPDATE account SET admin = 0 WHERE ID = {0}", ID);

            sql.ExecuteNonQuery(SQL);
        }

        public DataSet GetSearchAccounts(string searchQuery)
        {
            string SQL = string.Format("SELECT account.ID, user.name, user.adress, account.pin, account.blocked, " +
                "account.admin, account.balance From user INNER JOIN account ON user.ID = " +
                "account.userID " +
                "WHERE user.name LIKE '{0}' OR account.ID LIKE '{0}' " +
                "OR user.adress LIKE '{0}' ORDER BY name ASC", searchQuery);

            return sql.GetDataSet(SQL);
        }
    }
}
