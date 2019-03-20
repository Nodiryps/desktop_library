using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class User : EntityBase<Model>
    {
        [Required]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public RentalItem ActiveRentalItem { get; }
        public int Age { get; }
        public Rental Basket { get; }

        public Rental CreateBasket()
        {
            Rental newRental = Model.Rentals.Create();
            newRental.RentalDate = DateTime.Now;
            //var query = from rentalItem in Model.RentalItems
            //            where rentalItem.RentalItemId ==

            return newRental;
        }

        public RentalItem AddToBasket(Book book)
        {
            RentalItem newItem = null;
            if (book.NumAvailableCopies > 0)
            {
                //var bookCopy =  from copy in Model.BookCopies
                //                where book.BookId == copy.Book.BookId 
                //                select copy;

                Model.RentalItems.Create();
                newItem.RentalItemId = book.BookId;
                newItem.ReturnDate = null;
            }
            return newItem;
        }

        private bool IsBasketFull()
        {
            return Basket.Items.Count == 5;
        }

        public void RemoveFromBasket(RentalItem item)
        {
            Basket.RemoveItem(item);
        }

        public void ClearBasket()
        {
            Basket.Clear();
        }

        public void ConfirmBasket()
        {
            MsgConfirmBasket();
          
            foreach (RentalItem item in Basket.Items)
            {
                Rental rental = Model.Rentals.Create();
                rental.RentalId = item.RentalItemId;
                rental.RentalDate = DateTime.Now;
            }
        }

        private void MsgConfirmBasket()
        {
            if (Basket.IsEmpty())
                Console.WriteLine("Empty Basket !");
        }

        public void Return(BookCopy copy)
        {
            foreach(RentalItem item in Basket.Items)
            {
                if (item.RentalItemId == copy.BookCopyId)
                    item.ReturnDate = DateTime.Now;
            }
        }
    }
}
