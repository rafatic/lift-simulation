using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSimulate;

namespace liftSimulation
{
    public static class Program
    {
        private static Lift lift1, lift2;
        private static PersonGenerator personGenerator;


        

        static void Main(string[] args)
        {
            ShorterSeekTimeFirstOrdonancer sstf = new ShorterSeekTimeFirstOrdonancer((int)ShorterSeekTimeFirstOrdonancer.Heading.DOWNWARDS, 2);
            //int[] floors = new int[3] { 3, 1, 4};
            List<int> floors = new List<int>();

            floors.Add(3);
            floors.Add(1);
            floors.Add(4);
            floors.Add(0);
            floors.Add(5);
            floors.Add(9);
            floors.Add(7);
            floors.Add(12);
            floors.Add(-1);

            foreach (int f in floors)
            {
                Console.Write(f + " - ");
            }
            Console.WriteLine();
            floors = sstf.Sort(floors);
            foreach (int f in floors)
            {
                Console.Write(f + " - ");
            }

            /*using (var context = new SimulationContext(true))
            {
                InitiateModel(context, 7, 4);

                Simulator simulator = new Simulator();

                simulator.Simulate();
                
                Console.WriteLine("\n\n");
                Console.WriteLine("RESULTS LIFT 1 \n\n");
                Console.WriteLine(SimulationResultsToString(context, lift1));

                Console.WriteLine("\n\nRESULTS LIFT 2 \n\n");
                Console.WriteLine(SimulationResultsToString(context, lift2));
            }*/
            Console.ReadKey();
        }

        private static void InitiateModel(SimulationContext context, int nbFloors, int liftMaximumCapacity)
        {
            Random rand = new Random();

            List<ConcurrentQueue<Person>> personsQueues = new List<ConcurrentQueue<Person>>();

            for(int i = 0; i < nbFloors; i++)
            {
                personsQueues.Add(new ConcurrentQueue<Person>());
            }


            personGenerator = new PersonGenerator(personsQueues, nbFloors);

            lift1 = new Lift(liftMaximumCapacity, nbFloors, new Random(12345), personGenerator);
            lift2 = new Lift(liftMaximumCapacity, nbFloors, new Random(12345), personGenerator);




            new SimulationEndTrigger(() => context.TimePeriod >= 60*60);

            
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
