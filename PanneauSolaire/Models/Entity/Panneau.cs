using Npgsql;
using PanneauSolaire.Models.Cnx;

namespace PanneauSolaire.Models.Entity
{
    public class Panneau
    {
        private string _id = "";
        private string _refs = "";
        private double _puissance;

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
        public double Puissance
        {
            get { return _puissance; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Pourrais pas affecter une valeur negative au setters");
                }
                _puissance = value;
            }
        }

        public Panneau()
        {
        }

        public Panneau(string? id, string? refs, double puissance)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            Puissance = puissance;
        }
        public Panneau(string? refs, double puissance)
        {
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            Puissance = puissance;
        }

        /* ------- FONC. ------- */
        public double PuissanceFournit(Meteo meteo)
        {
            double puissanceFournit = (this.Puissance * meteo.Lumiere) / meteo.LumiereMax;
            return puissanceFournit;
        }

        public static double PuissanceTotaleFournit(Meteo? meteoDuJour, List<Panneau> panneaux)
        {
            double capPanneauxAct = 0;

            foreach (Panneau panneau in panneaux)
            {
                if (meteoDuJour != null)
                {
                    capPanneauxAct += panneau.PuissanceFournit(meteoDuJour);
                }
                else
                {
                    throw new Exception($"METEO NON COMPLET OU VIDE");
                }
            }

            return capPanneauxAct;
        }

        public static double PuissanceTotaleFournit(Meteo? meteoDuJour, Panneau[] panneaux)
        {
            double capPanneauxAct = 0;

            for(int i = 0; i < panneaux.Count(); i++)
            {
                if (meteoDuJour != null)
                {
                    capPanneauxAct += panneaux[i].PuissanceFournit(meteoDuJour);
                }
                else
                {
                    throw new Exception($"METEO NON COMPLET OU VIDE");
                }
            }

            return capPanneauxAct;
        }


        public static double PuissanceTotaleFournit(NpgsqlConnection cnx, Meteo? meteoDuJour, Secteur secteur, List<Panneau> panneaux)
        {
            double capPanneauxAct = 0;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            foreach (Panneau panneau in panneaux)
            {
                if (meteoDuJour != null)
                {
                    List<Secteur> secteurs = panneau.getSecteursUsingThisPanneau(cnx);

                    capPanneauxAct += panneau.PuissanceFournit(meteoDuJour);
                }
                else
                {
                    throw new Exception($"METEO NON COMPLET OU VIDE");
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return capPanneauxAct;
        }

        /* ----- FIN FONC. ----- */
        /* ------- CRUD -------- */
        public List<Secteur> getSecteursUsingThisPanneau(NpgsqlConnection cnx)
        {
            List<Secteur> secteurs = new List<Secteur>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM secteurpanneauxcomplet WHERE idpanneau=@idpanneau";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idpanneau", this.Id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Secteur? secteur = null;
                        while (reader.Read())
                        {
                            secteur = new Secteur(Convert.ToString(reader["idsecteur"]), Convert.ToString(reader["secteur"]));
                            secteurs.Add(secteur);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET LIST OF SECTEUR HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return secteurs;
        }

        public List<Secteur> getSecteursUsingThisPanneau(NpgsqlConnection cnx, string exceptIdSecteur)
        {
            List<Secteur> secteurs = new List<Secteur>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM secteurpanneauxcomplet WHERE idpanneau=@idpanneau AND idsecteur!=@idsecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idpanneau", this.Id);
                command.Parameters.AddWithValue("@idsecteur", exceptIdSecteur);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Secteur? secteur = null;
                        while (reader.Read())
                        {
                            secteur = new Secteur(Convert.ToString(reader["idsecteur"]), Convert.ToString(reader["secteur"]));
                            secteurs.Add(secteur);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET LIST OF SECTEUR HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return secteurs;
        }

        /* --------------------- */
    }
}
