using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace prbd_1819_g19 {
    public class Program {

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

        public class EntityBase<T> where T : DbContext {
        private static DbContext GetDbContextFromEntity(object entity) {
            var object_context = GetObjectContextFromEntity(entity);
            if (object_context == null || object_context.TransactionHandler == null) {
                return null;
            }
            return object_context.TransactionHandler.DbContext;
        }
        private static ObjectContext GetObjectContextFromEntity(object entity) {
            var field = entity.GetType().GetField("_entityWrapper");
            if (field == null) {
                return null;
            }
            var wrapper = field.GetValue(entity);
            if (wrapper == null) {
                return null;
            }
            var property = wrapper.GetType().GetProperty("Context");
            if (property == null) {
                return null;
            }
            var context = (ObjectContext)property.GetValue(wrapper, null);
            return context;
        }
        private T _model = null;
        public bool Attached { get { return Model != null; } }
        public bool Detached { get { return Model == null; } }
        public T Model {
            get {
                if (_model == null) {
                    _model = (T)GetDbContextFromEntity(this);
                }
                return _model;
            }
        }
    }
    }
}
