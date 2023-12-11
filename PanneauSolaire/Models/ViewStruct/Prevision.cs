using PanneauSolaire.Models.Entity;

namespace PanneauSolaire.Models.ViewStruct
{
    public class Prevision
    {
        DateOnly jour;
        TimeOnly heure;
        Meteo? meteo;
        double puissancePanneau;
        double puissanceBatterie;
        double nbPersonne;
        double consMoyenne;

        public DateOnly Jour
        {
            get { return jour; }
            set { jour = value; }
        }
        public TimeOnly Heure
        {
            get { return heure; }
            set { heure = value; }
        }
        public Meteo? Meteo
        {
            get { return meteo; }
            set { meteo = value; }
        }
        public double PuissancePanneau
        {
            get { return puissancePanneau; }
            set { puissancePanneau = Math.Round(value, 2); }
        }
        public double PuissanceBatterie
        {
            get { return puissanceBatterie; }
            set { puissanceBatterie = Math.Round(value, 2); }
        }
        public double NbPersonne
        {
            get { return nbPersonne; }
            set { nbPersonne = value; }
        }
        public double ConsMoyenne
        {
            get { return consMoyenne; }
            set { consMoyenne = Math.Round(value, 2); }
        }

        public Prevision(DateOnly jour, TimeOnly heure, Meteo? meteo, double puissancePanneau, double puissanceBatterie, double nbPersonne, double consMoyenne)
        {
            Jour = jour;
            Heure = heure;
            Meteo = meteo;
            PuissancePanneau = puissancePanneau;
            PuissanceBatterie = puissanceBatterie;
            NbPersonne = nbPersonne;
            ConsMoyenne = consMoyenne;
        }
    }
}
