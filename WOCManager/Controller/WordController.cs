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

        private RelayCommand? _refreshCategories;
        private RelayCommand? _addPicture;
        private RelayCommand? _addWord;
        private RelayCommand? _updateWord;
        private RelayCommand? _removeWord;

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

        public Word SelectedWord
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

        public RelayCommand RefreshCategories
        {
            get
            {
                return _refreshCategories ??
                    (_refreshCategories = new RelayCommand(async obj =>
                    {
                        SelectedWordCategory = null;
                        await Task.Run(() =>
                        {
                            WordCategories = CategoryData.GetCategories();
                        });
                    }));
            }
        }

        public RelayCommand AddPicture
        {
            get
            {
                return _addPicture ??
                    (_addPicture = new RelayCommand(obj =>
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

                            //ImgLoc = ofdPicture.FileName.ToString();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка при загрузке картинки", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
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
