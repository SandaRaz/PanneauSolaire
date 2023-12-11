using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.Specific;

namespace PanneauSolaire.Models.UnitTest
{
    public class TestConsommationMoyenne
    {
        public static void main(string[] args)
        {
            Console.WriteLine("***** TEST GET CONSO MOYENNE PAR ITERATION ***** \n");

            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            Secteur? secteur = Secteur.getById(cnx, "SEC7001");
            if (secteur != null)
            {
                DateOnly jour = new DateOnly(2023, 12, 5);

                double consMoy = secteur.getConsommationUnitaireMoyenneParIteration(cnx, jour);
                Console.WriteLine($"Consommation moyenne par iteration le {jour} : {consMoy}");

            }

            cnx.Close();

            Console.WriteLine("\n ********************************************* \n");
        }
    }
}
