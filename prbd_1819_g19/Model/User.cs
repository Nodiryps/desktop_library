using System;

namespace prbd_1819_g19
{
    public class User
    {
        public int UserId { get; set; }
        public string UsernName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Role Role { get; set; }
        public RentalItem ActiveRentalItem { get; }
        public int Age { get; }

        public Rental CreateBasket()
        {

        }

        public void AddToBasket(Book book)
        {

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
