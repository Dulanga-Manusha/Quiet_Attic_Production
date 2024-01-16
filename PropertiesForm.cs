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

        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-LT4EDDL6;Initial Catalog=film_productiondb;Integrated Security=True");

        private void Properties_Load(object sender, EventArgs e)
        {
            LoadProperties();
        }

        private void LoadProperties()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Properties", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string propertyType = textBox1.Text;
            string description = textBox2.Text;


            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into Clients table
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

                    // Update data in Locations table
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

        private void DeleteProperty(int propertyID)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

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

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedPropertyId = (int)dataGridView1.SelectedRows[0].Cells["property_ID"].Value;

                LoadPropertyData(selectedPropertyId);
            }
        }

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

        private void button5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
