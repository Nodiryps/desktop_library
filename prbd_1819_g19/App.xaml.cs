using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace prbd_1819_g19
{
    public enum AppMessages
    {
        MSG_NEW_BOOK,
        MSG_TIMER,
        MSG_CLOSE_TAB,
        MSG_BOOK_DETAIL,
        MSG_ADD_BOOK_TO_BASKET,
        MSG_ADD_CAT,
        MSG_UPDATE_CAT,
        MSG_DEL_CAT,
        MSG_DISPLAY_MEMBER,
        MSG_DISPLAY_CAT,
    }

    public partial class App : ApplicationBase
    {
        public static Model Model = Model.CreateModel(DbType.MsSQL);

        public static User CurrentUser { get; set; }

        //public static User CurrentUser { get => WindowsIdentity.GetCurrent(); }
        public static readonly string IMAGE_PATH = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../images");

        public App()
        {
            Model.ClearDatabase();
            Model.CreateTestData();
            InitializeComponent();
        }

        public static bool IsAdmin()
        {
            return CurrentUser.Role == Role.Admin;
        }

        //public class Model
        //{
        //}
    }
}
