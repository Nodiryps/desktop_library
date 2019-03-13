using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Rental : EntityBase<Model>
    {
        [Key]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems { get; }
        public virtual ICollection<RentalItem> ListRental { get; set; }

        public Rental(int id, DateTime? date)
        {
            RentalId = id;
            RentalDate = date;
        }

        public RentalItem RentCopy(BookCopy copy)
        {
            RentalItem item = Model.RentalItems.Create();

            Model.Rentals.Add();
        }

        public void RemoveCopy(BookCopy copy)
        {

        }

        public void RemoveItem(RentalItem item)
        {
            if (!IsEmpty())
                ListRental.Remove(item);
        }

        public bool IsEmpty()
        {
            return ListRental.Count == 0;
        }

        public void Return(RentalItem item)
        {
            item.ReturnDate = DateTime.Now;
        }

        public void Confirm()
        {
            RentalDate = DateTime.Now;
        }

        public void Clear()
        {
            if (!IsEmpty())
                ListRental.Clear();
        }
    }
}