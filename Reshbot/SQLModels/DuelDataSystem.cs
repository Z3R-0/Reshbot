using Microsoft.Data.Sqlite;
using ReshUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reshbot.SQLModels {

    public class Duel : ISQLModel {
        public int Id;
        public string ChallengerId;
        public string ChallengedId;
        public string VictorId;

        public Duel(int id) {
            Id = id;
        }
    }

    public class DuelDataSystem : ISQLiteDataSystem {
        public DuelDataSystem(Type dataModel) : base() { }

        private static DuelDataSystem _instance;

        public static DuelDataSystem instance {
            get {
                if (_instance == null)
                    _instance = new DuelDataSystem(typeof(Duel));

                return _instance;
            }

            private set { _instance = value; }
        }

        public override void CreateTable() {
            SqliteCommand sqlite_command = SqliteConnection.CreateCommand();

            sqlite_command.CommandText = "CREATE TABLE Duels(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ChallengerId TEXT NOT NULL," +
                "ChallengedId TEXT NOT NULL," +
                "VictorId TEXT NOT NULL);";

            sqlite_command.ExecuteNonQuery();
        }
    }
}
