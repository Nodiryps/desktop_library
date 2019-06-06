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
            set => SetProperty<ObservableCollection<Category>>(ref category, value, () => { });
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
            set
            {
                if (App.IsAdmin())
                {
                    thisCat = value;
                    ThisCatChanged();
                    RaisePropertyChanged(nameof(ThisCat));
                }
                
            }
        }

        private bool boolAdd;
        public bool BoolAdd
        {
            get => boolAdd;
            set => SetProperty<bool>(ref boolAdd, value);
        }

        private bool boolUpdate;
        public bool BoolUpdate
        {
            get => boolUpdate;
            set => SetProperty<bool>(ref boolUpdate, value);
        }

        private bool boolDelete;
        public bool BoolDelete
        {
            get => boolDelete;
            set => SetProperty<bool>(ref boolDelete, value);
        }

        private bool boolCancel;
        public bool BoolCancel
        {
            get => boolCancel;
            set => SetProperty<bool>(ref boolCancel, value);
        }

        

        private bool boolInput;
        public bool BoolInput
        {
            get => boolInput;
            set => SetProperty<bool>(ref boolInput, value);
        }

        private bool boolTable;
        public bool BoolTable
        {
            get => boolTable;
            set => SetProperty<bool>(ref boolTable, value);
        }

        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand SetThisCat { get; set; }

        /// <summary>
        /// CONSTRUCT//////////////////////////////////////////////////////
        /// </summary>
        public CategoriesView()
        {
            InitializeComponent();
            DataContext = this;
            Category = new ObservableCollection<Category>(App.Model.Categories);

            if (App.IsAdmin())
            {
                BoolTable = true;
                BoolInput = true;

                Add = new RelayCommand(() => {
                    var catToAdd = (from cat in App.Model.Categories
                                    where cat.Name != ThisCat
                                    select cat).FirstOrDefault();
                    if (catToAdd != null)
                    {
                        App.Model.CreateCategory(ThisCat);
                    }
                    ThisCat = "";
                    Category = new ObservableCollection<Category>(App.Model.Categories);
                    BoolAdd = false;
                    BoolCancel = false;
                });

                Update = new RelayCommand(() => {
                    if (SelectedCategory != null)
                    {
                        var catToUpdate = (from cat in App.Model.Categories
                                           where cat.Name == SelectedCategory.Name
                                           select cat).FirstOrDefault();
                        if (catToUpdate != null)
                        {
                            catToUpdate.Name = ThisCat;
                            App.Model.SaveChanges();
                        }
                        ThisCat = "";
                        Category = new ObservableCollection<Category>(App.Model.Categories);
                        BoolUpdate = false;
                        BoolDelete = false;
                        BoolCancel = false;
                    }

                });

                Delete = new RelayCommand(() => {
                    var catToDel = (from cat in App.Model.Categories
                                    where cat.Name == ThisCat
                                    select cat).FirstOrDefault();
                    if (catToDel != null)
                        App.Model.Categories.Remove(catToDel);
                    else
                        AddError("ThisCat", Properties.Resources.Error_Required);
                    App.Model.SaveChanges();
                    ThisCat = "";
                    Category = new ObservableCollection<Category>(App.Model.Categories);
                    BoolUpdate = false;
                    BoolAdd = false;
                    BoolDelete = false;
                    BoolCancel = false;
                });

                Cancel = new RelayCommand(() => {
                    ThisCat = "";
                    Category = new ObservableCollection<Category>(App.Model.Categories);
                    BoolUpdate = false;
                    BoolAdd = false;
                    BoolDelete = false;
                    BoolCancel = false;
                });

                SetThisCat = new RelayCommand(() =>
                {
                    ThisCat = SelectedCategory.Name;
                    BoolUpdate = true;
                    BoolDelete = true;
                    BoolCancel = true;
                    BoolAdd = false;
                });
            }

        }

        public void ThisCatChanged()
        {
            if (ThisCat != "" && BoolUpdate)
            {
                BoolAdd = false;

            }
            else if(ThisCat != "" && !BoolUpdate)
            {
                BoolAdd = true;
            }
            BoolCancel = true;

        }


    }
}
