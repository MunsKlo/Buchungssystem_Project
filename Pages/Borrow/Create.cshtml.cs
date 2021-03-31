using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buchungsystem_WindowsAuth.Pages.Borrow
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public string Distributor { get; set; }
        [BindProperty]
        public string Borrower { get; set; }
        [BindProperty]
        public string BorrowDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
        [BindProperty]
        public string Availability { get; set; }
        [BindProperty]
        public uint ItemId { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        [BindProperty]
        public string Item { get; set; }


        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        public List<ItemModel> ItemList { get; set; } = new List<ItemModel>();
        public List<SelectListItem> Items { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) == "J")
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

                        foreach (var item in ItemList)
                        {
                            var selectItem = new SelectListItem(item.Id + ". " + item.Description, item.Id.ToString());
                            Items.Add(selectItem);
                        }
                    }

                    if(Request.Query["id"].Count > 0)
                    {
                        var paraId = Request.Query["id"][0];

                        foreach (var item in Items)
                        {
                            if(item.Value == paraId)
                            {
                                item.Selected = true;
                            }
                        }

                        var res = DbConn.GetAllBorrowsWithThisItem(Convert.ToUInt32(paraId));

                        if(res[0].ToString() == "Success" && StatusMessage.Length > 0)
                        {
                            var lastDate = MainMethods.GetLastDate(Convert.ToUInt32(paraId));

                            StatusMessage += $" Item ist ab dem {lastDate:dd.MM.yyyy} wieder komplett verfügbar!";
                        }
                    }
                }
                else
                {
                    Response.Redirect("/Borrow/Index?status=3");
                }
            }
            else
            {
                Response.Redirect("/Borrow/Index?status=2");

            }
        }

        public void OnPost()
        {
            ItemId = Convert.ToUInt32(Item);
            var borrow = new BorrowModel(Distributor, Borrower, BorrowDate, Availability, ItemId);
            if(MainMethods.ItemCanBeBorrowInThisPeriod(ItemId, BorrowDate, Availability))
            {
                var res = DbConn.CreateNewBorrow(borrow);
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
                Response.Redirect($"/Borrow/Create?status=7&id={ItemId}");
            }
        }
    }
}
