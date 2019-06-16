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
using System.Windows;
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

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set => SetProperty<ObservableCollection<Category>>(ref categories, value);
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

        private bool boolGrid;
        public bool BoolGrid
        {
            get => boolGrid;
            set => SetProperty<bool>(ref boolGrid, value);
        }

        private bool boolCat;
        public bool BoolCat
        {
            get => boolCat;
            set => SetProperty<bool>(ref boolCat, value);
        }

        private bool boolSave;
        public bool BoolSave
        {
            get => boolSave;
            set => SetProperty<bool>(ref boolSave, value);
        }

        private bool boolQuantity;
        public bool BoolQuantity
        {
            get => boolQuantity;
            set => SetProperty<bool>(ref boolQuantity, value);
        }

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

        private string isbn;
        public string Isbn
        {
            get { return book.Isbn; }
            set
            {
                book.Isbn = value;
                SetProperty<string>(ref isbn, value, InputValidationsIsbn);
                App.NotifyColleagues(AppMessages.MSG_ISBN_CHANGED, string.IsNullOrEmpty(value) ? " ??? " : value);
            }
        }

        private int quantity = 0;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                SetProperty<int>(ref quantity, value, QuantityValidations);
            }
        }
        private string title;
        public string Title
        {
            get { return Book.Title; }
            set
            {
                Book.Title = value;
                SetProperty<string>(ref title, value, InputValidationsTitle);
            }
        }

        private string author;
        public string Author
        {
            get { return book.Author; }
            set
            {
                book.Author = value;
                SetProperty<string>(ref author, value, InputValidationsAuthor);
            }
        }

        private string editor;
        public string Editor
        {
            get { return book.Editor; }
            set
            {
                book.Editor = value;
                SetProperty<string>(ref editor, value, InputValidationsEditor);
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

        public string AbsolutePicturePath
        {
            get { return book.AbsolutePicturePath; }
            set {}
        }

        public string AddCopyVisible
        {
            get => HiddenShow();
        }

        public string IsAdmin
        {
            get => IsAdminHidden();
        }

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Exit { get; set; }
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
            Categories = new ObservableCollection<Category>(App.Model.Categories);
            CheckboxList = new ObservableCollection<CategoriesCheckboxList<Category>>();
            Copies = new ObservableCollection<BookCopy>(book.Copies);
            FillCheckboxList();
            imageHelper = new ImageHelper(App.IMAGE_PATH, Book.PicturePath);
            Exit = new RelayCommand(ExitAction);

            if (App.IsAdmin())
            {
                BoolGrid = true;

                Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
                Cancel = new RelayCommand(CancelAction);
                Delete = new RelayCommand(DeleteAction, () => !IsNew);
                LoadImage = new RelayCommand(LoadImageAction);
                ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);
                AddCopy = new RelayCommand(AddCopyBook);
            }


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
            AddCopies();
            Copies = new ObservableCollection<BookCopy>(book.Copies);
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
            Quantity = 0;
            BoolQuantity = false;
        }

        private void AddCopies()
        {
            if (selectedDate == null)
                selectedDate = DateTime.Now;
            if (Quantity > 0)
                book.AddCopies(Quantity, selectedDate);
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

        private void ExitAction()
        {
            if(!IsNew)
                ResetBookDatas();
            App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
        }

        private void SaveAction()
        {
            if (IsNew)
            {
                App.Model.Books.Add(Book);
                IsNew = false;
            }

            AddCopies();
            AddCategoriesCheckBox();
            imageHelper.Confirm(Book.Title);
            PicturePath = imageHelper.CurrentFile;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
        }

        private void AddCategoriesCheckBox()
        {
            Book.Categories.Clear();
            foreach (var c in CheckboxList)
                if (c.IsChecked)
                {
                    Book.Categories.Add(c.Item);

                }
        }

        private void DeleteAction()
        {
            MessageBoxResult dialog = MessageBox.Show("Are you sur you want to delete this book? (" + Book.Title + ')',
                                                      "DELETE CONFIRM",
                                                      MessageBoxButton.OKCancel);
            if (dialog == MessageBoxResult.OK)
            {
                CancelAction();
                if (File.Exists(PicturePath))
                {
                    File.Delete(PicturePath);
                }
                Book.Delete();
                App.Model.SaveChanges();
                App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
                App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
            }
        }

        private void CancelAction()
        {
            if (imageHelper.IsTransitoryState)
                imageHelper.Cancel();

            if (IsNew)
            {
                Isbn = null;
                Title = null;
                Author = null;
                Editor = null;
                PicturePath = imageHelper.CurrentFile;
                RaisePropertyChanged(nameof(Book));
                Book.Categories.Clear();
                CheckboxList.Clear();
                FillCheckboxList();
            }
            else
            {
                ResetBookDatas();
            }
        }

        private void ResetBookDatas()
        {
            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == Book
                          select c).FirstOrDefault();
            if (change != null)
            {
                change.Reload();
                RaisePropertyChanged(nameof(Isbn));
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(Author));
                RaisePropertyChanged(nameof(Editor));
                CheckboxList.Clear();
                FillCheckboxList();
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        private bool CanSaveOrCancelAction()
        {
            if (Validate())
                BoolSave = true;
            return BookChanged();
        }

        private bool BookChanged()
        {
            var cpt = 0;
            foreach (var c in CheckboxList)
                if (c.IsChecked)
                    ++cpt;

            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == Book 
                          select c).FirstOrDefault();
            return change != null && change.State != EntityState.Unchanged || change.Entity.Categories.Count != cpt;
        }

        public override bool Validate()
        {
            ClearErrors();

            InputsValidations();

            RaiseErrors();
            return !HasErrors;
        }

        private void InputsValidations()
        {
            InputValidationsTitle();
            InputValidationsAuthor();
            InputValidationsEditor();
            InputValidationsIsbn();
            QuantityValidations();
        }

        private void InputValidationsTitle()
        {
            if (IsOk(Title))
            {
                if (Title.Length < 2 || Title.Length > 50)
                {
                    BoolSave = false;
                    ClearErrors();
                    AddError("Title", Properties.Resources.Error_InputDetailString);
                }
                else
                    ClearErrors();
            }

            else
            {
                BoolSave = false;
                ClearErrors();
                AddError("Title", Properties.Resources.Error_Required);
            }

        }

        private void InputValidationsAuthor()
        {
            if (IsOk(Author))
            {
                if (Author.Length < 2 || Author.Length > 50)
                {
                    BoolSave = false;
                    ClearErrors();
                    AddError("Author", Properties.Resources.Error_InputDetailString);
                }
                else
                    ClearErrors();
            }
            else
            {
                BoolSave = false;
                ClearErrors();
                AddError("Author", Properties.Resources.Error_Required);
            }

        }

        private void InputValidationsEditor()
        {
            if (IsOk(Editor))
            {
                if (Editor.Length < 2)
                {
                    BoolSave = false;
                    ClearErrors();
                    AddError("Editor", Properties.Resources.Error_InputDetailString);
                }
                else
                    ClearErrors();
            }
            else
            {
                BoolSave = false;
                ClearErrors();
                AddError("Editor", Properties.Resources.Error_Required);
            }

        }

        private void InputValidationsIsbn()
        {
            if (IsOk(Isbn))
            {
                if (IsNumeric(Isbn))
                {
                    if (Isbn.Length == 3)
                    {
                        if (IsbnExists())
                        {
                            BoolSave = false;
                            ClearErrors();
                            AddError("Isbn", Properties.Resources.Error_AlreadyExists);
                        }
                        else
                            ClearErrors();
                    }
                    else
                    {
                        BoolSave = false;
                        ClearErrors();
                        AddError("Isbn", Properties.Resources.Error_IsbnLength);
                    }
                }
                else
                {
                    BoolSave = false;
                    ClearErrors();
                    AddError("Isbn", Properties.Resources.Error_IsbnNumeric);
                }
            }
            else
            {
                BoolSave = false;
                ClearErrors();
                AddError("Isbn", Properties.Resources.Error_Required);
            }
        }

        private void QuantityValidations()
        {
            //if (IsOk(Quantity.ToString()))
            //{
                if (IsNumeric(Isbn))
                {
                    if (Quantity < 0)
                    {
                        Console.WriteLine("premier if");
                        BoolSave = false;
                        ClearErrors();
                        AddError("Quantityz", Properties.Resources.Error_NbCopiesNotValid);
                    }
                    else
                    {
                        Console.WriteLine("else!!!!!!!");
                        ClearErrors();
                        BoolQuantity = true;
                    }
                        
                }
                else
                {
                    Console.WriteLine("deuxieme if");
                    BoolSave = false;
                    ClearErrors();
                    AddError("Quantityz", Properties.Resources.Error_IsbnNumeric);
                }
            //}
            //else
            //{

            //    Console.WriteLine("troisieme if");
            //    BoolSave = false;
            //    ClearErrors();
            //    AddError("Quantityz", Properties.Resources.Error_Required);
            //}
        }

        public bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        private bool IsbnExists()
        {
            return (from b in App.Model.Books
                    where b.Isbn.Equals(Isbn)
                       && b.BookId != book.BookId
                    select b).FirstOrDefault() != null;
        }

        private bool IsOk(string s)
        {
            return !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s);
        }

        private void EnableBoolCat()
        {
            BoolCat = true;
        }

        private string HiddenShow()
        {
            if (IsNew)
                return "hidden";
            else
                return "show";
        }


        public string IsAdminHidden()
        {
            if (!App.IsAdmin())
                return "hidden";
            else
                return "show";
        }

        private void FillCheckboxList()
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
            foreach (Category cat in Categories)
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
            foreach (Category cat in Categories)
                if (!book.Categories.Contains(cat))
                    CheckboxList.Add(new CategoriesCheckboxList<Category>(cat, false));
        }

        ////////////////////////////////////INNER CLASS////////////////////////////////////
        public class CategoriesCheckboxList<T> : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;


            private T item;
            public T Item
            {
                get { return item; }
                set
                {
                    item = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
                }
            }

            private bool isChecked;
            public bool IsChecked
            {
                get { return isChecked; }
                set
                {
                    isChecked = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }

            public CategoriesCheckboxList(T item, bool isChecked = false)
            {
                this.item = item;
                this.isChecked = isChecked;
            }
        }
    }
}
