using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.Specific;
using PanneauSolaire.Models.ViewStruct;

namespace PanneauSolaire.Models.UnitTest
{
    public class TestDetailPrevision
    {
        public static void main(string[] args)
        {
            Console.WriteLine("***** TEST DETAILS PREVISION ***** \n");

            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            Secteur? secteur = Secteur.getById(cnx, "SEC7001");
            if (secteur != null)
            {
                DateOnly jour = new DateOnly(2023, 12, 5);

                List<Panneau> panneaux = secteur.getPanneaux(cnx);
                List<Batterie> batteries = secteur.getBatteries(cnx);
                List<Salle> salles = secteur.getSalles(cnx);

                var heureDebutFin = secteur.getHeureDebutFin(cnx, jour);
                TimeOnly heureDebut = (heureDebutFin.Item1 != null) ? (TimeOnly)heureDebutFin.Item1 : new TimeOnly();
                TimeOnly heureFin = (heureDebutFin.Item2 != null) ? (TimeOnly)heureDebutFin.Item2 : new TimeOnly();

                List<Prevision> previsions = secteur.getDetailsPrevisions(cnx, jour);
                foreach(Prevision prevision in previsions)
                {
                    Console.WriteLine($" Heure: {prevision.Heure}");
                    if (prevision.Meteo != null)
                    {
                        Console.WriteLine($"    Lumiere: {prevision.Meteo.Lumiere}/{prevision.Meteo.LumiereMax}");
                    }
                    Console.WriteLine($"    Panneau: {prevision.PuissancePanneau}");
                    Console.WriteLine($"    Batterie: {prevision.PuissanceBatterie}");
                    Console.WriteLine($"    Personne present: {prevision.NbPersonne}");
                    Console.WriteLine($"    Consommation unitaire: {prevision.ConsMoyenne}");
                }

            }

            cnx.Close();

            Console.WriteLine("\n ******************************* \n");
        }
    }
}
