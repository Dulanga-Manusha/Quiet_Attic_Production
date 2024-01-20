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
    public partial class ProductionLocation : Form
    {
        public ProductionLocation()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");

        // Load event when the ProductionLocation form is loaded
        private void ProductionLocation_Load(object sender, EventArgs e)
        {
            // Load data into DataGridView and ComboBoxes on form load
            LoadProductionLocation();
            LoadProductionIDs();
            LoadLocationIDs();
        }
        // Load production location data into the DataGridView
        private void LoadProductionLocation()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionLocation", con);
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

        // Load location IDs into ComboBox2
        private void LoadLocationIDs()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT location_ID,name FROM Locations", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        comboBox2.Items.Add($"{reader["location_ID"]} - {reader["name"]}");
                    }
                }
            }
            con.Close();
        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Insert production location data into the database
            using (SqlCommand cmd = new SqlCommand("INSERT INTO ProductionLocation (production_ID,location_ID) " + "VALUES (@production_ID, @location_ID)", con))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract locationID from the selected item in comboBox1
                string selectedLocationItem = comboBox2.SelectedItem.ToString();
                int locationID = ExtractID(selectedLocationItem);

                // Add ProductionID and locationID to the parameters
                cmd.Parameters.AddWithValue("@production_ID", productionID);
                cmd.Parameters.AddWithValue("@location_ID", locationID);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Data added to the database.");

            // Reset ComboBoxes
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;

            // Refresh the DataGridView with the updated data
            LoadProductionLocation();

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
                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract LocationID from the selected item in comboBox2
                string selectedLocationItem = comboBox2.SelectedItem.ToString();
                int locationID = ExtractID(selectedLocationItem);


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in ProductionLocations table
                    string updateQuery = "UPDATE ProductionLocation SET production_ID = @productionID, " +
                                         "location_ID = @locationID " +
                                         "WHERE production_ID = @currentProductionId AND location_ID = @currentLocationId;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@productionID", productionID);
                        cmd.Parameters.AddWithValue("@locationID", locationID);
                        cmd.Parameters.AddWithValue("@currentProductionId", (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value);
                        cmd.Parameters.AddWithValue("@currentLocationId", (int)dataGridView1.SelectedRows[0].Cells["location_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    MessageBox.Show("Successfully Updated");

                    // Refresh the DataGridView with the updated data
                    LoadProductionLocation();
                    comboBox1.SelectedItem = null;
                    comboBox2.SelectedItem = null;
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
                int locationId = (int)dataGridView1.SelectedRows[0].Cells["location_ID"].Value;

                // Perform the deletion
                DeleteProductionLocation(productionId, locationId);

                // Refresh the DataGridView after deletion
                LoadProductionLocation();
            }
        }

        // Delete production Location from the database
        private void DeleteProductionLocation(int productionId, int locationId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string deleteQuery = "DELETE FROM ProductionLocation WHERE production_ID = @productionId AND location_ID = @locationId";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@locationId", locationId);
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
            // Reset ComboBoxes
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
        }

        // Click event for the "Select" button
        private void button6_Click(object sender, EventArgs e)
        {
            // Load data from the selected production location into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedProductionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;
                int selectedLocationId = (int)dataGridView1.SelectedRows[0].Cells["location_ID"].Value;

                LoadComboBox1Data(selectedProductionId);
                LoadComboBox2Data(selectedLocationId);
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

        // Load data into ComboBox2 based on the location ID
        private void LoadComboBox2Data(int locationId)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Locations WHERE location_ID = @locationId", con);
            cmd.Parameters.AddWithValue("@locationId", locationId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                // Display location_ID + name in comboBox2
                comboBox2.Text = $"{row["location_ID"]} - {row["name"]}";
            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the ProductionLocation form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
