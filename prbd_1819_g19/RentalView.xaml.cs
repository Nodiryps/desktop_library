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
using prbd_1819_g19;
using PRBD_Framework;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour Rentals.xaml
    /// </summary>
    public partial class RentalView : UserControlBase
    {
        public RentalView()
        {
            InitializeComponent();
            DataContext = this;
            EnableTable = true;
            if (App.IsAdmin())
            {
                enableTable = true;
            }

            Rentalz = new ObservableCollection<Rental>();
            Items = new ObservableCollection<RentalItem>();
            Rentalz = new ObservableCollection<Rental>(FillRentals());

            SetRental = new RelayCommand<Rental>(rental => { //Pour remplir le tableau de droite
                if(SelectedRental != null)
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
            });

            App.Register(this, AppMessages.MSG_CONFIRM_BASKET, () =>
            {
                Rentalz = new ObservableCollection<Rental>(FillRentals());
            });

            Return();
            Delete();

        }

        public ICommand SetRental { get; set; }
        public ICommand ReturnBtn { get; set; }
        public ICommand DeleteBtn { get; set; }

        private bool boolClicked = false;

        private ObservableCollection<Rental> rentalz;
        public ObservableCollection<Rental> Rentalz
        {
            get => rentalz;
            set => SetProperty<ObservableCollection<Rental>>(ref rentalz, value, () => { });
        }

        private ObservableCollection<RentalItem> items;
        public ObservableCollection<RentalItem> Items
        {
            get => items;
            set => SetProperty<ObservableCollection<RentalItem>>(ref items, value, () => { });
        }

        public Rental selectedRental;
        public Rental SelectedRental
        {
            get => selectedRental;
            set => SetProperty<Rental>(ref selectedRental, value);
        }

        private bool enableTable = false;
        public bool EnableTable
        {
            get => enableTable;
            set => RaisePropertyChanged(nameof(enableTable));
        }

        private void Return()
        {
            ReturnBtn = new RelayCommand<RentalItem>(ri => {
                if(ri.ReturnDate == null)
                {
                    boolClicked = true;
                    ri.DoReturn();
                }
                else
                {
                    ri.ReturnDate = null;
                    boolClicked = false;
                }
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
                Rentalz = new ObservableCollection<Rental>(Refresh());
            });
        }

        private void Delete()
        {
            DeleteBtn = new RelayCommand<RentalItem>(ri => {
                if(ri.ReturnDate != null)
                    SelectedRental.RemoveItem(ri);
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
                Rentalz = new ObservableCollection<Rental>(Refresh());
            });
        }

        private void RefreshView()
        {
            Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
            var tmp = new ObservableCollection<Rental>();
            foreach (var r in App.Model.Rentals)
                if (r.RentalDate != null && r.Items.Count() > 0)
                    tmp.Add(r);
            Rentalz = new ObservableCollection<Rental>(tmp);
        }

        private List<Rental> Refresh()
        {
            return (
                    from r in App.Model.Rentals
                    where r.RentalDate != null && r.Items.Count > 0
                    select r
                ).ToList();
        }

        private List<Rental> FillRentals()
        {
            return (from r in App.Model.Rentals
                    where r.RentalDate != null
                    //&&(from ri in r.Items where ri.ReturnDate != null select ri)
                    select r).ToList();
        }
    }
}
