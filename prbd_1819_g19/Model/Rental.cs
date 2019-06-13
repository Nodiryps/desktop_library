using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class Rental : EntityBase<Model>
    {
        [Key]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems
        {
            get => (from item in Items
                    where item.ReturnDate == null
                    select item).Count();
        }
        public virtual ICollection<RentalItem> Items { get; set; } = new HashSet<RentalItem>();
        public virtual User User { get; set; }

        protected Rental(){}/////////////////////////////CONSTRUCT/////////////////////////////

        public RentalItem RentCopy(BookCopy copy)
        {
            RentalItem item = null;
            if (copy != null)
            {
                item = Model.RentalItems.Create();
                
                item.BookCopy = copy;
                item.ReturnDate = null;
                item.Rental = this;
                Items.Add(item);
                copy.RentalItems.Add(item);
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
            Model.BookCopies.Remove(copy);
            Model.SaveChanges();
        }

        public void RemoveItem(RentalItem item)
        {
            if (!ItemsIsEmpty() && Items.Contains(item))
                Items.Remove(item);
            Model.SaveChanges();
        }

        public void Return(RentalItem item)
        {
            item.DoReturn();
        }

        public void Confirm()
        {
            RentalDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void Clear()
        {
            if (!ItemsIsEmpty())
                Items.Clear();
        }

        public bool ItemsIsEmpty()
        {
            return Items.Count == 0;
        }

        public bool IsFull()
        {
            return Items.Count == 5;
        }

        public override string ToString()
        {
            return RentalId.ToString();
        }
    }
}