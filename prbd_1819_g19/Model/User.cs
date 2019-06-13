using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PRBD_Framework;
using System.Linq;
using System.Security.Principal;

namespace prbd_1819_g19
{
    public class User : EntityBase<Model>
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public virtual RentalItem[] ActiveRentalItem { get; }
        public int Age { get => DateTime.Now.Year - BirthDate.Value.Year; }
        public virtual ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();
        public Rental Basket
        {
            get => (from rental in Model.Rentals
                    where rental.User.UserId == UserId && rental.RentalDate == null
                    select rental).FirstOrDefault();
        }

        protected User(){}////////////////////////////////////CONSTRUCT////////////////////////////////////


        public Rental CreateBasket()
        {
            Rental newRental = Model.Rentals.Create();
            newRental.User = this;
            Model.Rentals.Add(newRental);
            //Rentals.Add(newRental);

            Model.SaveChanges();
            return newRental;
        }
        
        public RentalItem AddToBasket(Book book)
        {
            Console.WriteLine("test");
            if (Basket == null)
                CreateBasket(); // si pas de rental.rentalDate à null, on en créé un nvx => rentalDate == null

            RentalItem ri = Model.RentalItems.Create();
            BookCopy copy = book.GetAvailableCopy();

            if (copy != null)
                Basket.RentCopy(copy);
                Model.SaveChanges();
            return ri;
        }

        public void RemoveFromBasket(RentalItem item)
        {
            if (Basket != null)
                Basket.RemoveItem(item);
        }

        public void ClearBasket()
        {
            if (Basket != null)
                Basket.Clear();
        }

        public void ConfirmBasket()
        {
            if (Basket != null)
                Basket.Confirm();
        }

        public void Return(BookCopy copy)
        {
            RentalItem ri = copy.RentalItems.FirstOrDefault( rental => rental.ReturnDate == null );
            if(ri != null)
            {
                ri.ReturnDate = DateTime.Now;
                Model.SaveChanges();
            }
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}
