using Npgsql;
using PanneauSolaire.Models.Cnx;

namespace PanneauSolaire.Models.Entity
{
    public class Meteo
    {
        private int _id;
        private DateOnly _jour;
        private TimeOnly _heure;
        private double _lumiere;
        private double _lumiereMax;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public DateOnly Jour
        {
            get { return _jour; }
            set { _jour = value; }
        }
        public TimeOnly Heure
        {
            get { return _heure; }
            set { _heure = value; }
        }
        public double Lumiere
        {
            get { return _lumiere; }
            set { _lumiere = value; }
        }
        public double LumiereMax
        {
            get { return _lumiereMax; }
            set { _lumiereMax = value; }
        }

        public Meteo()
        {
        }
        public Meteo(int id, DateOnly jour, TimeOnly heure, double lumiere, double lumiereMax)
        {
            Id = id;
            Jour = jour;
            Heure = heure;
            Lumiere = lumiere;
            LumiereMax = lumiereMax;
        }
        public Meteo(DateOnly jour, TimeOnly heure, double lumiere, double lumiereMax)
        {
            Jour = jour;
            Heure = heure;
            Lumiere = lumiere;
            LumiereMax = lumiereMax;
        }

        public static Meteo[]? getMeteos(NpgsqlConnection cnx, DateOnly jour)
        {
            Meteo[]? meteos = null;
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            NpgsqlCommand command;

            string countSql = "SELECT COUNT(*) AS ligne FROM Meteo WHERE jour=@jour";
            int row = 0;
            using(command = new NpgsqlCommand(countSql, cnx))
            {
                command.Parameters.AddWithValue("@jour", jour);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            row = Convert.ToInt32(reader["ligne"]);
                        }
                    }
                }
                if (row == 0)
                {
                    throw new Exception("GET METEO FROM THIS DATE HASN'T ANY ROW");
                }
                else
                {
                    meteos = new Meteo[row];
                }
            }
            string sql = "SELECT * FROM Meteo WHERE jour=@jour";
            using(command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@jour", jour);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            meteos[i] = new Meteo(Convert.ToInt32(reader["id"]), DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                TimeOnly.FromTimeSpan((TimeSpan)reader["heure"]), Convert.ToDouble(reader["lumiere"]), Convert.ToDouble(reader["lumieremax"])); 
                            i++;
                        }
                    }
                    else
                    {
                        throw new Exception("GET METEO FROM THIS DATE HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return meteos;
        }

        public static Meteo? getMeteoPrecis(NpgsqlConnection cnx, DateOnly jour, TimeOnly heure)
        {
            Meteo? meteo = null;
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM Meteo WHERE jour=@jour AND heure=@heure";
            using(NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@jour", jour);
                command.Parameters.AddWithValue("@heure", heure);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            meteo = new Meteo(Convert.ToInt32(reader["id"]), DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                TimeOnly.FromTimeSpan((TimeSpan)reader["heure"]), Convert.ToDouble(reader["lumiere"]), Convert.ToDouble(reader["lumieremax"]));
                        }
                    }
                    else
                    {
                        throw new Exception($"GET METEO BY DAY:{jour} AND HOUR:{heure} FROM METEO HASN'T ANY ROW (Pas de donnees meteorologique de cet heure dans la base)");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return meteo;
        }

        public static Meteo? getMeteoAssezProche(NpgsqlConnection cnx, DateOnly jour, TimeOnly heure)
        {
            Meteo? meteo = null;
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT* FROM Meteo WHERE jour=@jour AND ABS(EXTRACT(EPOCH FROM (heure::time - @heure::time)) / 60) <= 30;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@jour", jour);
                command.Parameters.AddWithValue("@heure", heure);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            meteo = new Meteo(Convert.ToInt32(reader["id"]), DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                TimeOnly.FromTimeSpan((TimeSpan)reader["heure"]), Convert.ToDouble(reader["lumiere"]), Convert.ToDouble(reader["lumieremax"]));
                        }
                    }
                    else
                    {
                        throw new Exception($"GET METEO BY DAY:{jour} AND HOUR:{heure} FROM METEO HASN'T ANY ROW (Pas de donnees meteorologique de cet heure dans la base)");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return meteo;
        }
    }
}
