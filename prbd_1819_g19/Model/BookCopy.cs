using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class BookCopy
    {
        [Key]
        public int BookCopyId { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public virtual User RentedBy { get; }
        public virtual Book Book { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; }= new HashSet<RentalItem>();

        protected BookCopy(){}/////////////////////////////CONSTRUCT/////////////////////////////

        public override string ToString()
        {
            return Book.Title.ToString();
        }
    }
}