using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Item
{
    public class IndexModel : PageModel
    {
        readonly MainMethods MainMethods = new MainMethods();
        readonly InterfaceDatabase DbConn = new InterfaceDatabase();
        public string StatusMessage { get; set; } = string.Empty;
        public List<ItemModel> ItemList { get; set; }
        public List<string> BorrowStatus { get; set; } = new List<string> { "Ja", "Nein" };

        [BindProperty]
        public string SearchString { get; set; }
        [BindProperty]
        public bool OnlyAvailable { get; set; }
        public void OnGet()
        {
            if(Request.Query["search"].Count > 0)
            {
                SearchString = Request.Query["search"][0];
                OnlyAvailable = Convert.ToBoolean(Request.Query["available"][0]);
                var resList = DbConn.GetAllSearchedItems(SearchString);

                if(resList[0].ToString() == "Success")
                {
                    var paraList = (List<object>)resList[1];
                    ItemList = new List<ItemModel>();

                    if(OnlyAvailable == true)
                    {
                        var list = MainMethods.FillItemList(paraList);
                        foreach (var item in list)
                        {
                            if (!IsItemBorrowed(item.Id))
                            {
                                ItemList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        ItemList = MainMethods.FillItemList(paraList);
                    }
                }
            }
            else
            {
                var resList = DbConn.GetAllItems();
                var parameter = Convert.ToInt32(Request.Query["status"]);

                StatusMessage = MainMethods.StatusHandler(parameter);

                if (resList[0].ToString() == "Success")
                {
                    var paraList = (List<object>)resList[1];

                    if (paraList.Count > 0)
                    {
                        ItemList = MainMethods.FillItemList(paraList);
                    }
                }
                else
                {
                    StatusMessage = "Beim Laden der Daten aus der Datenbank geschah ein Fehler!";
                }
            }

            
        }

        public void OnPost()
        {
            var resList = DbConn.GetAllSearchedItems(SearchString);

            if(resList[0].ToString() == "Success")
            {
                Response.Redirect($"/Item/Index?search={SearchString}&available={OnlyAvailable}");
            }
            else
            {
                Response.Redirect("/Item/Index?status=3");
            }
        }

        public bool IsItemBorrowed(uint itemId)
        {
            return MainMethods.IsItemTodayBorrowed(itemId);
        }
    }
}
