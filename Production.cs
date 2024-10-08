﻿using System;
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
    public partial class Production : Form
    {
        public Production()
        {
            InitializeComponent();
        }
        // Establishing a connection to the SQL Server database
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-PD67JB8B\SQLEXPRESS;Initial Catalog=productionDB;Integrated Security=True;Encrypt=False");


        // Load events when the Production form is loaded
        private void Production_Load(object sender, EventArgs e)
        {
            // Load data into DataGridView and ComboBox on form load
            LoadProductions();
            LoadClientsIDs();
        }

        // Load production data into the DataGridView
        private void LoadProductions()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productions", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            dataGridView1.DataSource = table;

        }

        // Load client IDs into the ComboBox
        private void LoadClientsIDs()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT client_ID FROM Clients", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each Client ID to the ComboBox
                        comboBox1.Items.Add(reader["client_ID"].ToString());
                       
                    }
                }
            }
            con.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Click event for the "Add" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes and ComboBox
            string productionType = textBox1.Text;
            string clientId = comboBox1.SelectedItem.ToString();
            string days = textBox2.Text;


            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Insert data into Productions table
                string insertQuery = "INSERT INTO Productions (client_ID,production_type, no_of_days) " +
                                     "VALUES (@clientId,@productionType, @days);";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@productionType", productionType);
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.Parameters.AddWithValue("@days", days);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the DataGridView with the updated data
                LoadProductions();
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

                string productionType = textBox1.Text;
                string clientId = comboBox1.SelectedItem.ToString();
                string days = textBox2.Text;


                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update data in Productions table
                    string updateQuery = "UPDATE Productions SET client_ID = @clientId, " +
                                         "production_type = @productionType, " +
                                         "no_of_days = @days " +
                                         "WHERE production_ID = @productionID;";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        cmd.Parameters.AddWithValue("@productionType", productionType);
                        cmd.Parameters.AddWithValue("@days", days);
                        cmd.Parameters.AddWithValue("@productionID", dataGridView1.SelectedRows[0].Cells["production_ID"].Value);

                        cmd.ExecuteNonQuery();

                    }

                    MessageBox.Show("Successfully Updated");

                    // Refresh the DataGridView with the updated data
                    LoadProductions();
                    textBox1.Text = "";
                    textBox2.Text = "";
                    comboBox1.SelectedItem = null;
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
            // Delete the selected production
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int productionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;

                // Perform the deletion
                DeleteProduction(productionId);

                // Refresh the DataGridView after deletion
                LoadProductions();
            }
        }

        // Delete production from the database
        private void DeleteProduction(int productionId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Delete related data from other tables before deleting from Productions table
                string deleteQuery = "DELETE FROM ProductionLocation WHERE production_ID = @productionId;" +
                                     "DELETE FROM ProductionProperty WHERE production_ID = @productionId;" +
                                     "DELETE FROM ProductionStaffType WHERE production_ID = @productionId;" +
                                     "DELETE FROM Productions WHERE production_ID = @productionId";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Production deleted successfully!");
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
        private void button4_Click_1(object sender, EventArgs e)
        {
            // Clear textboxes and reset ComboBox selection
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedItem = null;
        }

        // Click event for the "Select" button
        private void button6_Click(object sender, EventArgs e)
        {
            // Load data from the selected production into textboxes
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedProductionId = (int)dataGridView1.SelectedRows[0].Cells["production_ID"].Value;

                LoadProductionData(selectedProductionId);
            }
        }

        // Load data of a specific production into the textboxes
        private void LoadProductionData(int productionId)
        {

            SqlCommand cmd = new SqlCommand("SELECT * FROM Productions WHERE production_ID = @productionId", con);
            cmd.Parameters.AddWithValue("@productionId", productionId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            da.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                textBox1.Text = row["production_type"].ToString();
                comboBox1.Text = row["client_ID"].ToString();
                textBox2.Text = row["no_of_days"].ToString();
            }
        }

        // Click event for the "Back" button
        private void button5_Click(object sender, EventArgs e)
        {
            // Show the Dashboard form and hide the Production form
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
