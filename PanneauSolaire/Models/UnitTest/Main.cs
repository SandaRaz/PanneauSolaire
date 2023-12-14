using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.Specific;
using PanneauSolaire.Models.ViewStruct;

namespace PanneauSolaire.Models.UnitTest
{
    public class Main
    {
        public static void main(string[] args)
        {
            TimeOnly to = new TimeOnly(21,00);
            for (int i=0;i<10;i++)
            {
                to = to.AddHours(1);
                Console.WriteLine("Time to: " + to);
            }


            TimeOnly a = new TimeOnly(8,15);
            TimeOnly b = new TimeOnly(9,20);
            Console.WriteLine($"Minute de a - minute de b = {Math.Abs(a.Minute - b.Minute)}");
            Console.WriteLine($"a - b = {(a - b).TotalMinutes}");
;
            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            Salle? salle = Salle.getById(cnx, "SAL7001");
            if(salle != null)
            {
                Console.WriteLine(">>> " + salle.AnalysePersonnesPresentJourDeLaSemaine(cnx, new DateOnly(2023,12,11), new TimeOnly(08,00)));
            }

            Secteur? secteur = Secteur.getById(cnx, "SEC7001");
            if(secteur != null)
            {
                DateOnly jour = new DateOnly(2023, 12, 5);

                List<Panneau> panneaux = secteur.getPanneaux(cnx);
                List<Batterie> batteries = secteur.getBatteries(cnx);
                List<Salle> salles = secteur.getSalles(cnx);

            }

            cnx.Close();
        }
    }
}
