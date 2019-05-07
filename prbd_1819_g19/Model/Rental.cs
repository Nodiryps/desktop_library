using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class Rental : EntityBase<Model>
    {
        [Required]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems
        {
            get => (from item in Model.RentalItems
                    where item.ReturnDate == null
                    select item).Count();
        }
        public virtual ICollection<RentalItem> Items { get; set; }
        public virtual User User { get; set; }

        public RentalItem RentCopy(BookCopy copy)
        {
            //var rentalItem = (
            //                 from item in Model.BookCopies
            //                 where copy.BookCopyId == item.
            //                    && NumOpenItems > 0
            //                 select item
            //                 ).FirstOrDefault();

            RentalItem item = null;
            if (copy != null)
            {
                item = Model.RentalItems.Create();
                //item.BookCopy.BookCopyId = copy.BookCopyId;
                item.ReturnDate = null;
                Items.Add(item);
                Model.SaveChanges();
            }
            
            return item;
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
            if (!IsEmpty() && Items.Contains(item))
                Items.Remove(item);
        }

        public void Return(RentalItem item)
        {
            item.DoReturn();
        }

        public void Confirm()
        {
            foreach (var item in Items)
                Model.RentalItems.Add(item);
            RentalDate = DateTime.Now;
            Model.Rentals.Add(this);
            Model.SaveChanges();
        }

        public void Clear()
        {
            if (!IsEmpty())
                Items.Clear();
        }

        public bool IsEmpty()
        {
            return Items.Count == 0;
        }

        public bool IsFull()
        {
            return Items.Count == 5;
        }

        public override string ToString()
        {
            return "rentalId: " + RentalId.ToString();
        }
    }
}