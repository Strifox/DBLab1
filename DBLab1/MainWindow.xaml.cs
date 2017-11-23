using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBLab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection sqlUpdate = new SqlConnection();

        private string dbConnectionString =
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB Lab1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            ;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowDataBase()
        {
            SqlConnection con =new SqlConnection(dbConnectionString);

            try
            {
                con.Open();
                string query = "SELECT * FROM Author";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string row = "";

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row += reader[i] + " ";
                    }
                    listboxAuthor.Items.Add(row);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        private void txtBoxAuthorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(txtBoxAuthorName.Text))
            //    MessageBox.Show("Insert a name");
            //else
            //{
            //    btnAdd.IsEnabled = true;

            //}
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ShowDataBase();
        }
    }
}
