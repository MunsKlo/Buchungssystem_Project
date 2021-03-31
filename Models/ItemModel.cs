using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buchungsystem_WindowsAuth.Models
{
    public class ItemModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ItemModel(string _name, string _description)
        {
            Name = _name;
            Description = _description;
        }

        public ItemModel(uint _id, string _name, string _description)
        {
            Id = _id;
            Name = _name;
            Description = _description;
        }

        public ItemModel()
        {

        }
    }
}
