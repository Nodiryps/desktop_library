using System;
using System.Collections.Generic;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class User : EntityBase<Model>
    {
        public User(string userName, string password, string fullName, string email, DateTime? birthDate, Role role)
        {
            UserName = userName;
            Password = password;
            FullName = fullName;
            Email = email;
            BirthDate = birthDate;
            Role = role;
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public RentalItem ActiveRentalItem { get; }
        public int Age { get; }
        public List<RentalItem> Basket = new List<RentalItem>();

        //public Rental CreateBasket()
        //{

        //}

        public List<RentalItem> GetBasket()
        {
            return Basket;
        }

        public RentalItem AddToBasket(Book book)
        {

            Basket.Add(Book);
        }

        public void RemoveFromBasket(RentalItem item)
        {

        }

        public void ClearBasket()
        {

        }

        public void ConfirmBasket()
        {

        }

        public void Return(BookCopy copy)
        {

        }
    }
}
