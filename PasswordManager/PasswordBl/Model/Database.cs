using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordBl.Model
{
    public class Database
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Password { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
