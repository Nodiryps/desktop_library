using System;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class RentalItem : EntityBase<Model>
    {
        [Key]
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual BookCopy BookCopy { get; set; }
        public virtual Rental Rental { get; set; }

        protected RentalItem(){}/////////////////////////////CONSTRUCT/////////////////////////////

        public void DoReturn()
        {
            ReturnDate = DateTime.Now;
            //Rental.RemoveItem(this);
        }

        public void CancelReturn()
        {
            ReturnDate = null;
        }

        public override string ToString()
        {
            return BookCopy.Book.Title;
        }
    }
}