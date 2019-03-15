using System;
using System.ComponentModel.DataAnnotations;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class RentalItem : EntityBase<Model>
    {
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual BookCopy bookCopy { get; set; }

        public RentalItem(int id, DateTime? returnDate)
        {
            RentalItemId = id;
            ReturnDate = returnDate;
        }

        public void DoReturn()
        {
            ReturnDate = DateTime.Now;
        }

        public void CancelReturn()
        {
            ReturnDate = null;
        }
    }
}