using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using static Dapper.SqlMapper;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WOCManager.Model
{
    class CategoryData
    {
        static string connectionString = @"Data Source=SQL5110.site4now.net;Initial Catalog=db_a9a0f7_diplomawork;User Id=db_a9a0f7_diplomawork_admin;Password=uchiha322";

        public static ObservableCollection<Level> GetLevels()
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

                    if (!RemoveCategory(oldCategory))
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
    }
}
