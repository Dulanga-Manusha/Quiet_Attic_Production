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
    public partial class ProductionProperty : Form
    {
        public ProductionProperty()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the ProductionProperty form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }

        // Load event when the ProductionProperty form is loaded
        private void ProductionProperty_Load(object sender, EventArgs e)
        {
            // Load data into DataGridView and ComboBoxes on form load
            LoadProductionProperty();
            LoadProductionIDs();
            LoadPropertyIDs();
        }

        // Load production property data into the DataGridView
        private void LoadProductionProperty()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProductionProperty", con);
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

        // Load property IDs into ComboBox2
        private void LoadPropertyIDs()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT property_ID,property_type FROM Properties", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each ProductionID to comboBox1
                        //comboBox2.Items.Add(reader["property_ID"].ToString());
                        comboBox2.Items.Add($"{reader["property_ID"]} - {reader["property_type"]}");
                    }
                }
            }
            con.Close();
        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Insert production property data into the database
            using (SqlCommand cmd = new SqlCommand("INSERT INTO ProductionProperty (production_ID,property_ID) " + "VALUES (@production_ID, @property_ID)", con))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Extract ProductionID from the selected item in comboBox1
                string selectedProductionItem = comboBox1.SelectedItem.ToString();
                int productionID = ExtractID(selectedProductionItem);

                // Extract propertyID from the selected item in comboBox1
                string selectedPropertyItem = comboBox2.SelectedItem.ToString();
                int propertyID = ExtractID(selectedPropertyItem);

                // Add ProductionID and PropertyID to the parameters
                cmd.Parameters.AddWithValue("@production_ID", productionID);
                cmd.Parameters.AddWithValue("@property_ID", propertyID);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Data added to the database.");

            // Reset ComboBoxes
            comboBox1.SelectedItem = null; 
            comboBox2.SelectedItem = null;

            // Refresh the DataGridView with the updated data
            LoadProductionProperty();

            con.Close();
        }

        // Helper method to extract ProductionID from the concatenated string
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
                MessageBox.Show("Error extracting ProductionID.");
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

                // Extract PropertyID from the selected item in comboBox1
                string selectedPropertyItem = comboBox2.SelectedItem.ToString();
                int propertyID = ExtractID(selectedPropertyItem);


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in ProductionProperty table
                    string updateQuery = "UPDATE ProductionProperty SET production_ID = @productionID, " +
                                         "property_ID = @propertyID " +
                                         "WHERE production_ID = @currentProductionId AND property_ID = @currentPropertyId;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@productionID", productionID);
                        cmd.Parameters.AddWithValue("@propertyID", propertyID);
                        cmd.Parameters.AddWithValue("@currentProductionId", (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value);
                        cmd.Parameters.AddWithValue("@currentPropertyId", (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    MessageBox.Show("Successfully Updated");

                    // Refresh the DataGridView with the updated data
                    LoadProductionProperty();
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
                int propertyId = (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value;

                // Perform the deletion
                DeleteProductionProperty(productionId, propertyId);

                // Refresh the DataGridView after deletion
                LoadProductionProperty();
            }
        }

        // Delete production property from the database
        private void DeleteProductionProperty(int productionId, int propertyId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string deleteQuery = "DELETE FROM ProductionProperty WHERE production_ID = @productionId AND property_ID = @propertyId";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@propertyId", propertyId);
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
            // Load data from the selected production property into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedProductionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;
                int selectedPropertyId = (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value;

                LoadComboBox1Data(selectedProductionId);
                LoadComboBox2Data(selectedPropertyId);
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

        // Load data into ComboBox2 based on the selected property ID
        private void LoadComboBox2Data(int propertyId)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Properties WHERE property_ID = @propertyId", con);
            cmd.Parameters.AddWithValue("@propertyId", propertyId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                // Display property_ID + property_type in comboBox2
                comboBox2.Text = $"{row["property_ID"]} - {row["property_type"]}";
            }
        }

        
    }
}
