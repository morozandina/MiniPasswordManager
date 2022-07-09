using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordBl.Model
{
    public class Entry
    {
        public int EntryId { get; set; }
        public int Group { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Notes { get; set; }
    }
}
