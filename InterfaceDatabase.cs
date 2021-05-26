using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Buchungsystem_WindowsAuth.Models;
using System.Data;

namespace Buchungsystem_WindowsAuth
{
    public class InterfaceDatabase
    {
        string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\GITHUB\REPOS\BUCHUNGSYSTEM_WINDOWSAUTH\BUCHUNGDB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private readonly string tableItems  = "Items";
        private readonly string tableBorrorws = "Borrows";
        private readonly string tableUsers = "Users";
        public string CreateNewItem(ItemModel item)
        {
            var query = $"insert into Items (Name, Beschreibung) " +
                    $"values ('{ item.Name}', '{item.Description}')";

            return DoManipulateQuery(query);
        }

        public List<object> GetAllItems()
        {
            var query = $"select * from {tableItems}";
            return GetData(query);
        }

        public List<object> GetAllSearchedItems(string searchString)
        {
            var query = $"select * from {tableItems} where Name like '%{searchString}%' or Beschreibung like '%{searchString}%'";
            return GetData(query);
        }

        public string ChangeItemInDb(ItemModel newItem)
        {
            var query = $"update {tableItems} set Name='{newItem.Name}', Beschreibung='{newItem.Description}' where ItemId={newItem.Id}";

            return DoManipulateQuery(query);
        }

        public string DeleteItem(uint id)
        {
            var query = $"delete from {tableItems} where ItemId={id}";

            return DoManipulateQuery(query);
        }

        public List<object> GetItem(uint id)
        {
            var query = $"select * from {tableItems} where ItemId={id}";
            return GetData(query);
        }

        public List<object> GetAllBorrows()
        {
            var query = $"select * from {tableBorrorws}";
            return GetData(query);
        }

        public List<object> GetAllBorrowsWithThisItem(uint id)
        {
            var query = $"select * from {tableBorrorws} where Item={id}";
            return GetData(query);
        }

        public List<object> GetAllBorrowsWithBorrowDate()
        {
            var query = $"select * from {tableBorrorws} where MONTH(Ausleihdatum) = MONTH(GETDATE())";
            return GetData(query);
        }

        public List<object> GetAllBorrowsWithAvailableDate()
        {
            var query = $"select * from {tableBorrorws} where MONTHV(Verfügbarkeitsdatum) = MONTH(GETDATE())";
            return GetData(query);
        }

        public string CreateNewBorrow(BorrowModel borrow)
        {
            var query = $"insert into {tableBorrorws} (Verleiher, Ausleiher, Ausleihdatum, Verfügbarkeitsdatum, Item) " +
                    $"values ('{ borrow.Distributor}', '{borrow.Borrower}', '{borrow.BorrowDate}', '{borrow.Availability}', '{borrow.ItemId}')";

            return DoManipulateQuery(query);
        }

        public string DeleteBorrow(uint id)
        {
            var query = $"delete from {tableBorrorws} where AusleihId={id}";

            return DoManipulateQuery(query);
        }

        public List<object> GetBorrow(uint id)
        {
            var query = $"select * from {tableBorrorws} where AusleihId={id}";
            return GetData(query);
        }

        public string ChangeBorrowInDb(BorrowModel newBorrow)
        {
            var query = $"update {tableBorrorws} set Verleiher='{newBorrow.Distributor}', Ausleiher='{newBorrow.Borrower}', Ausleihdatum='{newBorrow.BorrowDate}'," +
                $" Verfügbarkeitsdatum='{newBorrow.Availability}', Item={newBorrow.ItemId} where AusleihId={newBorrow.Id}";

            return DoManipulateQuery(query);
        }

        public List<object> GetUser(string name)
        {
            var query = $"select * from {tableUsers} where Name='{name}'";
            return GetData(query);
        }

        public List<object> GetUser(uint id)
        {
            var query = $"select * from {tableUsers} where UserId={id}";
            return GetData(query);
        }

        public string CreateUser(string name)
        {
            var query = $"insert into {tableUsers} (Name, Ersteller, Admin) values '{name}', 'False', 'False'";
            return DoManipulateQuery(query);
        }

        public List<object> GetAllUsers()
        {
            var query = $"select * from {tableUsers}";
            return GetData(query);
        }

        public List<object> GetAllSearchedUsers(string searchString)
        {
            var query = $"select * from {tableUsers} where Name like '%{searchString}%'";
            return GetData(query);
        }

        public string ChangeUserCreator(uint id, bool changeStatus)
        {
            var query = $"update {tableUsers} set Ersteller='{changeStatus}' where UserId={id}";
            return DoManipulateQuery(query);
        }

        private string DoManipulateQuery(string query)
        {
            var statusMessage = string.Empty;
            try
            {
                using (var connection = new SqlConnection(conn))
                {


                    var command = new SqlCommand(query, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
                statusMessage = "Success";
            }
            catch(Exception e)
            {
                statusMessage = "Failed";
            }
            return statusMessage;
        }

        private List<object> GetData(string query)
        {
            var resList = new List<object>();
            var objList = new List<object>();
            object res;
            try
            {
                using (var connection = new SqlConnection(conn))
                {
                    
                    using var command = new SqlCommand(query, connection);
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            objList.Add(reader[i]);
                        }
                    }
                    reader.Close();
                    res = objList;
                }
                resList.Add("Success");
                resList.Add(res);
            }
            catch (Exception e)
            {
                resList.Add("Fail");
            }
            return resList;
        }
    }
}
