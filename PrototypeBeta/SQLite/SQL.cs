using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.SQLite
{
    class SQL
    {
        public static string querycheckExistRecentSearch = "SELECT RECORDID FROM RECENT_SEARCH_TABLE WHERE STARTSTATIONID=? AND ENDSTATIONID=? AND STARTTIME=? AND ENDTIME=?";
    }
}
