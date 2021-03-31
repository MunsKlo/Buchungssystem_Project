using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Item
{
    public class DeleteModel : PageModel
    {
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();
        readonly private MainMethods MainMethods = new MainMethods();
        public ItemModel Item { get; set; }
        public List<string> BorrowStatus { get; set; } = new List<string> { "Ja", "Nein" };
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public string FormId { get; set; }
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) == "J")
            {
                var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty;
                StatusMessage = Request.Query["message"].Count > 0 ? Request.Query["message"][0] : string.Empty;
                if (!uint.TryParse(parameter, out uint id))
                {
                    Item = new ItemModel();
                    Response.Redirect("/item/Index?status=3");
                }
                else
                {
                    Item = MainMethods.GetItem(id);

                    if(Item.Name == null)
                    {
                        Response.Redirect("/Item/Index?status=6");
                    }
                    else
                    {
                        if (MainMethods.IsItemInBorrows(Item.Id))
                        {
                            Response.Redirect("/Item/Index?status=4");
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("/item/Index?status=2");
            }
        }

        public void OnPost()
        {
            var id = Convert.ToUInt32(Request.Query["id"][0]);

            Item = MainMethods.GetItem(id);

            if (uint.TryParse(FormId, out uint numbId) && Item.Id == numbId)
            {
                var res = DbConn.DeleteItem(id);

                if (res == "Success")
                {
                    Response.Redirect("/Item/Index?status=1");
                }
                else
                {
                    Response.Redirect("/Item/Index?status=3");
                }
            }
            else
            {
                StatusMessage = "Falsche Id!";
                Response.Redirect($"/Item/Delete?id={Item.Id}&message={StatusMessage}");
            }
        }
    }
}
