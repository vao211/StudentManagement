using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StudentManagement
{
    public partial class Student : Form
    {
        public Student()
        {
            InitializeComponent();
        }


        void refresh()
        {
            datagrvStudent.DataSource = GetStudent().Tables[0];
            datagrvStudent.Refresh();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void Student_Load(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
            //Fill bảng vừa form
            datagrvStudent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            datagrvStudent.DataSource = GetStudent().Tables[0];
            // datagrvStudent.DataMember = "Students";
        }

        DataSet GetStudent(string search = "")
        {
            DataSet data = new DataSet();
            int id = 0;
            string query = "SELECT * FROM Students";

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out id))
            {
                query += " Where student_id = @id or department_id = @DepID or program_id = @ProID";
            }
            else if (!string.IsNullOrEmpty(search) && !int.TryParse(search, out _))
            {
                query += " WHERE first_name LIKE @FName +'%' OR last_name LIKE @LName +'%' OR department_name LIKE @DepName +'%'";
            }
           
            using (SqlConnection connection = new SqlConnection(Connection.connectionString))
            {
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@FName", txtSearch.Text.Trim());
            cmd.Parameters.AddWithValue("@LName", txtSearch.Text.Trim());
            cmd.Parameters.AddWithValue("@DepName", txtSearch.Text.Trim());
            cmd.Parameters.AddWithValue("@DepID", txtSearch.Text.Trim());
            cmd.Parameters.AddWithValue("@ProID", txtSearch.Text.Trim());
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
            sqlAdapter.Fill(data);
            connection.Close();
            return data;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = txtSearch.Text.Trim();
                datagrvStudent.DataSource = GetStudent(search).Tables[0];
                datagrvStudent.Refresh();
            }
            catch { }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "insert into Students values (@Id,@FName,@LName,@DepartmentID,@DepartmentName,@ProgramID)";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@Id", int.Parse(txtStuID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@FName", txtFName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LName", txtLName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DepartmentID", int.Parse(txtDepartmentID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@DepartmentName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgramID", int.Parse(txtProgramID.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (System.FormatException)
            { 

            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Lỗi trùng ID
                {
                    MessageBox.Show("ID cannot be duplicated!");
                }
                else if (ex.Number == 547) // Lỗi khóa ngoại không tồn tại
                {
                   MessageBox.Show("ID not exist!");
                  // MessageBox.Show(ex.Message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "UPDATE Students \n SET student_id = @Id, first_name = @FName, last_name = @LName," +
                        " department_id = @DepartmentID, department_name = @DepartmentName, program_id = @ProgramID \n WHERE student_id = @Id";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@Id", int.Parse(txtStuID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@FName", txtFName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LName", txtLName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DepartmentID", int.Parse(txtDepartmentID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@DepartmentName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgramID", int.Parse(txtProgramID.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (System.FormatException) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string delQry = "DELETE FROM Students WHERE student_id = @Id";
                    SqlCommand cmd = new SqlCommand(delQry, connection);
                    cmd.Parameters.AddWithValue("@Id", int.Parse(txtStuID.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void datagrvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rowData = datagrvStudent.Rows[e.RowIndex];

                txtStuID.Text = rowData.Cells["student_id"].Value.ToString();
                txtFName.Text = rowData.Cells["first_name"].Value.ToString();
                txtLName.Text = rowData.Cells["last_name"].Value.ToString();
                txtDepartmentID.Text = rowData.Cells["department_id"].Value.ToString();
                txtProgramID.Text = rowData.Cells["program_id"].Value.ToString();

                btnDelete.Enabled = true;
                btnEdit.Enabled = true;
            }
        }

        private void btnDepartmentView_Click(object sender, EventArgs e)
        {
            Department department = new Department();
            department.Show();
        }
    }
}
