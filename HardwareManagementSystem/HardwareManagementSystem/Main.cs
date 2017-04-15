using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HardwareManagementSystem
{
    public partial class frmMain : Form
    {
        public SqlConnection sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Jerad\documents\visual studio 2015\Projects\HardwareManagementSystem\HardwareManagementSystem\StockDB.mdf';Integrated Security=True;");

        public frmMain()
        {
            InitializeComponent();

            SqlCommand sqlQuery = new SqlCommand("SELECT * FROM tblStores", sqlConnection);
            populate(sqlQuery);
        }

        private void frmMain_Closed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void populate(SqlCommand sqlQuery)
        {
            try
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlQuery);
                DataTable dataTable = new DataTable();

                sqlAdapter.Fill(dataTable);

                listView.Items.Clear();

                foreach (DataRow dataReader in dataTable.Rows)
                {
                    ListViewItem item = new ListViewItem(dataReader["ID"].ToString());
                    item.SubItems.Add(dataReader["ITEM"].ToString());
                    item.SubItems.Add(dataReader["BRAND"].ToString());
                    item.SubItems.Add(dataReader["PRICE"].ToString());
                    item.SubItems.Add(dataReader["QTY"].ToString());

                    listView.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlCommand sqlQuery;
            if (txtSearch.Text == "")
            {
                sqlQuery = new SqlCommand("SELECT * FROM tblStores", sqlConnection);
            }
            else
            {
                sqlQuery = new SqlCommand("SELECT * FROM tblStores WHERE [ITEM] LIKE '" + txtSearch.Text + "%'", sqlConnection);
            }

            populate(sqlQuery);
        }

        private void resetFeilds()
        {
            txtItem.Text = "";
            txtBrand.Text = "";
            txtPrice.Text = "";
            txtQty.Text = "";        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtItem.Text.Trim()))
            {
                MessageBox.Show("Please fill the Item Feild to continue");
            }
            else if (String.IsNullOrEmpty(txtPrice.Text.Trim()))
            {
                MessageBox.Show("Please fill the Price Feild to continue");
            }
            else if (String.IsNullOrEmpty(txtQty.Text.Trim()))
            {
                MessageBox.Show("Please fill the Quantity (Qty) Feild to continue");
            }
            else
            {
                try { 
                    float price = float.Parse(txtPrice.Text);
                    int qty = int.Parse(txtQty.Text);

                    sqlConnection.Open();
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter("INSERT INTO [dbo].[tblStores] " +
                        "([ITEM], [BRAND], [PRICE], [QTY]) VALUES ('" +
                            txtItem.Text + "', '" +
                            txtBrand.Text + "', '" +
                            price + "', '" +
                            qty + "')", sqlConnection);
                    sqlAdapter.SelectCommand.ExecuteNonQuery();

                    sqlAdapter.SelectCommand.CommandText = "SELECT @@Identity";
                    string addedRecordID = sqlAdapter.SelectCommand.ExecuteScalar().ToString();

                    sqlConnection.Close();

                    ListViewItem item = new ListViewItem(addedRecordID);
                    item.SubItems.Add(txtItem.Text);
                    item.SubItems.Add(txtBrand.Text);
                    item.SubItems.Add(txtPrice.Text);
                    item.SubItems.Add(txtQty.Text);

                    listView.Items.Add(item);

                    resetFeilds();

                    MessageBox.Show("Added Record!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                string id = listView.SelectedItems[0].SubItems[0].Text;

                SqlDataAdapter sqlAdapter = new SqlDataAdapter("SELECT * FROM [dbo].[tblStores] WHERE [ID]=" + id, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlAdapter.Fill(dataTable);

                txtItem.Text = dataTable.Rows[0]["ITEM"].ToString();
                txtBrand.Text = dataTable.Rows[0]["BRAND"].ToString();
                txtPrice.Text = dataTable.Rows[0]["PRICE"].ToString();
                txtQty.Text = dataTable.Rows[0]["QTY"].ToString();

                btnAdd.Enabled = false;
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
                btnUpdate.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please select a record");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnChange.Enabled = true;
            btnDelete.Enabled = true;
            btnUpdate.Enabled = false;
            btnCancel.Enabled = false;

            resetFeilds();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtItem.Text.Trim()))
            {
                MessageBox.Show("Item feild cannot be empty");
            }
            else if (String.IsNullOrEmpty(txtPrice.Text.Trim()))
            {
                MessageBox.Show("Price feild cannot be empty");
            }
            else if (String.IsNullOrEmpty(txtQty.Text.Trim()))
            {
                MessageBox.Show("Quantity (Qty) feild cannot be empty");
            }
            else
            {
                try
                {
                    float price = float.Parse(txtPrice.Text);
                    int qty = int.Parse(txtQty.Text);

                    string id = listView.SelectedItems[0].SubItems[0].Text;

                    sqlConnection.Open();
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter("UPDATE [dbo].[tblStores] SET " +
                        "[ITEM]='" + txtItem.Text + "'," +
                        "[BRAND]='" + txtBrand.Text + "'," +
                        "[PRICE]='" + price + "'," +
                        "[QTY]='" + qty + "' " +
                        "WHERE [ID]=" + id, sqlConnection);
                    sqlAdapter.SelectCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    listView.SelectedItems[0].SubItems[1].Text = txtItem.Text;
                    listView.SelectedItems[0].SubItems[2].Text = txtBrand.Text;
                    listView.SelectedItems[0].SubItems[3].Text = txtPrice.Text;
                    listView.SelectedItems[0].SubItems[4].Text = txtQty.Text;

                    btnAdd.Enabled = true;
                    btnChange.Enabled = true;
                    btnDelete.Enabled = true;
                    btnUpdate.Enabled = false;
                    btnCancel.Enabled = false;

                    resetFeilds();

                    MessageBox.Show("Record Updated!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Delete", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string id = listView.SelectedItems[0].SubItems[0].Text;

                    sqlConnection.Open();
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter("DELETE FROM [dbo].[tblStores] WHERE [ID]=" + id, sqlConnection);
                    sqlAdapter.SelectCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    listView.SelectedItems[0].Remove();

                    MessageBox.Show("Record Deleted!");
                }
            }
            else
            {
                MessageBox.Show("Please select a record to delete");
            }
        }
    }
}
