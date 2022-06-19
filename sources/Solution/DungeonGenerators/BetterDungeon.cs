using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class BetterDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private List<Door> doorsToBeTested;
	private Random random;

	private int increment = 1;

	public BetterDungeon(Size pSize) : base(pSize) { autoDrawAfterGenerate = false; }

	private void Update()
	{
		if (Input.GetKeyDown(Key.A))
		{
			InternalGenerate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
		}

		for (int i = 48; i <= 57; i++)
		{
			if (Input.GetKeyDown(i))
			{
				InternalGenerate(AlgorithmsAssignment.MIN_ROOM_SIZE,i * increment);
			}
		}

		if (Input.GetKeyDown(Key.PLUS))
		{
			increment *= 10;
		}
		else if (Input.GetKeyDown(Key.MINUS) && increment > 1)
		{
			increment /= 10;
		}
		
		

		Generator.RegenerateRooms(this);
	}

	protected override void Generate(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
		doorsToBeTested = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		Generator.DivideRoomBetter(new Room(new Rectangle(0, 0, size.Width, size.Height)), random, doorsToBeTested, minimumRoomSize, roomsToBeAdded, false);
		
		//Remove smallest rooms
		foreach (Room room in Generator.GetRoomsWithSurface(roomsToBeAdded, Generator.GetSmallestSurface(roomsToBeAdded)))
		{
			roomsToBeAdded.Remove(room);
		}
		
		//Remove biggest rooms
		foreach (Room room in Generator.GetRoomsWithSurface(roomsToBeAdded, Generator.GetBiggestSurface(roomsToBeAdded)))
		{
			roomsToBeAdded.Remove(room);
		}
		
		//Add doors to rooms
		foreach (Room room in roomsToBeAdded)
		{
			Generator.AddDoorsOfRoomToList(room,roomsToBeAdded,doorsToBeAdded,random);
		}

		//Debug purposes
		// foreach (Room room in roomsToBeAdded)
		// {
		// 	string doorPositions = $"Room: {room.topLeft} with {room.doors.Count} doors";
		// 	
		// 	foreach (Door door in room.doors)
		// 	{
		// 		doorPositions += $" ,{door.location} (rooms: {door.roomA}, {door.roomB}";
		// 	}
		// 	Console.WriteLine(doorPositions);
		// }


		rooms.AddRange(roomsToBeAdded);
		doors.AddRange(doorsToBeAdded);
		

		//Draw everything
		graphics.Clear(Color.Transparent);
		
		foreach (Room room in rooms)
		{
			switch (room.doors.Count)
			{
				case 0:
					UseBrush(Brushes.Red);
					break;
				case 1:
					UseBrush(Brushes.Orange);
					break;
				case 2:
					UseBrush(Brushes.Yellow);
					break;
				default:
					UseBrush(Brushes.Green);
					break;
			}

			void UseBrush(Brush brush)
			{
				DrawRoom(room,Pens.Black,brush);
			}
		}
		
		DrawDoors(doors, Pens.GreenYellow);
	}
}