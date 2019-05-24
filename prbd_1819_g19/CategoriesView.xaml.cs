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

        
        
        /// <summary>
        /// CONSTRUCT//////////////////////////////////////////////////////
        /// </summary>
        public CategoriesView()
        {
            ViewSettings();
            App.Register<Category>(this, AppMessages.MSG_FILL_CAT_INPUT, cat => { SelectedCategory = cat; });
            SetCat();
            Buttons();
            
        }

        private void ViewSettings()
        {
            InitializeComponent();
            DataContext = this;
            Category = new ObservableCollection<Category>(App.Model.Categories);
        }

        private void Buttons()
        {
            if (App.IsAdmin())
            {
                EnableTable();
                EnableBtnsAndInput();

                Add = new RelayCommand(AddCat);
                Update = new RelayCommand(UpdateCat);
                Delete = new RelayCommand(DeleteCat);
                Cancel = new RelayCommand(CancelCat);
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
                if (SelectedCategory != null)
                    ThisCat = SelectedCategory.Name;
            });
        }
        
        private void AddCat()
        {
            var catToAdd = (from cat in App.Model.Categories
                               where cat.Name != ThisCat
                               select cat).FirstOrDefault();
            if (catToAdd != null)
            {
                App.Model.CreateCategory(ThisCat);
                
            }
            Reset();
            App.NotifyColleagues(AppMessages.MSG_CAT_CHANGED);
        }

        private void UpdateCat()
        {
            var catToUpdate = (from cat in App.Model.Categories
                         where cat.Name == selectedCategory.Name
                         select cat).FirstOrDefault();
            if (catToUpdate != null)
            {
                catToUpdate.Name = ThisCat;
                App.Model.SaveChanges();
                
            }
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
            Reset();
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
            foreach (var c in Category)
                return c.Name.ToUpper().Equals(ThisCat.ToUpper());
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
            selectedCategory = null;
            category = new ObservableCollection<Category>(App.Model.Categories);
        }

        private void EnableBtnsAndInput()
        {
            EnableInput();

            if (!IsNullOrEmpty(ThisCat))
            {
                if (ThisCatExists())
                {
                    EnableAdd();
                }
                else
                {
                    EnableUpdate();
                    EnableDelete();
                }
                EnableAdd();
                EnableUpdate();
                EnableDelete();
            }

        }

        private bool boolInput = false;
        public bool BoolInput
        {
            get => boolInput;
            set => RaisePropertyChanged(nameof(boolInput));
        }

        private bool boolTable = false;
        public bool BoolTable
        {
            get => boolTable;
            set => RaisePropertyChanged(nameof(boolTable));
        }

        private bool boolAdd = false;
        public bool BoolAdd
        {
            get => boolAdd;
            set => RaisePropertyChanged(nameof(boolAdd));
        }

        private bool boolUpdate = false;
        public bool BoolUpdate
        {
            get => boolUpdate;
            set => RaisePropertyChanged(nameof(boolUpdate));
        }

        private bool boolDelete = false;
        public bool BoolDelete
        {
            get => boolDelete;
            set => RaisePropertyChanged(nameof(boolDelete));
        }

        private void EnableInput() {boolInput = true;}

        private void EnableAdd() { boolAdd = true; }

        private void EnableUpdate() { boolUpdate = true; }

        private void EnableDelete() { boolDelete = true; }

        private void EnableTable() { boolTable = true; }
    }
}
