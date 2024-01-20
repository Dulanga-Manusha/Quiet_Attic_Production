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

namespace Quiet_Attic
{
    public partial class ProductionStafftype : Form
    {
        public ProductionStafftype()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");

        // Load event when the ProductionStafftype form is loaded
        private void ProductionStafftype_Load(object sender, EventArgs e)
        {
            // Load data into DataGridView and ComboBoxes on form load
            LoadProductionStafftype();
            LoadProductionIDs();
            LoadStaffTypeIDs();
        }

        // Load production staff type data into the DataGridView
        private void LoadProductionStafftype()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionStaffType", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        // Load production IDs into ComboBox1
        private void LoadProductionIDs()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT production_ID, production_type FROM Productions", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each ProductionID to comboBox1
                        comboBox1.Items.Add($"{reader["production_ID"]} - {reader["production_type"]}");
                    }
                }
            }
            con.Close();
        }

        // Load staff type IDs into ComboBox2
        private void LoadStaffTypeIDs()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT StaffType_ID,Staff_type FROM StaffTypes", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        comboBox2.Items.Add($"{reader["StaffType_ID"]} - {reader["Staff_type"]}");
                    }
                }
            }
            con.Close();
        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Insert production staff type data into the database
            using (SqlCommand cmd = new SqlCommand("INSERT INTO ProductionStaffType (production_ID,StaffType_ID,No_of_staff) " + "VALUES (@production_ID, @staffType_ID, @members)", con))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract StaffTypeID from the selected item in comboBox2
                string selectedStaffTypeItem = comboBox2.SelectedItem.ToString();
                int staffTypeID = ExtractID(selectedStaffTypeItem);

                string members = textBox1.Text;

                // Add ProductionID , StaffTypeID and No_of_staff to the parameters
                cmd.Parameters.AddWithValue("@production_ID", productionID);
                cmd.Parameters.AddWithValue("@staffType_ID", staffTypeID);
                cmd.Parameters.AddWithValue("@members", members);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Data added to the database.");

            // Reset ComboBoxes and TextBox
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            textBox1.Text = "";

            // Refresh the DataGridView with the updated data
            LoadProductionStafftype();

            con.Close();
        }

        // Helper method to extract ID from the concatenated string
        private int ExtractID(string selectedItem)
        {
            
            string IDString = selectedItem.Split('-')[0].Trim();
            int iD;
            if (int.TryParse(IDString, out iD))
            {
                return iD;
            }
            else
            {
                // Handle the case where the extraction fails 
                MessageBox.Show("Error extracting ID.");
                return -1;
            }
        }

        // Click event for the "Update" button
        private void button2_Click(object sender, EventArgs e)
        {
            // Update the selected row with the new data from text boxes
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Extract ProductionID from the selected item in comboBox
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract StaffTypeID from the selected item in comboBox2
                string selectedStaffTypeItem = comboBox2.SelectedItem.ToString();
                int staffTypeID = ExtractID(selectedStaffTypeItem);

                string members = textBox1.Text;


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in ProductionStaffType table
                    string updateQuery = "UPDATE ProductionStaffType SET production_ID = @productionID, " +
                                         "StaffType_ID = @staffTypeID, " +
                                         "No_of_staff = @members " +
                                         "WHERE production_ID = @currentProductionId AND StaffType_ID = @currentStaffTypeId;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@productionID", productionID);
                        cmd.Parameters.AddWithValue("@staffTypeID", staffTypeID);
                        cmd.Parameters.AddWithValue("@members", members);
                        cmd.Parameters.AddWithValue("@currentProductionId", (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value);
                        cmd.Parameters.AddWithValue("@currentStaffTypeId", (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    MessageBox.Show("Successfully Updated");

                    // Refresh the DataGridView with the updated data
                    LoadProductionStafftype();
                    comboBox1.SelectedItem = null;
                    comboBox2.SelectedItem = null;
                    textBox1.Text = "";
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
                int productionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;
                int staffTypeId = (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value;

                // Perform the deletion
                DeleteProductionStaffType(productionId, staffTypeId);

                // Refresh the DataGridView after deletion
                LoadProductionStafftype();
            }
        }

        // Delete production staff type from the database
        private void DeleteProductionStaffType(int productionId, int staffTypeId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string deleteQuery = "DELETE FROM ProductionStaffType WHERE production_ID = @productionId AND StaffType_ID = @staffTypeId";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@staffTypeId", staffTypeId);
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
            // Reset ComboBoxes and TextBox
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            textBox1.Text = "";
        }

        // Click event for the "Select" button
        private void button6_Click(object sender, EventArgs e)
        {
            // Load data from the selected production staff type into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedProductionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;
                int selectedStaffTypeId = (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value;
                int selectedMembers = (int)dataGridView1.SelectedRows[0].Cells["No_of_staff"].Value;

                textBox1.Text = selectedMembers.ToString();

                LoadComboBox1Data(selectedProductionId);
                LoadComboBox2Data(selectedStaffTypeId);
            }
        }

        // Load data into ComboBox1 based on the selected production ID
        private void LoadComboBox1Data(int productionId)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productions WHERE production_ID = @productionId", con);
            cmd.Parameters.AddWithValue("@productionId", productionId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                // Display production_ID + production_type in comboBox1
                comboBox1.Text = $"{row["production_ID"]} - {row["production_type"]}";

            }
        }

        // Load data into ComboBox2 based on the selected staff type ID
        private void LoadComboBox2Data(int StaffTypeId)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StaffTypes WHERE StaffType_ID = @StaffTypeId", con);
            cmd.Parameters.AddWithValue("@StaffTypeId", StaffTypeId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                // Display StaffType_ID + Staff_type in comboBox2
                comboBox2.Text = $"{row["StaffType_ID"]} - {row["Staff_type"]}";
            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the ProductionStafftype form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
