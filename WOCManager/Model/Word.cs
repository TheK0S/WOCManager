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

        public override bool Equals(object obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;

            return Equals(obj as Word);
        }

        public bool Equals(Word other)
        {
            if (other is null) return false;
            return CategoryName == other.CategoryName
                && Words == other.Words
                && Transcriptions == other.Transcriptions
                && Sentence == other.Sentence
                && TranslateWords == other.TranslateWords
                && TransSentence == other.TransSentence
                && Picture == other.Picture;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Words != null ? Words.GetHashCode() : 0);
                hash = hash * 23 + (Transcriptions != null ? Transcriptions.GetHashCode() : 0);
                hash = hash * 23 + (Sentence != null ? Sentence.GetHashCode() : 0);
                hash = hash * 23 + (TranslateWords != null ? TranslateWords.GetHashCode() : 0);
                hash = hash * 23 + (TransSentence != null ? TransSentence.GetHashCode() : 0);
                hash = hash * 23 + (Picture != null ? Picture.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(Word word1, Word word2)
        {
            if (ReferenceEquals(word1, null) || ReferenceEquals(word2, null))
                return false;

            if (ReferenceEquals(word1, word2))
                return true;

            return word1.Equals(word2);
        }

        public static bool operator !=(Word word1, Word word2)
        {
            return !(word1 == word2);
        }
    }
}
