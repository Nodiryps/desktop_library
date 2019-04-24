using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;
using System.Linq;

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
        public virtual ICollection<Rental> Rentals { get; set; }
        public Rental Basket
        {
            get => (from rental in Model.Rentals
                    where rental.User.UserId == UserId && rental.RentalDate == null
                    select rental).FirstOrDefault();
        }


        public Rental CreateBasket()
        {
            Rental newRental = Model.Rentals.Create();
            newRental.User = this;
            Model.Rentals.Add(newRental);
            Rentals.Add(newRental);

            //Console.WriteLine(newRental);
            //Console.WriteLine(Rentals.Count);
            //Console.Read();

            return newRental;
        }

        public RentalItem AddToBasket(Book book)
        {
            Rental newRental = CreateBasket();
            newRental.User = this;

            BookCopy copy = book.GetAvailableCopy();
            RentalItem item = newRental.RentCopy(copy);

            newRental.Items.Add(item);

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
            Model.SaveChanges();
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
            foreach (RentalItem item in Basket.Items)
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
