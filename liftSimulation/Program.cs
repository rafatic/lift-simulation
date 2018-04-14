using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSimulate;
namespace liftSimulation
{
    public static class Program
    {
        private static Lift lift;
        private static PersonGenerator personGenerator;
        static void Main(string[] args)
        {


            using (var context = new SimulationContext(true))
            {
                InitiateModel(context, 2, 4);

                Simulator simulator = new Simulator();

                simulator.Simulate();
                
                Console.WriteLine("\n\n");
                Console.WriteLine(SimulationResultsToString(context, lift));
            }
            Console.ReadKey();
        }

        private static void InitiateModel(SimulationContext context, int nbFloors, int liftMaximumCapacity)
        {
            Random rand = new Random();

            List<Queue<Person>> personsQueues = new List<Queue<Person>>();

            for(int i = 0; i < nbFloors; i++)
            {
                personsQueues.Add(new Queue<Person>());
            }


            personGenerator = new PersonGenerator(personsQueues, nbFloors);

            lift = new Lift(personsQueues, liftMaximumCapacity, nbFloors, new Random(12345), personGenerator);





            new SimulationEndTrigger(() => context.TimePeriod >= 10000);

            
        }

        private static string SimulationResultsToString(SimulationContext context, Lift lift)
        {
            float avgServiceTime, avgWaitTime;

            avgServiceTime = avgWaitTime = 0.0f;
            string str = "";
            str += "-------------------------- SIMULATION ENDED -----------------------\n\n";
            str += lift.ProcessedCount + " Persons processed in "+ context.TimePeriod + " seconds\n";
            str += "                            PERSONS RECAP \n";
            str += "\t\t UP \t\t\t DOWN\n";
            str += "ID\tBEGIN QUEUE\t ENTERING LIFT\t LEAVING LIFT\tBEGIN QUEUE\t ENTERING LIFT\t LEAVING LIFT\n";

            foreach(Person p in lift.ProcessedPersons)
            {
                avgWaitTime += p.TotalQueueTime;
                avgServiceTime += p.TotalTimeInLift;

                str += p.ToString();
            }

            avgServiceTime /= lift.ProcessedCount;
            avgWaitTime /= lift.ProcessedCount;

            str += "\n-----------------------------------------------------------------\n";
            str += "Average waiting time : " + avgWaitTime + "\n";
            str += "Average service time : " + avgServiceTime + "\n";

            


            return str;
        }

        
        
    }
}
