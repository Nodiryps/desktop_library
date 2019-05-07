using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PRBD_Framework;

namespace prbd_1819_g19
{
    public class Book : EntityBase<Model>
    {
        [Required]
        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        [NotMapped]
        public string AbsolutePicturePath
        {
            get => PicturePath != null ? App.IMAGE_PATH + "\\" + PicturePath : null; 
        }

        public int NumAvailableCopies { get => Copies.Count(); }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<BookCopy> Copies { get; set; }

        public void AddCategory(Category category)
        {
            if (!Categories.Contains(category))
                Categories.Add(category);

        }

        public void AddCategories(Category[] tab)
        {
            foreach (Category category in tab)
                AddCategory(category);
        }

        public void RemoveCategory(Category category)
        {
            if (Categories.Contains(category))
                Categories.Remove(category);
        }

        public void AddCopies(int quantity, DateTime date)
        {
            for (int i = 0; i < quantity; ++i)
            {
                BookCopy copy = Model.BookCopies.Create();
                copy.Book = this;
                copy.AcquisitionDate = date;
                Copies.Add(copy);

                Model.BookCopies.Add(copy);
                Model.SaveChanges();
            }
        }

        public BookCopy GetAvailableCopy()
        {
            return (
                    from copy in Model.BookCopies
                    where copy.Book.BookId == this.BookId &&
                        (from item in copy.RentalItems
                         where item.ReturnDate == null
                         select item
                        ).Count() == 0
                    select copy
            ).FirstOrDefault();

        }

        public void DeleteCopy(BookCopy copy)
        {
            Console.Write(copy);
            if (copy != null)
                Model.BookCopies.Remove(copy);
            Copies.Remove(copy);
            Model.SaveChanges();
        }

        public void Delete()
        {
            if (Model.Books.Find(BookId) != null)
            {
                Model.Books.Remove(this);
                Model.SaveChanges();
            }
        }

        public override string ToString()
        {
            return "book: " + Title.ToString();
        }
    }
}

