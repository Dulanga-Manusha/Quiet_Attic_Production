using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Quiet_Attic
{
    public partial class StaffTypes : Form
    {
        public StaffTypes()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");

        // Load events when the StaffTypes form is loaded
        private void StaffTypes_Load(object sender, EventArgs e)
        {
            LoadStaffTypes();
        }

        // Load data into the DataGridView
        private void LoadStaffTypes()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StaffTypes", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes
            string StaffType = textBox1.Text;
            string fee = textBox2.Text;


            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into StaffTypes table
                string insertQuery = "INSERT INTO StaffTypes (Staff_type,fee) " +
                                     "VALUES (@StaffType, @fee);";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@StaffType", StaffType);
                    cmd.Parameters.AddWithValue("@fee", fee);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the DataGridView with the updated data
                LoadStaffTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Click event for the "Update" button
        private void button2_Click(object sender, EventArgs e)
        {
            // Update the selected row with the new data from text boxes
            if (dataGridView1.SelectedRows.Count > 0)
            {

                string staffType = textBox1.Text;
                string fee = textBox2.Text;

                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in StaffTypes table
                    string updateQuery = "UPDATE StaffTypes SET Staff_type = @staffType, " +
                                         "fee = @fee " +
                                         "WHERE StaffType_ID = @staffTypeID;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@staffType", staffType);
                        cmd.Parameters.AddWithValue("@fee", fee);
                        cmd.Parameters.AddWithValue("@staffTypeID", dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    // Refresh the DataGridView with the updated data
                    LoadStaffTypes();
                    textBox1.Text = "";
                    textBox2.Text = "";

                    MessageBox.Show("successfully updated!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        // Click event for the "Delete" button
        private void button3_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedId = (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value;

                // Perform the deletion
                DeleteStaffType(selectedId);

                // Refresh the DataGridView after deletion
                LoadStaffTypes();
            }
        }

        // Delete StaffType from the database
        private void DeleteStaffType(int staffTypeID)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                // Delete related data from other tables before deleting from StaffTypes table
                string deleteQuery = "DELETE FROM ProductionStaffType WHERE StaffType_ID = @staffTypeID;" +
                                     "DELETE FROM StaffTypes WHERE StaffType_ID = @staffTypeID";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@staffTypeID", staffTypeID);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Item deleted successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Click event for the "Clear" button
        private void button4_Click(object sender, EventArgs e)
        {
            // Clear textboxes
            textBox1.Text = "";
            textBox2.Text = "";
        }

        // Click event for the "Select" button
        private void button6_Click(object sender, EventArgs e)
        {
            // Load data from the selected StaffType into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedId = (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value;

                LoadStaffTypeData(selectedId);
            }
        }

        // Load data of a specific Stafftype into the textboxes
        private void LoadStaffTypeData(int staffTypeId)
        {

            SqlCommand cmd = new SqlCommand("SELECT * FROM StaffTypes WHERE StaffType_ID = @staffTypeId", con);
            cmd.Parameters.AddWithValue("@staffTypeId", staffTypeId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                textBox1.Text = row["Staff_type"].ToString();
                textBox2.Text = row["fee"].ToString();

            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the StaffType form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
