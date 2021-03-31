using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buchungsystem_WindowsAuth.Models;
using Buchungsystem_WindowsAuth;

namespace Buchungsystem_WindowsAuth.Pages.Item
{
    public class CreateModel : PageModel
    {
        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Description { get; set; }
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) != "J")
            {
                Response.Redirect("/item/Index?status=2");
            }
        }

        public void OnPost()
        {
            var item = new ItemModel(Name, Description);
            var res = DbConn.CreateNewItem(item);
            if(res == "Success")
            {
                Response.Redirect("/Item/Index?status=1");
            }
            else
            {
                Response.Redirect("/Item/Index?status=3");
            }
        }
    }
}
