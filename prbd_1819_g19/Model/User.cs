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

        protected User(string userName, string password, string fullName, string email, DateTime? birthDate, Role role)
        {
            UserName = userName;
            Password = password;
            FullName = fullName;
            Email = email;
            BirthDate = birthDate;
            Role = role;
        }

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
            if (book.NumAvailableCopies >= 1)
            {
                var bookCopy = from copy in Model.BookCopies
                            where copy.BookCopyId == book.BookId 
                            select copy;

                Model.RentalItems.Create();
                newItem.RentalItemId = book.BookId;
                newItem.ReturnDate = null;
            }
            return newItem;
        }

        private bool IsBasketFull()
        {
            return Basket.ListRental.Count == 5;
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
          
            foreach (RentalItem item in Basket.ListRental)
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
            foreach(RentalItem item in Basket.ListRental)
            {
                if (item.RentalItemId == copy.BookCopyId)
                    item.ReturnDate = DateTime.Now;
            }
        }
    }
}
