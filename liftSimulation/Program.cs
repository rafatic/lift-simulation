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
        static void Main(string[] args)
        {
            using (var context = new SimulationContext(true))
            {
                var lift = CreateModel(context, 4, 3, 4);

                var simulator = new Simulator();

                simulator.Simulate();

                //Console.WriteLine("Persons processed in {0} seconds", context.TimePeriod);

                //Console.WriteLine("Lift processed {0} persons", lift.ProcessedCount);
                Console.WriteLine("\n\n");
                Console.WriteLine(SimulationResultsToString(context, lift));
            }
            Console.ReadKey();
        }

        private static Lift CreateModel(SimulationContext context, int nbPersons, int nbFloors, int liftMaximumCapacity)
        {
            var rand = new Random();

            var waitingPersons = new List<Person>();

            var personsQueue = new Queue<Person>();

            var random = new Random();

            var lift = new Lift(personsQueue, waitingPersons, liftMaximumCapacity, nbFloors, random);

            Console.WriteLine("Persons : ");
            for(int i = 0; i < nbPersons; i++)
            {
                var newPerson = new Person(i, 0, rand.Next(1, nbFloors));

                personsQueue.Enqueue(newPerson);

                waitingPersons.Add(newPerson);
                Console.WriteLine("Person {0} : going from floor {1} to {2}", i, newPerson.Departure, newPerson.Destination);
            }
            Console.WriteLine("-----------------------------------------------");

            new SimulationEndTrigger(() => waitingPersons.Count == 0);

            return lift;
        }

        private static string SimulationResultsToString(SimulationContext context, Lift lift)
        {
            float avgServiceTime, avgWaitTime;

            avgServiceTime = avgWaitTime = 0.0f;
            string str = "";
            str += "-------------------------- SIMULATION ENDED -----------------------\n\n";
            str += context.TimePeriod  + " Persons processed in "+ lift.ProcessedCount + " seconds\n";
            str += "                            PERSONS RECAP \n";
            str += "BEGIN QUEUE TIME\t ENTERING LIFT\t LEAVING LIFT\n";

            foreach(Person p in lift.ProcessedPersons)
            {
                avgWaitTime += (p.EnteringLiftTime - p.BeginQueueTime);
                avgServiceTime += (p.ExitingLiftTime - p.EnteringLiftTime);
                str += p.BeginQueueTime + "                \t " + p.EnteringLiftTime + "             \t " + p.ExitingLiftTime + " \n";
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
