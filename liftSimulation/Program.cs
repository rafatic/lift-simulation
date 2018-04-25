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
        private static List<Lift> lifts;
        private static PersonGenerator personGenerator;

        static void Main(string[] args)
        {
            int nbLifts = int.Parse(args[0]);
            int liftMaximumCapacity = int.Parse(args[1]);
            int nbFloors = int.Parse(args[2]);
            int simulationMinuteTime = int.Parse(args[3]);
            int seed = int.Parse(args[4]);
            long meanWorkTime = long.Parse(args[5]);
            double meanPerson = double.Parse(args[6]);

            for (int liftMethod = 0; liftMethod < 2; liftMethod++)
            {
                using (var context = new SimulationContext(true))
                {
                    Console.WriteLine("\n\n");
                    if (liftMethod == 0)
                    {
                        Console.WriteLine("RESULTS LINEAR SCAN ORDONNANCER");
                    }
                    if (liftMethod == 1)
                    {
                        Console.WriteLine("RESULTS SHORTER SEEK TIME ORDONNANCER");
                    }
                    if (liftMethod == 2)
                    {
                        Console.WriteLine("RESULTS DEFAULT ORDONNANCER");
                    }

                    InitiateModel(context, nbLifts, nbFloors, liftMaximumCapacity, liftMethod, simulationMinuteTime, seed, meanWorkTime, meanPerson);

                    Simulator simulator = new Simulator();

                    simulator.Simulate();

                    for (int j=0; j < lifts.Count; j++)
                    {
                        Console.WriteLine("\n\n");
                        Console.WriteLine("RESULTS LIFT " + (j+1) + " \n\n");
                        Console.WriteLine(SimulationResultsToString(context, lifts[j]));
                    }
                }
            }
            Console.ReadKey();
        }

        private static void InitiateModel(SimulationContext context, int nbLifts, int nbFloors, int liftMaximumCapacity, int liftMethod, int simulationMinuteTime, int seed, long meanWorkTime, double meanPerson)
        {
            lifts = new List<Lift>();
            Random rand = new Random();

            List<ConcurrentQueue<Person>> personsQueues = new List<ConcurrentQueue<Person>>();

            for(int i = 0; i < nbFloors; i++)
            {
                personsQueues.Add(new ConcurrentQueue<Person>());
            }

            personGenerator = new PersonGenerator(personsQueues, nbFloors, meanPerson, simulationMinuteTime);

            for(int i=0; i < nbLifts; i++)
            {
                if (liftMethod == 0)
                {
                    lifts.Add(new Lift(liftMaximumCapacity, nbFloors, new Random(seed), personGenerator, new LinearScanOrdonancer((int)LinearScanOrdonancer.Heading.UPWARDS, 0), meanWorkTime));
                }
                if (liftMethod == 1)
                {
                    lifts.Add(new Lift(liftMaximumCapacity, nbFloors, new Random(seed), personGenerator, new ShorterSeekTimeFirstOrdonancer((int)LinearScanOrdonancer.Heading.UPWARDS, 0), meanWorkTime));
                }
                if (liftMethod == 2)
                {
                    lifts.Add(new Lift(liftMaximumCapacity, nbFloors, new Random(seed), personGenerator, new DefaultOrdonancer((int)LinearScanOrdonancer.Heading.UPWARDS, 0), meanWorkTime));
                }
            }
            
            new SimulationEndTrigger(() => isFinished(context));
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

        public static bool isFinished(SimulationContext context)
        {
            if(context.TimePeriod >= 60 * 60)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
