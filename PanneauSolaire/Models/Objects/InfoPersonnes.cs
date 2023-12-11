namespace PanneauSolaire.Models.Objects
{
    public class InfoPersonnes
    {
        private string _idSalle = "";
        private DateOnly _jour;
        private TimeOnly _heure;
        private double _nbPersonne;

        public string IdSalle
        {
            get { return _idSalle; }
            set { _idSalle = value; }
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
        public double NbPersonne
        {
            get { return _nbPersonne; }
            set {
                if (value < 0)
                {
                    throw new Exception("La valeur en Set ne peut pas etre null");
                }
                _nbPersonne = value;
            }
        }

        public InfoPersonnes(string? idSalle, DateOnly jour, TimeOnly heure, double nbPersonne)
        {
            IdSalle = idSalle != null ? idSalle : "";
            Jour = jour;
            Heure = heure;
            NbPersonne = nbPersonne;
        }

        public InfoPersonnes(string? idSalle, DateTime jourHeure, TimeSpan heure, double nbPersonne)
        {
            IdSalle = idSalle != null ? idSalle : "";
            Jour = DateOnly.FromDateTime(jourHeure);
            Heure = TimeOnly.FromTimeSpan(heure);
            NbPersonne = nbPersonne;
        }
    }
}
