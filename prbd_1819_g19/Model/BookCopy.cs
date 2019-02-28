using System;

namespace prbd_1819_g19
{
    public class BookCopy
    {
        public int BookCopyId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public User RentedBy { get; }
    }
}