using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour RentalBy.xaml
    /// </summary>
    public partial class RentalBy : UserControlBase
    {
        private ObservableCollection<CheckboxList<Book>> booksCheckboxList;
        public ObservableCollection<CheckboxList<Book>> BooksCheckboxList
        {
            get => booksCheckboxList;
            set => SetProperty<ObservableCollection<CheckboxList<Book>>>(ref booksCheckboxList, value);
        }

        private ObservableCollection<Rental> rentals;
        public ObservableCollection<Rental> Rentals
        {
            get => rentals;
            set => SetProperty<ObservableCollection<Rental>>(ref rentals, value);
        }
        
        private ObservableCollection<Book> books;
        public ObservableCollection<Book> Books
        {
            get => books;
            set => SetProperty<ObservableCollection<Book>>(ref books, value);
        }

        public Rental SelectedRental { get; set; }
        public ICommand SetBooks { get; set; }

        public RentalBy()   //////////////////////////////////////CONSTRUCT()//////////////////////////////////////
        {
            InitializeComponent();
            DataContext = this;
            Rentals = new ObservableCollection<Rental>(FillRentals());

            Books = new ObservableCollection<Book>(App.Model.Books);
            
            AddBooksUnchecked();

            SetBooks = new RelayCommand<Rental>(rental =>
            {
                if (SelectedRental != null)
                {
                    BooksCheckboxList.Clear();
                    FillBooksItems();
                }
            });

            App.Register(this, AppMessages.MSG_CONFIRM_BASKET, () => {
                Rentals = new ObservableCollection<Rental>(FillRentals());
            });

        }  //////////////////////////////////////CONSTRUCT()//////////////////////////////////////

        private void FillBooksItems()
        {
            var query = (from ri in App.Model.RentalItems
                         where ri.Rental.RentalId == SelectedRental.RentalId
                         select ri.BookCopy.Book).Distinct().ToList();
            foreach(var b in App.Model.Books)
                if (query.Contains(b))
                    BooksCheckboxList.Add(new CheckboxList<Book>(b, true));
                else
                    BooksCheckboxList.Add(new CheckboxList<Book>(b, false));
        }

        private List<Rental> FillRentals()
        {
            return (from r in App.Model.Rentals
                    where r.RentalDate != null
                    select r).ToList();
            //return (from r in App.Model.RentalItems //////////////// autre façon de faire
            //        where r.Rental.RentalDate != null
            //        select r.Rental).Distinct().ToList();
        }

        private void AddBooksUnchecked()
        {
            BooksCheckboxList = new ObservableCollection<CheckboxList<Book>>();
            foreach (var b in Books)
                BooksCheckboxList.Add(new CheckboxList<Book>(b, false));
        }
        
        public class CheckboxList<T> : INotifyPropertyChanged ////////////////////////////////////INNER CLASS////////////////////////////////////
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool isChecked;
            private T item;

            public CheckboxList()
            { }

            public CheckboxList(T item, bool isChecked = false)
            {
                this.item = item;
                this.isChecked = isChecked;
            }

            public T Item
            {
                get { return item; }
                set
                {
                    item = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
                }
            }


            public bool IsChecked
            {
                get { return isChecked; }
                set
                {
                    isChecked = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        } ////////////////////////////////////INNER CLASS////////////////////////////////////
    }
}
