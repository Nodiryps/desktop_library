using PRBD_Framework;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
        public User Member { get; set; }
        //private ImageHelper imageHelper;

        private ObservableCollection<Category> cats;
        public ObservableCollection<Category> Cats
        {
            get => cats;
            set => SetProperty<ObservableCollection<Category>>(ref cats, value);
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
        

        //public string Profile
        //{
        //    get { return Member.Profile; }
        //    set
        //    {
        //        Member.Profile = value;
        //        RaisePropertyChanged(nameof(Profile));
        //    }
        //}
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
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get { return book.Title; }
            set
            {
                book.Title = value;
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

        public string Editor
        {
            get { return book.Editor; }
            set
            {
                book.Editor = value;
                RaisePropertyChanged(nameof(Editor));
            }
        }

        public string PicturePath
        {
            get { return book.AbsolutePicturePath; }
            set
            {
                book.PicturePath = value;
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        public string AbsolutePicturePath
        {
            get { return book.AbsolutePicturePath; }
            set { }
        }        

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand LoadImage { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand FollowUnfollow { get; set; }

#if DEBUG_USERCONTROLS_WITH_TIMER
        private Timer timer = new Timer(1000);
#endif

        public BookDetailView(Book book, bool isNew)
        {
            InitializeComponent();

            DataContext = this;
            Book = book;
            IsNew = isNew;
            Cats = new ObservableCollection<Category>(App.Model.Categories);
            //imageHelper = new ImageHelper(App.IMAGE_PATH, Member.PicturePath);

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            Delete = new RelayCommand(DeleteAction, () => !IsNew);
            //LoadImage = new RelayCommand(LoadImageAction);
            //ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);
            FollowUnfollow = new RelayCommand(FollowUnfollowAction);

#if DEBUG_USERCONTROLS_WITH_TIMER
            App.Register<string>(this, AppMessages.MSG_TIMER, s => {
                Console.WriteLine($"{Member.Pseudo} received: {s}");
            });

            timer.Elapsed += (o, e) => { App.NotifyColleagues(AppMessages.MSG_TIMER, $"Timer from {Member.Pseudo}"); };
            timer.Start();
#endif
        }

        private void DeleteAction()
        {
            //this.CancelAction();
            //if (File.Exists(PicturePath))
            //{
            //    File.Delete(PicturePath);
            //}
            //Member.Delete();
            //App.Model.SaveChanges();
            //App.NotifyColleagues(AppMessages.MSG_MEMBER_CHANGED, Member);
            //App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
        }

        private void LoadImageAction()
        {
            //var fd = new OpenFileDialog();
            //if (fd.ShowDialog() == true)
            //{
            //    imageHelper.Load(fd.FileName);
            //    PicturePath = imageHelper.CurrentFile;
            //}
        }

        private void ClearImageAction()
        {
            //imageHelper.Clear();
            //PicturePath = imageHelper.CurrentFile;
        }

        public override void Dispose()
        {
            //#if DEBUG_USERCONTROLS_WITH_TIMER
            //            timer.Stop();
            //#endif
            //            base.Dispose();
            //            if (imageHelper.IsTransitoryState)
            //            {
            //                imageHelper.Cancel();
            //                PicturePath = imageHelper.CurrentFile;
            //            }
        }

        private void SaveAction()
        {
            if (IsNew)
            {
                // Un petit raccourci ;-)
                Member.Password = Member.UserName;
                App.Model.Users.Add(Member);
                IsNew = false;
            }
            //imageHelper.Confirm(Member.UserName);
            //PicturePath = imageHelper.CurrentFile;
            App.Model.SaveChanges();
            //App.NotifyColleagues(AppMessages.MSG_MEMBER_CHANGED, Member);
        }

        private void CancelAction()
        {
            //if (imageHelper.IsTransitoryState)
            //{
            //    imageHelper.Cancel();
            //}
            if (IsNew)
            {
                //Pseudo = null;
                //Profile = null;
                //PicturePath = imageHelper.CurrentFile;
                RaisePropertyChanged(nameof(Member));
            }
            else
            {
                var change = (from c in App.Model.ChangeTracker.Entries<User>()
                              where c.Entity == Member
                              select c).FirstOrDefault();
                if (change != null)
                {
                    change.Reload();
                    //RaisePropertyChanged(nameof(Profile));
                    //RaisePropertyChanged(nameof(PicturePath));
                }
            }
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
            {
                //return !string.IsNullOrEmpty(Pseudo) && !HasErrors;
            }
            var change = (from c in App.Model.ChangeTracker.Entries<User>()
                          where c.Entity == Member
                          select c).FirstOrDefault();
            return change != null && change.State != EntityState.Unchanged;
        }

        private void FollowUnfollowAction()
        {
            //App.CurrentUser.ToggleFollowUnfollow(Member);
            App.Model.SaveChanges();
            //RaisePropertyChanged(nameof(FollowUnfollowLabel));
            //RaisePropertyChanged(nameof(FollowStatus));
            //App.NotifyColleagues(AppMessages.MSG_MEMBER_CHANGED, Member);
        }
    }
}
