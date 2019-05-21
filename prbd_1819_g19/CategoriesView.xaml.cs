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
        public string ThisCat
        {
            get => thisCat;
            set => SetProperty<string>(ref thisCat, value, () => Validate());
        }

        private bool boolInput;
        
        /// <summary>
        /// CONSTRUCT//////////////////////////////////////////////////////
        /// </summary>
        public CategoriesView()
        {
            ViewSettings();
            SetCat();
            Buttons();
        }

        private void ViewSettings()
        {
            InitializeComponent();
            DataContext = this;
            Category = new ObservableCollection<Category>(App.Model.Categories);
            DisableBtnsAndInput();
        }

        private void Buttons()
        {
            if (App.IsUserLogged())
            {
                CancelBtn();
                if (App.IsAdmin())
                {
                    EnableBtnsAndInput();
                    
                    AddBtn();
                    UpdateBtn();
                    DeleteBtn();
                }
            }
        }

        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand SetThisCat { get; set; }

        private void SetCat()
        {
            SetThisCat = new RelayCommand(() =>
            {
                if(!IsNullOrEmpty(SelectedCategory.Name))
                    ThisCat = SelectedCategory.Name;
            });
        }

        private void AddBtn()
        {
            Add = new RelayCommand(AddCat,() =>
            {
                return AddBtnCondition();
            });
        }

        private bool AddBtnCondition()
        {
            //return IsNullOrEmpty(ThisCat) && IsNullOrEmpty(SelectedCategory.Name); 
            return true;
        }

        private void UpdateBtn()
        {
            Update = new RelayCommand(UpdateCat, () =>
            {
                //return IsNullOrEmpty(SelectedCategory.Name);
                return true;
            });
        }

       private void DeleteBtn()
        {
             Delete = new RelayCommand(DeleteCat, () =>
            {
                //return IsNullOrEmpty(SelectedCategory.Name);
                return true;

            });
        } 

        private void CancelBtn()
        {
            Reset();
        }

        private void AddCat()
        {
            App.Model.CreateCategory(ThisCat);
            Reset();
            App.NotifyColleagues(AppMessages.MSG_CAT_CHANGED);
        }

        private void UpdateCat()
        {
            var catToUpdate = (from cat in App.Model.Categories
                         where cat.Name == selectedCategory.Name
                         select cat).FirstOrDefault();
            catToUpdate.Name = ThisCat;
            App.Model.SaveChanges();
            Reset();
            App.NotifyColleagues(AppMessages.MSG_CAT_CHANGED);
        }

        private void DeleteCat()
        {
            var catToDel = (from cat in App.Model.Categories
                         where cat.Name == ThisCat
                         select cat).FirstOrDefault();
            if (catToDel != null)
                App.Model.Categories.Remove(catToDel);
            else
                AddError("DeleteCat", Properties.Resources.Error_Required);
            App.Model.SaveChanges();
            Reset();
            App.NotifyColleagues(AppMessages.MSG_CAT_DEL);
        }

        private void CancelCat()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB);
        }
        
        public override bool Validate()
        {
            ClearErrors();
            if (IsNullOrEmpty(thisCat))
                AddError("EditCategory", Properties.Resources.Error_Required);
            else
            {
                if (ThisCatExists())
                    AddError("EditCategory", Properties.Resources.Error_AlreadyExists);
                 else
                    AddError("EditCategory", Properties.Resources.Error_DoesNotExist);
            }
            RaiseErrors();
            return !HasErrors;
        }

        private bool ThisCatExists()
        {
            //if(!IsNullOrEmpty(thisCat))
            foreach (Category c in App.Model.Categories)
                return c.Name.ToUpper().Equals(thisCat.ToUpper());
            return false;
        }

        private bool IsNullOrEmpty(string cat)
        {
            return string.IsNullOrEmpty(cat) 
                || string.IsNullOrWhiteSpace(cat);
        }
        
        private void Reset()
        {
            ThisCat = "";
            DisableBtnsAndInput();
            selectedCategory = null;
            Category = new ObservableCollection<Category>(App.Model.Categories);
        }

        private void DisableBtnsAndInput()
        {
            EnableAdd(false);
            EnableUpdate(false);
            EnableDelete(false);
            EnableInput(false);
        }

        private void EnableBtnsAndInput()
        {
            EnableInput(true);

            //if (!IsNullOrEmpty(ThisCat))
            //{
                //if (ThisCatExists())
                //{
                //    EnableAdd(true);
                //}
                //else
                //{
                //    EnableUpdate(true);
                //    EnableDelete(true);
                //}
            EnableAdd(true);
            EnableUpdate(true);
            EnableDelete(true);
            //}

        }

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

        private void EnableInput(bool b) {boolInput = b;}

        private void EnableAdd(bool b) { boolAdd = b; }

        private void EnableUpdate(bool b) { boolUpdate = b; }

        private void EnableDelete(bool b) { boolDelete = b; }
    }
}
