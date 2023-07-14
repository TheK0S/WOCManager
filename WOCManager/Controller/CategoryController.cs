using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WOCManager.Model;
using WOCManager.View;

namespace WOCManager.Controller
{
    internal class CategoryController : INotifyPropertyChanged
    {
        public ObservableCollection<Level> Levels { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        private Level? _selectedLevel;
        private Category? _selectedCategory;
        private string? _categoryName;

        public Level? SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
            }
        }
        
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                SelectedLevel = Levels.First(l => l.Id == _selectedCategory?.LevelsId);
                CategoryName = _selectedCategory?.CategoriesName;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public string? CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }


        private RelayCommand? _addCategory;
        private RelayCommand? _updateCategory;
        private RelayCommand? _removeCategory;
        

        public RelayCommand AddCategory
        {
            get
            {
                return _addCategory??
                    (_addCategory = new RelayCommand(async obj =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() => 
                        {
                            if(CategoryName != null && SelectedLevel != null)
                            {
                                var newCategory = new Category { LevelsId = SelectedLevel.Id, CategoriesName = CategoryName };

                                if (SelectedCategory == null || (SelectedCategory is not null && SelectedCategory != newCategory))
                                {
                                    Data.CreateCategory(newCategory);
                                    Categories = Data.GetCategories();
                                }
                                else
                                    MessageBox.Show("Категория уже существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show("Не заполнены поля для добавления категории", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    }));
            }
        }

        public RelayCommand UpdateCategory
        {
            get
            {
                return _updateCategory ??
                    (_updateCategory = new RelayCommand(async obj =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (CategoryName != null && SelectedLevel != null && SelectedCategory is not null)
                            {
                                var newCategory = new Category { LevelsId = SelectedLevel.Id, CategoriesName = CategoryName };

                                if(SelectedCategory != newCategory)
                                {
                                    Data.UpdateCategory(newCategory, SelectedCategory);
                                    Categories = Data.GetCategories();
                                }                                    
                                else
                                    MessageBox.Show("Нет изменений для сохранения. Внесите изменения в поля категории и попробуйте снова", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show("Не заполнены поля для изменения категории", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    }));
            }
        }

        public RelayCommand RemoveCategory
        {
            get
            {
                return _removeCategory ??
                    (_removeCategory = new RelayCommand(async obj =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (SelectedCategory is not null)
                            {
                                if(MessageBox.Show("Удаление категории приведет к потере всех слов добавленных в категорию.\n\t\tВы уверены?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    Data.RemoveCategory(SelectedCategory);
                                    Categories = Data.GetCategories();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Не выбрана категория для удаления", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    }));
            }
        }

        public CategoryController()
        {
            //Application.Current.Dispatcher.InvokeAsync(async () =>
            //{
            //    Categories = await Data.GetCategories();
            //});

            //Application.Current.Dispatcher.InvokeAsync(async () =>
            //{
            //    Levels = await Data.GetLevels();
            //});

            Categories = Data.GetCategories();
            Levels = Data.GetLevels();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
