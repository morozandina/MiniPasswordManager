using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordBl.Model
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}
