using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class GoodDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private List<Door> doorsToBeTested;
	private Random random;

	private int increment = 1;

	public GoodDungeon(Size pSize) : base(pSize) { autoDrawAfterGenerate = false; }

	private void Update()
	{
		if (Input.GetKeyDown(Key.A))
		{
			Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
		}

		for (int i = 48; i <= 57; i++)
		{
			if (Input.GetKeyDown(i))
			{
				Generate(AlgorithmsAssignment.MIN_ROOM_SIZE,i * increment);
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

	protected override void Make(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
		doorsToBeTested = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		Generator.DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)), random, doorsToBeTested, minimumRoomSize, roomsToBeAdded);
		
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
		
		//Move doors to another random position if they are placed on the corner of a room
		for (int index = doorsToBeTested.Count-1; index >= 0; index--)
		{
			Door door = doorsToBeTested[index];
			Generator.TestDoor(door, roomsToBeAdded, random, doorsToBeAdded);
		}

		//Assign the doors to the rooms, if the door doesn't have any rooms to get assigned to it gets deleted
		for (int index = doorsToBeAdded.Count - 1; index >= 0; index--)
		{
			Door door = doorsToBeAdded[index];
			Generator.AssignDoor(door, roomsToBeAdded);
			
			if (door.roomB == null || door.roomA == null) doorsToBeAdded.Remove(door);
			else
			{
				door.roomA?.doors.Add(door);
				door.roomB?.doors.Add(door);
			}
		}

		//Debug purposes
		foreach (Room room in roomsToBeAdded)
		{
			string doorPositions = $"Room: {room.topLeft} with {room.doors.Count} doors";
			
			foreach (Door door in room.doors)
			{
				doorPositions += $" ,{door.location} (rooms: {door.roomA}, {door.roomB}";
			}
			Console.WriteLine(doorPositions);
		}
		


		//Draw everything
		graphics.Clear(Color.Transparent);
		
		foreach (Room room in roomsToBeAdded)
		{
			Console.WriteLine(room.doors.Count);
			
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
		
		DrawDoors(doorsToBeAdded, Pens.White);
	}
}