package com.company;

import java.util.ArrayList;

public class building
{
    private int nbFloors;
	private floor[] floors;

	public building(int nbFloors)
	{

		this.nbFloors = nbFloors;
		if(this.nbFloors < 1)
		{
			throw new IllegalArgumentException("Must have at least 1 floor");
		}
		else
		{
			this.floors = new floor[nbFloors];
			for(int i = 0; i < nbFloors; i++)
			{
				floors[i] = new floor(i);
			}
		}

	}

}
