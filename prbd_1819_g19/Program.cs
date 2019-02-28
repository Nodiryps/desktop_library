using MySql.Data.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_gXX {
    partial class Program {
        static void Main(string[] args) {
#if MSSQL
            var type = DbType.MsSQL;
#else
            var type = DbType.MySQL;
#endif
            using (var model = Model.CreateModel(type)) {
                model.ClearDatabase();
                model.CreateTestData();
            }
        }
    }
}
