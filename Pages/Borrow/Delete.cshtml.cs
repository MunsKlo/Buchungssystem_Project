using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Borrow
{
    public class DeleteModel : PageModel
    {
        public string StatusMessage { get; set; } = string.Empty;
        public BorrowModel Borrow { get; set; }
        public ItemModel Item { get; set; }

        [BindProperty]
        public string FormId { get; set; }


        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) == "J")
            {
                var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty;
                StatusMessage = Request.Query["message"].Count > 0 ? Request.Query["message"][0] : string.Empty;

                if(!uint.TryParse(parameter, out uint id))
                {
                    Response.Redirect("/Borrows/Index?status=3");
                }
                else
                {
                    Borrow = MainMethods.GetBorrow(id);
                    if(Borrow == null)
                    {
                        Response.Redirect("/Borrow/Index?status=6");
                    }
                    Item = MainMethods.GetItem(Borrow.ItemId);
                }
            }
            else
            {
                Response.Redirect("/Borrow/Index?status=2");
            }
        }

        public void OnPost()
        {
            var id = Convert.ToUInt32(Request.Query["id"].Count > 0 ? Request.Query["id"][0] : "");

            Borrow = MainMethods.GetBorrow(id);
            Item = MainMethods.GetItem(Borrow.ItemId);

            if (uint.TryParse(this.FormId, out uint numbId) && Borrow.Id == numbId)
            {
                var res = DbConn.DeleteBorrow(id);

                if (res == "Success")
                {
                    Response.Redirect("/Borrow/Index?status=1");         
                }
                else
                {
                    Response.Redirect("/Borrow/Index?status=3");
                }
            }
            else
            {
                StatusMessage = "Falsche Id!";
                Response.Redirect($"/Borrow/Delete?id={Borrow.Id}&message={StatusMessage}");
            }
        }
    }
}
