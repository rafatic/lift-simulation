using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

            string result = "";
            string QueueHistoryFilePath = "QueueHistory";
            string resultsFilePath = "result.txt";

            for (int liftMethod = 0; liftMethod < 3; liftMethod++)
            {
                using (var context = new SimulationContext(true))
                {
                    Console.WriteLine("\n\n");
                    result += "\n\n--------------------------------------------------------------------------------------------------\n\n";
                    if (liftMethod == 0)
                    {
                        
                        result += "\t\tRESULTS LINEAR SCAN ORDONANCER\n";
                    }
                    if (liftMethod == 1)
                    {
                        result += "\t\tRESULTS SHORTER SEEK TIME ORDONNANCER\n";
                    }
                    if (liftMethod == 2)
                    {
                        result += "\t\tRESULTS DEFAULT ORDONNANCER\n";
                    }
                    result += "\n\n--------------------------------------------------------------------------------------------------\n\n";

                    InitiateModel(context, nbLifts, nbFloors, liftMaximumCapacity, liftMethod, simulationMinuteTime, seed, meanWorkTime, meanPerson);

                    Simulator simulator = new Simulator();

                    simulator.Simulate();
                    
                    result += LiftsResultsToString(context, lifts) + "\n\n";
                    result += FloorsResultsToString(context, personGenerator.Floors);


                    

                    foreach(Floor f in personGenerator.Floors)
                    {
                        PrintQueueSizeHistoryToFile(QueueHistoryFilePath, f);
                    }
                    PrintToFile(resultsFilePath, LiftsResultsToCsv(context, lifts) + "\n\n" + FloorsResultsToCsv(context, personGenerator.Floors));
                }
            }
            Console.WriteLine(result);
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
            
            new SimulationEndTrigger(() => context.TimePeriod >= simulationMinuteTime * 60 && IsModelEmpty(context));
        }

        
        private static string LiftsResultsToCsv(SimulationContext context, List<Lift> lifts)
        {
            string str = "";
            int i = 0;
            
            str += ",Proportion of time busy\n";

            foreach(Lift l in lifts)
            {
                str += "Lift " + i + ", " + Math.Round(((double)l.TimeBusy / (double)context.TimePeriod), 2) + "\n";
                i++;
            }

            return str;
        }

        private static string LiftsResultsToString(SimulationContext context, List<Lift> lifts)
        {
            string str = "";
            int i = 0;
            str += "\t Proportion of time busy\n";
            foreach(Lift l in lifts)
            {
                str += "Lift " + i + "\t\t" + Math.Round(((double)l.TimeBusy / (double)context.TimePeriod), 2) + "\n";
                i++;
            }
            return str;
        }

        private static string FloorsResultsToCsv(SimulationContext context, List<Floor> floors)
        {
            string str = "";

            str += ", Average Queue Size, Maximum Queue Size, Average Wait time, Maximum Wait Time\n";
            foreach(Floor f in floors)
            {
                str += f.Id + ", " + Math.Round(f.GetAverageQueueSize(), 2) + " ," + f.MaximumQueueSize + ", " + Math.Round(f.GetAverageWaitingTime(), 2) + ", " + f.MaximumQueueSize + "\n";
            }

            return str;
        }
        
        private static string FloorsResultsToString(SimulationContext context, List<Floor> floors)
        {
            string str = "";

            str += "\tAverage Queue Size\tMaximum Queue Size\tAverage Wait time\tMaximum Wait Time\n";
            foreach (Floor f in floors)
            {
                str += f.Id + "\t\t" + Math.Round(f.GetAverageQueueSize(), 2) + "\t\t" + f.MaximumQueueSize + "\t\t\t" + Math.Round(f.GetAverageWaitingTime(), 2) + "\t\t\t" + f.MaximumQueueSize + "\n";
            }

            return str;
        }

        


        private static void PrintQueueSizeHistoryToFile(string filePath, Floor f)
        {
            filePath += f.Id + ".csv";
            File.WriteAllText(filePath, string.Empty);
            

            using (StreamWriter sw = File.AppendText(filePath))
            {

                sw.Write(f.QueueHistoryToCsv());
                sw.Close();
            }

        }

        private static void PrintToFile(string filePath, string content)
        {
            File.WriteAllText(filePath, string.Empty);

            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.Write(content);
                sw.Close();
            }
        }

        private static bool IsModelEmpty(SimulationContext context)
        {
            bool isEmpty = true;
            if(personGenerator.PersonsPool.Count > 0)
            {
                isEmpty = false;
            }

            foreach(Lift l in lifts)
            {
                if(l.CurrentLoad > 0)
                {
                    isEmpty = false;
                }

                if(l.RequestedFloors.Count > 0)
                {
                    isEmpty = false;
                }
            }

            foreach(Floor f in personGenerator.Floors)
            {
                if(f.PersonsWaiting.Count > 0)
                {
                    isEmpty = false;
                }
            }
            return isEmpty;
        }



}
}
