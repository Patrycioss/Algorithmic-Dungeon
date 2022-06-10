using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class SufficientDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private Random random;
	public SufficientDungeon(Size pSize) : base(pSize){}
	private void Update()
	{
		if (Input.GetKeyDown(Key.A))
		{
			Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
		}

		Generator.RegenerateRooms(this);
		Generator.DrawDebugLines(rooms, size, game);
	}
		
	protected override void Make(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		Generator.DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)), random, doorsToBeAdded, minimumRoomSize, rooms);
			
		// RemoveSmallestRooms(GetSmallestSurface(roomsToBeAdded));
		rooms.AddRange(roomsToBeAdded);
		roomsToBeAdded.Clear();

		//Add doors and fix them if bad
		foreach (Door door in doorsToBeAdded)
		{
			Generator.TestDoor(door, rooms, random, doors);
		}
		doorsToBeAdded.Clear();
	}
}