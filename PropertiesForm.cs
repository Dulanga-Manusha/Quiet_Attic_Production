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
    public partial class PropertiesForm : Form
    {
        public PropertiesForm()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");

        // Load events when the Properties form is loaded
        private void Properties_Load(object sender, EventArgs e)
        {
            LoadProperties();
        }

        // Load data into the DataGridView
        private void LoadProperties()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Properties", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes
            string propertyType = textBox1.Text;
            string description = textBox2.Text;


            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into Properties table
                string insertQuery = "INSERT INTO Properties (property_type,description) " +
                                     "VALUES (@propertyType, @description);";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@propertyType", propertyType);
                    cmd.Parameters.AddWithValue("@description", description);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the DataGridView with the updated data
                LoadProperties();
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

                string propertyType = textBox1.Text;
                string description = textBox2.Text;


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in Properties table
                    string updateQuery = "UPDATE Properties SET property_type = @propertyType, " +
                                         "description = @description " +
                                         "WHERE property_ID = @propertyID;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@propertyType", propertyType);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@propertyID", dataGridView1.SelectedRows[0].Cells["property_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    // Refresh the DataGridView with the updated data
                    LoadProperties();
                    textBox1.Text = "";
                    textBox2.Text = "";

                    MessageBox.Show("Property updated successfully!");
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
                int selectedPropertyId = (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value;

                // Perform the deletion
                DeleteProperty(selectedPropertyId);

                // Refresh the DataGridView after deletion
                LoadProperties();
            }
        }

        // Delete Property from the database
        private void DeleteProperty(int propertyID)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                // Delete related data from other tables before deleting from Properties table
                string deleteQuery = "DELETE FROM ProductionProperty WHERE property_ID = @propertyID;" +
                                     "DELETE FROM Properties WHERE property_ID = @propertyID";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@propertyID", propertyID);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Property deleted successfully!");
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
            // Load data from the selected property into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedPropertyId = (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value;

                LoadPropertyData(selectedPropertyId);
            }
        }

        // Load data of a specific property into the textboxes
        private void LoadPropertyData(int propertyId)
        {

            SqlCommand cmd = new SqlCommand("SELECT * FROM Properties WHERE property_ID = @propertyId", con);
            cmd.Parameters.AddWithValue("@propertyId", propertyId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                textBox1.Text = row["property_type"].ToString();
                textBox2.Text = row["description"].ToString();

            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the Properties form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
