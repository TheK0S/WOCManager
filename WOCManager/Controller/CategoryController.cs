using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOCManager.Model;

namespace WOCManager.Controller
{
    internal class CategoryController
    {
        public ObservableCollection<Level> Levels { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public CategoryController()
        {
            Categories = Data.GetCategories();
            Levels = Data.GetLevels();
        }
    }
}
