using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using System.Data.SqlClient;
using static Dapper.SqlMapper;
using System.Windows.Input;
using Svg;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace WOCManager.Model
{
    class Data
    {
        static string connectionString = @"Data Source=SQL5110.site4now.net;Initial Catalog=db_a9a0f7_diplomawork;User Id=db_a9a0f7_diplomawork_admin;Password=uchiha322";
        //static string connectionString = @"Data Source = DESKTOP-HHO6PH0; Initial Catalog = WordsDB; Trusted_Connection=True; Encrypt = False";

        public static  ObservableCollection<Level> GetLevels()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sqlCommand = @"SELECT * FROM Levels";
                    return new ObservableCollection<Level>(db.Query<Level>(sqlCommand));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке уровней", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Level>();
            }
        }

        public static async Task<ObservableCollection<Level>> GetLevelsAsync()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sqlCommand = @"SELECT * FROM Levels";
                    var levels = await db.QueryAsync<Level>(sqlCommand);
                    return new ObservableCollection<Level>(levels);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке уровней", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Level>();
            }
        }

        public static ObservableCollection<Category> GetCategories()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sqlCommand = @"SELECT * FROM Categories";
                    return new ObservableCollection<Category>(db.Query<Category>(sqlCommand));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке категорий", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Category>();
            }
        }

        public static async Task<ObservableCollection<Category>> GetCategoriesAsync()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sqlCommand = @"SELECT * FROM Categories";
                    var categories = await db.QueryAsync<Category>(sqlCommand);
                    return new ObservableCollection<Category>(categories);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке категорий", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Category>();
            }
        }

        public static ObservableCollection<Word> GetWords(string wordCategory)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sqlCommand = $"SELECT * FROM [{wordCategory}]";
                    return new ObservableCollection<Word>(db.Query<Word>(sqlCommand).ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке списка слов", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Word>();
            }
        }

        public static bool RemoveCategory(Category category)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();

                    command.CommandText = $"DROP TABLE [{category.CategoriesName}]";
                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    command.CommandText = $"DELETE FROM Categories WHERE Categories.Id = {category.Id}";
                    command.ExecuteNonQuery();

                    MessageBox.Show($"Категория {category.CategoriesName} удалена", "Выполнено");

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении категории", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool RemoveWord(Word word)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                                                                                
                    command.CommandText = $"DELETE FROM [{word.CategoryName}] WHERE [{word.CategoryName}].Id = {word.Id}";
                    command.Connection = connection;

                    command.ExecuteNonQuery();                    

                    connection.Close();

                    MessageBox.Show($"Слово {word.Words} удалено", "Выполнено");

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении слова", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }            
        }

        public static void CreateCategory(Category category)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"INSERT INTO Categories VALUES({category.LevelsId},'{category.CategoriesName}')";
                    command.Connection = connection;
                    command.ExecuteNonQuery();

                    command.CommandText = $"CREATE TABLE [{category.CategoriesName}] (" +
                        $"Id INT IDENTITY PRIMARY KEY," +
                        $" CategoryName NVARCHAR(50) REFERENCES Categories(CategoriesName) ON DELETE CASCADE," +
                        $" Words NVARCHAR(20) NOT NULL," +
                        $" Transcriptions NVARCHAR(50) NOT NULL," +
                        $" Sentence NVARCHAR(120) NOT NULL," +
                        $" TranslateWords NVARCHAR(20) NOT NULL," +
                        $" TransSentence NVARCHAR(120) NOT NULL," +
                        $" Picture VARBINARY(MAX) NOT NULL," +
                        $" Is_completed INT NOT NULL DEFAULT 0)";

                    command.ExecuteNonQuery();
                                        
                    MessageBox.Show($"Категория {category.CategoriesName} создана", "Выполнено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при создании категории", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void UpdateCategory(Category newCategory, Category oldCategory)
        {
            try
            {               
                using (IDbConnection db = new SqlConnection(connectionString))
                {                    
                    CreateCategory(newCategory);

                    string sqlCommand = $"INSERT INTO [{newCategory.CategoriesName}] " +
                        $"SELECT '{newCategory.CategoriesName}', Words, Transcriptions, Sentence, TranslateWords, TransSentence, Picture, Is_completed" +
                        $" FROM [{oldCategory.CategoriesName}]";
                    db.Query<Category>(sqlCommand);

                    if(!RemoveCategory(oldCategory))
                        MessageBox.Show($"Не удалена категория с именем {oldCategory.CategoriesName}", "Ошибка при удалении категории",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                    MessageBox.Show($"Категория изменена", "Выполнено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при изменении категории", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool AddWord(Word word)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"INSERT INTO [{word.CategoryName}] (CategoryName, Words, Transcriptions, Sentence, TranslateWords, TransSentence, Picture, Is_completed) " +
                        $"VALUES (" +
                        $"'{word.CategoryName}', " +
                        $"'{word.Words}', " +
                        $"N'{word.Transcriptions}', " +
                        $"'{word.Sentence}', " +
                        $"'{word.TranslateWords}', " +
                        $"'{word.TransSentence}', " +
                        $"@Picture," +
                        $"{word.Is_completed})";

                    command.Parameters.Add("@Picture", SqlDbType.VarBinary, 1000000);
                    command.Parameters["@Picture"].Value = word.Picture;
                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    MessageBox.Show($"Слово {word.Words} добавлено", "Выполнено");

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при добавлении слова", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool UpdateWord(Word newWord, Word oldWord)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();

                    if(newWord.Picture is not null)
                    {
                        command.Parameters.Add("@Picture", SqlDbType.VarBinary, 1000000);
                        command.Parameters["@Picture"].Value = newWord.Picture;
                        command.CommandText = $"UPDATE [{oldWord.CategoryName}] SET " +
                        $"CategoryName = '{oldWord.CategoryName}', " +
                        $"Words = '{newWord.Words}', " +
                        $"Transcriptions = N'{newWord.Transcriptions}', " +
                        $"Sentence = '{newWord.Sentence}', " +
                        $"TranslateWords = '{newWord.TranslateWords}', " +
                        $"TransSentence = '{newWord.TransSentence}', " +
                        $"Picture = @Picture, " +
                        $"Is_completed = {newWord.Is_completed}" +
                        $"WHERE Id = {newWord.Id}";
                    }
                    else
                    {                        
                        command.CommandText = $"UPDATE [{oldWord.CategoryName}] SET " +
                        $"CategoryName = '{oldWord.CategoryName}', " +
                        $"Words = '{newWord.Words}', " +
                        $"Transcriptions = N'{newWord.Transcriptions}', " +
                        $"Sentence = '{newWord.Sentence}', " +
                        $"TranslateWords = '{newWord.TranslateWords}', " +
                        $"TransSentence = '{newWord.TransSentence}', " +
                        $"Is_completed = {newWord.Is_completed}" +
                        $"WHERE Id = {newWord.Id}";
                    }

                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    MessageBox.Show($"Слово {oldWord.Words} изменено", "Выполнено");

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при изменении слова", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static ImageSource ByteArrToImageSource(byte[] byteArray)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
                    var svgBitmap = svgDocument.Draw();
                    var svgImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        svgBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    return svgImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при приобразовании изображения");
                return null;
            }
        }

        public static byte[] ImageSourceToByteArray(ImageSource imageSource)
        {
            try
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource as BitmapSource));

                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при преобразовании изображения в массив байт");
                return null;
            }
        }
    }
}
