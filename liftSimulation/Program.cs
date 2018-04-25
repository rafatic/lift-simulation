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
        private static Lift lift1, lift2;
        private static PersonGenerator personGenerator;


        

        static void Main(string[] args)
        {

            string QueueHistoryFilePath = "QueueHistory.csv";
            string ResultsFilePath = "results.txt";
            string results = "";
            using (var context = new SimulationContext(true))
            {
                InitiateModel(context, 7, 21);

                Simulator simulator = new Simulator();

                simulator.Simulate();

                results += "\n\n";
                results += "RESULTS LIFT 1 \n\n";
                results += SimulationResultsToString(context, lift1) + "\n";
                results += "\n\nRESULTS LIFT 2 \n\n";
                results += SimulationResultsToString(context, lift2) + "\n";
                results += "\n\n RESULTS FLOORS\n\n";
                foreach (Floor f in personGenerator.Floors)
                {
                    Console.WriteLine(f.GetResults());
                    results += f.GetResults() + "\n";
                }
                /*Console.WriteLine("\n\n");
                Console.WriteLine("RESULTS LIFT 1 \n\n");
                Console.WriteLine(SimulationResultsToString(context, lift1));

                Console.WriteLine("\n\nRESULTS LIFT 2 \n\n");
                Console.WriteLine(SimulationResultsToString(context, lift2));

                Console.WriteLine("\n\n RESULTS FLOORS\n\n");
                
                foreach(Floor f in personGenerator.Floors)
                {
                    Console.WriteLine(f.GetResults());
                }*/
                Console.WriteLine(results);
                PrintQueueSizeHistoryToFile(QueueHistoryFilePath, personGenerator.Floors);
                PrintToFile(ResultsFilePath, results);

            }
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

            lift1 = new Lift(liftMaximumCapacity, nbFloors, new Random(12345), personGenerator, new LinearScanOrdonancer((int)LinearScanOrdonancer.Heading.UPWARDS, 0));
            lift2 = new Lift(liftMaximumCapacity, nbFloors, new Random(12345), personGenerator, new LinearScanOrdonancer((int)LinearScanOrdonancer.Heading.UPWARDS, 0));




            new SimulationEndTrigger(() => context.TimePeriod >= 60*60);

            
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

        
        
    }
}
