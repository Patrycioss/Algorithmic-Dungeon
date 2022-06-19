using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

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
	}

	protected override void Generate(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
		doorsToBeTested = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		
		//Remove smallest rooms
		foreach (Room room in GetRoomsWithSurface(GetSmallestSurface())) roomsToBeAdded.Remove(room);

		//Remove biggest rooms
		foreach (Room room in GetRoomsWithSurface(GetBiggestSurface())) roomsToBeAdded.Remove(room);

		//Move doors to another random position if they are placed on the corner of a room
		foreach (Door door in doorsToBeTested) TestDoor(door);
		
		//Assign the doors to the rooms, if the door doesn't have any rooms to get assigned to it gets deleted
		for (int index = doorsToBeAdded.Count - 1; index >= 0; index--)
		{
			Door door = doorsToBeAdded[index];
			AssignDoor(door);
			
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
			newPoint.X = random.Next(minimum, maximum);

			//Room1 (Left)
			Redo(new Room(room.area with {Width = newPoint.X - room.area.X + 1}));

			//Room2 (Right)
			Redo(new Room(room.area with {X = newPoint.X, Width = room.area.Width - (newPoint.X - room.area.X)}));
			
			Point boundaries = new(room.area.Y + 1, room.area.Bottom - 1);
			
			doorsToBeTested.Add(new Door(newPoint with {Y = random.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
		}

		else if (room.area.Height >= minimumRoomSize * 2)
		{
			minimum = room.area.Y + minimumRoomSize;
			maximum = room.area.Bottom - minimumRoomSize;
			newPoint.Y = random.Next(minimum, maximum);

			//Room1 (Top)
			Redo(new Room(room.area with {Height = newPoint.Y - room.area.Y + 1}));

			//Room2 (Bottom)
			Redo(new Room(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)}));

			Point boundaries = new(room.area.X + 1, room.area.Right - 1);
			
			doorsToBeTested.Add(new Door(newPoint with {X = random.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}

		else roomsToBeAdded.Add(room);

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
		}
	}
	
	/// <summary>
	/// Fix a door by giving it another valid random position.
	/// </summary>
	private void RepositionDoor(Door door)
	{
		if (door.orientation == Horizontal)
		{
			door = new Door(door.location with {X = random.Next(door.boundaries.X, door.boundaries.Y)}, Horizontal,
				door.boundaries);
		}
		else
		{
			door = new Door(door.location with {Y = random.Next(door.boundaries.X, door.boundaries.Y)},
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