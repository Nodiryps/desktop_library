using Microsoft.Win32;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Input;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour BookDetailView.xaml
    /// </summary>
    public partial class BookDetailView : UserControlBase
    {

        private ImageHelper imageHelper;
        public User Member { get; set; }

        private ObservableCollection<CategoriesCheckboxList<Category>> checkboxList;
        public ObservableCollection<CategoriesCheckboxList<Category>> CheckboxList
        {
            get => checkboxList;
            set => SetProperty<ObservableCollection<CategoriesCheckboxList<Category>>>(ref checkboxList, value);
        }

        private ObservableCollection<Category> cats;
        public ObservableCollection<Category> Cats
        {
            get => cats;
            set => SetProperty<ObservableCollection<Category>>(ref cats, value);
        }

        private ObservableCollection<BookCopy> copies;
        public ObservableCollection<BookCopy> Copies
        {
            get => copies;
            set => SetProperty<ObservableCollection<BookCopy>>(ref copies, value);
        }


        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));
                RaisePropertyChanged(nameof(IsExisting));
            }
        }

        public bool IsExisting { get => !isNew; }


        public string PicturePath
        {
            get { return Book.AbsolutePicturePath; }
            set
            {
                Book.PicturePath = value;
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        private Book book;
        public Book Book
        {
            get => book;
            set => SetProperty<Book>(ref book, value);
        }

        public string ISBN
        {
            get { return book.Isbn; }
            set
            {
                book.Isbn = value;
                RaisePropertyChanged(nameof(ISBN));
                App.NotifyColleagues(AppMessages.MSG_ISBN_CHANGED, string.IsNullOrEmpty(value) ? "<new book>" : value);
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                RaisePropertyChanged(nameof(Quantity));
            }
        }

        private bool boolCat;
        public bool BoolCat
        {
            get => boolCat;
            set
            {
                boolCat = value;
                RaisePropertyChanged(nameof(BoolCat));
            }
        }

        //private void ValidateQuantite()
        //{
        //    ClearErrors();
        //    if (Quantity < 0)
        //        AddError("Quantity", Properties.Resources.Error_NbCopiesNotValid);
        //    RaiseErrors();
        //}

        public string Title
        {
            get { return Book.Title; }
            set
            {
                Book.Title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Author
        {
            get { return book.Author; }
            set
            {
                book.Author = value;
                RaisePropertyChanged(nameof(Author));
            }
        }

        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                RaisePropertyChanged(nameof(SelectedDate));
            }
        }

        public string Editor
        {
            get { return book.Editor; }
            set
            {
                book.Editor = value;
                RaisePropertyChanged(nameof(Editor));
            }
        }

        public string AbsolutePicturePath
        {
            get { return book.AbsolutePicturePath; }
            set { }
        }

        public string AddCopyVisible
        {
            get => HiddenShow();
        }

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand LoadImage { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand AddCopy { get; set; }
        public ICommand CatChecked { get; set; }

#if DEBUG_USERCONTROLS_WITH_TIMER
        private Timer timer = new Timer(1000);
#endif
        /////////////////////////////////////////////////////////CONSTRUCT/////////////////////////////////////////////////////////

        public BookDetailView(Book book, bool isNew)
        {
            InitializeComponent();
            DataContext = this;
            Book = book;
            IsNew = isNew;
            Cats = new ObservableCollection<Category>(App.Model.Categories);
            CheckboxList = new ObservableCollection<CategoriesCheckboxList<Category>>();
            if (App.IsAdmin())
            {
                Copies = new ObservableCollection<BookCopy>(book.Copies);
            }
            FillCHeckboxList();
            imageHelper = new ImageHelper(App.IMAGE_PATH, Book.PicturePath);

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            Delete = new RelayCommand(DeleteAction, () => !IsNew);
            LoadImage = new RelayCommand(LoadImageAction);
            ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);
            AddCopy = new RelayCommand(AddCopyBook);
            CatChecked = new RelayCommand(EnableBoolCat);

#if DEBUG_USERCONTROLS_WITH_TIMER
            App.Register<string>(this, AppMessages.MSG_TIMER, s => {
                Console.WriteLine($"{Member.Pseudo} received: {s}");
            });

            timer.Elapsed += (o, e) => { App.NotifyColleagues(AppMessages.MSG_TIMER, $"Timer from {Member.Pseudo}"); };
            timer.Start();
#endif
        }

        private void AddCopyBook()
        {
            if (selectedDate == null)
            {
                selectedDate = DateTime.Now;
            }
            book.AddCopies(Quantity, selectedDate);
            Copies = new ObservableCollection<BookCopy>(book.Copies);
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
        }

        private void DeleteAction()
        {
            this.CancelAction();
            if (File.Exists(PicturePath))
            {
                File.Delete(PicturePath);
            }
            Book.Delete();
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
        }

        private void LoadImageAction()
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                imageHelper.Load(fd.FileName);
                PicturePath = imageHelper.CurrentFile;
            }
        }

        private void ClearImageAction()
        {
            imageHelper.Clear();
            PicturePath = imageHelper.CurrentFile;
        }


        public override void Dispose()
        {
#if DEBUG_USERCONTROLS_WITH_TIMER
            timer.Stop();
#endif
            base.Dispose();
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Cancel();
                PicturePath = imageHelper.CurrentFile;
            }
        }

        private void SaveAction()
        {
            book.Categories.Clear();
            foreach (var c in CheckboxList)
            {
                if (c.IsChecked)
                {
                    book.Categories.Add(c.Item);
                }
            }
            if (IsNew)
            {
                AddCatCheckBox();
                App.Model.Books.Add(Book);
                IsNew = false;
            }
            imageHelper.Confirm(Book.Title);
            PicturePath = imageHelper.CurrentFile;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
        }

        private void AddCatCheckBox()
        {
            book.Categories = new ObservableCollection<Category>();
            foreach (var v in ListCheckboxes())
                book.AddCategory(v);
            book.Categories = new ObservableCollection<Category>(App.Model.Categories);
        }

        private List<Category> ListCheckboxes()
        {
            List<Category> list = new List<Category>();
            foreach (var checkBox in CheckboxList)
                if (checkBox.IsChecked)
                    list.Add(checkBox.Item);
            return list;
        }

        private void CancelAction()
        {
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Cancel();
            }
            if (IsNew)
            {
                ISBN = null;
                Title = null;
                Author = null;
                Editor = null;
                PicturePath = imageHelper.CurrentFile;
                RaisePropertyChanged(nameof(Book));
            }
            else
            {
                var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                              where c.Entity == Book
                              select c).FirstOrDefault();
                if (change != null)
                {
                    change.Reload();
                    RaisePropertyChanged(nameof(ISBN));
                    RaisePropertyChanged(nameof(Title));
                    RaisePropertyChanged(nameof(Author));
                    RaisePropertyChanged(nameof(Editor));
                    RaisePropertyChanged(nameof(PicturePath));
                }
            }
        }

        private string HiddenShow()
        {
            if (IsNew)
                return "hidden";
            else
                return "show";
        }

        private bool CanSaveOrCancelAction()
        {
            if (App.IsAdmin() && IsNew)
            {
                return CheckBoxesCondition() && InputsOk();
            }
            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == Book
                          select c).FirstOrDefault();
            return change != null && change.State != EntityState.Unchanged;
        }

        private bool InputsOk()
        {
            return IsOk(ISBN) && IsOk(Title) && IsOk(Author) && IsOk(Editor) && Quantity >= 1;
        }

        private bool IsOk(string s)
        {
            return !string.IsNullOrEmpty(s) && !HasErrors;
        }

        private void EnableBoolCat()
        {
            BoolCat = true;
        }

        private bool CheckBoxesCondition()
        {
            return boolCat == true;
        }

        private void FillCHeckboxList()
        {
            if (book.Categories.Count() == 0)
                AddBookCatUnchecked();
            else
            {
                AddBookCatChecked();
                AddCheckboxListRest();
            }
        }

        private void AddCheckboxListRest()
        {
            foreach (Category cat in Cats)
                if (!book.Categories.Contains(cat))
                    CheckboxList.Add(new CategoriesCheckboxList<Category>(cat, false));
        }

        private void AddBookCatChecked()
        {
            foreach (Category cat in book.Categories)
                CheckboxList.Add(new CategoriesCheckboxList<Category>(cat, true));
        }

        private void AddBookCatUnchecked()
        {
            foreach (Category cat in Cats)
                if (!book.Categories.Contains(cat))
                    CheckboxList.Add(new CategoriesCheckboxList<Category>(cat, false));
        }

        private bool IsEmpty()
        {
            return CheckboxList.Count() <= 0;
        }

        ////////////////////////////////////INNER CLASS////////////////////////////////////

        public class CategoriesCheckboxList<T> : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool isChecked;
            private T item;

            public CategoriesCheckboxList()
            { }

            public CategoriesCheckboxList(T item, bool isChecked = false)
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
    }
}
