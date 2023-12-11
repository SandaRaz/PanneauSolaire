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
                    partADeduire = chargeADeduire / possedeCharge;
                    i = 0;
                }
            }
        }
    }
}
