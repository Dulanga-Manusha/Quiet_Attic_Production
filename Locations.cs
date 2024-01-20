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
    public partial class Locations : Form
    {
        public Locations()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");


        // Load events when the Locations form is loaded
        private void Locations_Load(object sender, EventArgs e)
        {
            LoadLocations();
        }

        // Load data into the DataGridView
        private void LoadLocations()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Locations", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        // Click event for the "Select" button
        private void button6_Click(object sender, EventArgs e)
        {
            // Load data from the selected loaction into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedLocationId = (int)dataGridView1.SelectedRows[0].Cells["location_ID"].Value;

                LoadLocationData(selectedLocationId);
            }
        }

        // Load data of a specific location into the textboxes
        private void LoadLocationData(int locationId)
        {

            SqlCommand cmd = new SqlCommand("SELECT * FROM Locations WHERE location_ID = @locationId", con);
            cmd.Parameters.AddWithValue("@locationId", locationId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                textBox1.Text = row["name"].ToString();
                textBox2.Text = row["address"].ToString();
                textBox3.Text = row["contact_number"].ToString();
                
            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the Location form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }

        // Click event for the "Clear" button
        private void button4_Click(object sender, EventArgs e)
        {
            // Clear textboxes
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        // Click event for the "Delete" button
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedLocationId = (int)dataGridView1.SelectedRows[0].Cells["location_ID"].Value;

                // Perform the deletion
                DeleteLocation(selectedLocationId);

                // Refresh the DataGridView after deletion
                LoadLocations();
            }
        }

        // Delete Location from the database
        private void DeleteLocation(int locationID)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Delete related data from other tables before deleting from Location table
                string deleteQuery = "DELETE FROM ProductionLocation WHERE location_ID = @locationID;" +
                                     "DELETE FROM Locations WHERE location_ID = @locationID;";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@locationID", locationID);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Location deleted successfully!");
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
                
                string name = textBox1.Text;
                string address = textBox2.Text;
                string contactNumber = textBox3.Text;
                

                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in Locations table
                    string updateQuery = "UPDATE Locations SET name = @name, " +
                                         "contact_number = @contactNumber, " +
                                         "address = @address " +
                                         "WHERE location_ID = @locationID;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@contactNumber", contactNumber);
                        cmd.Parameters.AddWithValue("@locationID", dataGridView1.SelectedRows[0].Cells["location_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    // Refresh the DataGridView with the updated data
                    LoadLocations();
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
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

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes
            string name = textBox1.Text;
            string address = textBox2.Text;
            string contactNumber = textBox3.Text;
            

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into Locations table
                string insertQuery = "INSERT INTO Locations (name,address, contact_number) " +
                                     "VALUES (@name, @address, @contactNumber);";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@contactNumber", contactNumber);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the DataGridView with the updated data
                LoadLocations();
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
    }
}
