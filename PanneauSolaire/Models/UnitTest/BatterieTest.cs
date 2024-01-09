using PanneauSolaire.Models.Entity;

namespace PanneauSolaire.Models.UnitTest
{
    public class BatterieTest
    {
        public static void DisplayBattery(Batterie batterie)
        {
            Console.WriteLine($"{batterie.Refs} : max {batterie.Puissance}, actuelle {batterie.Charge}");
        }

        public static void main(string[] args)
        {
            Batterie bat1 = new Batterie("Batterie 1",10000,50);
            Batterie bat2 = new Batterie("Batterie 2", 15000, 50);
            double panneau = 3000;

            DisplayBattery(bat1);
            DisplayBattery(bat2);
            Console.WriteLine("\n Deduction charge \n");
            bat1.Charge -= 2000;
            bat2.Charge -= 2500;
            DisplayBattery(bat1);
            DisplayBattery(bat2);
            List<Batterie> batteries = new List<Batterie> { bat1 };
            Console.WriteLine("\n Charger batterie \n");
            Batterie.ChargerBatterie(panneau, batteries);
            DisplayBattery(bat1);
            DisplayBattery(bat2);
        }
    }
}
