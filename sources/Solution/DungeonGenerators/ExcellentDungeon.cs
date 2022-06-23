using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class ExcellentDungeon : Dungeon
{
	private Random random;
	private int seedIncrement = 1;

	public ExcellentDungeon(Size pSize) : base(pSize) { autoDrawAfterGenerate = true; }

	protected override void Generate(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
			
		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		if (debugMode) Console.WriteLine("------------------------------------------------------");
		
		//Remove smallest rooms
		foreach (Room room in GetRoomsWithSurface(GetSmallestSurface()))
		{
			rooms.Remove(room);
			if (debugMode) Console.WriteLine($"Removed room at {room.topLeft}");
		}

		//Remove biggest rooms
		foreach (Room room in GetRoomsWithSurface(GetBiggestSurface()))
		{
			rooms.Remove(room);
			if (debugMode) Console.WriteLine($"Removed room at {room.topLeft}");
		}
		if (debugMode) Console.WriteLine("------------------------------------------------------");


		//Downsize the rooms
		foreach (Room room in rooms)
		{
			if (debugMode) Console.WriteLine($"Before deflation at: {room.topLeft} with width: {room.area.Width} and height: {room.area.Height}");
			room.area.Inflate(random.Next(-2, -1), random.Next(-2, -1));
			if (debugMode) Console.WriteLine($"After deflation at {room.topLeft} with width: {room.area.Width} and height: {room.area.Height}");
		}
		if (debugMode) Console.WriteLine("------------------------------------------------------");


		//Add doors
		foreach (Room room in rooms) {AddDoorsOfRoom(room);}
		if (debugMode) Console.WriteLine("------------------------------------------------------");
		
		
		//Make doors into corridors based on orientation
		for (int i = 0; i < doors.Count; i++) TurnDoorIntoCorridor(doors[i]);
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
		graphics.Clear(Color.Black);
		
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

			void UseBrush(Brush brush) => DrawRoom(room,Pens.Black,brush);
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
			
			if (debugMode) Console.WriteLine($"Dividing vertically at x = {newPoint.X}...");

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
			
			if (debugMode) Console.WriteLine($"Dividing horizontally at y = {newPoint.Y}...");
			
			//Room1 (Top)
			Redo(new Room(room.area with {Height = newPoint.Y - room.area.Y + 1}));

			//Room2 (Bottom)
			Redo(new Room(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)}));
		}

		else
		{
			rooms.Add(room);
			if (debugMode) Console.WriteLine($"Created room at {room.topLeft} corner");
		}

		void Redo(Room roomToBeRedone) => DivideRoom(roomToBeRedone);
	}

	/// <param name="surface">The specified surface)</param>
	/// <returns>A list of rooms with the specified surface</returns>
	private List<Room> GetRoomsWithSurface(int surface)
	{
		List<Room> roomsWithSurface = new List<Room>();
		for (int i = rooms.Count - 1; i >= 0; i--) if (rooms[i].surface == surface) roomsWithSurface.Add(rooms[i]);
		return roomsWithSurface;
	}

	/// <returns>Returns the surface of the biggest room</returns>
	private int GetBiggestSurface()
	{
		int biggestSurface = int.MinValue;
		foreach (Room room in rooms) if (room.surface > biggestSurface) biggestSurface = room.surface;
		if (debugMode) Console.WriteLine($"BiggestSurface is: {biggestSurface}");
		return biggestSurface;
	}

	/// <returns>Returns the surface of the smallest room</returns>
	private int GetSmallestSurface()
	{
		int smallestSurface = int.MaxValue;
		foreach (Room room in rooms) if (room.surface < smallestSurface) smallestSurface = room.surface;
		if (debugMode) Console.WriteLine($"SmallestSurface is: {smallestSurface}");
		return smallestSurface;
	}

	private void TurnDoorIntoCorridor(Door door)
	{
		if (door.roomA == null || door.roomB == null) return;
		
		Room roomB = door.roomB;
		Door lastDoor = null;
			
		if (door.orientation == Horizontal)
		{
			int distance = roomB.area.Y - door.roomA.area.Bottom;
			if (distance > 0)
			{
				for (int i = 1; i < distance + 2; i++)
				{
					Door newDoor = new (door.location with{Y = door.location.Y + i}, Horizontal);
					doors.Add(newDoor);
						
					if (i == distance + 1) lastDoor = newDoor;
				}
				door.roomB = null;
			}
			if (debugMode) Console.WriteLine($"Added {distance} doors to make a hallway from roomA at {door.roomA.topLeft} to roomB at {roomB.topLeft}");
		}
		else
		{
			int distance = roomB.area.X - door.roomA.area.Right;
			if (distance > 0)
			{
				for (int i = 1; i < distance + 2; i++)
				{
					Door newDoor = new(door.location with {X = door.location.X + i}, Vertical);
					doors.Add(newDoor);

					if (i == distance + 1) lastDoor = newDoor;
				}
				door.roomB = null;
			}
			
			if (debugMode) Console.WriteLine($"Added {distance} doors to make a hallway from roomA at {door.roomA.topLeft} to roomB at {roomB.topLeft}");
		}
		if (lastDoor != null) lastDoor.roomB = roomB;
	}

	private void AddDoorsOfRoom(Room room)
	{
		List<Room> viableHorizontals = new List<Room>();
		List<Room> viableVerticals = new List<Room>();

		foreach (Room otherRoom in rooms)
		{
			if (room.Equals(otherRoom)) continue;

			if (otherRoom.area.Y >= room.area.Y)
			{
				if ((otherRoom.area.X < room.area.Right && otherRoom.area.Right >= room.area.X) ||
				    (room.area.X < otherRoom.area.Right && room.area.Right >= otherRoom.area.X))
				{
					viableVerticals.Add(otherRoom);
				}
			}

			if (otherRoom.area.X >= room.area.X)
			{
				if ((otherRoom.area.Y < room.area.Bottom && otherRoom.area.Bottom >= room.area.Y) ||
				    (room.area.Y < otherRoom.area.Bottom && room.area.Bottom >= otherRoom.area.Y))
				{
					viableHorizontals.Add(otherRoom);
				}
			}

		}

		if (viableHorizontals.Count > 0)
		{
			Room mostViableHorizontal = null;

			if (viableHorizontals.Count > 1)
			{
				foreach (Room viableHorizontal in viableHorizontals)
				{
					if (mostViableHorizontal == null) mostViableHorizontal = viableHorizontal;
					else if (mostViableHorizontal.area.X > viableHorizontal.area.X) mostViableHorizontal = viableHorizontal;
				}
			}
			else mostViableHorizontal = viableHorizontals[0];

			MakeHorizontalDoorToViableRoom(mostViableHorizontal);
		}

		if (viableVerticals.Count > 0)
		{
			Room mostViableVertical = null;

			if (viableVerticals.Count > 1)
			{
				foreach (Room viableVertical in viableVerticals)
				{
					if (mostViableVertical == null) mostViableVertical = viableVertical;
					else if (mostViableVertical.area.Y > viableVertical.area.Y) mostViableVertical = viableVertical;
				}
			}
			else mostViableVertical = viableVerticals[0];

			MakeVerticalDoorToViableRoom(mostViableVertical);
			
			
		}
		
		void MakeHorizontalDoorToViableRoom(Room viableRoom)
		{
			Point boundaries = new()
			{
				X = room.area.Y > viableRoom.area.Y ? room.area.Y + 1 : viableRoom.area.Y + 1,
				Y = room.area.Bottom < viableRoom.area.Bottom ? room.area.Bottom - 1 : viableRoom.area.Bottom - 1
			};

			if (boundaries.X >= boundaries.Y) return;

			Door door = new(new Point(room.area.Right - 1, random.Next(boundaries.X, boundaries.Y)), Vertical, boundaries);
			doors.Add(door);
			room.doors.Add(door);
			viableRoom.doors.Add(door);
			door.roomA = room;
			door.roomB = viableRoom;

			if (debugMode) Console.WriteLine($"Created horizontal door at {door.location} between roomA at {door.roomA.topLeft} and roomB at {door.roomB.topLeft}");
		}

		
		void MakeVerticalDoorToViableRoom(Room viableRoom)
		{
			Point boundaries = new()
			{
				X = room.area.X > viableRoom.area.X ? room.area.X + 1 : viableRoom.area.X + 1,
				Y = room.area.Right < viableRoom.area.Right ? room.area.Right - 1 : viableRoom.area.Right - 1
			};

			if (boundaries.X >= boundaries.Y)
			{
				Console.WriteLine($"RAR: {boundaries}, {room.area}, {viableRoom.area}");
				return;
			}

			Door door = new(new Point(random.Next(boundaries.X, boundaries.Y), room.area.Bottom - 1), Horizontal, boundaries);
			doors.Add(door);
			room.doors.Add(door);
			viableRoom.doors.Add(door);
			door.roomA = room;
			door.roomB = viableRoom;
			
			if (debugMode) Console.WriteLine($"Created vertical door at {door.location} between roomA at {door.roomA.topLeft} and roomB at {door.roomB.topLeft}");
		}
	}
}