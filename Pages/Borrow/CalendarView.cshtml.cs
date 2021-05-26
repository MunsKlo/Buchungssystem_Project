using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.Borrow
{
    public class CalendarViewModel : PageModel
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
        public uint DaysInMonth { get; set; }
        public uint Start { get; set; }
        public List<List<uint>> ListOfWeeks { get; set; }
        public List<BorrowModel> BorrowList { get; set; }
        public List<BorrowModel> AvaiableList { get; set; }
        public List<ItemModel> ItemList { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();
        public void OnGet()
        {
            FirstDay = new DateTime(Date.Year, Date.Month, 1);
            LastDay = FirstDay.AddMonths(2).AddDays(-1);
            GetStartOfMonth(FirstDay.DayOfWeek.ToString());
            DaysInMonth = Convert.ToUInt32(DateTime.DaysInMonth(FirstDay.Year, FirstDay.Month));
            FillListOfWeeks();
            FillBorrowList();
            FillItemList();
        }

        private void FillBorrowList()
        {
            var res = DbConn.GetAllBorrows();

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];

                var list = MainMethods.FillBorrowList(paraList);
                BorrowList = GetAllBorrowForThisMonth(list);
                AvaiableList = GetAllReturnBorrowsForThisMonth(list);
            }
            else
            {
                StatusMessage = "Fehler in der Datenbank!";
            }
        }

        private void FillItemList()
        {
            var res = DbConn.GetAllItems();

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];

                ItemList = MainMethods.FillItemList(paraList);
            }
            else
            {
                StatusMessage = "Fehler in der Datenbank";
            }
        }
        public string InfoStringItem(ItemModel item)
        {
            return $"{item.Id}. {item.Name}";
        }

        private void GetStartOfMonth(string day)
        {
            switch (day)
            {
                case "Monday":
                    Start = 1;
                    break;
                case "Tuesday":
                    Start = 2;
                    break;
                case "Wednsday":
                    Start = 3;
                    break;
                case "Thursday":
                    Start = 4;
                    break;
                case "Friday":
                    Start = 5;
                    break;
                case "Saturday":
                    Start = 6;
                    break;
                case "Sunday":
                    Start = 7;
                    break;
            }
        }

        private void FillListOfWeeks()
        {
            ListOfWeeks = new List<List<uint>>();
            var week = new List<uint>();
            var d = 0;
            for (uint i = 0; i < DaysInMonth; i++)
            {
                if (d % 7 == 0 && week.Count > 0)
                {
                    ListOfWeeks.Add(week);
                    week = new List<uint>();
                    week.Add(i + 1);
                    d++;
                }
                else
                {
                    if (i == 0)
                    {
                        for (uint j = 0; j < Start - 1; j++)
                        {
                            week.Add(0);
                            d++;
                        }
                    }
                    week.Add(i + 1);
                    d++;
                }
            }

            if (week.Count > 0)
            {
                ListOfWeeks.Add(week);
            }
        }

        private List<BorrowModel> GetAllBorrowForThisMonth(List<BorrowModel> list)
        {
            var newList = new List<BorrowModel>();
            foreach (var borrow in list)
            {
                var checkedDate = Convert.ToDateTime(borrow.BorrowDate);
                while(checkedDate != Convert.ToDateTime(borrow.Availability))
                {
                    if(checkedDate.Month == DateTime.Now.Month)
                    {
                        newList.Add(borrow);
                        break;
                    }
                    checkedDate = checkedDate.AddDays(1);
                }
            }
            return newList;
        }

        private List<BorrowModel> GetAllReturnBorrowsForThisMonth(List<BorrowModel> list)
        {
            var newList = new List<BorrowModel>();
            foreach (var borrow in list)
            {
                if(Convert.ToDateTime(borrow.Availability).Month == DateTime.Now.Month)
                {
                    newList.Add(borrow);
                }
            }
            return newList;
        }
    }
}
