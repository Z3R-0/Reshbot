﻿using Microsoft.Data.Sqlite;
using Reshbot.Config;
using Reshbot.ReshDiscordUtils;
using ReshUtils.Data;

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

    public class Leaderboard {
        public List<LeaderboardRow> Rows = new List<LeaderboardRow>();

        public override string ToString() {
            string result = "";

            foreach (var row in Rows) {
                result += $"\n<@{row.UserId}> --- {row.Wins}";
            }

            return result;
        }
    }

    public class LeaderboardRow {
        public string UserId;
        public int Wins;

        public LeaderboardRow(string userId, int wins) {
            UserId = userId;
            Wins = wins;
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

        /// <summary>
        /// This method first checks if the necessary table exists, and creates it if the table does not exist
        /// Then it opens a connection and uses that connection to retrieve a Command
        /// </summary>
        /// <param name="guildId">The guildId to use when checking if a table exists or not</param>
        /// <returns>The SqliteCommand for use in other methods</returns>
        private SqliteCommand OpenSqlConnection(string guildId) {
            CreateTableIfNotExists(guildId);

            SqliteConnection.Open();
            return SqliteConnection.CreateCommand();
        }

        public void Insert(Duel duel, string guildId) {
            SqliteCommand insert_command = OpenSqlConnection(guildId);

            insert_command.CommandText = $"INSERT INTO Duels{guildId} (ChallengerId, ChallengedId, VictorId) " +
                $"VALUES ({duel.ChallengerId}, {duel.ChallengedId}, {duel.VictorId})";

            insert_command.ExecuteNonQuery();
        }

        public void InsertMany(List<Duel> duelList, string guildId) {
            foreach (Duel duel in duelList) {
                Insert(duel, guildId);
            }
        }


        public override void CreateTableIfNotExists(string guildId) {
            SqliteConnection.Open();
            SqliteCommand sqlite_command = SqliteConnection.CreateCommand();

            sqlite_command.CommandText = $"CREATE TABLE IF NOT EXISTS Duels{guildId}(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ChallengerId TEXT NOT NULL," +
                "ChallengedId TEXT NOT NULL," +
                "VictorId TEXT NOT NULL);";

            sqlite_command.ExecuteNonQuery();
        }

        public Leaderboard GetLeaderboard(string guildId) {
            Leaderboard leaderboard = new Leaderboard();

            SqliteDataReader reader = SelectDuelsWithQuery($"VictorId, COUNT(VictorId) FROM Duels{guildId} " +
                            "GROUP BY VictorId " +
                            "ORDER BY COUNT(VictorId) DESC " +
                            "LIMIT 15;", guildId);

            while (reader.Read()) {
                leaderboard.Rows.Add(new LeaderboardRow(reader.GetString(0), reader.GetInt32(1)));
            }

            reader.Close();

            return leaderboard;
        }
        
        private SqliteDataReader SelectDuelsWithQuery(string query, string guildId) {
            SqliteCommand sqlite_command = OpenSqlConnection(guildId);

            sqlite_command.CommandText = "SELECT " + query;
            return sqlite_command.ExecuteReader();
        }

        public List<Duel> GetDuels(string guildId) {
            List<Duel> duels = new List<Duel>();
            SqliteCommand sqlite_command = OpenSqlConnection(guildId);

            sqlite_command.CommandText = $"SELECT * FROM Duels{guildId}";
            SqliteDataReader sqliteDataReader = sqlite_command.ExecuteReader();

            while (sqliteDataReader.Read()) {
                Duel new_duel = new Duel(sqliteDataReader.GetString(1), sqliteDataReader.GetString(2), sqliteDataReader.GetString(3));

                new_duel.Id = sqliteDataReader.GetInt32(0);

                duels.Add(new_duel);
            }

            sqliteDataReader.Close();

            return duels;
        }
    }
}
