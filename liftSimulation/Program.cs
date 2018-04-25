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
            string QueueHistoryFilePath = "QueueHistory.csv";
            string resultsFilePath = "result.txt";

            for (int liftMethod = 0; liftMethod < 2; liftMethod++)
            {
                using (var context = new SimulationContext(true))
                {
                    Console.WriteLine("\n\n");
                    result += "\n\n--------------------------------------------------------------------------------------------------\n\n";
                    if (liftMethod == 0)
                    {
                        
                        result += "\t\tRESULTS LINEAR SCAN ORDONANCER\n";
                        //Console.WriteLine("RESULTS LINEAR SCAN ORDONNANCER");
                    }
                    if (liftMethod == 1)
                    {
                        result += "\t\tRESULTS SHORTER SEEK TIME ORDONNANCER\n";
                        //Console.WriteLine("RESULTS SHORTER SEEK TIME ORDONNANCER");
                    }
                    if (liftMethod == 2)
                    {
                        result += "\t\tRESULTS DEFAULT ORDONNANCER\n";
                        //Console.WriteLine("RESULTS DEFAULT ORDONNANCER");
                    }
                    result += "\n\n--------------------------------------------------------------------------------------------------\n\n";

                    InitiateModel(context, nbLifts, nbFloors, liftMaximumCapacity, liftMethod, simulationMinuteTime, seed, meanWorkTime, meanPerson);

                    Simulator simulator = new Simulator();

                    simulator.Simulate();

                    for (int j=0; j < lifts.Count; j++)
                    {
                        result += "\n\n";
                        result += "RESULTS LIFT " + (j + 1) + " \n\n";
                        result += SimulationResultsToString(context, lifts[j]) + "\n";
                        /*Console.WriteLine("\n\n");
                        Console.WriteLine("RESULTS LIFT " + (j+1) + " \n\n");
                        Console.WriteLine(SimulationResultsToString(context, lifts[j]));*/
                    }

                    Console.WriteLine(result);

                    PrintQueueSizeHistoryToFile(QueueHistoryFilePath, personGenerator.Floors);
                    PrintToFile(resultsFilePath, result);
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
            
            new SimulationEndTrigger(() => context.TimePeriod >= simulationMinuteTime * 60 && IsModelEmpty(context));
        }

        

        

        private static string SimulationResultsToString(SimulationContext context, Lift lift)
        {
            float avgServiceTime, avgWaitTime;

            avgServiceTime = avgWaitTime = 0.0f;
            string str = "";
            str += "-------------------------- SIMULATION ENDED -----------------------\n\n";
            str += lift.ProcessedCount + " Persons processed in "+ context.TimePeriod + " seconds\n";
            str += "\t\t\t\t\t\tPERSONS RECAP \n";
            str += "\t\t\tUP\t\t\t\t\t\tDOWN\n";
            str += "ID\tBEGIN QUEUE\tWAIT TIME\tSERVICE TIME\tBEGIN QUEUE\tWAIT TIME\tSERVICE TIME\n";

            foreach(Person p in lift.ProcessedPersons)
            {
                avgWaitTime += p.TotalQueueTime;
                avgServiceTime += p.TotalTimeInLift;

                str += p.ToString();
            }

            avgServiceTime /= lift.ProcessedCount;
            avgWaitTime /= lift.ProcessedCount;

            str += "\n-----------------------------------------------------------------\n";
            str += "Average waiting time : " + Math.Round(avgWaitTime, 2) + "\n";
            str += "Average service time : " + Math.Round(avgServiceTime, 2) + "\n";


            str += "Proportion lift busy : " + Math.Round(((double)lift.TimeBusy / (double)context.TimePeriod), 2);


            

                
            
            return str;
        }


        private static void PrintQueueSizeHistoryToFile(string filePath, List<Floor> floors)
        {

            File.WriteAllText(filePath, string.Empty);
            

            using (StreamWriter sw = File.AppendText(filePath))
            {
                
                foreach (Floor f in floors)
                {
                    sw.Write(f.QueueHistoryToCsv());
                    
                }
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
