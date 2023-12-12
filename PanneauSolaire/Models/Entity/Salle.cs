using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Objects;

namespace PanneauSolaire.Models.Entity
{
    public class Salle
    {
        private string _id = "";
        private string _refs = "";
        private double _consMoyenne;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Refs
        {
            get { return _refs; }
            set { _refs = value; }
        }
        public double ConsMoyenne
        {
            get { return _consMoyenne; }
            set { _consMoyenne = value; }
        }

        public Salle()
        {
        }

        public Salle(string? id, string? refs, double consMoyenne)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            ConsMoyenne = consMoyenne;
        }

        public Salle(string? refs, double consMoyenne)
        {
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            ConsMoyenne = consMoyenne;
        }

        /* ------- CRUD ------- */
        
        public static Salle? getById(NpgsqlConnection cnx, string id)
        {
            Salle? salle = null;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM Salle WHERE id=@id";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("id", id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            salle = new Salle(Convert.ToString(reader["id"]), Convert.ToString(reader["refs"]), 
                                Convert.ToDouble(reader["consmoyenne"]));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"GET SALLE BY ID {id} HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return salle;
        } 
        public List<InfoSalle> GetInfoSalles(NpgsqlConnection cnx, DateOnly jour)
        {
            List<InfoSalle> infosSalles = new List<InfoSalle>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM InfosSalle WHERE idSalle=@idSalle and jour=@jour";
            using(NpgsqlCommand command = new NpgsqlCommand(sql, cnx)){
                command.Parameters.AddWithValue("@idSalle", this.Id);
                command.Parameters.AddWithValue("@jour", jour);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        InfoSalle? infoSalle = null;
                        while (reader.Read())
                        {   
                            infoSalle = new InfoSalle(Convert.ToInt32(reader["id"]), Convert.ToString(reader["idSalle"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])), TimeOnly.FromTimeSpan((TimeSpan)reader["heuredebut"]),
                                TimeOnly.FromTimeSpan((TimeSpan)reader["heurefin"]), Convert.ToDouble(reader["nombrepersonne"]));
                            infosSalles.Add(infoSalle);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET InfosDalle FROM SALLE HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return infosSalles;
        }

        /* ----- FIN CRUD ----- */
        public static double MoyenneConsommationParSalle(List<Salle> salles)
        {
            double consommation = 0;
            if (salles.Count > 0)
            {
                foreach (Salle salle in salles)
                {
                    consommation += salle.ConsMoyenne;
                }
                consommation = (consommation / salles.Count);
            }
            return consommation;
        }
        public InfoPersonnes? PersonnePresent(NpgsqlConnection cnx, DateOnly jour, TimeOnly heure)
        {
            InfoPersonnes? infos = null;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM infosalle WHERE idsalle=@idsalle AND jour=@jour AND " +
                "heureDebut<=@heure1 AND heureFin>@heure2";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idsalle", this.Id);
                command.Parameters.AddWithValue("@jour", jour);
                command.Parameters.AddWithValue("@heure1", heure);
                command.Parameters.AddWithValue("@heure2", heure);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader["nombrePersonne"] != DBNull.Value)
                            {
                                infos = new InfoPersonnes(Convert.ToString(reader["idsalle"]), jour, heure, Convert.ToDouble(reader["nombrePersonne"]));
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"GET InfoPersonnes ({this.Refs}, {jour}, {heure}) FROM SALLE HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return infos;
        }

        public static double totalNombrePersonnesPresents(NpgsqlConnection cnx, DateOnly jour, TimeOnly heureActuelle, List<Salle> salles)
        {
            double nbPersonnePresent = 0;

            bool isclosed = false;
            if(cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            foreach (Salle salle in salles)
            {
                InfoPersonnes? infoPersonnesPresent = salle.PersonnePresent(cnx, jour, heureActuelle);
                if (infoPersonnesPresent == null)
                {
                    /* --- Manao analyse de donnees @ Presence par salle tany aloha --- */
                    Console.WriteLine("Mila manao analyse de donnees tany aloha: ");
                    nbPersonnePresent += salle.AnalysePersonnesPresentJourDeLaSemaine(cnx, jour, heureActuelle);
                }
                else
                {
                    nbPersonnePresent += infoPersonnesPresent.NbPersonne;
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return nbPersonnePresent;
        }

        public double AnalysePersonnesPresentJourDeLaSemaine(NpgsqlConnection cnx, DateOnly jour, TimeOnly heure)
        {
            double nombrePersonnes = 0;
            int dayOfWeek = (int)jour.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            List<InfoPersonnes> infosPersonnes = new List<InfoPersonnes>();

            string sql = "SELECT * FROM infosalle WHERE idsalle=@idsalle AND " +
                "(EXTRACT(IsoDoW FROM infosalle.jour)=@dayOfWeek) AND heureDebut<=@heure1 AND heureFin>@heure2";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idsalle", this.Id);
                command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                command.Parameters.AddWithValue("@heure1", heure);
                command.Parameters.AddWithValue("@heure2", heure);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        InfoPersonnes? infoPersonne = null;
                        while (reader.Read())
                        {
                            if (reader["nombrePersonne"] != DBNull.Value)
                            {
                                if (reader["jour"] != DBNull.Value && reader["heuredebut"] != DBNull.Value && reader["heurefin"] != DBNull.Value)
                                {
                                    infoPersonne = new InfoPersonnes(Convert.ToString(reader["idsalle"]), 
                                        DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])), heure, 
                                        Convert.ToDouble(reader["nombrePersonne"]));

                                    infosPersonnes.Add(infoPersonne);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"GET InfoPersonnes ({this.Refs}, {jour}, {heure}) FROM SALLE HASN'T ANY ROW");
                    }
                }
            }

            double totalePersonnes = 0;
            foreach (InfoPersonnes infoPersonne in infosPersonnes)
            {
                totalePersonnes += infoPersonne.NbPersonne;
            }
            if (infosPersonnes.Count > 0)
            {
                nombrePersonnes = totalePersonnes / infosPersonnes.Count;
            }

            if (isclosed)
            {
                cnx.Close();
            }

            return Convert.ToInt32(nombrePersonnes);
        }
    }
}
