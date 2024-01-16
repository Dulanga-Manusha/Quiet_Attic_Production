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
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-LT4EDDL6;Initial Catalog=film_productiondb;Integrated Security=True");

        private void ProductionStafftype_Load(object sender, EventArgs e)
        {
            LoadProductionStafftype();
            LoadProductionIDs();
            LoadStaffTypeIDs();
        }

        private void LoadProductionStafftype()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionStaffType", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            // Insert salary data into the database
            using (SqlCommand cmd = new SqlCommand("INSERT INTO ProductionStaffType (production_ID,StaffType_ID,No_of_staff) " + "VALUES (@production_ID, @staffType_ID, @members)", con))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract ProductionID from the selected item in comboBox1
                string selectedStaffTypeItem = comboBox2.SelectedItem.ToString();
                int staffTypeID = ExtractID(selectedStaffTypeItem);

                string members = textBox1.Text;

                // Add ProductionID and PropertyID to the parameters
                cmd.Parameters.AddWithValue("@production_ID", productionID);
                cmd.Parameters.AddWithValue("@staffType_ID", staffTypeID);
                cmd.Parameters.AddWithValue("@members", members);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Data added to the database.");

            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            textBox1.Text = "";

            LoadProductionStafftype();

            con.Close();
        }

        // Helper method to extract ID from the concatenated string
        private int ExtractID(string selectedItem)
        {
            // Assuming the ProductionID is at the beginning of the string and is an integer
            string IDString = selectedItem.Split('-')[0].Trim();
            int iD;
            if (int.TryParse(IDString, out iD))
            {
                return iD;
            }
            else
            {
                // Handle the case where the extraction fails (e.g., show an error message)
                MessageBox.Show("Error extracting ID.");
                return -1; // or throw an exception or handle accordingly
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Update the selected row with the new data from text boxes
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract ProductionID from the selected item in comboBox1
                string selectedStaffTypeItem = comboBox2.SelectedItem.ToString();
                int staffTypeID = ExtractID(selectedStaffTypeItem);

                string members = textBox1.Text;


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in Locations table
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

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
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

                // Display property_ID + property_type in comboBox2
                comboBox2.Text = $"{row["StaffType_ID"]} - {row["Staff_type"]}";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
