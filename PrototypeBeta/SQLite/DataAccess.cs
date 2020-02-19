using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using PrototypeBeta.Util;
using System.Collections.ObjectModel;

namespace PrototypeBeta.SQLite
{
    class DataAccess
    {
        private static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "trainschedule.db"));
        private static SQLiteConnection dbConn;
        private static SQLiteAsyncConnection _dbConn;

        public static void createConnection()
        {
            if (dbConn == null)
            {
                dbConn = new SQLiteConnection(DB_PATH);
            }
            
        }

        public void closeConnection()
        {

            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        public static void createTables()
        {
            createConnection();
            dbConn.CreateTable<Recent_Search_Table>();
            dbConn.CreateTable<Favorite_Schedule_Table>();
        }
        public static List<Recent_Search_Table> getRecentSearches()
        {
            List<Recent_Search_Table> retrievedData = dbConn.Table<Recent_Search_Table>().ToList<Recent_Search_Table>();
            return retrievedData;
        }

        public static void addRecentSearch(Recent_Search_Table recentSeachRecord)
        {
            if (!checkExistRecentSearch(recentSeachRecord))
            {
                handleExceedMaxLimit();
                dbConn.Insert(recentSeachRecord);
            }

        }
        public static void removeRecentSearches()
        {
            dbConn.DeleteAll<Recent_Search_Table>();
        }
        public static void removeRecentSearch(Recent_Search_Table recentSeachRecord)
        {
            dbConn.Delete(recentSeachRecord);

        }

        public static List<Favorite_Schedule_Table> getFavoriteSchedule()
        {

            dbConn.CreateTable<Favorite_Schedule_Table>();
            List<Favorite_Schedule_Table> retrievedData = dbConn.Table<Favorite_Schedule_Table>().ToList<Favorite_Schedule_Table>();

            return retrievedData;
        }

        public static void adFavoriteSchedule(Favorite_Schedule_Table favoriteSchedule)
        {
            dbConn.Insert(favoriteSchedule);
        }

        public static void removeFavoriteSchedules()
        {
            dbConn.DeleteAll<Favorite_Schedule_Table>();
        }
        public static void removeFavoriteSchedule(Favorite_Schedule_Table favoriteSchedule)
        {
            dbConn.Delete(favoriteSchedule);
        }
        public static void modifyFavoriteSchedule(Favorite_Schedule_Table favoriteSchedule)
        {
            dbConn.Update(favoriteSchedule);

        }

        static bool checkExistRecentSearch(Recent_Search_Table recentSeachRecord)
        {

            List<Recent_Search_Table> resultsList = dbConn.Query<Recent_Search_Table>(SQL.querycheckExistRecentSearch, recentSeachRecord.startStationId, recentSeachRecord.endStationId, recentSeachRecord.startTime, recentSeachRecord.endTime);

            if (resultsList.Count > 0)
            {
                return true;
            }

            return false;
        }

        static void handleExceedMaxLimit()
        {

            List<Recent_Search_Table> retrievedData = dbConn.Table<Recent_Search_Table>().ToList<Recent_Search_Table>();
            if (retrievedData.Count >= APPCONTEXT.MAX_RECENT_SEARCH)
            {
                dbConn.Delete(retrievedData[retrievedData.Count - 1]);
            }

        }



    }
}
