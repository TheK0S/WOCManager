using Microsoft.Win32;
using Svg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using WOCManager.Model;
using System.IO;

namespace WOCManager.Controller
{
    internal class WordController : INotifyPropertyChanged
    {
        private ObservableCollection<Word>? _words;
        private Word? _selectedWord;
        private BitmapSource? _selectedImageSource;
        private ObservableCollection<Word>? _filteredWords;
        private ObservableCollection<Category>? _wordCategories;
        private Category? _selectedWordCategory;
        private string? _searchText;
        private string? _wordField;
        private string? _translateWordField;
        private string? _sentenceWordField;
        private string? _transSentenceWordField;
        private string? _transcriptionWordField;
        private string? _imgLoc;

        private RelayCommand? _refreshCategoriesCommand;
        private RelayCommand? _addPictureCommand;
        private RelayCommand? _addWordCommand;
        private RelayCommand? _updateWordCommand;
        private RelayCommand? _removeWordCommand;

        public WordController()
        {
            WordCategories = CategoryData.GetCategories();
        }

        public ObservableCollection<Word> Words
        {
            get => _words ?? new ObservableCollection<Word>();
            set
            {
                _words = value;
                OnPropertyChanged(nameof(Words));
            }
        }

        public Word? SelectedWord
        {
            get => _selectedWord ?? new Word();
            set
            {
                _selectedWord = value;
                if(_selectedWord is not null) FillInTheFields(_selectedWord);
                OnPropertyChanged(nameof(SelectedWord));
            }
        }
        
        public BitmapSource? SelectedImageSource
        {
            get => _selectedImageSource;
            set
            {
                _selectedImageSource = value;
                OnPropertyChanged(nameof(SelectedImageSource));
            }
        }

        public ObservableCollection<Word> FilteredWords
        {
            get => _filteredWords ?? new ObservableCollection<Word>();
            set
            {
                _filteredWords = value;
                OnPropertyChanged(nameof(FilteredWords));
            }
        }

        public ObservableCollection<Category> WordCategories
        {
            get => _wordCategories ?? new ObservableCollection<Category>();
            set
            {
                _wordCategories = value;
                OnPropertyChanged(nameof(WordCategories));
            }
        }

        public Category? SelectedWordCategory
        {
            get => _selectedWordCategory;
            set
            {
                _selectedWordCategory = value;                
                OnPropertyChanged(nameof(SelectedWordCategory));
                if (SelectedWordCategory is not null)
                {
                    Words = WordData.GetWords(SelectedWordCategory.CategoriesName);
                    SearchText = string.Empty;
                    ApplyWordFilter();
                }
            }
        }

        public string SearchText
        {
            get => _searchText ?? string.Empty;
            set
            {
                _searchText = value;
                ApplyWordFilter();
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public string WordField
        {
            get =>  _wordField ?? string.Empty;
            set
            {
                _wordField = value;
                OnPropertyChanged(nameof(WordField));
            }
        }

        public string SentenceWordField
        {
            get => _sentenceWordField ?? string.Empty;
            set
            {
                _sentenceWordField = value;
                OnPropertyChanged(nameof(SentenceWordField));
            }
        }

        public string TranslateWordField
        {
            get => _translateWordField ?? string.Empty;
            set
            {
                _translateWordField = value;
                OnPropertyChanged(nameof(TranslateWordField));
            }
        }

        public string TransSentenceWordField
        {
            get => _transSentenceWordField ?? string.Empty;
            set
            {
                _transSentenceWordField = value;
                OnPropertyChanged(nameof(TransSentenceWordField));
            }
        }

        public string TranscriptionWordField
        {
            get => _transcriptionWordField ?? string.Empty;
            set
            {
                _transcriptionWordField = value;
                OnPropertyChanged(nameof(TranscriptionWordField));
            }
        }

        public RelayCommand RefreshCategoriesCommand
        {
            get
            {
                return _refreshCategoriesCommand ??
                    (_refreshCategoriesCommand = new RelayCommand(async obj =>
                    {
                        SelectedWordCategory = null;
                        await Task.Run(() =>
                        {
                            WordCategories = CategoryData.GetCategories();
                        });
                    }));
            }
        }

        public RelayCommand AddPictureCommand
        {
            get
            {
                return _addPictureCommand ??
                    (_addPictureCommand = new RelayCommand(obj =>
                    {
                        SVGLoad();
                    }));
            }
        }
                

        public RelayCommand? AddWordCommand
        {
            get
            {
                return _addWordCommand ??
                    (_addWordCommand = new RelayCommand(async obj =>
                    {
                        if (SelectedWordCategory is null)
                        {
                            MessageBox.Show("Не выбрана категория для слова", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (!IsWordFieldsValid())
                        {
                            MessageBox.Show("Не все поля заполнены", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Word newWord = CreateWordFromDataInFields();

                        await Task.Run(() =>
                        {
                            if(!WordContains(newWord))
                            {
                                if(WordData.AddWord(newWord))
                                {
                                    Words = WordData.GetWords(SelectedWordCategory.CategoriesName);
                                    SearchText = string.Empty;
                                    ApplyWordFilter();
                                    SelectedWord = null;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Слово не добавлено так, как оно уже существует", "Ошибка при добавлении слова", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });                        
                    }));
            }
        }

        
        public RelayCommand? UpdateWordCommand
        {
            get
            {
                return _updateWordCommand ??
                    (_updateWordCommand = new RelayCommand(async obj =>
                    {
                        await Task.Run(() =>
                        {
                            if (SelectedWordCategory is not null && SelectedWord is not null)
                            {
                                if(IsWordFieldsValid())
                                {
                                    WordData.UpdateWord(CreateWordFromDataInFields() , SelectedWord);
                                }
                                else
                                {
                                    MessageBox.Show("Не все поля заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Не выбрана категория или слово для изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            
                        });
                    }));
            }
        }

        public RelayCommand? RemoveWordCommand
        {
            get
            {
                return _removeWordCommand ??
                    (_removeWordCommand = new RelayCommand(async obj =>
                    {
                        if(SelectedWord is not null && MessageBox.Show($"Вы хотите удалить слово {SelectedWord?.Words} из категории {SelectedWord?.CategoryName}",
                            "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            await Task.Run(() =>
                            {
                                WordData.RemoveWord(SelectedWord);
                            });
                        }
                    }));
            }
        }

        private bool WordContains(Word word)
        {
            bool isContains = false;

            foreach (var item in Words)
            {
                if(word == item)
                {
                    isContains = true;
                    break;
                }
            }

            return isContains;
        }


        private Word CreateWordFromDataInFields()
        {
            return new Word
            {
                Id = 0,
                CategoryName = SelectedWordCategory?.CategoriesName.Replace("\'", "\'\'"),
                Words = WordField.Replace("\'", "\'\'"),
                TranslateWords = TranslateWordField.Replace("\'", "\'\'"),
                Sentence = SentenceWordField.Replace("\'", "\'\'"),
                TransSentence = TransSentenceWordField.Replace("\'", "\'\'"),
                Transcriptions = TranscriptionWordField.Replace("\'", "\'\'"),
                Picture = GetPictureByteArray(),
                Is_completed = 0
            };
        }

        private byte[] GetPictureByteArray()
        {
            byte[] byteArray;

            if (SelectedWord?.Picture != null)
            {
                byteArray = SelectedWord.Picture;
            }
            else
            {
                FileStream file = new FileStream(_imgLoc, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(file);
                byteArray = binaryReader.ReadBytes((int)file.Length);
            }

            return byteArray;
        }

        private void SVGLoad()
        {
            try
            {
                OpenFileDialog ofdPicture = new OpenFileDialog();
                ofdPicture.Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*";
                ofdPicture.FilterIndex = 1;

                if (ofdPicture.ShowDialog() == true)
                {
                    SvgDocument svgDocument = SvgDocument.Open(ofdPicture.FileName);
                    var svgBitmap = svgDocument.Draw();
                    var svgImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        svgBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    SelectedImageSource = svgImage;
                }

                _imgLoc = ofdPicture.FileName.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке картинки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillInTheFields(Word selectedWord)
        {
            SelectedImageSource = (BitmapSource)WordData.ByteArrToImageSource(selectedWord.Picture);
            WordField = selectedWord.Words ?? string.Empty;
            TranslateWordField = selectedWord.TranslateWords ?? string.Empty;
            SentenceWordField = selectedWord.Sentence ?? string.Empty;
            TransSentenceWordField = selectedWord.TransSentence ?? string.Empty;
            TranscriptionWordField = selectedWord.Transcriptions ?? string.Empty;
        }

        private bool IsWordFieldsValid()
        {
            if (   WordField?.Length > 0 
                && TranslateWordField?.Length > 0
                && TranscriptionWordField?.Length > 0
                && SentenceWordField?.Length > 0
                && TransSentenceWordField?.Length > 0
                && SelectedImageSource != null)
            {
                return true;
            }
            else { return false; }
        }

        private void ApplyWordFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredWords = Words;
            }
            else
            {
                string lowerSearchText = SearchText.ToLower();

                FilteredWords = new ObservableCollection<Word>(
                    Words.Where(w =>
                       w.Words.ToLower().Contains(lowerSearchText)
                    || w.TranslateWords.ToLower().Contains(lowerSearchText)
                    ));
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
