using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOCManager.Model
{
    public class Level
    {
        public int Id { get; set; }
        public string LevelName { get; set; }

        public override string ToString()
        {
            return LevelName;
        }
    }
}
