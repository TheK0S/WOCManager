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
using System.Threading;

namespace WOCManager.Model
{
    class WordData
    {
        static string connectionString = @"Data Source=SQL5110.site4now.net;Initial Catalog=db_a9a0f7_diplomawork;User Id=db_a9a0f7_diplomawork_admin;Password=uchiha322";
        //static string connectionString = @"Data Source = DESKTOP-HHO6PH0; Initial Catalog = WordsDB; Trusted_Connection=True; Encrypt = False";

                
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
