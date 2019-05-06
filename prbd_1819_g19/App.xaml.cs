using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace prbd_1819_g19
{
    public enum AppMessages
    {
        MSG_NEW_MEMBER,
        MSG_PSEUDO_CHANGED,
        MSG_MEMBER_CHANGED,
        MSG_DISPLAY_MEMBER,
        MSG_TIMER,
        MSG_CLOSE_TAB
    }

    public partial class App : ApplicationBase
    {
        public static Model Model = Model.CreateModel(DbType.MsSQL);
        public static User CurrentUser { get; set; }
        public static readonly string IMAGE_PATH = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../images");

        public App()
        {
            Model.ClearDatabase();
            Model.CreateTestData();
            InitializeComponent();
        }

        public void Msg(Object obj)
        {
            Console.WriteLine(obj);
        }
    }

   
}
