using Microsoft.AspNetCore.Mvc;
using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.ViewStruct;

namespace PanneauSolaire.Controllers
{
    public class SpecificController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Coupure()
        {
            return View();
        }

        public IActionResult ListePrevision()
        {
            return View();
        }

        public IActionResult HeureCoupure(DateTime jour)
        {
            DateOnly datejour = DateOnly.FromDateTime(jour);

            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            List<(Secteur, List<Prevision>)> tuplesPrevisions = new List<(Secteur, List<Prevision>)>();

            List<Secteur> secteurs = Secteur.getAll(cnx);
            foreach (Secteur secteur in secteurs)
            {
                List<Prevision> previsions = secteur.getDetailsPrevisions(cnx, datejour);
                tuplesPrevisions.Add((secteur, previsions));
            }

            cnx.Close();

            ViewBag.TuplesPrevisions = tuplesPrevisions;
            return View("ListePrevision");
        }

        public IActionResult HeureCoupureSecteur(string idsecteur, DateTime jour)
        {
            DateOnly datejour = DateOnly.FromDateTime(jour);

            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            List<(Secteur, List<Prevision>)> tuplesPrevisions = new List<(Secteur, List<Prevision>)>();

            Secteur? secteur = Secteur.getById(cnx, idsecteur);
            if (secteur != null)
            {
                try
                {
                    List<Prevision> previsions = secteur.getDetailsPrevisions(cnx, datejour);
                    tuplesPrevisions.Add((secteur, previsions));
                }
                catch(Exception e)
                {
                    ViewBag.Exceptions = e;
                }

                ViewBag.TuplesPrevisions = tuplesPrevisions;
            }
            

            cnx.Close();

            return View("ListePrevision");
        }
    }
}
