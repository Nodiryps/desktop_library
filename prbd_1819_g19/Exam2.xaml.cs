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
    public partial class Exam2 : UserControlBase
    {
        private ObservableCollection<RentalItem> rentalItems;
        public ObservableCollection<RentalItem> RentalItems
        {
            get => rentalItems;
            set => SetProperty<ObservableCollection<RentalItem>>(ref rentalItems, value);
        }

        private ObservableCollection<CheckboxList<Book>> booksCheckboxList;
        public ObservableCollection<CheckboxList<Book>> BooksCheckboxList
        {
            get => booksCheckboxList;
            set => SetProperty<ObservableCollection<CheckboxList<Book>>>(ref booksCheckboxList, value);
        }

        public Book SelectedBook {get; set;}
        public ICommand SetItems { get; set; }

        //////////////////////////////////////CONSTRUCT()//////////////////////////////////////
        public Exam2() 
        {
            InitializeComponent();
            DataContext = this;

            FillBooks();
            
            SetItems = new RelayCommand<CheckboxList<Book>>(list => {
                Console.WriteLine(SelectedBook);
                GetItems();
            });

        }
        //////////////////////////////////////CONSTRUCT()//////////////////////////////////////

        private void GetItems()
        {
            RentalItems = new ObservableCollection<RentalItem>();

            var query = (from ri in App.Model.RentalItems
                         where ri.BookCopy.Book.BookId == SelectedBook.BookId
                         select ri).ToList();
            RentalItems = new ObservableCollection<RentalItem>(query);
        }

        private void FillBooks()
        {
            BooksCheckboxList = new ObservableCollection<CheckboxList<Book>>();

            foreach (var b in App.Model.Books)
                BooksCheckboxList.Add(new CheckboxList<Book>(b, false));
        }

        ////////////////////////////////////INNER CLASS////////////////////////////////////
        public class CheckboxList<T> : INotifyPropertyChanged 
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
        } 
        ////////////////////////////////////INNER CLASS////////////////////////////////////
    }
}
