using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Item
{
    public class OverviewModel : PageModel
    {
        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();
        public List<BorrowModel> BorrowList { get; set; }
        public ItemModel Item { get; set; }
        public void OnGet()
        {
            var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty;

            if(uint.TryParse(parameter, out uint id))
            {
                Item = MainMethods.GetItem(id);
                
                if(Item.Name != null)
                {
                    var res = DbConn.GetAllBorrowsWithThisItem(id);

                    if(res[0].ToString() == "Success")
                    {
                        var paraList = (List<object>)res[1];

                        BorrowList = MainMethods.FillBorrowList(paraList);
                    }
                }
                else
                {
                    Response.Redirect("/Item/Index?status=6");
                }
            }
            else
            {
                Response.Redirect("/Item/Index?status=3");
            }
        }
    }
}
