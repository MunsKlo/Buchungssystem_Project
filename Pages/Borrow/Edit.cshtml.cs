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
    public class EditModel : PageModel
    {
        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        public BorrowModel Borrow { get; set; }
        public ItemModel Item { get; set; }
        public ItemModel NewItem { get; set; }
        public List<ItemModel> ItemList { get; set; }
        public List<SelectListItem> Items { get; set; } = new List<SelectListItem>();
        public string StatusMessage { get; set; }

        [BindProperty]
        public string Distributor { get; set; }
        [BindProperty]
        public string Borrower { get; set; }
        [BindProperty]
        public string BorrowDate { get; set; }
        [BindProperty]
        public string Availability { get; set; }
        [BindProperty]
        public uint ItemId { get; set; }
        public void OnGet()
        {
            if (MainMethods.IsUserCreator(User.Identity.Name) == "J")
            {
                var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty;

                if (uint.TryParse(parameter, out uint id))
                {
                    Borrow = MainMethods.GetBorrow(id);
                    Item = MainMethods.GetItem(Borrow.ItemId);

                    BorrowDate = Convert.ToDateTime(Borrow.BorrowDate).ToString("yyyy-MM-dd");
                    Availability = Convert.ToDateTime(Borrow.Availability).ToString("yyyy-MM-dd");

                    var res = DbConn.GetAllItems();

                    if (res[0].ToString() == "Success")
                    {
                        var paraList = (List<object>)res[1];
                        ItemList = MainMethods.FillItemList(paraList);
                        foreach (var item in ItemList)
                        {
                            var selectItem = new SelectListItem(item.Id + ". " + item.Description, item.Id.ToString());
                            Items.Add(selectItem);
                        }

                        if (Request.Query["itemId"].Count > 0)
                        {
                            var paraId = Request.Query["itemId"][0];

                            foreach (var item in Items)
                            {
                                if (item.Value == paraId)
                                {
                                    item.Selected = true;
                                }
                            }
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
            var id = Convert.ToUInt32(Request.Query["id"][0]);

            Borrow = MainMethods.GetBorrow(id);
            Item = MainMethods.GetItem(Borrow.ItemId);

            var newDistributor = Distributor != Borrow.Distributor && Distributor.Length > 0 ? Distributor : Borrow.Distributor;
            var newBorrower = Borrower != Borrow.Borrower && Borrower.Length > 0 ? Borrower : Borrow.Borrower;
            var newBorrowDate = BorrowDate != Borrow.BorrowDate && BorrowDate.Length > 0 ? BorrowDate : Borrow.BorrowDate;
            var newAvailability = Availability != Borrow.Availability && Availability.Length > 0 ? Availability : Borrow.Availability;
            var newItemId = Borrow.ItemId;

            var newBorrow = new BorrowModel(Borrow.Id, newDistributor, newBorrower, newBorrowDate, newAvailability, newItemId);

            if(MainMethods.ItemCanBeBorrowInThisPeriod(Borrow.ItemId, newBorrowDate, newAvailability))
            {
                var res = DbConn.ChangeBorrowInDb(newBorrow);

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
                Response.Redirect("/Borrow/Index?status=7");
            }
        }
    }
}
