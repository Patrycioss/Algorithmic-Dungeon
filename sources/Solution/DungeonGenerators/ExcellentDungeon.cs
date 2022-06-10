using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class ExcellentDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private List<Door> doorsToBeTested;
	private Random random;

	private int increment = 1;

	public ExcellentDungeon(Size pSize) : base(pSize) { autoDrawAfterGenerate = false; }

	private void Update()
	{
		if (Input.GetKeyDown(Key.A))
		{
			Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
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
		Console.Clear();
		
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
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
		
		//Downsize the rooms
		foreach (Room room in roomsToBeAdded)
		{
			room.area.Inflate(random.Next(-3,-1),random.Next(-3,-1));
		}
		
		//Add doors
		foreach (Room room in roomsToBeAdded)
		{
			Generator.AddDoorsOfRoomToListExcellent(room, roomsToBeAdded, doorsToBeAdded, random);
		}
		
		//Make doors into corridors
		List<Door> corridorDoors = new List<Door>();

		foreach (Door door in doorsToBeAdded)
		{
			if (door.roomA == null || door.roomB == null) continue;

			Room roomB = door.roomB;
			
			Door lastDoor = null;
			
			if (door.orientation == Orientation.Horizontal)
			{
				int distance = roomB.area.Y - door.roomA.area.Bottom;
				if (distance > 0)
				{
					for (int i = 0; i < distance + 2; i++)
					{
						Door newDoor = new (door.location with{Y = door.location.Y + i}, Orientation.Horizontal);
						corridorDoors.Add(newDoor);
						
						if (i == distance + 1)
						{
							lastDoor = newDoor;
						}
					}
					
					door.roomB = null;

				}
			}
			else
			{
				int distance = roomB.area.X - door.roomA.area.Right;
				if (distance > 0)
				{
					for (int i = 0; i < distance + 2; i++)
					{
						Door newDoor = new(door.location with {X = door.location.X + i}, Orientation.Vertical);
						corridorDoors.Add(newDoor);

						if (i == distance + 1)
						{
							lastDoor = newDoor;
						}
					}
					
					door.roomB = null;

				}
			}
			
			
			if (lastDoor != null)
			{
				lastDoor.roomB = roomB;
			}

			
			
		}
		
		doorsToBeAdded.AddRange(corridorDoors);

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
		


		//Draw everything
		graphics.Clear(Color.Black);
		
		rooms.AddRange(roomsToBeAdded);
		
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
		
		doors.AddRange(doorsToBeAdded);
		DrawDoors(doors, Pens.GreenYellow);
	}
}