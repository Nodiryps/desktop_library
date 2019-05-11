using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PRBD_Framework;
using prbd_1819_g19;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView : UserControlBase
    {
        //public string SelectedCat { get; set; }

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

        private ObservableCollection<Category> category;
        public ObservableCollection<Category> Category
        {
            get => category;
            set => SetProperty<ObservableCollection<Category>>(ref category, value, () => {} );
        }

        public Category selectedCategory;
        public Category SelectedCategory
        {
            get => selectedCategory;
            set => SetProperty<Category>(ref selectedCategory, value);
        }

        public string thisCat;
        public string ThisCat { get => thisCat; set => SetProperty<string>(ref thisCat, value, () => ButtonsMngmt()); }

        private bool boolInput;
        public bool BoolInput
        {
            get => boolInput;
            set => RaisePropertyChanged(nameof(boolInput));
        }

        private bool boolAdd;
        public bool BoolAdd
        {
            get => boolAdd;
            set => RaisePropertyChanged(nameof(boolAdd));
        }

        private bool boolUpdate;
        public bool BoolUpdate
        {
            get => boolUpdate;
            set => RaisePropertyChanged(nameof(boolUpdate));
        }

        private bool boolDelete;
        public bool BoolDelete
        {
            get => boolDelete;
            set => RaisePropertyChanged(nameof(boolDelete));
        }

        private bool boolCancel;
        public bool BoolCancel
        {
            get => boolCancel;
            set => RaisePropertyChanged(nameof(boolCancel));
        }
        /// <summary>
        /// CONSTRUCT//////////////////////////////////////////////////////
        /// </summary>
        public CategoriesView(ObservableCollection<Category> list, bool isnew)
        {
            isnew = isNew;
            category = list; //pour rafraichir, en theorie

            InitializeComponent();
            DataContext = this;

            DisableBtnsAndInput();
            NbBooksByCat();
            DisableCancel(false);
            CancelBtn();
            Display();

            if (App.IsAdmin())
            {
                ButtonsMngmt();
                AddBtn();
                UpdateBtn();
                DeleteBtn();
            }
        }

        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand DisplayCat { get; set; }

        private void Display()
        {
            DisplayCat = new RelayCommand(() =>
            {
                ThisCat = SelectedCategory.Name;
                if (App.IsAdmin())
                    DisableDelete(false);
            });
        }

        private void AddBtn()
        {
            Add = new RelayCommand(() =>
            {
                AddCat();
                Reset();
                App.NotifyColleagues(AppMessages.MSG_ADD_CAT);
            });
        }

        private void UpdateBtn()
        {
            Update = new RelayCommand(() =>
            {
                UpdateCat();
                Reset();
                App.NotifyColleagues(AppMessages.MSG_UPDATE_CAT);
            });
        }

        private void DeleteBtn()
        {
             Delete = new RelayCommand(() =>
            {
                DeleteCat();
                Reset();
                App.NotifyColleagues(AppMessages.MSG_DEL_CAT);
            });
        }

        private void CancelBtn()
        {
            Cancel = new RelayCommand(() => { App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB); });
        }

        private void AddCat()
        {
            App.Model.CreateCategory(ThisCat);
        }

        private void UpdateCat()
        {
            var catToUpdate = (from cat in App.Model.Categories
                         where cat.Name == selectedCategory.Name
                         select cat).FirstOrDefault();
            catToUpdate.Name = ThisCat;
            App.Model.SaveChanges();
        }

        private void DeleteCat()
        {
            var catToDel = (from cat in App.Model.Categories
                         where cat.Name == selectedCategory.Name
                         select cat).FirstOrDefault();
            App.Model.Categories.Remove(catToDel);
            App.Model.SaveChanges();
        }

        private void ButtonsMngmt()
        {
            DisableInput(false);
            if (IsExisting)
            {
                DisableUpdate(false);
                DisableDelete(false);
            }
            else 
            {
                DisableAdd(false);
            }
        }

        //private bool CatExists()
        //{
        //    foreach (Category c in App.Model.Categories)
        //        return c.Name.ToUpper() == thisCat.ToUpper();
        //    return false;
        //}

        private bool IsCatNull(Category c)
        {
            return c == null;
        }

        private List<int> NbBooksByCat()
        {
            List<int> list = new List<int>();
            foreach (Category cat in App.Model.Categories)
                list.Add(cat.Books.Count());
            return list;
        }

        private void Reset()
        {
            ThisCat = "";
            DisableBtnsAndInput();
            selectedCategory = null;
        }

        private void DisableBtnsAndInput()
        {
            DisableAdd(true);
            DisableUpdate(true);
            DisableDelete(true);
            DisableCancel(true);
            DisableInput(true);
        }

        private void DisableInput(bool b) {boolInput = b;}

        private void DisableAdd(bool b) { boolAdd = b; }

        private void DisableUpdate(bool b) { boolUpdate = b; }

        private void DisableDelete(bool b) { boolDelete = b; }

        private void DisableCancel(bool b) { boolCancel = b; }
    }
}
