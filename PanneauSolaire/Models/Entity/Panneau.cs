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
        public double PuissanceActuelle(Meteo meteo)
        {
            double puissanceActuelle = (this.Puissance * meteo.Lumiere) / meteo.LumiereMax;
            return puissanceActuelle;
        }

        /* ----- FIN FONC. ----- */
    }
}
