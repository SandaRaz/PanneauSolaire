using Npgsql;
using PanneauSolaire.Models.Cnx;

namespace PanneauSolaire.Models.Entity
{
    public class Batterie
    {
        private string _id = "";
        private string _refs = "";
        private double _puissance;
        private double _limitCons;
        private double _charge;

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

        public double LimitCons
        {
            get { return _limitCons; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Pourrais pas affecter une valeur negative au setters");
                }
                _limitCons = value;
            }
        }

        public double Charge
        {
            get { return _charge; }
            set { _charge = value; }
        }

        public Batterie()
        {
        }

        public Batterie(string? id, string? refs, double puissance, double limitCons)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            Puissance = puissance;
            LimitCons = limitCons;

            Charge = this.CapaciteDisponible();
        }
        public Batterie(string? refs, double puissance, double limitCons)
        {
            Refs = refs ?? throw new ArgumentNullException(nameof(refs));
            Puissance = puissance;
            LimitCons = limitCons;

            Charge = this.CapaciteDisponible();
        }

        public double CapaciteDisponible()
        {
            double disponible = (this.Puissance * this.LimitCons) / 100;
            return disponible;
        }

        public static double CapaciteReele(List<Batterie> batteries)
        {
            double chargeReelle = 0;
            foreach (Batterie batterie in batteries)
            {
                chargeReelle += batterie.Charge + ((batterie.Puissance * batterie.LimitCons) / 100);
            }
            return chargeReelle;
        }

        public static double puissanceTotale(List<Batterie> batteries)
        {
            double puissance = 0;
            foreach (Batterie batterie in batteries)
            {
                puissance += batterie.Puissance;
            }
            return puissance;
        }

        public static double chargeDisponible(List<Batterie> batteries)
        {
            double chargeTotale = 0;
            foreach(Batterie batterie in batteries)
            {
                chargeTotale += batterie.Charge;
            }
            return chargeTotale;
        }
        public static double chargeDisponible(Batterie[] batteries)
        {
            double chargeTotale = 0;
            for(int i = 0; i < batteries.Count(); i++)
            {
                chargeTotale += batteries[i].Charge;
            }
            return chargeTotale;
        }

        public static int IsanyBatterieMananaChargeDispo(List<Batterie> batteries)
        {
            int isany = 0;
            foreach(Batterie batterie in batteries)
            {
                if (batterie.Charge > 0)
                {
                    isany++;
                }
            }
            return isany;
        }
        public static int IsanyBatterieMananaChargeDispo(Batterie[] batteries)
        {
            int isany = 0;
            for(int i = 0; i < batteries.Count(); i++)
            {
                if (batteries[i].Charge > 0)
                {
                    isany++;
                }
            }
            return isany;
        }

        public static void VerifierAutresSecteur(NpgsqlConnection cnx, string idSecteur, DateOnly jour, TimeOnly heure, double consMoy, double trancheHeure, List<Batterie> batteries)
        {
            bool isclosed = false;
            if(cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            foreach(Batterie batterie in batteries)
            {
                List<Secteur> secteurs = batterie.getSecteursUsingThisBatterie(cnx, idSecteur);
                foreach(Secteur secteur in secteurs)
                {
                    List<Batterie> batteriesDuSecteur = secteur.getBatteries(cnx);
                    for(int i = 0; i< batteriesDuSecteur.Count; i++)
                    {
                        if (batteriesDuSecteur[i].Id.Equals(batterie.Id))
                        {
                            batteriesDuSecteur[i] = batterie;
                        }
                    }

                    /* --------- Capacite batteries --------- */
                    double capBatteriesAct = Batterie.chargeDisponible(batteriesDuSecteur);
                    /* -------------------------------------- */
                    /* --- Info sur nombre eleve et salle --- */
                    double nbPersonnePresent = Salle.totalNombrePersonnesPresents(cnx, jour, heure, secteur.getSalles(cnx));
                    /* -------------------------------------- */
                    /* ------ Meteo et Panneau solaire ------ */
                    Meteo? meteoDuJour = Meteo.getMeteoAssezProche(cnx, jour, heure);
                    double capPanneauxAct = Panneau.PuissanceTotaleFournit(meteoDuJour, secteur.getPanneaux(cnx));
                    /* -------------------------------------- */

                    double consTotale = nbPersonnePresent * (consMoy * trancheHeure);
                    double chargeADeduireBatteries = consTotale - (capPanneauxAct * trancheHeure);
                    if (Batterie.IsanyBatterieMananaChargeDispo(batteriesDuSecteur) > 0)
                    {
                        if (Batterie.chargeDisponible(batteriesDuSecteur) >= chargeADeduireBatteries)
                        {
                            Batterie.DeduireChargeAuBatteries(chargeADeduireBatteries, batteriesDuSecteur);
                        }
                    }
                }
            }

            if (isclosed)
            {
                cnx.Close();
            }
        }

        public static void DeduireChargeAuBatteries(double chargeADeduire, List<Batterie> batteries)
        {
            int possedeCharge = Batterie.IsanyBatterieMananaChargeDispo(batteries);
            double partADeduire = chargeADeduire / possedeCharge;

            int i = 0;
            while (i < batteries.Count)
            {
                if (chargeADeduire <= 0 || Batterie.chargeDisponible(batteries) <= 0)
                {
                    break;
                }

                if (batteries[i].Charge >= partADeduire)
                {
                    chargeADeduire -= partADeduire;
                    batteries[i].Charge -= partADeduire;
                }
                else
                {
                    chargeADeduire -= batteries[i].Charge;
                    batteries[i].Charge = 0;
                }
                i++;

                if (i == (batteries.Count) && (chargeADeduire > 0))
                {
                    possedeCharge = Batterie.IsanyBatterieMananaChargeDispo(batteries);
                    if (possedeCharge > 0)
                    {
                        partADeduire = chargeADeduire / possedeCharge;
                    }
                    i = 0;
                }
            }
        }

        public static void DeduireChargeAuBatteries(double chargeADeduire, Batterie[] batteries)
        {
            int possedeCharge = Batterie.IsanyBatterieMananaChargeDispo(batteries);
            double partADeduire = chargeADeduire / possedeCharge;

            int i = 0;
            while (i < batteries.Count())
            {
                if (chargeADeduire <= 0 || Batterie.chargeDisponible(batteries) <= 0)
                {
                    break;
                }

                if (batteries[i].Charge >= partADeduire)
                {
                    chargeADeduire -= partADeduire;
                    batteries[i].Charge -= partADeduire;
                }
                else
                {
                    chargeADeduire -= batteries[i].Charge;
                    batteries[i].Charge = 0;
                }
                i++;

                if (i == (batteries.Count()) && (chargeADeduire > 0))
                {
                    possedeCharge = Batterie.IsanyBatterieMananaChargeDispo(batteries);
                    if (possedeCharge > 0)
                    {
                        partADeduire = chargeADeduire / possedeCharge;
                    }
                    i = 0;
                }
            }
        }

        public static void ChargerBatterie(double chargePlus, List<Batterie> batteries)
        {
            int isanaBatterie = batteries.Count();
            if (isanaBatterie > 0)
            {
                double partCharge = chargePlus / isanaBatterie;
                double reste = 0;
                int i = 0;
                while (i < isanaBatterie)
                {
                    double puissanceValide = ((batteries[i].Puissance * 50) / 100);
                    if ((batteries[i].Charge + partCharge) > puissanceValide)
                    {
                        reste += (batteries[i].Charge + partCharge) - puissanceValide;
                    }
                    batteries[i].Charge = Math.Min((batteries[i].Charge + partCharge), puissanceValide);

                    i++;
                }
            }
        }
        public static void ChargerBatterie(double chargePlus, Batterie[] batteries)
        {
            int isanaBatterie = batteries.Count();
            if (isanaBatterie > 0)
            {
                double partCharge = chargePlus / isanaBatterie;
                double reste = 0;
                int i = 0;
                while (i < isanaBatterie)
                {
                    double puissanceValide = ((batteries[i].Puissance * 50) / 100);
                    if ((batteries[i].Charge + partCharge) > puissanceValide)
                    {
                        reste += (batteries[i].Charge + partCharge) - puissanceValide;
                    }
                    batteries[i].Charge = Math.Min((batteries[i].Charge + partCharge), puissanceValide);

                    i++;
                }
            }
        }


        /* ----- CRUD ----- */
        public List<Secteur> getSecteursUsingThisBatterie(NpgsqlConnection cnx)
        {
            List<Secteur> secteurs = new List<Secteur>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM secteurbatteriescomplet WHERE idbatterie=@idbatterie";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idbatterie", this.Id);
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

        public List<Secteur> getSecteursUsingThisBatterie(NpgsqlConnection cnx, string exceptIdSecteur)
        {
            List<Secteur> secteurs = new List<Secteur>();

            bool isclosed = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                cnx = Connex.getConnection();
                cnx.Open();
                isclosed = true;
            }

            string sql = "SELECT * FROM secteurbatteriescomplet WHERE idbatterie=@idbatterie AND idsecteur!=@idsecteur";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, cnx))
            {
                command.Parameters.AddWithValue("@idbatterie", this.Id);
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

        /* ---------------- */
    }
}
