using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class BookCopy
    {
        public int BookCopyId { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public virtual Book RentedBy { get; }
        public virtual Book Book { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; }

        protected BookCopy() { }

        public override string ToString()
        {
            return "copy: " + Book.Title.ToString();
        }
    }
}