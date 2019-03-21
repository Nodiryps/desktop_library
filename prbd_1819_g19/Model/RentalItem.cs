using System;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class RentalItem : EntityBase<Model>
    {
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual BookCopy BookCopy { get; set; }

        public void DoReturn()
        {
            ReturnDate = DateTime.Now;
        }

        public void CancelReturn()
        {
            ReturnDate = null;
        }

        public override string ToString()
        {
            return "item:" + BookCopy.Book.Title.ToString();
        }
    }
}