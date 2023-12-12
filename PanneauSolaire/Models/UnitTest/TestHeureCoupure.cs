﻿using Npgsql;
using PanneauSolaire.Models.Cnx;
using PanneauSolaire.Models.Entity;
using PanneauSolaire.Models.Specific;

namespace PanneauSolaire.Models.UnitTest
{
    public class TestHeureCoupure
    {
        public static void main(string[] args)
        {
            Console.WriteLine("***** TEST GET HEURE DE PREVISION COUPURE ***** \n");

            NpgsqlConnection cnx = Connex.getConnection();
            cnx.Open();

            Secteur? secteur = Secteur.getById(cnx, "SEC7001");
            if (secteur != null)
            {
                DateOnly jour = new DateOnly(2023, 12, 5);

                var heureDebutFin = secteur.getHeureDebutFin(cnx, jour);
                TimeOnly heureDebut = heureDebutFin.Item1;
                TimeOnly heureFin = heureDebutFin.Item2;

                double suppCons = 60;

                TimeOnly? heureCoupure = Coupure.getHeurePrevisionCoupure(cnx, secteur, jour, heureDebut, 
                    heureFin, suppCons);
                if (heureCoupure != null)
                {
                    Console.WriteLine($"Pour consommation unitaire {suppCons} le {jour} coupure a : {heureCoupure}");
                }

            }

            cnx.Close();

            Console.WriteLine("\n *********************************************** \n");
        }
    }
}
