using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class BookCopy : EntityBase<Model>
    {
        [Key]
        public int BookCopyId { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public virtual User RentedBy { get => (from ri in Model.RentalItems
                                               where ri.BookCopy != null && ri.Rental.RentalDate != null && ri.Rental != null 
                                               select ri.Rental.User).FirstOrDefault(); }
        public virtual Book Book { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; }= new HashSet<RentalItem>();

        protected BookCopy(){}/////////////////////////////CONSTRUCT/////////////////////////////

        public override string ToString()
        {
            return Book.Title.ToString();
        }
    }
}