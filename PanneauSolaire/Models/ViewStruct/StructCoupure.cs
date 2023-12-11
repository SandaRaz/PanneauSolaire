using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;

namespace PanneauSolaire.Models.ViewStruct
{
    public class StructCoupure
    {
        public List<Secteur> secteurs;
        public StructCoupure()
        {
            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            secteurs = Secteur.getAll(cnx);

            cnx.Close();
        }
    }
}
