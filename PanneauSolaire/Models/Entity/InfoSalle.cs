namespace PanneauSolaire.Models.Entity
{
    public class InfoSalle
    {
        private int _id;
        private string _idSalle = "";
        private DateOnly _jour;
        private TimeOnly _heureDebut;
        private TimeOnly _heureFin;
        private double _nombrePersonne;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
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
        public TimeOnly HeureDebut
        {
            get { return _heureDebut; }
            set { _heureDebut = value; }
        }
        public TimeOnly HeureFin
        {
            get { return _heureFin; }
            set { _heureFin = value; }
        }
        public double NombrePersonne
        {
            get { return _nombrePersonne; }
            set { _nombrePersonne = value; }
        }

        public InfoSalle()
        {
        }

        public InfoSalle(int id, string? idSalle, DateOnly jour, TimeOnly heureDebut, TimeOnly heureFin, 
            double nombrePersonne)
        {
            Id = id;
            IdSalle = idSalle ?? throw new ArgumentNullException(nameof(idSalle));
            Jour = jour;
            HeureDebut = heureDebut;
            HeureFin = heureFin;
            NombrePersonne = nombrePersonne;
        }

        public InfoSalle(string? idSalle, DateOnly jour, TimeOnly heureDebut, TimeOnly heureFin,
            double nombrePersonne)
        {
            IdSalle = idSalle ?? throw new ArgumentNullException(nameof(idSalle));
            Jour = jour;
            HeureDebut = heureDebut;
            HeureFin = heureFin;
            NombrePersonne = nombrePersonne;
        }
    }
}
