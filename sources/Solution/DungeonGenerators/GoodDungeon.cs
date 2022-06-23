using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.AlgorithmsAssignment;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class GoodDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private List<Door> doorsToBeTested;
	
	public GoodDungeon(Size pSize, int pScale) : base(pSize, pScale) { autoDrawAfterGenerate = false; }

	protected override void Generate(int pMinimumRoomSize)
	{
		doorsToBeAdded = new List<Door>();
		doorsToBeTested = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		if (debugMode) Console.WriteLine("------------------------------------------------------");
		
		//Remove smallest rooms
		foreach (Room room in GetRoomsWithSurface(GetSmallestSurface()))
		{
			roomsToBeAdded.Remove(room);
			if (debugMode) Console.WriteLine($"Removed room at {room.topLeft}");
		}

		//Remove biggest rooms
		foreach (Room room in GetRoomsWithSurface(GetBiggestSurface()))
		{
			roomsToBeAdded.Remove(room);
			if (debugMode) Console.WriteLine($"Removed room at {room.topLeft}");
		}
		if (debugMode) Console.WriteLine("------------------------------------------------------");

		//Move doors to another random position if they are placed on the corner of a room
		foreach (Door door in doorsToBeTested) TestDoor(door);
		if (debugMode) Console.WriteLine("------------------------------------------------------");
		
		//Assign the doors to the rooms, if the door doesn't have any rooms to get assigned to it gets deleted
		for (int index = doorsToBeAdded.Count - 1; index >= 0; index--)
		{
			Door door = doorsToBeAdded[index];
			AssignDoor(door);

			if (door.roomB == null || door.roomA == null)
			{
				doorsToBeAdded.Remove(door);
				if (debugMode) Console.WriteLine($"Removed door {index} in doorsToBeAdded");
			}
			else
			{
				door.roomA?.doors.Add(door);
				door.roomB?.doors.Add(door);
				
				if (debugMode) Console.WriteLine($"Assigned door {index} in doorsToBeAdded to roomA at {door.roomA?.topLeft} and roomB at {door.roomB?.topLeft}");
			}
		}
		if (debugMode) Console.WriteLine("------------------------------------------------------");

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
		graphics.Clear(Color.Transparent);
		
		rooms.AddRange(roomsToBeAdded);
		doors.AddRange(doorsToBeAdded);
		
		foreach (Room room in rooms)
		{
			switch (room.doors.Count)
			{
				case 0:
					DrawRoomWithBrush(Brushes.Red);
					break;
				case 1:
					DrawRoomWithBrush(Brushes.Orange);
					break;
				case 2:
					DrawRoomWithBrush(Brushes.Yellow);
					break;
				default:
					DrawRoomWithBrush(Brushes.Green);
					break;
			}

			void DrawRoomWithBrush(Brush brush)
			{
				DrawRoom(room,Pens.Black,brush);
			}
		}
		
		DrawDoors(doorsToBeAdded, Pens.White);
		
		doors.AddRange(doors);
		
	}
	
	/// <summary>
	/// Divides a room, if the resulting rooms can be divided further, it divides them as well
	/// </summary>
	/// <param name="room">Target room</param>
	private void DivideRoom(Room room)
	{
		int minimum;
		int maximum;

		Point newPoint = new(0, 0);

		if (room.area.Width >= minimumRoomSize * 2)
		{
			minimum = room.area.X + minimumRoomSize;
			maximum = room.area.Right - minimumRoomSize;
			newPoint.X = randomNumberGenerator.Next(minimum, maximum);
			
			if (debugMode) Console.WriteLine($"Dividing vertically at x = {newPoint.X}...");
			
			//Room1 (Left)
			Redo(new Room(room.area with {Width = newPoint.X - room.area.X + 1}));

			//Room2 (Right)
			Redo(new Room(room.area with {X = newPoint.X, Width = room.area.Width - (newPoint.X - room.area.X)}));
			
			Point boundaries = new(room.area.Y + 1, room.area.Bottom - 1);
			
			doorsToBeTested.Add(new Door(newPoint with {Y = randomNumberGenerator.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
		}

		else if (room.area.Height >= minimumRoomSize * 2)
		{
			minimum = room.area.Y + minimumRoomSize;
			maximum = room.area.Bottom - minimumRoomSize;
			newPoint.Y = randomNumberGenerator.Next(minimum, maximum);

			if (debugMode) Console.WriteLine($"Dividing horizontally at y = {newPoint.Y}...");

			//Room1 (Top)
			Redo(new Room(room.area with {Height = newPoint.Y - room.area.Y + 1}));

			//Room2 (Bottom)
			Redo(new Room(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)}));

			Point boundaries = new(room.area.X + 1, room.area.Right - 1);
			
			doorsToBeTested.Add(new Door(newPoint with {X = randomNumberGenerator.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}

		else
		{
			roomsToBeAdded.Add(room);
			if (debugMode) Console.WriteLine($"Created room at {room.topLeft} corner");
		}

		void Redo(Room roomToBeRedone)
		{
			DivideRoom(roomToBeRedone);
		}
	}

	/// <param name="surface">The specified surface)</param>
	/// <returns>A list of rooms with the specified surface</returns>
	private List<Room> GetRoomsWithSurface(int surface)
	{
		List<Room> roomsWithSurface = new List<Room>();

		for (int i = roomsToBeAdded.Count - 1; i >= 0; i--)
		{
			Room room = roomsToBeAdded[i];
			if (room.surface == surface)
			{
				roomsWithSurface.Add(room);
			}
		}
		return roomsWithSurface;
	}

	/// <returns>Returns the surface of the biggest room</returns>
	public int GetBiggestSurface()
	{
		int biggestSurface = int.MinValue;

		foreach (Room room in roomsToBeAdded)
		{
			if (room.surface > biggestSurface)
			{
				biggestSurface = room.surface;
			}
		}
		if (debugMode) Console.WriteLine($"BiggestSurface is: {biggestSurface}");
		return biggestSurface;
	}
	
	/// <returns>Returns the surface of the smallest room</returns>
	public int GetSmallestSurface()
	{
		int smallestSurface = int.MaxValue;

		foreach (Room room in roomsToBeAdded)
		{
			if (room.surface < smallestSurface)
			{
				smallestSurface = room.surface;
			}
		}

		if (debugMode) Console.WriteLine($"SmallestSurface is: {smallestSurface}");
		return smallestSurface;
	}
	
	/// <summary>
	/// Tests whether a door is in an illegal place (on a corner).
	/// </summary>
	/// <param name="door"></param>
	private void TestDoor(Door door)
	{
		if (roomsToBeAdded.Any(room => door.location == room.topLeft || door.location == room.topRight ||
		                      door.location == room.bottomLeft || door.location == room.bottomRight))
		{
			RepositionDoor(door);
		}
		else
		{
			doorsToBeAdded.Add(door);
			if (debugMode) Console.WriteLine($"Tested door number {doorsToBeAdded.Count-1} in doorsToBeAdded at {door.location}");
		}
	}
	
	/// <summary>
	/// Fix a door by giving it another valid random position.
	/// </summary>
	private void RepositionDoor(Door door)
	{
		if (door.orientation == Horizontal)
		{
			door = new Door(door.location with {X = randomNumberGenerator.Next(door.boundaries.X, door.boundaries.Y)}, Horizontal,
				door.boundaries);
		}
		else
		{
			door = new Door(door.location with {Y = randomNumberGenerator.Next(door.boundaries.X, door.boundaries.Y)},
				Vertical, door.boundaries);
		}
		TestDoor(door);
	}

	/// <summary>
	/// Assign a door to its rooms
	/// </summary>
	/// <param name="door"></param>
	private void AssignDoor(Door door)
	{
		foreach (Room room in roomsToBeAdded)
		{
			if (door.location.X < room.area.Right && door.location.X >= room.area.Left && door.location.Y >= room.area.Top &&
			    door.location.Y < room.area.Bottom)
			{
				if (door.roomA == null) door.roomA = room;
				else door.roomB = room;
			}
		}
	}
}