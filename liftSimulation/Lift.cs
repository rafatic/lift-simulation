using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSimulate;
using NSimulate.Instruction;


namespace liftSimulation
{
    public class Lift : Process
    {
        private List<Person> _waitingPersons = null;
        private Random _random = null;

        


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

        public Queue<Person> PersonsQueue
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

        public int ProcessTime
        {
            get;
            private set;
        }

        

        public int ProcessedCount
        {
            get;
            private set;
        }

        public Lift(Queue<Person> personsQueue, List<Person> waitingPersonsList, int maxCapacity, int nbFloors, Random random) : base()
        {
            this.PersonsQueue = personsQueue;
            this._waitingPersons = waitingPersonsList;
            this.MaxCapacity = maxCapacity;
            this.CurrentLoad = 0;
            this.PersonsInLift = new List<Person>();
            this.NbFloors = nbFloors;
            this.CurrentFloor = 0;
            this._random = random;
            this.RequestedFloors = new List<int>();
            this.ProcessTime = 0;
            this.ProcessedPersons = new List<Person>();

        }

        
        public override IEnumerator<InstructionBase> Simulate()
        {

            foreach(Person p in _waitingPersons)
            {
                p.BeginQueueTime = Context.TimePeriod;
            }
            while(true)
            {
                if (PersonsQueue.Count == 0)
                {
                    yield return new WaitConditionInstruction(() => PersonsQueue.Count > 0);
                }
                else
                {
                    
                    if(CurrentFloor == 0)
                    {
                        Console.WriteLine("On ground floor, {0} persons are waiting", PersonsQueue.Count);
                        while (PersonsQueue.Count > 0 && CurrentLoad < MaxCapacity)
                        {
                            var enteringPerson = PersonsQueue.Dequeue();
                            PersonsInLift.Add(enteringPerson);
                            enteringPerson.EnteringLiftTime = Context.TimePeriod;
                            

                            Console.WriteLine("Person {0} is entering the lift", enteringPerson.Id);

                            if (!RequestedFloors.Contains(enteringPerson.Destination))
                            {
                                RequestedFloors.Add(enteringPerson.Destination);
                            }
                            CurrentLoad++;
                            
                            yield return new WaitInstruction(10);

                        }
                        RequestedFloors.Sort();

                        Console.WriteLine("Requested floors : ");
                        foreach(int floor in RequestedFloors)
                        {
                            Console.WriteLine("    - {0}", floor);
                        }

                        Console.WriteLine("---------------------------------");
                    }

                    if(CurrentLoad > 0)
                    {
                        bool hasAnyoneLeft = false;
                        do
                        {
                            hasAnyoneLeft = false;
                            Console.WriteLine("Going up ! (from {0} to {1})", CurrentFloor, CurrentFloor + 1);
                            Up();
                            yield return new WaitInstruction(20);
                            
                            for(int i = 0; i < PersonsInLift.Count; i++)
                            {
                                if(PersonsInLift[i].Destination == CurrentFloor)
                                {
                                    Console.WriteLine("Person {0} has reached its destination", PersonsInLift[i].Id);

                                    hasAnyoneLeft = true;
                                    LeaveLift(PersonsInLift[i]);

                                    i--;
                                    
                                }
                            }
                            Console.WriteLine("There are now {0} persons in the lift", PersonsInLift.Count);


                            if (hasAnyoneLeft)
                            {
                                yield return new WaitInstruction(5);
                            }
                        } while (CurrentLoad > 0 && CurrentFloor != NbFloors - 1);

                        
                    }
                    else
                    {
                        while (CurrentFloor != 0)
                        {
                            Console.WriteLine("Going down ! (from {0} to {1})", CurrentFloor, CurrentFloor - 1);
                            Down();
                            yield return new WaitInstruction(20);
                        }
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
            p.ExitingLiftTime = Context.TimePeriod;
            PersonsInLift.Remove(p);
            _waitingPersons.Remove(p);
            ProcessedPersons.Add(p);
            ProcessedCount++;
            CurrentLoad--;
        }
    }
}
