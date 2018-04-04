package com.company;

public class lift
{

    //region Attributes
    public static final int MAX_CAPACITY = 8;

    public static enum STATE
    {
        IDLE, ASCENDING, DESCENDING, OFFLINE
    };

    private int nbPassengers, id;

    private floor currentFloor;
    //endregion

    //region Constructors
    public lift(int id)
    {
        this.id = id;
    }

	public lift(int id, floor currentFloor)
	{
		this.id = id;
		this.currentFloor = currentFloor;
	}

    //endregion


    //region Accessors


    public floor getCurrentFloor()
    {
        return currentFloor;
    }

    public void setCurrentFloor(floor currentFloor)
    {
        this.currentFloor = currentFloor;
    }

    public int getNbPassengers()
    {
        return nbPassengers;
    }

    public void setNbPassengers(int nbPassengers)
    {
        this.nbPassengers = nbPassengers;
    }

    public int getId()
    {
        return id;
    }
    //endregion


}
