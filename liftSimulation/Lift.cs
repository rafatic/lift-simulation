using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using NSimulate;
using NSimulate.Instruction;


namespace liftSimulation
{
    public class Lift : Process
    {
        #region Attributes
        private Exponential expo;
        private Random rand;
        
        public PersonGenerator personsGenerator
        {
            get;
            private set;
        }

        public int NbFloors
        {
            get;
            private set;
        }

        public int CurrentFloor
        {
            get;
            private set;
        }

        public int MaxCapacity
        {
            get;
            private set;
        }

        public int CurrentLoad
        {
            get;
            private set;
        }
        
        public List<Person> PersonsInLift
        {
            get;
            private set;
        }

        public List<Person> ProcessedPersons
        {
            get;
            private set;
        }

        public List<int> RequestedFloors
        {
            get;
            private set;
        }

        public long meanWorkTime
        {
            get;
            private set;
        }



        public int ProcessedCount
        {
            get;
            private set;
        }

        public FloorOrdonancer Ordonancer
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        public Lift(int maxCapacity, int nbFloors, Random random, PersonGenerator generator, FloorOrdonancer ordonancer, long meanWorkTime) : base()
        {
            //this.PersonsQueue = personsQueue;
            this.MaxCapacity = maxCapacity;
            this.CurrentLoad = 0;
            this.PersonsInLift = new List<Person>();
            this.NbFloors = nbFloors;
            this.CurrentFloor = 0;
            this.rand = random;
            this.expo = new Exponential(1.0);
            this.RequestedFloors = new List<int>();
            this.ProcessedPersons = new List<Person>();
            this.personsGenerator = generator;
            this.Ordonancer = ordonancer;
            this.meanWorkTime = meanWorkTime;

            //RequestedFloors.Add(0);
        }
        #endregion  


        public override IEnumerator<InstructionBase> Simulate()
        {

            

            //RequestedFloors = personsGenerator.getFloorsWhereLiftIsNeeded();
            while(true)
            {
                if (CurrentLoad < MaxCapacity)
                {
                    RequestedFloors.AddRange(personsGenerator.getFloorsWhereLiftIsNeeded());
                    RequestedFloors = RequestedFloors.Union(RequestedFloors).ToList();
                    RequestedFloors = Ordonancer.Sort(RequestedFloors);
                }


                if (RequestedFloors.Count == 0)
                {
                    //Console.WriteLine("Waiting");
                    yield return new WaitConditionInstruction(() => personsGenerator.getFloorsWhereLiftIsNeeded().Count > 0);
                }
                else
                {
                    if (personsGenerator.PersonsWaiting[CurrentFloor].Count > 0 && CurrentLoad < MaxCapacity)
                    {
                        int nbNewComers = Board();
                        yield return new WaitInstruction(3 * nbNewComers);
                    }
                        


                    
                    if (RequestedFloors.Count > 0)
                    {

                        var originalFloor = CurrentFloor;
                        MoveToFloor(RequestedFloors[0]);
                        yield return new WaitInstruction((Math.Abs(CurrentFloor - originalFloor)) * 20);


                        if(CurrentLoad != 0)
                        {
                            int nbPeopleLeft = Disembark();
                            yield return new WaitInstruction(3 * nbPeopleLeft);
                        }
                        RequestedFloors.RemoveAt(0);

                    }
                }
            }

        }

        private void Up()
        {
            if(CurrentFloor < NbFloors - 1)
            {
                CurrentFloor++;
                
            }
            else
            {
                Console.WriteLine("[Lift.Up] Error : Cannot go up, already at floor {0}", CurrentFloor);
            }
        }

        private void Down()
        {
            if (CurrentFloor > 0)
            {
                CurrentFloor--;
            }
            else
            {
                Console.WriteLine("[Lift.Down] Error : Cannot go down, already at floor {0}", CurrentFloor);
            }
        }

        

        private void LeaveLift(Person p)
        {
           
            


            if(p.Departure == 0)
            {
                p.ExitingLiftTimeGoingUp = Context.TimePeriod;
                p.TotalTimeInLift = Context.TimePeriod - p.EnteringLiftTimeGoingUp;
                p.Departure = p.Destination;
                p.Destination = 0;
                p.TimeBeforeGoingActive = (long)meanWorkTime * (long)expo.Density(rand.NextDouble());
                
                personsGenerator.PersonsPool.Add(p);
            }
            else
            {
                p.ExitingLiftTimeGoingDown = Context.TimePeriod;
                p.TotalTimeInLift += Context.TimePeriod - p.EnteringLiftTimeGoingDown;
                ProcessedPersons.Add(p);


                ProcessedCount++;
            }
            
            PersonsInLift.Remove(p);
            
            
            CurrentLoad--;
        }

        private void EnterLift(Person p)
        {
            if(p.Departure == 0)
            {
                p.EnteringLiftTimeGoingUp = Context.TimePeriod;
                p.TotalQueueTime = Context.TimePeriod - p.BeginQueueTimeGoingUp;
            }
            else
            {
                p.EnteringLiftTimeGoingDown = Context.TimePeriod;
                p.TotalQueueTime += Context.TimePeriod - p.BeginQueueTimeGoingDown;
            }

            PersonsInLift.Add(p);

            


            Console.WriteLine("Person {0} is entering the lift", p.Id);

            if (!RequestedFloors.Contains(p.Destination))
            {
                RequestedFloors.Add(p.Destination);
            }
            CurrentLoad++;
            
        }

        private void MoveToFloor(int destination)
        {
            
            if (destination > CurrentFloor)
            {

                while(CurrentFloor != destination)
                {
                    Up();
                }

            }
            else if (destination < CurrentFloor)
            {
                
                while (CurrentFloor != destination)
                {
                    Down();
                }

            }

            
            
        }

        private int Board()
        {
            int nbNewcomers = 0;
            if (personsGenerator.PersonsWaiting[CurrentFloor].Count > 0 && CurrentLoad < MaxCapacity)
            {
                Console.WriteLine("On floor {0}, {1} persons are waiting", CurrentFloor, personsGenerator.PersonsWaiting[CurrentFloor].Count);

                bool dequeueSuccesfull = false;
                while (personsGenerator.PersonsWaiting[CurrentFloor].Count > 0 && CurrentLoad < MaxCapacity)
                {
                    //Person enteringPerson = personsGenerator.PersonsWaiting[CurrentFloor].Dequeue();
                    Person enteringPerson = null;
                    while (dequeueSuccesfull == false)
                    {
                        dequeueSuccesfull =  personsGenerator.PersonsWaiting[CurrentFloor].TryDequeue(out enteringPerson);
                    }
                    
                    if(enteringPerson != null)
                    {
                        EnterLift(enteringPerson);
                        nbNewcomers++;
                    }
                    dequeueSuccesfull = false;
                    

                }
                //RequestedFloors.Sort();
                RequestedFloors = Ordonancer.Sort(RequestedFloors);

                Console.WriteLine("Requested floors : ");
                foreach (int floor in RequestedFloors)
                {
                    Console.WriteLine("    - {0}", floor);
                }

                Console.WriteLine("---------------------------------");
            }

            return nbNewcomers;
        }

        private int Disembark()
        {
            int nbPersonLeaving = 0;
            if (RequestedFloors[0] == CurrentFloor)
            {
                for (int i = 0; i < PersonsInLift.Count; i++)
                {
                    if (PersonsInLift[i].Destination == CurrentFloor)
                    {
                        Console.WriteLine("Person {0} has reached its destination", PersonsInLift[i].Id);
                        
                        LeaveLift(PersonsInLift[i]);
                        nbPersonLeaving++;
                        i--;

                    }
                }
                Console.WriteLine("There are now {0} persons in the lift", PersonsInLift.Count);
            }
            return nbPersonLeaving;
        }


        
    }
}
