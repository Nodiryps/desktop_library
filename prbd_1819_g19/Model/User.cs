using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class User : EntityBase<Model>
    {
        protected User(string userName, string password, string fullName, string email, DateTime? birthDate, Role role)
        {
            UserName = userName;
            Password = password;
            FullName = fullName;
            Email = email;
            BirthDate = birthDate;
            Role = role;
        }

        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public RentalItem ActiveRentalItem { get; }
        public int Age { get; }
        public virtual ICollection<RentalItem> Basket { get; set; }

        public Rental CreateBasket()
        {
            Rental newRental = Model.Rentals.Create();
            newRental.RentalDate = DateTime.Now;
            return newRental;
        }

        public RentalItem AddToBasket(Book book)
        {
            RentalItem newItem = null;
            if (!IsBasketFull() && book.NumAvailableCopies >= 1)
            {
                Model.RentalItems.Create();
                newItem.RentalItemId = book.BookId;
                newItem.ReturnDate = null;
                Basket.Add(newItem);
            }
            return newItem;
        }

        private bool IsBasketEmpty()
        {
            return Basket.Count == 0;
        }

        private bool IsBasketFull()
        {
            return Basket.Count == 5;
        }

        private bool MaxRents()
        {
            int cpt = 0;
            foreach (Rental r in Model.Rentals)
                ++cpt;
            return Basket.Count + cpt  > 5;
        }

        public void RemoveFromBasket(RentalItem item)
        {
            if (!IsBasketEmpty())
                Basket.Remove(item);
        }

        public void ClearBasket()
        {
            if (!IsBasketEmpty())
                Basket.Clear();
        }

        public void ConfirmBasket()
        {
            MsgConfirmBasket();

            if (!IsBasketEmpty() && !MaxRents())
            {
                foreach (RentalItem item in Basket)
                {
                    Rental rental = Model.Rentals.Create();
                    rental.RentalId = item.RentalItemId;
                    rental.RentalDate = DateTime.Now;
                }
            }            
        }

        private void MsgConfirmBasket()
        {
            if (IsBasketEmpty())
                Console.WriteLine("Empty Basket !");
            if (MaxRents())
                Console.WriteLine("You already have 5 rents !");
        }

        public void Return(BookCopy copy)
        {
            foreach(RentalItem item in Basket)
            {
                if (item.RentalItemId == copy.BookCopyId)
                    item.ReturnDate = DateTime.Now;
            }
        }
    }
}
