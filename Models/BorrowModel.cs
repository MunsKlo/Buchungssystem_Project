using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buchungsystem_WindowsAuth.Models
{
    public class BorrowModel
    {
        public uint Id { get; set; }
        public string Distributor { get; set; }
        public string Borrower { get; set; }
        public string BorrowDate { get; set; }
        public string Availability { get; set; }
        public uint ItemId { get; set; }

        public BorrowModel(string _distributor, string _borrower, string _borrowDate, string _availability, uint _itemId)
        {
            Distributor = _distributor;
            Borrower = _borrower;
            BorrowDate = _borrowDate;
            Availability = _availability;
            ItemId = _itemId;
        }
        public BorrowModel(uint _id, string _distributor, string _borrower, string _borrowDate, string _availability, uint _itemId)
        {
            Id = _id;
            Distributor = _distributor;
            Borrower = _borrower;
            BorrowDate = _borrowDate;
            Availability = _availability;
            ItemId = _itemId;
        }

        public BorrowModel()
        {

        }
    }
}
