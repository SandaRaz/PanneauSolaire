using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Objects;
using PanneauSolaire.Models.Specific;
using PanneauSolaire.Models.ViewStruct;

namespace PanneauSolaire.Models.Entity
{
    public class Secteur
    {
        private string _id = "";
        private string _refs = "";

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

        /* --- constructor --- */
        public Secteur()
        {
        }

        public Secteur(string? id, string? refs)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
        }
        public Secteur(string? refs)
        {
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
        }

        /* ---- FONCTIONNALITE ---- */

        public double getConsommationUnitaireMoyenneParIteration(NpgsqlConnection cnx, DateOnly jour)
        {
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            double suppositionConsUnitaire = Salle.MoyenneConsommationParSalle(this.getSalles(cnx));
            Console.WriteLine($"    MOYENNE SUPPOSITION DE CONSOMMATION UNITAIRE: {suppositionConsUnitaire}");

            var heures = this.getHeureDebutFin(cnx, jour);
            TimeOnly heureDebut = heures.Item1;
            TimeOnly heureFin = heures.Item2;

            List<double> consommations = new List<double>();
            List<Coupure> coupures = Coupure.getCoupures(cnx);

            /* --- Iteration @ liste de coupure tany aloha --- */
            Console.WriteLine("***** --- Iteration @ liste de coupure tany aloha --- *****");
            foreach (Coupure coupure in coupures)
            {
                double incr = 0.3;
                DateTime? testPrevision = Coupure.getHeurePrevisionCoupure(cnx, this, jour, heureDebut, heureFin, suppositionConsUnitaire);

                if (testPrevision != null && coupure.HeureCoupure != null)
                {
                    DateTime dateHeurePrevision = (DateTime)testPrevision;
                    TimeOnly heureCoupure = (TimeOnly)coupure.HeureCoupure;
                    DateTime dateHeureCoupure = new DateTime(coupure.Jour.Year, coupure.Jour.Month, coupure.Jour.Day,
                        heureCoupure.Hour,heureCoupure.Minute,0);

                    if (dateHeurePrevision < dateHeureCoupure)
                    {
                        incr *= -1;
                    }

                    double vraiConsUnitaire = suppositionConsUnitaire;
                    double difference = (dateHeureCoupure > dateHeurePrevision) ?
                            Math.Abs((dateHeureCoupure - dateHeurePrevision).TotalMinutes) :
                            Math.Abs((dateHeurePrevision - dateHeureCoupure).TotalMinutes);

                    if (incr < 0)
                    {
                        while (dateHeurePrevision < dateHeureCoupure) /* dt > 10 minutes */
                        {
                            vraiConsUnitaire += incr;
                            testPrevision = Coupure.getHeurePrevisionCoupure(cnx, this, jour, heureDebut, heureFin, vraiConsUnitaire);
                            if (testPrevision != null)
                            {
                                dateHeurePrevision = (DateTime)testPrevision;
                            }
                            if (dateHeureCoupure == dateHeurePrevision)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        while (dateHeurePrevision > dateHeureCoupure)
                        {
                            vraiConsUnitaire += incr;
                            testPrevision = Coupure.getHeurePrevisionCoupure(cnx, this, jour, heureDebut, heureFin, vraiConsUnitaire);
                            if (testPrevision != null)
                            {
                                dateHeurePrevision = (DateTime)testPrevision;
                            }
                            if (dateHeureCoupure == dateHeurePrevision)
                            {
                                break;
                            }
                        }
                    }
                    consommations.Add(vraiConsUnitaire);
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }

            double result = 0;
            foreach (double cons in consommations)
            {
                result += cons;
            }

            if (consommations.Count > 0)
            {
                return result / consommations.Count;
            }
            else
            {
                return suppositionConsUnitaire;
            }
        }

        public List<Prevision> getDetailsPrevisions(NpgsqlConnection cnx, DateOnly jour)
        {
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            List<Prevision> previsions = new List<Prevision>();

            /* --- collecting data from database --- */
            List<Panneau> panneaux = this.getPanneaux(cnx);
            List<Batterie> batteries = this.getBatteries(cnx);
            List<Salle> salles = this.getSalles(cnx);
            /* ------------------------------------- */

            /* --- alaina ny heure voalohany sy ny heure farany ao anaty Presence(InfoSalle) --- */
            var heures = this.getHeureDebutFin(cnx, jour);
            TimeOnly heureDebut = heures.Item1;
            TimeOnly heureFin = heures.Item2;
            /* --------------------------------------------------------------------------------- */

            double trancheHeure = 1;
            DateTime dateHeureActuelle = new DateTime(jour.Year,jour.Month,jour.Day,heureDebut.Hour,heureDebut.Minute,0);
            DateTime dateHeureFin = new DateTime(jour.Year, jour.Month, jour.Day, heureFin.Hour, heureFin.Minute, 0);
            DateTime dateHeureMaxJour = new DateTime(jour.Year, jour.Month, jour.Day, 23, 59, 0);

            while (dateHeureActuelle <= dateHeureFin)
            {
                /* --------- Capacite batteries --------- */
                double capBatteriesAct = Batterie.chargeDisponible(batteries);
                /* -------------------------------------- */
                /* --- Info sur nombre eleve et salle --- */
                double nbPersonnePresent = Salle.totalNombrePersonnesPresents(cnx, DateOnly.FromDateTime(dateHeureActuelle), 
                    TimeOnly.FromDateTime(dateHeureActuelle), salles);
                /* -------------------------------------- */
                /* --- Moyenne consommation par eleve --- */
                double moyenneConsUnitaire = this.getConsommationUnitaireMoyenneParIteration(cnx, jour);
                /* -------------------------------------- */
                /* ------ Meteo et Panneau solaire ------ */
                Console.WriteLine("Dernier heure actuellement >>>>> "+dateHeureActuelle);
                Meteo? meteoDuJour = Meteo.getMeteoAssezProche(cnx, jour, TimeOnly.FromDateTime(dateHeureActuelle));
                double capPanneauxAct = Panneau.PuissanceTotaleFournit(meteoDuJour, panneaux);
                /* -------------------------------------- */

                previsions.Add(new Prevision(jour, TimeOnly.FromDateTime(dateHeureActuelle), meteoDuJour, capPanneauxAct, Batterie.CapaciteReele(batteries),
                    nbPersonnePresent, moyenneConsUnitaire));

                double consTotale = nbPersonnePresent * (moyenneConsUnitaire * trancheHeure);
                double chargeADeduireBatteries = consTotale - (capPanneauxAct * trancheHeure);
                Console.WriteLine("Charge a deduire batterie: " + chargeADeduireBatteries);
                Console.WriteLine("Batterie.IsanyBatterieMananaChargeDispo(batteries): " + Batterie.IsanyBatterieMananaChargeDispo(batteries));
                if (Batterie.IsanyBatterieMananaChargeDispo(batteries) > 0)
                {
                    Batterie.VerifierAutresSecteur(cnx, this.Id, jour, TimeOnly.FromDateTime(dateHeureActuelle), moyenneConsUnitaire, trancheHeure, batteries);

                    if (Batterie.chargeDisponible(batteries) >= chargeADeduireBatteries)
                    {
                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);
                    }
                    else /* ------ Coupure de courant ------ */
                    {
                        double resteBatterie = Batterie.chargeDisponible(batteries);
                        double hPrev = (resteBatterie / (chargeADeduireBatteries));
                        DateTime heureCoupure = dateHeureActuelle.AddHours(hPrev);

                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                        previsions.Add(new Prevision(jour, TimeOnly.FromDateTime(heureCoupure), meteoDuJour, capPanneauxAct, Batterie.CapaciteReele(batteries),
                        nbPersonnePresent, moyenneConsUnitaire));

                        break;
                    }
                }
                else /* ------ Coupure de courant ------ */
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------");
                    double resteBatterie = Batterie.chargeDisponible(batteries);
                    double hPrev = (resteBatterie / (chargeADeduireBatteries));
                    DateTime heureCoupure = dateHeureActuelle.AddHours(hPrev);

                    Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                    previsions.Add(new Prevision(jour, TimeOnly.FromDateTime(heureCoupure), meteoDuJour, capPanneauxAct, Batterie.CapaciteReele(batteries),
                    nbPersonnePresent, moyenneConsUnitaire));

                    break;
                }

                Console.WriteLine($"Heure: {TimeOnly.FromDateTime(dateHeureActuelle)},panneauCap: {capPanneauxAct}, capBat: {capBatteriesAct} , totalCons: {consTotale}");

                dateHeureActuelle = dateHeureActuelle.AddHours(trancheHeure);
                
                if (dateHeureActuelle >= dateHeureMaxJour)
                {
                    jour = jour.AddDays(1);
                    dateHeureActuelle = new DateTime(jour.Year, jour.Month, jour.Day, dateHeureActuelle.Hour, dateHeureActuelle.Minute, 0);
                    heures = this.getHeureDebutFin(cnx, jour);
                    heureFin = heures.Item2;
                    dateHeureFin = new DateTime(jour.Year, jour.Month, jour.Day, heureFin.Hour, heureFin.Minute, 0);
                    dateHeureMaxJour = new DateTime(jour.Year, jour.Month, jour.Day, 23, 59, 0);
                }
                
            }

            if (isclosed)
            {
                cnx.Close();
            }

            return previsions;
        }


        public double getConsommationUnitaireMoyenneParIteration2(NpgsqlConnection cnx, DateOnly jour)
        {
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            double suppositionConsUnitaire = Salle.MoyenneConsommationParSalle(this.getSalles(cnx));
            Console.WriteLine($"    MOYENNE SUPPOSITION DE CONSOMMATION UNITAIRE: {suppositionConsUnitaire}");

            var heures = this.getHeureDebutFin(cnx, jour);
            TimeOnly heureDebut = heures.Item1;
            TimeOnly heureFin = heures.Item2;

            List<double> consommations = new List<double>();
            List<Coupure> coupures = Coupure.getCoupures(cnx);

            /* --- Iteration @ liste de coupure tany aloha --- */
            Console.WriteLine("***** --- Iteration @ liste de coupure tany aloha --- *****");
            foreach (Coupure coupure in coupures)
            {
                double incr = 0.3;
                TimeOnly? testPrevision = Coupure.getHeurePrevisionCoupure2(cnx, this, jour, heureDebut, heureFin, suppositionConsUnitaire);

                if (testPrevision != null && coupure.HeureCoupure != null)
                {
                    TimeOnly heurePrevision = (TimeOnly)testPrevision;
                    TimeOnly heureCoupure = (TimeOnly)coupure.HeureCoupure;

                    if (heurePrevision < heureCoupure)
                    {
                        incr *= -1;
                    }

                    double vraiConsUnitaire = suppositionConsUnitaire;
                    double difference = (heureCoupure > heurePrevision) ?
                            Math.Abs((heureCoupure - heurePrevision).TotalMinutes) :
                            Math.Abs((heurePrevision - heureCoupure).TotalMinutes);

                    if (incr < 0)
                    {
                        while (heurePrevision < heureCoupure) /* dt > 10 minutes */
                        {
                            vraiConsUnitaire += incr;
                            testPrevision = Coupure.getHeurePrevisionCoupure2(cnx, this, jour, heureDebut, heureFin, vraiConsUnitaire);
                            if (testPrevision != null)
                            {
                                heurePrevision = (TimeOnly)testPrevision;
                            }
                            if (heureCoupure == heurePrevision)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        while (heurePrevision > heureCoupure)
                        {
                            vraiConsUnitaire += incr;
                            testPrevision = Coupure.getHeurePrevisionCoupure2(cnx, this, jour, heureDebut, heureFin, vraiConsUnitaire);
                            if (testPrevision != null)
                            {
                                heurePrevision = (TimeOnly)testPrevision;
                            }
                            if (heureCoupure == heurePrevision)
                            {
                                break;
                            }
                        }
                    }
                    consommations.Add(vraiConsUnitaire);
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }

            double result = 0;
            foreach (double cons in consommations)
            {
                result += cons;
            }

            if (consommations.Count > 0)
            {
                return result / consommations.Count;
            }
            else
            {
                return suppositionConsUnitaire;
            }
        }
        public List<Prevision> getDetailsPrevisions2(NpgsqlConnection cnx, DateOnly jour)
        {
            bool isclosed = false;
            if(cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            List<Prevision> previsions = new List<Prevision>();

            /* --- collecting data from database --- */
            List<Panneau> panneaux = this.getPanneaux(cnx);
            List<Batterie> batteries = this.getBatteries(cnx);
            List<Salle> salles = this.getSalles(cnx);
            /* ------------------------------------- */

            /* --- alaina ny heure voalohany sy ny heure farany ao anaty Presence(InfoSalle) --- */
            var heures = this.getHeureDebutFin(cnx, jour);
            TimeOnly heureDebut = heures.Item1;
            TimeOnly heureFin = heures.Item2;
            /* --------------------------------------------------------------------------------- */
            
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
                /* --- Moyenne consommation par eleve --- */
                double moyenneConsUnitaire = this.getConsommationUnitaireMoyenneParIteration2(cnx, jour);
                /* -------------------------------------- */
                /* ------ Meteo et Panneau solaire ------ */
                Meteo? meteoDuJour = Meteo.getMeteoAssezProche(cnx, jour, heureActuelle);
                double capPanneauxAct = Panneau.PuissanceTotaleFournit(meteoDuJour, panneaux);
                /* -------------------------------------- */

                previsions.Add(new Prevision(jour, heureActuelle, meteoDuJour, capPanneauxAct, Batterie.CapaciteReele(batteries),
                    nbPersonnePresent, moyenneConsUnitaire));

                double consTotale = nbPersonnePresent * (moyenneConsUnitaire * trancheHeure);
                double chargeADeduireBatteries = consTotale - (capPanneauxAct * trancheHeure);
                Console.WriteLine("Charge a deduire batterie: "+chargeADeduireBatteries);
                Console.WriteLine("Batterie.IsanyBatterieMananaChargeDispo(batteries): " + Batterie.IsanyBatterieMananaChargeDispo(batteries));
                if (Batterie.IsanyBatterieMananaChargeDispo(batteries) > 0)
                {
                    Batterie.VerifierAutresSecteur(cnx, this.Id, jour, heureActuelle, moyenneConsUnitaire, trancheHeure, batteries);
                    
                    if (Batterie.chargeDisponible(batteries) >= chargeADeduireBatteries)
                    {
                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);
                    }
                    else /* ------ Coupure de courant ------ */
                    {
                        double resteBatterie = Batterie.chargeDisponible(batteries);
                        double hPrev = (resteBatterie / (chargeADeduireBatteries));
                        TimeOnly heureCoupure = heureActuelle.AddHours(hPrev);

                        Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteries);

                        previsions.Add(new Prevision(jour, heureCoupure, meteoDuJour, capPanneauxAct, Batterie.CapaciteReele(batteries),
                        nbPersonnePresent, moyenneConsUnitaire));

                        break;
                    }
                }

                Console.WriteLine($"Heure: {heureActuelle},panneauCap: {capPanneauxAct}, capBat: {capBatteriesAct} , totalCons: {consTotale}");

                heureActuelle = heureActuelle.AddHours(trancheHeure);
                /*
                if (heureActuelle >= new TimeOnly(23, 59))
                {
                    heureActuelle = new TimeOnly(00, 00);
                    jour = jour.AddDays(1);
                    heures = this.getHeureDebutFin(cnx, jour);
                    heureFin = heures.Item2;
                }
                */
            }

            if (isclosed)
            {
                cnx.Close();
            }

            return previsions;
        }

    /* ------ FIN FONC. ------- */

    /* ------- CRUD ------ */
        public static List<Secteur> getAll(NpgsqlConnection cnx)
        {
            List<Secteur> secteurs = new List<Secteur>();
            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM Secteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                using(NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Secteur? secteur = null;
                        while (reader.Read())
                        {
                            secteur = new Secteur(Convert.ToString(reader["id"]), Convert.ToString(reader["refs"]));
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

        public static Secteur? getById(NpgsqlConnection cnx, string id)
        {
            Secteur? secteur = null;

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM Secteur WHERE id=@id";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@id", id);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            secteur = new Secteur(Convert.ToString(reader["id"]), Convert.ToString(reader["refs"]));
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET SECTEUR BY ID HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return secteur;
        }

        public List<Panneau> getPanneaux(NpgsqlConnection cnx)
        {
            List<Panneau> panneaux = new List<Panneau>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM SecteurPanneauxComplet WHERE idSecteur=@idSecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idSecteur", this.Id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Panneau? panneau = null;
                        while (reader.Read())
                        {
                            panneau = new Panneau(Convert.ToString(reader["idpanneau"]), Convert.ToString(reader["panneau"]),
                                Convert.ToDouble(reader["puissance"]));
                            panneaux.Add(panneau);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET Salles FROM SECTEUR HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return panneaux;
        }

        public List<Batterie> getBatteries(NpgsqlConnection cnx)
        {
            List<Batterie> batteries = new List<Batterie>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM SecteurBatteriesComplet WHERE idSecteur=@idSecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idSecteur", this.Id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Batterie? batterie = null;
                        while (reader.Read())
                        {
                            batterie = new Batterie(Convert.ToString(reader["idbatterie"]), Convert.ToString(reader["batterie"]),
                                Convert.ToDouble(reader["puissance"]), Convert.ToDouble(reader["limitcons"]));
                            batteries.Add(batterie);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET Salles FROM SECTEUR HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return batteries;
        }

        public List<Salle> getSalles(NpgsqlConnection cnx)
        {
            List<Salle> salles = new List<Salle>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM SecteurSallesComplet WHERE idSecteur=@idSecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idSecteur", this.Id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Salle? salle = null;
                        while (reader.Read())
                        {
                            salle = new Salle(Convert.ToString(reader["idsalle"]), Convert.ToString(reader["salle"]), Convert.ToDouble(reader["consmoyenne"]));
                            salles.Add(salle);
                        }
                    }
                    else
                    {
                        Console.WriteLine("GET Salles FROM SECTEUR HASN'T ANY ROW");
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
            return salles;
        }

        public static List<InfoSalle> getInfosSalles(NpgsqlConnection cnx, List<Salle> salles, DateOnly jour)
        {
            List<InfoSalle> infosSalles = new List<InfoSalle>();
            try
            {
                List<InfoSalle>? infos;
                foreach (Salle salle in salles)
                {
                    infos = salle.GetInfoSalles(cnx, jour);
                    infosSalles.AddRange(infos);
                }
                infos = null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return infosSalles;
        }

        public (TimeOnly, TimeOnly) getHeureDebutFin(NpgsqlConnection cnx, DateOnly jour)
        {
            TimeOnly debut = new TimeOnly(08,00);
            TimeOnly fin = new TimeOnly(17,00);

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT min(heuredebut) as heuredebut,max(heurefin) as heurefin FROM secteursallesinfoscomplet WHERE idsecteur=@idsecteur and jour=@jour";
            using (NpgsqlCommand command = new NpgsqlCommand(sql,cnx))
            {
                command.Parameters.AddWithValue("@idsecteur", this.Id);
                command.Parameters.AddWithValue("@jour", jour);
                using(NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader["heuredebut"] != DBNull.Value && reader["heurefin"] != DBNull.Value)
                            {
                                debut = TimeOnly.FromTimeSpan((TimeSpan)reader["heuredebut"]);
                                fin = TimeOnly.FromTimeSpan((TimeSpan)reader["heurefin"]);
                            }
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }

            return (debut, fin);
        }
    /* ----- FIN CRUD ---- */
    }
}
