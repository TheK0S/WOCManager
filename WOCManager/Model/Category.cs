using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WOCManager.Model
{
    public class Category : INotifyPropertyChanged
    {
        private int _id;
        private int _levelsId;
        private string _categoriesName;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public int LevelsId
        {
            get => _levelsId;
            set { _levelsId = value; OnPropertyChanged(nameof(LevelsId)); }
        }
        public string CategoriesName
        {
            get => _categoriesName;
            set { _categoriesName = value; OnPropertyChanged(nameof(CategoriesName)); }
        }

        public override string ToString()
        {
            return CategoriesName;
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

            return Equals(obj as Category);
        }

        public bool Equals(Category other)
        {
            if (other is null) return false;
            return LevelsId == other.LevelsId && CategoriesName == other.CategoriesName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + LevelsId.GetHashCode();
                hash = hash * 23 + (CategoriesName != null ? CategoriesName.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(Category category1, Category category2)
        {
            if (ReferenceEquals(category1, null) || ReferenceEquals(category2, null))
                return false;

            if (ReferenceEquals(category1, category2))
                return true;            

            return category1.Equals(category2);
        }

        public static bool operator !=(Category category1, Category category2)
        {
            return !(category1 == category2);
        }
    }
}
