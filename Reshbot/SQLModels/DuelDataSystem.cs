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

        public Duel(string challenger_id, string challenged_id, string victor_id) {
            ChallengerId = challenger_id;
            ChallengedId = challenged_id;
            VictorId = victor_id;
        }
    }

    public class DuelDataSystem : ISQLiteDataSystem {
        public DuelDataSystem(Type dataModel) : base() {
            CreateTableIfNotExists();
        }

        private static DuelDataSystem _instance;

        public static DuelDataSystem instance {
            get {
                if (_instance == null)
                    _instance = new DuelDataSystem(typeof(Duel));

                return _instance;
            }

            private set { _instance = value; }
        }

        public void Insert(Duel duel) {
            SqliteConnection.Open();
            SqliteCommand insert_command = SqliteConnection.CreateCommand();

            insert_command.CommandText = "INSERT INTO Duels (ChallengerId, ChallengedId, VictorId) " +
                $"VALUES ({duel.ChallengerId}, {duel.ChallengedId}, {duel.VictorId})";

            insert_command.ExecuteNonQuery();
        }

        public void InsertMany(List<Duel> duelList) {
            foreach (Duel duel in duelList) {
                Insert(duel);
            }
        }

        public override void CreateTableIfNotExists() {
            SqliteConnection.Open();
            SqliteCommand sqlite_command = SqliteConnection.CreateCommand();

            sqlite_command.CommandText = "CREATE TABLE IF NOT EXISTS Duels(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ChallengerId TEXT NOT NULL," +
                "ChallengedId TEXT NOT NULL," +
                "VictorId TEXT NOT NULL);";

            sqlite_command.ExecuteNonQuery();
        }
    }
}
