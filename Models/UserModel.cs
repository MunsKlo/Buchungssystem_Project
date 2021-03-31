using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buchungsystem_WindowsAuth.Models
{
    public class UserModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public bool IsCreator { get; set; }
        public bool IsAdmin { get; set; }

        public UserModel(string _name)
        {
            Name = _name;
            IsCreator = false;
            IsAdmin = false;
        }

        public UserModel(uint _id, string _name, bool _isCreator, bool _isAdmin)
        {
            Id = _id;
            Name = _name;
            IsCreator = _isCreator;
            IsAdmin = _isAdmin;
        }

        public UserModel()
        {

        }
    }
}
