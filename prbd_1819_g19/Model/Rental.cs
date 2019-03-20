using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Rental : EntityBase<Model>
    {
        [Required]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems { get; }
        public virtual ICollection<RentalItem> Items { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Rental(int id, DateTime? date)
        {
            RentalId = id;
            RentalDate = date;
        }

        public RentalItem RentCopy(BookCopy copy)
        {
            var rentalItem = (from item in Model.RentalItems
                             where copy.BookCopyId == item.BookCopy.BookCopyId
                                && NumOpenItems > 0
                             select item).FirstOrDefault();
            Items.Add(rentalItem);
            return rentalItem;
        }

        public void RemoveCopy(BookCopy copy)
        {
            var rentalItem = (from item in Model.RentalItems
                              where copy.BookCopyId == item.BookCopy.BookCopyId
                                 && NumOpenItems > 0
                              select item).FirstOrDefault();
            Items.Remove(rentalItem);
        }

        public void RemoveItem(RentalItem item)
        {
            if (!IsEmpty())
                Items.Remove(item);
        }

        public bool IsEmpty()
        {
            return Items.Count == 0;
        }

        public void Return(RentalItem item)
        {
            item.ReturnDate = DateTime.Now;
        }

        public void Confirm()
        {
            
        }

        public void Clear()
        {
            if (!IsEmpty())
                Items.Clear();
        }
    }
}