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
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-LT4EDDL6;Initial Catalog=film_productiondb;Integrated Security=True");


        private void StaffTypes_Load(object sender, EventArgs e)
        {
            LoadStaffTypes();
        }

        private void LoadStaffTypes()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StaffTypes", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string StaffType = textBox1.Text;
            string fee = textBox2.Text;


            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into Clients table
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

                    // Update data in Locations table
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

        private void DeleteStaffType(int staffTypeID)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

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

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedId = (int)dataGridView1.SelectedRows[0].Cells["StaffType_ID"].Value;

                LoadStaffTypeData(selectedId);
            }
        }

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

        private void button5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
