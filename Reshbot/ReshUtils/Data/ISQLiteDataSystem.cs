using Microsoft.Data.Sqlite;
using Reshbot.Config;

namespace ReshUtils.Data {
    public class ISQLModel {

    }

    public class ISQLiteDataSystem {
        private SqliteConnection _sqliteConnection;

        protected SqliteConnection SqliteConnection {
            get {
                if (_sqliteConnection == null) {
                    _sqliteConnection = new SqliteConnection($"Data Source={config.database};Cache=Shared");

                    try {
                        SqliteConnection.Open();
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }

                return _sqliteConnection;
            }
        }

        public virtual void CreateTable() { }
    }
}
