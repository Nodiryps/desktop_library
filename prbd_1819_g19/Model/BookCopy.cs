using System;
using System.ComponentModel.DataAnnotations;

namespace prbd_1819_g19
{
    public class BookCopy
    {
        [Key]
        public int BookCopyId { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public User RentedBy { get; }

        public BookCopy(int id, User rentedBy)
        {
            BookCopyId = id;
            AcquisitionDate = DateTime.Now;
            RentedBy = rentedBy;
        }
    }
}