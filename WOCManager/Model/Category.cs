using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOCManager.Model
{
    public class Category
    {
        public int Id { get; set; }
        public int LevelsId { get; set; }
        public string CategoriesName { get; set; }

        public override string ToString()
        {
            return CategoriesName;
        }
    }
}
