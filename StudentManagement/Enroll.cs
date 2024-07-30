﻿using System;
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
    public partial class Enroll : Form
    {
        public Enroll()
        {
            InitializeComponent();
        }

        private void Enroll_Load(object sender, EventArgs e)
        {
            btnDeleteStu.Enabled = false;
            btnEditStu.Enabled = false;
            //Fill bảng vừa form
            datagrvEnroll.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            datagrvEnroll.DataSource = GetEnroll().Tables[0];
        }

        DataSet GetEnroll(string search = "")
        {
            DataSet data = new DataSet();
            int id = 0;
            string query = "SELECT * FROM Enrollment";

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out id))
            {
                query += " Where student_id = @StuId OR class_id = @ClassID";
            }
            else if (!string.IsNullOrEmpty(search) && !int.TryParse(search, out _))
            {
                query += " WHERE first_name LIKE @FName +'%' OR last_name LIKE @LName +'%' OR class_name LIKE @ClassName +'%'";
            }

            //sqlConnection
            using (SqlConnection connection = new SqlConnection(Connection.connectionString))
            {
                connection.Open();
                //sqlCommand
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StuID", id);
                cmd.Parameters.AddWithValue("@ClassID", id);
                cmd.Parameters.AddWithValue("@FName", txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@LName", txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@ClassName", txtSearch.Text.Trim());
                //sqlDataAdepter
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(data);
                connection.Close();
            }
            return data;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            datagrvEnroll.DataSource = GetEnroll(search).Tables[0];
            datagrvEnroll.Refresh();
        }

        void refresh()
        {
            datagrvEnroll.DataSource = GetEnroll().Tables[0];
            datagrvEnroll.Refresh();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddStu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "insert into Enrollment values (@StuID,@ClassID,@FName,@LName,@ClassName,@Grade)";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@StuId", int.Parse(txtStuID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@ClassID", int.Parse(txtClassID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@FName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClassName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Grade", double.Parse(txtGrade.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check your entered information!");
            }
        }

        private void btnEditStu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "UPDATE Enrollment \n SET student_id = @StuId, class_id = @ClassID, " +
                        "first_name = @FName, last_name = @LName, class_name = @ClassName, grade = @Grade WHERE student_id = @StuId";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@StuID", int.Parse(txtStuID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@ClassID", int.Parse(txtClassID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@FName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClassName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Grade", double.Parse(txtGrade.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnDeleteStu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string delQry = "DELETE FROM Enrollment WHERE student_id = @StuId and class_id = @ClassID";
                    SqlCommand cmd = new SqlCommand(delQry, connection);
                    cmd.Parameters.AddWithValue("@StuId", int.Parse(txtStuID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@ClassID", int.Parse(txtClassID.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void datagrvEnroll_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rowData = datagrvEnroll.Rows[e.RowIndex];

                txtStuID.Text = rowData.Cells["student_id"].Value.ToString();
                txtClassID.Text = rowData.Cells["class_id"].Value.ToString();
                txtGrade.Text = rowData.Cells["grade"].Value.ToString();
                btnEditStu.Enabled = true;
                btnDeleteStu.Enabled = true;
            }
        }

        private void btnStudentView_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            student.StartPosition = FormStartPosition.Manual;
            student.Show();
        }

        private void btnClassView_Click(object sender, EventArgs e)
        {
            Class _class = new Class();
            _class.StartPosition = FormStartPosition.Manual;
            _class.Show();
        }
    }
}
