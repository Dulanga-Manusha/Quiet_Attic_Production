using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quiet_Attic
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            ProductionLocation productionLocation= new ProductionLocation();
            productionLocation.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ProductionStafftype productionstafftype =new ProductionStafftype();
            productionstafftype.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProductionProperty productionproperty = new ProductionProperty();
            productionproperty.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Production production = new Production();
            production.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Locations location = new Locations();
            location.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PropertiesForm propertiesform = new PropertiesForm();
            propertiesform.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StaffTypes staff = new StaffTypes();
            staff.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clients clients = new Clients();
            clients.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }
    }
}
