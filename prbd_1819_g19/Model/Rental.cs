using System;

namespace prbd_1819_g19
{
    public class Rental
    {
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems { get; }

        public Rental(int id, DateTime? date)
        {
            RentalId = id;
            RentalDate = date;
        }

        public RentalItem RentCopy(BookCopy copy)
        {

        }

        public void RemoveCopy(BookCopy copy)
        {

        }

        public void RemoveItem(RentalItem item)
        {

        }

        public void Return(RentalItem item)
        {

        }

        public void Confirm()
        {

        }

        public void Clear()
        {

        }
    }
}