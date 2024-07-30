using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Are you sure to exit ?", "", MessageBoxButtons.YesNo);
            if (Result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            loginRequest();
        }

        private void loginRequest()
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();
            //sqlConnection
            string query = "select * from Accounts where username = '" + username + "' and password = '" + password + "'";
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    //sqlCommand
                    SqlCommand cmd = new SqlCommand(query, connection);

                    //sqlDataReader
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string role = reader["role"].ToString();
                        Main main = new Main();
                        this.Hide();
                        main.GetRole(role);
                        main.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login Failed!");
                    }
                    connection.Close();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
