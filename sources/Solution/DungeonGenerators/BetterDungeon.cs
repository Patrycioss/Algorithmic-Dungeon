using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.AlgorithmsAssignment;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class BetterDungeon : Dungeon
{
	private Random random;
	private int seedIncrement = 1;

	public BetterDungeon(Size pSize) : base(pSize) { autoDrawAfterGenerate = false; }

	protected override void Generate(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);

		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		if (DEBUG_MODE) Console.WriteLine("------------------------------------------------------");

		
		//Remove smallest rooms
		foreach (Room room in GetRoomsWithSurface(GetSmallestSurface()))
		{
			rooms.Remove(room);
			if (DEBUG_MODE) Console.WriteLine($"Removed room at {room.topLeft}");
		}

		//Remove biggest rooms
		foreach (Room room in GetRoomsWithSurface(GetBiggestSurface()))
		{
			rooms.Remove(room);
			if (DEBUG_MODE)Console.WriteLine($"Removed room at {room.topLeft}");
		}
		if (DEBUG_MODE)Console.WriteLine("------------------------------------------------------");


		//Add doors to rooms
		foreach (Room room in rooms) AddDoorsOfRoom(room);
		if (DEBUG_MODE)Console.WriteLine("------------------------------------------------------");


		//Debug purposes
		// foreach (Room room in rooms)
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

	/// <summary>
	/// Divides a room in a different way, if the resulting rooms can be divided further, it divides them as well
	/// </summary>
	/// <param name="room">Target room</param>
	private void DivideRoom(Room room)
	{
		int minimum;
		int maximum;

		Point newPoint = new(0, 0);
		
		if (room.area.Height > room.area.Width) goto horizontal;

		
		if (room.area.Width >= minimumRoomSize * 2)
		{
			minimum = room.area.X + minimumRoomSize;
			maximum = room.area.Right - minimumRoomSize;
			newPoint.X = random.Next(minimum, maximum);

			if (DEBUG_MODE)Console.WriteLine($"Dividing vertically at x = {newPoint.X}...");
			
			//Room1 (Left)
			Redo(new Room(room.area with {Width = newPoint.X - room.area.X + 1}));

			//Room2 (Right)
			Redo(new Room(room.area with {X = newPoint.X, Width = room.area.Width - (newPoint.X - room.area.X)}));
			return;
		}

		horizontal:
		if (room.area.Height >= minimumRoomSize * 2)
		{
			minimum = room.area.Y + minimumRoomSize;
			maximum = room.area.Bottom - minimumRoomSize;
			newPoint.Y = random.Next(minimum, maximum);
			
			if (DEBUG_MODE)Console.WriteLine($"Dividing horizontally at y = {newPoint.Y}...");

			//Room1 (Top)
			Redo(new Room(room.area with {Height = newPoint.Y - room.area.Y + 1}));

			//Room2 (Bottom)
			Redo(new Room(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)}));
		}

		else
		{
			rooms.Add(room);
			if (DEBUG_MODE)Console.WriteLine($"Created room at {room.topLeft} corner");
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

		for (int i = rooms.Count - 1; i >= 0; i--)
		{
			Room room = rooms[i];
			if (room.surface == surface)
			{
				roomsWithSurface.Add(room);
			}
		}
		return roomsWithSurface;
	}

	/// <returns>Returns the surface of the biggest room</returns>
	private int GetBiggestSurface()
	{
		int biggestSurface = int.MinValue;

		foreach (Room room in rooms)
		{
			if (room.surface > biggestSurface)
			{
				biggestSurface = room.surface;
			}
		}
		if (DEBUG_MODE) Console.WriteLine($"BiggestSurface is: {biggestSurface}");
		return biggestSurface;
	}

	/// <returns>Returns the surface of the smallest room</returns>
	private int GetSmallestSurface()
	{
		int smallestSurface = int.MaxValue;

		foreach (Room room in rooms)
		{
			if (room.surface < smallestSurface)
			{
				smallestSurface = room.surface;
			}
		}
		if (DEBUG_MODE)Console.WriteLine($"SmallestSurface is: {smallestSurface}");
		return smallestSurface;
	}
	
	
	/// <summary>
	/// Adds doors to an existing room by working from the topLeft room to the bottomRight one
	/// </summary>
	/// <param name="room"></param>
	private void AddDoorsOfRoom(Room room)
	{
		foreach (Room otherRoom in rooms)
		{
			if (room.Equals(otherRoom)) continue;
			Door door = null;

			
			//If rooms overlap on the horizontal axis
			if (otherRoom.area.Top + 1 == room.area.Bottom && 
			    ((otherRoom.area.X < room.area.Right && otherRoom.area.X >= room.area.X) 
			     || (otherRoom.area.Right <= room.area.Right && otherRoom.area.Right > room.area.X)))
			{
				//Calculate the boundaries of where the door can be
				Point boundaries = new()
				{
					X = room.area.X > otherRoom.area.X ? room.area.X + 1 : otherRoom.area.X + 1,
					Y = room.area.Right < otherRoom.area.Right ? room.area.Right - 1 : otherRoom.area.Right - 1
				};

				//If something goes wrong with the boundaries continue to prevent a crash
				if (boundaries.X >= boundaries.Y) continue;

				door = new Door(new Point(random.Next(boundaries.X, boundaries.Y), otherRoom.area.Top), Horizontal, boundaries)
				{
					roomA = room,
					roomB = otherRoom
				};
				doors.Add(door);
				if (DEBUG_MODE)Console.WriteLine($"Added door {doors.Count-1} in doors at {door.location} with roomA at {door.roomA.topLeft} and roomB at {door.roomB.topLeft}");
			}
			
			//if rooms overlap on the vertical axis
			else if (otherRoom.area.Left + 1 == room.area.Right && 
			         ((otherRoom.area.Y < room.area.Bottom && otherRoom.area.Y >= room.area.Top) 
			          || (otherRoom.area.Bottom <= room.area.Bottom && otherRoom.area.Bottom > room.area.Top)))
			{
				//Calculate the boundaries of where the door can be
				Point boundaries = new()
				{
					X = room.area.Y > otherRoom.area.Y ? room.area.Y + 1 : otherRoom.area.Y + 1,
					Y = room.area.Bottom < otherRoom.area.Bottom ? room.area.Bottom - 1 : otherRoom.area.Bottom - 1
				};

				//If something goes wrong with the boundaries continue to prevent a crash
				if (boundaries.X >= boundaries.Y) continue;
				
				door = new Door(new Point(otherRoom.area.Left, random.Next(boundaries.X, boundaries.Y)), Vertical, boundaries)
				{
					roomA = room,
					roomB = otherRoom
				};
				doors.Add(door);
				if (DEBUG_MODE)Console.WriteLine($"Added door {doors.Count-1} in doors at {door.location} with roomA at {door.roomA.topLeft} and roomB at {door.roomB.topLeft}");
			}

			if (door != null)
			{
				room.doors.Add(door);
				otherRoom.doors.Add(door);
			}
		}
	}
}