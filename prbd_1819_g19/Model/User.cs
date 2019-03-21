using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

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
        public virtual RentalItem[] ActiveRentalItem { get; }
        public int Age { get => DateTime.Now.Year - BirthDate.Value.Year; }

        public Rental Basket { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }

        public Rental CreateBasket()
        {
            Basket = Model.Rentals.Create();
            Model.Rentals.Add(Basket);
            return Basket;
        }

        public RentalItem AddToBasket(Book book)
        {
            CreateBasket();
            RentalItem item = null;
            if (book.NumAvailableCopies > 0)
            {
                item = Model.RentalItems.Create();
                BookCopy copy = book.GetAvailableCopy();
                item.BookCopy = copy;
                Basket.Items.Add(item);
                Model.RentalItems.Add(item);
            }
            Model.SaveChanges();
            return item;
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
            MsgErrConfirmBasket();
            Rental rental = Model.Rentals.Create();

            foreach (RentalItem item in Basket.Items)
            {
                rental.RentalId = item.RentalItemId;
                rental.RentalDate = DateTime.Now;
            }
            Model.Rentals.Add(rental);
            Rentals.Add(rental);
        }

        private void MsgErrConfirmBasket()
        {
            if (Basket.IsEmpty())
                Console.WriteLine("!!! EMPTY Basket !!!");
            if (Basket.IsFull())
                Console.WriteLine("!!! Basket FULL !!!");
        }

        public void Return(BookCopy copy)
        {
            foreach(RentalItem item in Basket.Items)
            {
                if (item.RentalItemId == copy.BookCopyId)
                    item.ReturnDate = DateTime.Now;
            }
        }

        public override string ToString()
        {
            return "username: " + UserName.ToString();
        }
    }
}
