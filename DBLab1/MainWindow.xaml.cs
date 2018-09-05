using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using SqlCommand = System.Data.SqlClient.SqlCommand;

namespace DBLab1
{
    //TODO: Edit btns should not change to blank. Add regex
    //Todo: edit should not change to ?!?!. Add Regex
    //Todo: Add btn should not add ?"!?!?"#. add regex

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static string dbConnectionString =
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB Lab1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            ;

        public MainWindow()
        {
            InitializeComponent();
            Height += 10;
            Width += 10;
        }

        #region Buttons
        //Set Author buttons to true or false
        private void txtBoxAuthorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AuthorNameList(listboxAuthor).Contains(txtBoxAuthorName.Text))
                SetButtonFalse(btnAddAuthor);
            else
                SetButtonTrue(btnAddAuthor);

        }
        //Sets Book buttons to true or false
        private void txtBoxBookTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoxBookTitle.Text))
                SetButtonFalse(btnAddBook);
            else
                SetButtonTrue(btnAddBook);
        }

        private void SetButtonTrue(Button btn)
        {
            btn.IsEnabled = true;
        }
        private void SetButtonFalse(Button btn)
        {
            btn.IsEnabled = false;
        }

     
        #endregion
        #region Author
        #region Author Buttons
        private void btnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddAuthor();
        }
        private void btnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            DeleteAuthor();
        }
        #endregion
        #region Author ListBox
        private void UpdateAuthorListBox()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlDataReader reader;
            SqlCommand cmd;
            string query = "SELECT ID , Name, Age, Birthdate, Gender FROM Author";
            Dispatcher.Invoke(listboxAuthor.Items.Clear);
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            listboxAuthor.Items.Add(new Author(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                                reader.GetDateTime(3), reader.GetString(4)));
                        });
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
            });
        }
        private void listboxAuthor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAuthorListBox();
        }
        private void listboxAuthor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBookListBox();
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlDataReader reader;
            SqlCommand cmd;
            string query = "Select Id, Name, Age, Birthdate, Gender From Author";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (listboxAuthor.SelectedItem == null)
                            {
                                ClearAuthorTextBoxes(); //Clear textboxes
                                                        //Set buttons
                                SetButtonTrue(btnAddAuthor);
                                SetButtonFalse(btnEditBook); //btn Edit
                                SetButtonFalse(btnDeleteBook); // btn Delete
                            }
                            //Updates Author textboxes
                            if (listboxAuthor.SelectedItem != null)
                            {

                                txtBoxAuthorID.Text =
                                    ((Author)listboxAuthor.SelectedItem).Id.ToString(); //Author ID Textbox
                                txtBoxAuthorName.Text =
                                    ((Author)listboxAuthor.SelectedItem).Name; //Author Name Textbox
                                txtBoxAuthorAge.Text =
                                    ((Author)listboxAuthor.SelectedItem).Age.ToString(); // Author Age Textbox
                                txtBoxAuthorBirthDate.Text =
                                    ((Author)listboxAuthor.SelectedItem).BirthDate
                                    .ToString("yyyy-MM-dd"); // Author Birthdate Textbox
                                txtBoxAuthorGender.Text =
                                    ((Author)listboxAuthor.SelectedItem).Gender; // Author Gender Textbox

                                SetButtonTrue(btnEditAuthor);
                                SetButtonTrue(btnDeleteAuthor);
                                SetButtonFalse(btnAddAuthor);
                                //Set buttons
                            }


                        });

                    }
                }
                catch (Exception ed)
                {
                    MessageBox.Show(ed.Message);
                }
                finally
                {
                    con.Close();
                }
            });
        }
        #endregion
        private void DeleteAuthor()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;

            //Removes selected index and then sets the selected index to the next index


            string query = $"Delete FROM Author WHERE Id = {((Author)listboxAuthor.SelectedItem).Id}";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    con.Close();
                    UpdateAuthorListBox();
                    ClearAuthorTextBoxes();
                }
            });
        }
        private void AddAuthor()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;
            #region RegEx
            string patternName = @"^[a-z]+\s?[a-z]*$"; //RegEx Pattern Name
            string patternAge = @"[0-9]{1}[0-9]{0,2}"; //RegEx Pattern Age
            string patternGender = @"^M(ale)?$|^F(emale)?$"; //RegEx Pattern Gender
            Match matchName = Regex.Match(txtBoxAuthorName.Text.Trim(), patternName, RegexOptions.IgnoreCase);
            Match matchAge = Regex.Match(txtBoxAuthorAge.Text, patternAge, RegexOptions.IgnoreCase);
            Match matchGender = Regex.Match(txtBoxAuthorGender.Text, patternGender, RegexOptions.IgnoreCase);

            if (!matchName.Success)
            {
                MessageBox.Show("Invalid Name");
                return;
            }
            if (txtBoxAuthorBirthDate.Text == String.Empty || txtBoxAuthorGender.Text == String.Empty ||
                txtBoxAuthorAge.Text == string.Empty)
            {
                MessageBox.Show("Fill all textboxes");
                return;
            }
            if (!IsBirthdateValid(txtBoxAuthorBirthDate.Text))
            {
                MessageBox.Show("Invalid birthdate, format to yyyy-mm-dd");
                return;
            }
            if (!matchAge.Success)
            {
                MessageBox.Show("Invalid age, type a number");
                return;
            }
            if (!matchGender.Success)
            {
                MessageBox.Show("Invalid Gender, type Male or Female");
                return;
            }
            #endregion

            string name = txtBoxAuthorName.Text;
            string gender = txtBoxAuthorGender.Text;
            string query =
                $"INSERT INTO Author (Name, Age, Birthdate, Gender) VALUES ('{name.Trim()}', '{txtBoxAuthorAge.Text}', '{txtBoxAuthorBirthDate.Text}', '{gender.Trim()}')";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                    UpdateAuthorListBox();
                    ClearAuthorTextBoxes();
                }
            });
        }
        private void btnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            #region RegEx
            string patternName = @"^[a-z]+\s?[a-z]*$"; //RegEx Pattern Name
            string patternAge = @"[0-9]{1}[0-9]{0,2}"; //RegEx Pattern Age
            string patternGender = @"^M(ale)?$|^F(emale)?$"; //RegEx Pattern Gender
            Match matchName = Regex.Match(txtBoxAuthorName.Text.Trim(), patternName, RegexOptions.IgnoreCase);
            Match matchAge = Regex.Match(txtBoxAuthorAge.Text, patternAge, RegexOptions.IgnoreCase);
            Match matchGender = Regex.Match(txtBoxAuthorGender.Text, patternGender, RegexOptions.IgnoreCase);
            if (txtBoxAuthorBirthDate.Text == String.Empty || txtBoxAuthorGender.Text == String.Empty ||
                txtBoxAuthorAge.Text == string.Empty)
            {
                MessageBox.Show("Fill all textboxes");
                return;
            }
            if (!matchName.Success)
            {
                MessageBox.Show("Invalid Name");
                return;
            }
            if (!IsBirthdateValid(txtBoxAuthorBirthDate.Text))
            {
                MessageBox.Show("Invalid birthdate, format to yyyy-mm-dd");
                return;
            }
            if (!matchAge.Success)
            {
                MessageBox.Show("Invalid age, type a number");
                return;
            }
            if (!matchGender.Success)
            {
                MessageBox.Show("Invalid Gender, type Male or Female");
                return;
            }
            #endregion

            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;
            string query =
                $"Update Author Set [Name] = '{txtBoxAuthorName.Text}', [Age] = '{Int32.Parse(txtBoxAuthorAge.Text)}', [Birthdate] = '{DateTime.Parse(txtBoxAuthorBirthDate.Text)}', [Gender] = '{txtBoxAuthorGender.Text}' WHERE ID = '{((Author)listboxAuthor.SelectedItem).Id}'";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlException)
                {
                    MessageBox.Show(sqlException.Message);
                }
                finally
                {
                    con.Close();
                    UpdateAuthorListBox();
                }
            });
        }
        #endregion
        #region Book
        #region Book Buttons
        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            AddBook();
        }
        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            DeleteBook();
        }
        #endregion
        #region Book ListBox
        private void UpdateBookListBox()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlDataReader reader;
            SqlCommand cmd;
            bool isNull = false;
            Dispatcher.Invoke(() =>
            {
                if (listboxAuthor.SelectedItem == null) isNull = true; //Return null from dispatcher to skip another thread bug
            });
            if (isNull) return;
            string query = ""; // Sets the string to empty instead of being default
            Dispatcher.Invoke(() =>
            {
                query =
                    $"SELECT ID, [Author ID], Title, Genre, [Author Name] FROM Book Where [Author ID] = {((Author)listboxAuthor.SelectedItem).Id}"; // Shows selected Author's books
                listBoxBook.Items.Clear(); // Listbox clear
            });
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            listBoxBook.Items.Add(new Book(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2),
                                reader.GetString(3), reader.GetString(4)));
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    ClearAuthorTextBoxes();
                    con.Close();
                }
            });
        }
        private void listBoxBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlDataReader reader;
            SqlCommand cmd;
            string query = "Select Id, [Author Id], Title, Genre, [Author Name] From Book";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (listBoxBook.SelectedItem == null)
                            {

                                ClearBookTextBoxes();
                                SetButtonFalse(btnEditBook);
                                SetButtonFalse(btnDeleteBook);
                                SetButtonFalse(btnAddBook);

                            }
                            if (listBoxBook.SelectedItem != null)
                            {
                                txtBoxBookID.Text = ((Book)listBoxBook.SelectedItem).Id.ToString();
                                txtBoxBookTitle.Text = ((Book)listBoxBook.SelectedItem).Title;
                                txtBoxBookGenre.Text = ((Book)listBoxBook.SelectedItem).Genre;
                                txtBoxBookAuthor.Text = ((Author)listboxAuthor.SelectedItem).Name;
                                txtBoxBookAuthorID.Text = ((Book)listBoxBook.SelectedItem).AuthorId.ToString();
                                SetButtonTrue(btnEditBook);
                                SetButtonTrue(btnDeleteBook);
                                SetButtonFalse(btnAddBook);

                            }

                        });
                    }
                }
                catch (Exception ed)
                {
                    MessageBox.Show(ed.Message);
                }
                finally
                {
                    con.Close();
                }
            });
        }
        #endregion
        private void DeleteBook()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;
            string query = $"Delete FROM Book WHERE Id = {((Book)listBoxBook.SelectedItem).Id}";

            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    con.Close();
                    UpdateBookListBox();
                    ClearAuthorTextBoxes();
                }
            });
        }
        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtBoxBookTitle.Text))
            {
                SetButtonFalse(btnAddBook);
                MessageBox.Show("Invalid Title");
                return;
            }

            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;
            string query =
                $"Update Book Set [Author ID] = '{((Author)listboxAuthor.SelectedItem).Id}', [Title] = '{txtBoxBookTitle.Text}', [Genre] = '{txtBoxBookGenre.Text}', [Author Name] = '{txtBoxBookAuthor.Text}' WHERE ID = '{((Book)listBoxBook.SelectedItem).Id}'";
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    con.Close();
                    UpdateBookListBox();
                }
            });
        }
        private void AddBook()
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            SqlCommand cmd;
            if (string.IsNullOrWhiteSpace(txtBoxBookTitle.Text))
                MessageBox.Show("Must add Book Title");
            string title = txtBoxBookTitle.Text; //Sets variable to the textboxs text
            string genre = txtBoxBookGenre.Text; //Sets variable to the textboxs text
            string authorName = txtBoxBookAuthor.Text; //Sets variable to the textboxs text
            int authorId = ((Author)listboxAuthor.SelectedItem).Id; //Sets variable to the textboxs text
            string query = $"INSERT INTO Book ([Author ID], Title, Genre, [Author Name]) VALUES ('{authorId}', '{title.Trim()}', '{genre.Trim()}', '{authorName}')"; //Query to add books to Database Book
            cmd = new SqlCommand(query, con);
            Task.Run(() =>
            {
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    con.Close();
                    UpdateBookListBox();
                    ClearBookTextBoxes();
                }
            });

        }
        #endregion
        private void ClearAuthorTextBoxes()
        {
            Dispatcher.Invoke(() =>
            {
                txtBoxAuthorID.Clear();
                txtBoxAuthorName.Clear();
                txtBoxAuthorAge.Clear();
                txtBoxAuthorBirthDate.Clear();
                txtBoxAuthorGender.Clear();
            });
        }
        private void ClearBookTextBoxes()
        {
            Dispatcher.Invoke(() =>
            {
                txtBoxBookGenre.Clear();
                txtBoxBookAuthor.Clear();
                txtBoxBookAuthorID.Clear();
                txtBoxBookID.Clear();
                txtBoxBookTitle.Clear();
            });
        }
        //Returns a user name
        private List<string> AuthorNameList(ListBox lb)
        {
            List<string> objectName = new List<string>();
            for (int i = 0; i < lb.Items.Count; i++)
            {
                objectName.Add(((Author)lb.Items.GetItemAt(i)).Name);
            }
            return objectName;
        }
        //Try to parse birthdate string to datetime
        private bool IsBirthdateValid(string birthday)
        {

            DateTime dt;

            bool BirthdayIsDate = DateTime.TryParseExact(birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

            if (BirthdayIsDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
