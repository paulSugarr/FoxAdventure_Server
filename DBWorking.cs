using System;
using System.Data.SQLite;

namespace Server
{
    public class DBWorking
    {
        private static string dbFileName = "players.db3";
        private static string connectionString = string.Format("Data source = {0}; Version = 3;", dbFileName);

        public static bool ExistsUser(string login)
        {
            string sql = "SELECT Login FROM Accounts WHERE Login = @Login";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Login", System.Data.DbType.String);
                cmd.Parameters["@Login"].Value = login;
                string answer;
                try
                {
                    conn.Open();
                    answer = cmd.ExecuteScalar().ToString();
                    if (answer == login) { return true; }
                    else { return false; }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static bool RegisterAccount(string login, string password)
        {
            if (ExistsUser(login)) { return false; }
            string sql =
                "INSERT INTO Accounts (Login, Password) VALUES (@Login, @Password); INSERT INTO Score (Score) VALUES ('0');";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Login", System.Data.DbType.String);
                cmd.Parameters["@Login"].Value = login;
                cmd.Parameters.Add("@Password", System.Data.DbType.String);
                cmd.Parameters["@Password"].Value = password;
                try
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static string GetPassword(string login)
        {
            string sql = "SELECT Password FROM Accounts WHERE Login = @Login";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Login", System.Data.DbType.String);
                cmd.Parameters["@Login"].Value = login;
                string answer;
                try
                {
                    conn.Open();
                    answer = cmd.ExecuteScalar().ToString();
                    return answer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public static void SetScore(string login, int score)
        {
            int id = GetId(login);

            string sql = "UPDATE Score SET Score=@Score WHERE IdAccount=@Id";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
                cmd.Parameters["@Id"].Value = id;
                cmd.Parameters.Add("@Score", System.Data.DbType.Int32);
                cmd.Parameters["@Score"].Value = score;
                try
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        public static int GetId(string login)
        {
            string sql = "SELECT Id FROM Accounts WHERE Login = @Login";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Login", System.Data.DbType.String);
                cmd.Parameters["@Login"].Value = login;
                int answer;
                try
                {
                    conn.Open();
                    answer = Convert.ToInt32(cmd.ExecuteScalar());
                    return answer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }
        }

        public static int GetScore(string login)
        {
            int id = GetId(login);
            string sql = "SELECT Score FROM Score WHERE IdAccount = @Id";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
                cmd.Parameters["@Id"].Value = id;
                int answer;
                try
                {
                    conn.Open();
                    answer = Convert.ToInt32(cmd.ExecuteScalar());
                    return answer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }


        }
    }
}
