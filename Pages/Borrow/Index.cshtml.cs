using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Borrow
{
    public class IndexModel : PageModel
    {
        public List<BorrowModel> BorrowList { get; set; }
        public List<ItemModel> ItemList { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public string CreateLink { get; set; } = string.Empty;

        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        public void OnGet()
        {
            var resList = DbConn.GetAllBorrows();
            var parameter = Convert.ToInt32(Request.Query["status"]);

            StatusMessage = MainMethods.StatusHandler(parameter);

            if (resList[0].ToString() == "Success")
            {
                var paraList = (List<object>)resList[1];

                if (paraList.Count > 0)
                {
                    BorrowList = MainMethods.FillBorrowList(paraList);

                    //Wenn alles bei den Verleihen geklappt hat werden nun die Items dazu aus der Datenbank geholt
                    resList = DbConn.GetAllItems();

                    if(resList[0].ToString() == "Success")
                    {
                        paraList = (List<object>)resList[1];

                        if(paraList.Count > 0)
                        {
                            ItemList = MainMethods.FillItemList(paraList);
                        }
                    }
                }
            }
            else
            {
                StatusMessage = "Beim Laden der Daten aus der Datenbank geschah ein Fehler!";
            }
        }

        public string InfoStringItem(ItemModel item)
        {
            return $"{item.Id}. {item.Name}";
        }
    }
}
