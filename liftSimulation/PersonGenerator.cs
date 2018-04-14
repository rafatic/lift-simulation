using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSimulate;
using NSimulate.Instruction;

namespace liftSimulation
{
    public class PersonGenerator : Process
    {
        private int personId;
        private int nbFloors;
        private Random rand;
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

        public PersonGenerator(List<ConcurrentQueue<Person>> personsWaiting, int nbFloors, int seed = 12345) :base()
        {
            personId = 0;
            rand = new Random(seed);
            this.PersonsPool = new List<Person>();
            this.PersonsWaiting = personsWaiting;


            this.nbFloors = nbFloors;
        }

        public override IEnumerator<InstructionBase> Simulate()
        {
            int spawnTime = 0;
            for(int i = 0; i < 11; i++)
            {
                spawnTime = rand.Next(600);
                PersonsPool.Add(new Person(personId, 0, rand.Next(1, nbFloors), spawnTime, spawnTime));
                personId++;
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

                        
                        PersonsWaiting[PersonsPool[i].Departure].Enqueue(PersonsPool[i]);
                        PersonsPool.RemoveAt(i);
                        i--;
                    }
                }



                yield return new WaitInstruction(1);
            }

        }

        public List<int> getFloorsWhereLiftIsNeeded()
        {
            List<int> floors = new List<int>();

            for(int i = 0; i < nbFloors; i++)
            {
                if(PersonsWaiting[i].Count > 0 )
                {
                    floors.Add(i);
                    
                }
            }

            return floors;
        }


        

    }
}
