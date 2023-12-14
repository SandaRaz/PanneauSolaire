using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.Objects;

namespace PanneauSolaire.Models.Specific
{
    public class Coupure
    {
        private string _id = "";
        private string _idSecteur = "";
        private DateOnly _jour;
        private TimeOnly? _heureCoupure;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string IdSecteur
        {
            get { return _idSecteur; }
            set { _idSecteur = value; }
        }
        public DateOnly Jour
        {
            get { return _jour; }
            set { _jour = value; }
        }
        public TimeOnly? HeureCoupure
        {
            get { return _heureCoupure; }
            set { _heureCoupure = value; }
        }

    /* ------- Constructor ------- */
        public Coupure()
        {
        }

        public Coupure(string? id, string? idSecteur, DateOnly jour, TimeOnly? heureCoupure)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IdSecteur = idSecteur ?? throw new ArgumentNullException(nameof(idSecteur));
            Jour = jour;
            HeureCoupure = heureCoupure;
        }
        public Coupure(string? idSecteur, DateOnly jour, TimeOnly? heureCoupure)
        {
            IdSecteur = idSecteur ?? throw new ArgumentNullException(nameof(idSecteur));
            Jour = jour;
            HeureCoupure = heureCoupure;
        }

        /* --------------------------- */

        /* ---------- CRUD ---------- */
        public int Save(NpgsqlConnection cnx)
        {
            int affectedRow = 0;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "INSERT INTO Coupure(id,idsecteur,jour,heurecoupure)" +
                " VALUES(@id,@idsecteur,@jour,@heurecoupure)";
            using(NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                string newid = "";
                if (this.Id == null)
                {
                    newid = Connex.createId(cnx, "sequence_coupure", "COU", 7);
                }
                else
                {
                    newid = this.Id;
                }

                command.Parameters.AddWithValue("@id", newid);
                command.Parameters.AddWithValue("@idsecteur", this.IdSecteur);
                command.Parameters.AddWithValue("@jour", this.Jour);
                if (this.HeureCoupure != null)
                {
                    command.Parameters.AddWithValue("@heurecoupure", this.HeureCoupure);
                }

                affectedRow = command.ExecuteNonQuery();
            }


            if (isclosed)
            {
                cnx.Close();
            }
            return affectedRow;
        }
        
        public static Coupure? getCoupure(NpgsqlConnection cnx, string idSecteur, DateOnly jour)
        {
            Coupure? coupure = null;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM coupure WHERE idsecteur=@idsecteur AND jour=@jour";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idsecteur", idSecteur);
                command.Parameters.AddWithValue("@jour", jour);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Object? hCoup = reader["heurecoupure"];
                            TimeOnly? heurecoupure = (hCoup != DBNull.Value) ? TimeOnly.FromTimeSpan((TimeSpan)hCoup) : null;
                            
                            coupure = new Coupure(Convert.ToString(reader["id"]), Convert.ToString(reader["idsecteur"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                heurecoupure);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return coupure;
        }
        public static List<Coupure> getCoupures(NpgsqlConnection cnx)
        {
            List<Coupure> coupures = new List<Coupure>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM coupure";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Coupure? coupure = null;
                        while (reader.Read())
                        {
                            Object? hCoup = reader["heurecoupure"];
                            TimeOnly? heurecoupure = (hCoup != DBNull.Value) ? TimeOnly.FromTimeSpan((TimeSpan)hCoup) : null;

                            coupure = new Coupure(Convert.ToString(reader["id"]), Convert.ToString(reader["idsecteur"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                heurecoupure);

                            coupures.Add(coupure);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return coupures;
        }

        public static List<Coupure> getCoupures(NpgsqlConnection cnx, string idSecteur)
        {
            List<Coupure> coupures = new List<Coupure>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM coupure WHERE idsecteur=@idsecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idsecteur", idSecteur);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Coupure? coupure = null;
                        while (reader.Read())
                        {
                            Object? hCoup = reader["heurecoupure"];
                            TimeOnly? heurecoupure = (hCoup != DBNull.Value) ? TimeOnly.FromTimeSpan((TimeSpan)hCoup) : null;

                            coupure = new Coupure(Convert.ToString(reader["id"]), Convert.ToString(reader["idsecteur"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                heurecoupure);

                            coupures.Add(coupure);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return coupures;
        }
        public static List<Coupure> getCoupuresJourDeLaSemaine(NpgsqlConnection cnx, DateOnly jour)
        {
            List<Coupure> coupures = new List<Coupure>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            int dayOfWeek = (int)jour.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            string sql = "SELECT * FROM coupure WHERE (EXTRACT(IsoDoW FROM coupure.jour) = @dayOfWeek)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Coupure? coupure = null;
                        while (reader.Read())
                        {
                            Object? hCoup = reader["heurecoupure"];
                            TimeOnly? heurecoupure = (hCoup != DBNull.Value) ? TimeOnly.FromTimeSpan((TimeSpan)hCoup) : null;

                            coupure = new Coupure(Convert.ToString(reader["id"]), Convert.ToString(reader["idsecteur"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                heurecoupure);

                            coupures.Add(coupure);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return coupures;
        }

        public static List<Coupure> getCoupuresJourDeLaSemaine(NpgsqlConnection cnx, string idSecteur, DateOnly jour)
        {
            List<Coupure> coupures = new List<Coupure>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            int dayOfWeek = (int)jour.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            string sql = "SELECT * FROM coupure WHERE idsecteur=@idsecteur AND (EXTRACT(IsoDoW FROM coupure.jour) = @dayOfWeek)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idsecteur", idSecteur);
                command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Coupure? coupure = null;
                        while (reader.Read())
                        {
                            Object? hCoup = reader["heurecoupure"];
                            TimeOnly? heurecoupure = (hCoup != DBNull.Value) ? TimeOnly.FromTimeSpan((TimeSpan)hCoup) : null;

                            coupure = new Coupure(Convert.ToString(reader["id"]), Convert.ToString(reader["idsecteur"]),
                                DateOnly.FromDateTime(Convert.ToDateTime(reader["jour"])),
                                heurecoupure);

                            coupures.Add(coupure);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return coupures;
        }


        /* -------------------------- */

        public static DateTime? getHeurePrevisionCoupure(NpgsqlConnection cnx, Secteur secteur, DateOnly jour, TimeOnly heureDebut, TimeOnly heureFin, double consUnitaire)
        {
            /* --- collecting data from database --- */
            List<Panneau> panneaux = secteur.getPanneaux(cnx);
            List<Batterie> batteries = secteur.getBatteries(cnx);
            List<Salle> salles = secteur.getSalles(cnx);
            /* ------------------------------------- */

            DateTime? dateHeurePrevision = null;

            double trancheHeure = 1;
            DateTime dateHeureActuelle = new DateTime(jour.Year, jour.Month, jour.Day, heureDebut.Hour, heureDebut.Minute, 0);
            DateTime dateHeureFin = new DateTime(jour.Year, jour.Month, jour.Day, heureFin.Hour, heureFin.Minute, 0);
            DateTime dateHeureMaxJour = new DateTime(jour.Year, jour.Month, jour.Day, 23, 59, 0);
            while (dateHeureActuelle <= dateHeureFin)
            {
                /* --------- Capacite batteries --------- */
                double capBatteriesAct = Batterie.chargeDisponible(batteries);
                /* -------------------------------------- */
                /* --- Info sur nombre eleve et salle --- */
                double nbPersonnePresent = Salle.totalNombrePersonnesPresents(cnx, jour, TimeOnly.FromDateTime(dateHeureActuelle), salles);
                /* -------------------------------------- */
                /* ------ Meteo et Panneau solaire ------ */
                Console.WriteLine("Date actuellement (ao anaty Coupure.cs) >>>>> "+dateHeureActuelle);
                Meteo? meteoDuJour = Meteo.getMeteoAssezProche(cnx, jour, TimeOnly.FromDateTime(dateHeureActuelle));
                double capPanneauxAct = Panneau.PuissanceTotaleFournit(meteoDuJour, panneaux);
                /* -------------------------------------- */

                double totaleConsommation = nbPersonnePresent * (consUnitaire * trancheHeure);
                double chargeADeduireBatteries = totaleConsommation - (capPanneauxAct * trancheHeure);
                if (Batterie.IsanyBatterieMananaChargeDispo(batteries) > 0)
                {
                    Batterie.VerifierAutresSecteur(cnx, secteur.Id, jour, TimeOnly.FromDateTime(dateHeureActuelle), consUnitaire, trancheHeure, batteries);

                    if (Batterie.chargeDisponible(batteries) >= chargeADeduireBatteries)
                    {
                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);
                    }
                    else /* ------ Coupure de courant ------ */
                    {
                        double resteBatterie = Batterie.chargeDisponible(batteries);
                        double hPrev = (resteBatterie / (chargeADeduireBatteries));

                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                        dateHeurePrevision = dateHeureActuelle.AddHours(hPrev);
                        break;
                    }
                }
                else /* ------ Coupure de courant ------ */
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------");
                    double resteBatterie = Batterie.chargeDisponible(batteries);
                    double hPrev = (resteBatterie / (chargeADeduireBatteries));

                    Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                    dateHeurePrevision = dateHeureActuelle.AddHours(hPrev);
                    break;
                }

                dateHeureActuelle = dateHeureActuelle.AddHours(trancheHeure);

                if (dateHeureActuelle >= dateHeureMaxJour)
                {
                    jour = jour.AddDays(1);
                    dateHeureActuelle = new DateTime(jour.Year, jour.Month, jour.Day, dateHeureActuelle.Hour, dateHeureActuelle.Minute, 0);
                    var heures = secteur.getHeureDebutFin(cnx, jour);
                    heureFin = heures.Item2;
                    dateHeureFin = new DateTime(jour.Year, jour.Month, jour.Day, heureFin.Hour, heureFin.Minute, 0);
                    dateHeureMaxJour = new DateTime(jour.Year, jour.Month, jour.Day, 23, 59, 0);
                }
            }
            Console.WriteLine($"Pour ConsMoy {consUnitaire} coupure dans {TimeOnly.FromDateTime(dateHeureActuelle)}");

            return dateHeurePrevision;
        }


        public static TimeOnly? getHeurePrevisionCoupure2(NpgsqlConnection cnx, Secteur secteur,DateOnly jour, TimeOnly heureDebut, TimeOnly heureFin, double consUnitaire)
        {
            /* --- collecting data from database --- */
            List<Panneau> panneaux = secteur.getPanneaux(cnx);
            List<Batterie> batteries = secteur.getBatteries(cnx);
            List<Salle> salles = secteur.getSalles(cnx);
            /* ------------------------------------- */

            TimeOnly? heurePrevision = null;
            double trancheHeure = 1;
            TimeOnly heureActuelle = heureDebut;
            while (heureActuelle <= heureFin)
            {
                /* --------- Capacite batteries --------- */
                double capBatteriesAct = Batterie.chargeDisponible(batteries);
                /* -------------------------------------- */
                /* --- Info sur nombre eleve et salle --- */
                double nbPersonnePresent = Salle.totalNombrePersonnesPresents(cnx, jour, heureActuelle, salles);
                /* -------------------------------------- */
                /* ------ Meteo et Panneau solaire ------ */
                Meteo? meteoDuJour = Meteo.getMeteoAssezProche(cnx, jour, heureActuelle);
                double capPanneauxAct = Panneau.PuissanceTotaleFournit(meteoDuJour, panneaux);
                /* -------------------------------------- */

                double totaleConsommation = nbPersonnePresent * (consUnitaire * trancheHeure);
                double chargeADeduireBatteries = totaleConsommation - (capPanneauxAct * trancheHeure);
                if (Batterie.IsanyBatterieMananaChargeDispo(batteries) > 0)
                {
                    Batterie.VerifierAutresSecteur(cnx, secteur.Id, jour, heureActuelle, consUnitaire, trancheHeure, batteries);

                    if (Batterie.chargeDisponible(batteries) >= chargeADeduireBatteries)
                    {
                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);
                    }
                    else /* ------ Coupure de courant ------ */
                    {
                        double resteBatterie = Batterie.chargeDisponible(batteries);
                        double hPrev = (resteBatterie / (chargeADeduireBatteries));

                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                        heurePrevision = heureActuelle.AddHours(hPrev);
                        break;
                    }
                }
                heureActuelle = heureActuelle.AddHours(trancheHeure);
            }
            Console.WriteLine($"Pour ConsMoy {consUnitaire} coupure dans {heurePrevision}");

            return heurePrevision;
        }
    
    }
}
