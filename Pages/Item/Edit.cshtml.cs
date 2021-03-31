using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Item
{
    public class EditModel : PageModel
    {
        readonly private InterfaceDatabase dbConn = new InterfaceDatabase();
        readonly private MainMethods MainMethods = new MainMethods();
        public ItemModel Item { get; set; }
        public List<string> BorrowStatus { get; set; } = new List<string> { "Ja", "Nein" };
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Description { get; set; }
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) == "J")
            {
                var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : "";
                if (!uint.TryParse(parameter, out uint id))
                {
                    Response.Redirect("/item/Index?status=3");
                }
                else
                {
                    Item = MainMethods.GetItem(id);
                }
            }
            else
            {
                Response.Redirect("/item/Index?status=2");
            }
        }

        public void OnPost()
        {
            var parameter = Request.Query["id"][0];

            if (!uint.TryParse(parameter, out uint id))
            {
                Response.Redirect("/item/Index?status=3");
            }
            else
            {
                Item = MainMethods.GetItem(id);

                var newName = Name != Item.Name && Name.Length > 0 ? Name : Item.Name;
                var newDescription = Description != Item.Description && Description.Length > 0 ? Description : Item.Description;
                var newItem = new ItemModel(id, newName, newDescription);
                var res = dbConn.ChangeItemInDb(newItem);

                if (res == "Success")
                {
                    Response.Redirect("/item/Index");
                }
                else
                {
                    Response.Redirect("/item/Index?status=3");
                }
            }
            
        }

        public bool IsItemBorrowed(uint itemId)
        {
            return MainMethods.IsItemTodayBorrowed(itemId);
        }
    }
}
