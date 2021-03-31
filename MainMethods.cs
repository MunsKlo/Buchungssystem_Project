using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buchungsystem_WindowsAuth.Pages
{
    public class MainMethods
    {
        public List<ItemModel> ItemList { get; set; } = new List<ItemModel>();
        public List<BorrowModel> BorrowList { get; set; } = new List<BorrowModel>();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        public string StatusMessage { get; set; } = string.Empty;

        public string IsUserAdmin(string name)
        {
            var username = CreateUserName(name);

            if (!ExistUser(username))
            {
                var resCreate = DbConn.CreateUser(username);

                if(resCreate[0].ToString() == "Fail")
                {
                    return "F";
                }
            }

            var res = DbConn.GetUser(username);

            if (res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];
                var user = FillUserList(paraList)[0];

                return user.IsAdmin ? "J" : "N";
            }
            return "F";
        }

        public bool ExistUser(string username)
        {
            var res = DbConn.GetUser(username);

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];

                if (paraList.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public string IsUserCreator(string name)
        {
            var username = CreateUserName(name);

            if (!ExistUser(username))
            {
                var resCreate = DbConn.CreateUser(username);

                if (resCreate[0].ToString() == "Fail")
                {
                    return "F";
                }
            }

            var res = DbConn.GetUser(username);

            if (res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];
                var user = FillUserList(paraList)[0];

                return user.IsCreator ? "J" : "N";
            }
            return "F";
        }

        public string StatusHandler(int status)
        {
            var statusMessage = string.Empty;
            switch (status)
            {
                case 1:
                    statusMessage = "Erfolgreich ausgeführt!";
                    break;
                case 2:
                    statusMessage = "Sie haben nicht genügend Benutzerrechte!";
                    break;
                case 3:
                    statusMessage = "Fehler in der Datenbank";
                    break;
                case 4:
                    statusMessage = "Item kann nicht gelöscht werden, da es noch Ausgeliehen wird!";
                    break;
                case 5:
                    statusMessage = "Es existieren keine vorhandenen Items oder ein Fehler ist in der Datenbank aufgetaucht";
                    break;
                case 6:
                    statusMessage = "Objekt mit dieser Id existiert nicht!";
                    break;
                case 7:
                    statusMessage = "Zeitraum des Verleihs überschneidet sich mit dem eines anderen!";
                    break;
            }
            return statusMessage;
        }

        internal DateTime GetLastDate(uint id)
        {
            var res = DbConn.GetAllBorrowsWithThisItem(id);

            var lastDate = DateTime.Now;

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];

                var list = FillBorrowList(paraList);

                lastDate = Convert.ToDateTime(list[0].Availability);

                foreach (var borrow in list)
                {
                    var listDate = Convert.ToDateTime(borrow.Availability);
                    if (listDate > lastDate)
                    {
                        lastDate = listDate;
                    }
                }
            }
            return lastDate;
        }

        public List<ItemModel> FillItemList(List<object> paraList)
        {
            var listProps = new List<string>();
            var ItemList = new List<ItemModel>();
            foreach (var item in paraList)
            {
                if (int.TryParse(item.ToString(), out int result) && listProps.Count > 0)
                {
                    ItemList.Add(new ItemModel(Convert.ToUInt32(listProps[0]), listProps[1], listProps[2]));
                    listProps = new List<string>();
                    listProps.Add(item.ToString());
                }
                else if (int.TryParse(item.ToString(), out int res))
                {
                    listProps.Add(item.ToString());
                }
                else
                {
                    listProps.Add(item.ToString());
                }
            }
            ItemList.Add(new ItemModel(Convert.ToUInt32(listProps[0]), listProps[1], listProps[2]));
            return ItemList;
        }

        public List<BorrowModel> FillBorrowList(List<object> paraList)
        {
            var listProps = new List<string>();
            var BorrowList = new List<BorrowModel>();
            var isItemId = false;
            foreach (var item in paraList)
            {
                if (int.TryParse(item.ToString(), out int result) && listProps.Count > 0)
                {
                    if (!isItemId)
                    {
                        isItemId = true;
                        listProps.Add(item.ToString());
                    }
                    else
                    {
                        isItemId = false;
                        BorrowList.Add(new BorrowModel(Convert.ToUInt32(listProps[0]), listProps[1],
                        listProps[2], ConvertDateTime(listProps[3]), ConvertDateTime(listProps[4]), Convert.ToUInt32(listProps[5])));

                        listProps = new List<string>();
                        listProps.Add(item.ToString());
                    }
                }
                else if (int.TryParse(item.ToString(), out int res))
                {
                    listProps.Add(item.ToString());
                }
                else
                {
                    listProps.Add(item.ToString());
                }
            }
            if (listProps.Count > 0)
            {
                BorrowList.Add(new BorrowModel(Convert.ToUInt32(listProps[0]), listProps[1],
                        listProps[2], ConvertDateTime(listProps[3]), ConvertDateTime(listProps[4]), Convert.ToUInt32(listProps[5])));
            }
            
            return BorrowList;
        }

        public List<UserModel> FillUserList(List<object> paraList)
        {
            var listProps = new List<string>();
            var ItemList = new List<UserModel>();
            foreach (var item in paraList)
            {
                if (int.TryParse(item.ToString(), out int result) && listProps.Count > 0)
                {
                    ItemList.Add(new UserModel(Convert.ToUInt32(listProps[0]), listProps[1], Convert.ToBoolean(listProps[2]), Convert.ToBoolean(listProps[3])));
                    listProps = new List<string>();
                    listProps.Add(item.ToString());
                }
                else if (int.TryParse(item.ToString(), out int res))
                {
                    listProps.Add(item.ToString());
                }
                else
                {
                    listProps.Add(item.ToString());
                }
            }
            if (listProps.Count > 0)
            {
                ItemList.Add(new UserModel(Convert.ToUInt32(listProps[0]), listProps[1], Convert.ToBoolean(listProps[2]),
                    Convert.ToBoolean(listProps[3])));
            }
            return ItemList;
        }

        private ItemModel GetItemFromList(uint id)
        {
            var correctItem = new ItemModel();
            foreach (var item in ItemList)
            {
                if(item.Id == id)
                {
                    correctItem = item;
                }
            }
            return correctItem;
        }

        /*public bool ExistItems()
        {
            var res = DbConn.GetAllItems();
            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];
                if (paraList.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }*/

        public ItemModel GetItem(uint id)
        {
            var resList = DbConn.GetItem(id);
            var paraList = (List<object>)resList[1];
            var item = new ItemModel();

            if (paraList.Count > 0)
            {
                item = new ItemModel(Convert.ToUInt32(paraList[0]), paraList[1].ToString(), paraList[2].ToString());
            }
            return item;
        }

        public UserModel GetUser(uint id)
        {
            var resList = DbConn.GetUser(id);
            var paraList = (List<object>)resList[1];
            var user = new UserModel();

            if (paraList.Count > 0)
            {
                user = new UserModel(Convert.ToUInt32(paraList[0]), paraList[1].ToString(), Convert.ToBoolean(paraList[2]), Convert.ToBoolean(paraList[3]));
            }
            return user;
        }

        public BorrowModel GetBorrow(uint id)
        {
            var resList = DbConn.GetBorrow(id);
            var paraList = (List<object>)resList[1];
            var borrow = new BorrowModel();

            if(paraList.Count > 0)
            {
                borrow = new BorrowModel(Convert.ToUInt32(paraList[0]), paraList[1].ToString(), paraList[2].ToString(), ConvertDateTime(paraList[3].ToString()), ConvertDateTime(paraList[4].ToString()), Convert.ToUInt32(paraList[5]));
            }
            return borrow;
        }

        public string ConvertDateTime(string dateTime)
        {
            var date = Convert.ToDateTime(dateTime);
            return date.ToString("dd-MM-yyyy").Replace("-", ".");
        }

        private string CreateUserName(string name)
        {
            string username = string.Empty;
            for (int i = name.Length - 1; i > 0; i--)
            {
                if (name[i] == '\\')
                {
                    break;
                }
                username += name[i];
            }
            return ReverseString(username);
        }

        private string ReverseString(string reverse)
        {
            char[] charArray = reverse.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public List<BorrowModel> GetListOfBorrowsWithItemId(uint itemId)
        {
            var res = DbConn.GetAllBorrowsWithThisItem(itemId);
            var list = new List<BorrowModel>();

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];
                list = FillBorrowList(paraList);
            }

            return list;
        }

        public bool IsItemTodayBorrowed(uint itemId)
        {
            var list = GetListOfBorrowsWithItemId(itemId);
            foreach (var borrow in list)
            {
                if(DateTime.Today >= Convert.ToDateTime(borrow.BorrowDate) && DateTime.Today <= Convert.ToDateTime(borrow.Availability))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsItemInBorrows(uint itemId) 
        {
            var list = GetListOfBorrowsWithItemId(itemId);

            if(list.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool ItemCanBeBorrowInThisPeriod(uint itemId, string borrowDate, string availableDate)
        {
            var res = DbConn.GetAllBorrowsWithThisItem(itemId);

            if(res[0].ToString() == "Success")
            {
                var paraList = (List<object>)res[1];

                BorrowList = FillBorrowList(paraList);

                foreach (var borrow in BorrowList)
                {
                    if(CheckEveryDateInPeriodIsInOtherPeriod(Convert.ToDateTime(borrowDate), Convert.ToDateTime(availableDate), Convert.ToDateTime(borrow.BorrowDate), Convert.ToDateTime(borrow.Availability)))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool CheckIfDateInThisPeriod(DateTime checkedDate, DateTime start, DateTime end)
        {
            if(checkedDate > start && checkedDate < end)
            {
                return true;
            }
            return false;
        }

        private bool CheckEveryDateInPeriodIsInOtherPeriod(DateTime newBorrowDate, DateTime newAvailableDate, DateTime borrowDate, DateTime availableDate)
        {
            var checkedDate = newBorrowDate;
            while (checkedDate != newAvailableDate)
            {
                if(CheckIfDateInThisPeriod(checkedDate, borrowDate, availableDate))
                {
                    return true;
                }
                checkedDate = checkedDate.AddDays(1);
            }
            return false;
        }
    }
}
