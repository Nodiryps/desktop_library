using PRBD_Framework;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {
        private string pseudo;
        public string Pseudo { get => pseudo; set => SetProperty<string>(ref pseudo, value, () => Validate()); }

        private string password;
        public string Password { get => password; set => SetProperty<string>(ref password, value, () => Validate()); }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }

        public LoginView()
        {
            InitializeComponent();
            DataContext = this;
            Login = new RelayCommand(LoginAction, () => { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => Close());
        }

        private void LoginAction()
        {

            if (Validate() && !HasErrors)
            { // si aucune erreurs
                var member = App.Model.Users.Find(UserId()); // on recherche le membre 
                App.CurrentUser = member; // le membre connecté devient le membre courant

                if (member.Role == Role.Admin)
                {

                    ShowMainView(); // ouverture de la fenêtre principale
                    Close(); // fermeture de la fenêtre de login
                }
                else
                {
                    ShowMainView(); // ouverture de la fenêtre principale
                    //ShowUserView();
                    Close(); // fermeture de la fenêtre de login
                }
            }
        }

        private static void ShowMainView()
        {
            var mainView = new MainWindow();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        private static void ShowUserView()
        {
            var userView = new MainWindow();
            userView.Show();
            Application.Current.MainWindow = userView;
        }

        public override bool Validate()
        {
            ClearErrors();
            var member = App.Model.Users.Find(UserId());

            ValidatePseudo(member);
            ValidatePassword(member);

            RaiseErrors();
            return !HasErrors;
        } 

        private int UserId()
        {
            return (from u in App.Model.Users 
                    where u.UserName == pseudo
                    select u.UserId).FirstOrDefault();
        }

        private void ValidatePseudo(User member)
        {
            if (string.IsNullOrEmpty(Pseudo))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else
            {
                if (Pseudo.Length < 3)
                {
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else
                {
                    if (PseudoDoesntExist())
                    {
                        AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                    }
                }
            }
        }

        private void ValidatePassword(User member)
        {
            if (string.IsNullOrEmpty(Password))
            {
                AddError("Password", Properties.Resources.Error_Required);
            }
            else
            {
                if (Password.Length < 3)
                {
                    AddError("Password", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else if (!IsPasswordOk())
                {
                    AddError("Password", Properties.Resources.Error_WrongPassword);
                }
            }
        }

        private bool PseudoDoesntExist()
        {
            return (from u in App.Model.Users
                    where u.UserName == pseudo
                    select u
                    ).FirstOrDefault() == null;
        }

        private bool IsPasswordOk()
        {
            return (from user in App.Model.Users
                    where user.UserName == pseudo
                        && user.Password == password
                    select user
                    ).FirstOrDefault() != null;
        }
    }
}
