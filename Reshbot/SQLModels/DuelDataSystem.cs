using Microsoft.Data.Sqlite;
using Reshbot.ReshDiscordUtils;
using ReshUtils.Data;

namespace Reshbot.SQLModels {

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

            insert_command.CommandText = $"INSERT INTO Duels{guildId} (ChallengerId, ChallengedId, VictorId, TimeOfDuel, VictorResponseTime) " +
                $"VALUES ({duel.ChallengerId}, {duel.ChallengedId}, {duel.VictorId}, \"{duel.TimeOfDuel}\", {duel.VictorResponseTime})";

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
                "VictorId TEXT NOT NULL," +
                "TimeOfDuel TEXT NOT NULL," +
                "VictorResponseTime INTEGER NOT NULL);";

            sqlite_command.ExecuteNonQuery();
        }

        public DuelLeaderboard GetLeaderboard(string guildId) {
            DuelLeaderboard leaderboard = new DuelLeaderboard();

            SqliteDataReader reader = SelectDuelsWithQuery($"VictorId, COUNT(VictorId) FROM Duels{guildId} " +
                            "GROUP BY VictorId " +
                            "ORDER BY COUNT(VictorId) DESC " +
                            "LIMIT 15;", guildId);

            while (reader.Read()) {
                leaderboard.Rows.Add(new DuelLeaderboardRow(reader.GetString(0), reader.GetInt32(1)));
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
                Duel new_duel = new Duel(sqliteDataReader.GetString(1), sqliteDataReader.GetString(2), sqliteDataReader.GetString(3), sqliteDataReader.GetDateTime(4), sqliteDataReader.GetInt32(5));

                new_duel.Id = sqliteDataReader.GetInt32(0);

                duels.Add(new_duel);
            }

            sqliteDataReader.Close();

            return duels;
        }

        public DuelStats GetDuelStats(string guildId, string userId) {
            SqliteDataReader reader = SelectDuelsWithQuery($"COUNT(VictorId), COUNT(ID), AVG(VictorResponseTime) " +
                                                           $"FROM Duels{guildId} " +
                                                           $"WHERE (ChallengedId = {userId} OR ChallengerId = {userId});", guildId);

            object stats = null;

            while (reader.Read()) {
                stats = new DuelStats(userId, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(0) / reader.GetInt32(1) * 100, reader.GetInt32(2));
            }

            if(stats != null)
                return (DuelStats)stats;
            else
                return null;
        }
    }

    public class Duel : ISQLModel {
        public int Id;
        public string ChallengerId;
        public string ChallengedId;
        public string VictorId;
        public DateTime TimeOfDuel;
        public int VictorResponseTime;

        public Duel(string challenger_id, string challenged_id, string victor_id, DateTime timeOfDuel, int victorResponseTime) {
            ChallengerId = challenger_id;
            ChallengedId = challenged_id;
            VictorId = victor_id;
            TimeOfDuel = timeOfDuel;
            VictorResponseTime = victorResponseTime;
        }
    }

    public class DuelLeaderboard {
        public List<DuelLeaderboardRow> Rows = new List<DuelLeaderboardRow>();

        public override string ToString() {
            string result = "";

            foreach (var row in Rows) {
                result += $"\n<@{row.UserId}> --- {row.Wins}";
            }

            return result;
        }
    }

    public class DuelLeaderboardRow {
        public string UserId;
        public int Wins;

        public DuelLeaderboardRow(string userId, int wins) {
            UserId = userId;
            Wins = wins;
        }
    }

    public class DuelStats {
        public string UserId;
        public int NumberOfWins;
        public int NumberOfDuels;
        public int WinRate;
        public int AverageResponseTime;

        public DuelStats(string userId, int numberOfWins, int numberOfDuels, int winRate, int averageResponseTime) {
            UserId = userId;
            NumberOfWins = numberOfWins;
            NumberOfDuels = numberOfDuels;
            WinRate = winRate;
            AverageResponseTime = averageResponseTime;
        }
    }
}
