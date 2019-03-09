using System;
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