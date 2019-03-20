using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace prbd_1819_g19
{
    public class BookCopy
    {
        public int BookCopyId { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public User RentedBy { get; }

        public virtual Book Book { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; }
    }
}