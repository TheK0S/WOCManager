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
    }
}
