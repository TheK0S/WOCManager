using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WOCManager.Model
{
    public class Word : INotifyPropertyChanged
    {
        private int _id;
        private string? _categoryName;
        private string? _words;
        private string? _transcriptions;
        private string? _sentence;
        private string? _translateWords;
        private string? _transSentence;
        private byte[] _picture;
        private int _is_completed;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string? CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(nameof(CategoryName)); }
        }
        public string? Words
        {
            get => _words;
            set { _words = value; OnPropertyChanged(nameof(Words)); }
        }
        public string? Transcriptions
        {
            get => _transcriptions;
            set { _transcriptions = value; OnPropertyChanged(nameof(Transcriptions)); }
        }
        public string? Sentence
        {
            get => _sentence;
            set { _sentence = value; OnPropertyChanged(nameof(Sentence)); }
        }
        public string? TranslateWords
        {
            get => _translateWords;
            set { _translateWords = value; OnPropertyChanged(nameof(TranslateWords)); }
        }
        public string? TransSentence
        {
            get => _transSentence;
            set { _transSentence = value; OnPropertyChanged(nameof(TransSentence)); }
        }
        public byte[] Picture
        {
            get => _picture;
            set { _picture = value; OnPropertyChanged(nameof(Picture)); }
        }
        public int Is_completed
        {
            get => _is_completed;
            set { _is_completed = value; OnPropertyChanged(nameof(Is_completed)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
