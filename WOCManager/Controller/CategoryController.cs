using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WOCManager.Model;
using WOCManager.View;

namespace WOCManager.Controller
{
    internal class CategoryController : INotifyPropertyChanged
    {
        private ObservableCollection<Category>? _categories;
        private ObservableCollection<Category>? _filteredCategories;
        private Level? _selectedLevel;
        private Category? _selectedCategory;
        private string? _categoryName;
        private string? _searchText;
        private RelayCommand? _addCategory;
        private RelayCommand? _updateCategory;
        private RelayCommand? _removeCategory;

        public ObservableCollection<Level> Levels { get; set; }
        
        public ObservableCollection<Category> Categories
        {
            get => _categories ?? new ObservableCollection<Category>();
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
                ApplyFilter();
            }
        }

        public ObservableCollection<Category> FilteredCategories
        {
            get => _filteredCategories ?? new ObservableCollection<Category>();
            set
            {
                _filteredCategories = value;
                OnPropertyChanged(nameof(FilteredCategories));
            }
        }

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

        public string SearchText
        {
            get => _searchText ?? "";
            set
            {
                _searchText = value;
                ApplyFilter();
                OnPropertyChanged(nameof(SearchText));
            }
        }


        public RelayCommand AddCategory
        {
            get
            {
                return _addCategory??
                    (_addCategory = new RelayCommand(async obj =>
                    {
                        await Task.Run(() =>
                        {
                            if (CategoryName is null || SelectedLevel is null)
                            {
                                MessageBox.Show("Не заполнены поля для добавления категории", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            
                            var newCategory = new Category { LevelsId = SelectedLevel.Id, CategoriesName = CategoryName };

                            if (SelectedCategory is null || (SelectedCategory is not null && SelectedCategory != newCategory))
                            {
                                CategoryData.CreateCategory(newCategory);
                                Categories = CategoryData.GetCategories();
                            }
                            else
                            {
                                MessageBox.Show("Категория уже существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        if (MessageBox.Show("Будет создана новая таблица и все поля текущей, будут скопированы в новую. После чего текущая будет удалена.\n\t\tВы уверены?", 
                            "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No) 
                            return;                   

                        await Task.Run(() =>
                        {
                            if (CategoryName is null || SelectedLevel is null || SelectedCategory is null)
                            {
                                MessageBox.Show("Не заполнены поля для изменения категории", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            var newCategory = new Category { LevelsId = SelectedLevel.Id, CategoriesName = CategoryName };

                            if (SelectedCategory == newCategory)
                            {
                                MessageBox.Show("Нет изменений для сохранения. Внесите изменения в поля категории и попробуйте снова", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                                
                            CategoryData.UpdateCategory(newCategory, SelectedCategory);
                            Categories = CategoryData.GetCategories();    
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
                        await Task.Run(() =>
                        {
                            if (SelectedCategory is null)
                            {
                                MessageBox.Show("Не выбрана категория для удаления", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            if (MessageBox.Show("Удаление категории приведет к потере всех слов добавленных в категорию.\n\t\t\tВы уверены?", "Внимание!",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                CategoryData.RemoveCategory(SelectedCategory);
                                Categories = CategoryData.GetCategories();
                            }
                        });
                    }));
            }
        }

        public CategoryController()
        {
            Categories = CategoryData.GetCategories();
            FilteredCategories = Categories;
            Levels = CategoryData.GetLevels();            
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredCategories = Categories;
            }
            else
            {
                FilteredCategories = new ObservableCollection<Category>(
                    Categories.Where(c => c.CategoriesName.ToLower().Contains(SearchText.ToLower())));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
