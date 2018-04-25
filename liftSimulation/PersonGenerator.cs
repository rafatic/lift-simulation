using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSimulate;
using NSimulate.Instruction;
using MathNet.Numerics.Distributions;

namespace liftSimulation
{
    public class PersonGenerator : Process
    {
        private int SimulationMinuteTime;

        // Lois distribution
        private Poisson poisson;

        private int personId;
        private int nbFloors;
        private Random rand;


        public int AverageQueueSize
        {
            get;
            private set;
        }

        public int MaximumQueueSize
        {
            get;
            private set;
        }

        public List<Person> PersonsInClass
        {
            get;
            private set;
        }

        public List<Person> PersonsPool
        {
            get;
            private set;
        }
        public List<ConcurrentQueue<Person>> PersonsWaiting
        {
            get;
            private set;
        }

        public List<Floor> Floors
        {
            get;
            private set;
        }

        public PersonGenerator(List<ConcurrentQueue<Person>> personsWaiting, int nbFloors, int seed = 12345) :base()
        {
            SimulationMinuteTime = 60;

            poisson = new Poisson(1.5);
            rand = new Random(seed);

            personId = 0;

            this.PersonsPool = new List<Person>();
            this.PersonsWaiting = personsWaiting;

            this.nbFloors = nbFloors;

            Floors = new List<Floor>();
            for(int i = 0; i < nbFloors; i++)
            {
                Floors.Add(new Floor(i));
            }
        }

        public override IEnumerator<InstructionBase> Simulate()
        {
            for(int i = 0; i < SimulationMinuteTime; i++)
            {
                PersonsArrival(i);
            }

            while(true)
            {

                for (int i = 0; i < PersonsPool.Count; i++)
                {
                    PersonsPool[i].TimeBeforeGoingActive--;

                    if (PersonsPool[i].TimeBeforeGoingActive <= 0)
                    {
                        Console.Write("Person " + PersonsPool[i].Id + " is active");
                        if(PersonsPool[i].Departure == 0)
                        {
                            Console.WriteLine(" (Going up)");
                            PersonsPool[i].BeginQueueTimeGoingUp = Context.TimePeriod;
                        }
                        else
                        {
                            PersonsPool[i].BeginQueueTimeGoingDown = Context.TimePeriod;
                            Console.WriteLine(" (Going down)");
                        }

                        
                        //PersonsWaiting[PersonsPool[i].Departure].Enqueue(PersonsPool[i]);
                        Floors[PersonsPool[i].Departure].PersonsWaiting.Enqueue(PersonsPool[i]);
                        if(!Floors[PersonsPool[i].Departure].QueueSizeHistory.ContainsKey(Context.TimePeriod))
                        {
                            Floors[PersonsPool[i].Departure].QueueSizeHistory.Add(Context.TimePeriod, Floors[PersonsPool[i].Departure].PersonsWaiting.Count);
                        }
                        else
                        {
                            Floors[PersonsPool[i].Departure].QueueSizeHistory[Context.TimePeriod] = Floors[PersonsPool[i].Departure].PersonsWaiting.Count;
                        }

                        if(Floors[PersonsPool[i].Departure].MaximumQueueSize < Floors[PersonsPool[i].Departure].PersonsWaiting.Count)
                        {
                            Floors[PersonsPool[i].Departure].MaximumQueueSize = Floors[PersonsPool[i].Departure].PersonsWaiting.Count;
                        }
                        
                        PersonsPool.RemoveAt(i);
                        i--;
                    }
                }



                yield return new WaitInstruction(1);
            }

        }

        public int NumberOfArrival()
        {
            double randomNumber = rand.NextDouble();
            int k = 0;
            
            while (poisson.CumulativeDistribution(k) <= randomNumber)
            {
                k++;
            }

            return k;
        }

        public void PersonsArrival(int minute)
        {
            int NbArrivals = NumberOfArrival();

            for (int i=0; i < NbArrivals; i++)
            {
                PersonsPool.Add(new Person(personId, 0, rand.Next(1, nbFloors), minute * 60, minute * 60));
                personId++;
            }
        }

        public List<int> getFloorsWhereLiftIsNeeded()
        {
            List<int> floors = new List<int>();

            for(int i = 0; i < nbFloors; i++)
            {
                /*if(PersonsWaiting[i].Count > 0 )
                {
                    floors.Add(i);
                    
                }*/
                if(Floors[i].PersonsWaiting.Count > 0)
                {
                    floors.Add(i);
                }
            }

            return floors;
        }


        

    }
}
