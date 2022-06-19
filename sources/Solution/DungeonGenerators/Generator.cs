using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.AddOns;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

/// <summary>
/// Class that contains all the functions necessary for my dungeons to generate. This is done to prevent code duplication.
/// </summary>
public static class Generator
{
	/// <summary>
	/// Divides a room, if the resulting rooms can be divided further, it divides them as well
	/// </summary>
	/// <param name="room">Target room</param>
	/// <param name="random">A seeded random</param>
	/// <param name="doorsToBeAdded">List of doors that need to be added</param>
	/// <param name="minimumRoomSize">The minimum room size</param>
	/// <param name="rooms">List of rooms</param>
	/// <param name="addDoors">Whether doors are added during the generation of the rooms</param>
	public static void DivideRoom(Room room, Random random, List<Door> doorsToBeAdded, int minimumRoomSize, List<Room> rooms, bool addDoors = true)
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
			
			if (addDoors) doorsToBeAdded.Add(new Door(newPoint with {Y = random.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
			
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
			
			if (addDoors) doorsToBeAdded.Add(new Door(newPoint with {X = random.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}

		else rooms.Add(room);

		void Redo(Room roomToBeRedone)
		{
			DivideRoom(roomToBeRedone,random,doorsToBeAdded,minimumRoomSize,rooms,addDoors);
		}
	}
	
	/// <summary>
	/// Divides a room in a different way, if the resulting rooms can be divided further, it divides them as well
	/// </summary>
	/// <param name="room">Target room</param>
	/// <param name="random">A seeded random</param>
	/// <param name="doorsToBeAdded">List of doors that need to be added</param>
	/// <param name="minimumRoomSize">The minimum room size</param>
	/// <param name="rooms">List of rooms</param>
	/// <param name="addDoors">Whether doors are added during the generation of the rooms</param>
	public static void DivideRoomBetter(Room room, Random random, List<Door> doorsToBeAdded, int minimumRoomSize, List<Room> rooms, bool addDoors = true)
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

			//Room1 (Left)
			Redo(new Room(room.area with {Width = newPoint.X - room.area.X + 1}));

			//Room2 (Right)
			Redo(new Room(room.area with {X = newPoint.X, Width = room.area.Width - (newPoint.X - room.area.X)}));

			Point boundaries = new(room.area.Y + 1, room.area.Bottom - 1);

			if (addDoors) doorsToBeAdded.Add(new Door(newPoint with {Y = random.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
			return;
		}

		horizontal:
		if (room.area.Height >= minimumRoomSize * 2)
		{
			minimum = room.area.Y + minimumRoomSize;
			maximum = room.area.Bottom - minimumRoomSize;
			newPoint.Y = random.Next(minimum, maximum);

			//Room1 (Top)
			Redo(new Room(room.area with {Height = newPoint.Y - room.area.Y + 1}));

			//Room2 (Bottom)
			Redo(new Room(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)}));

			Point boundaries = new(room.area.X + 1, room.area.Right - 1);
			
			if (addDoors) doorsToBeAdded.Add(new Door(newPoint with {X = random.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}

		else rooms.Add(room);

		void Redo(Room roomToBeRedone)
		{
			DivideRoomBetter(roomToBeRedone,random,doorsToBeAdded,minimumRoomSize,rooms,addDoors);
		}
	}

	/// <summary>
	/// Fix a door by giving it another valid random position.
	/// </summary>
	/// <param name="door"></param>
	/// <param name="generatedRooms"></param>
	/// <param name="random"></param>
	/// <param name="doors"></param>
	public static void RepositionDoor(Door door, List<Room> generatedRooms, Random random, List<Door> doors)
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
		TestDoor(door,generatedRooms,random,doors);
	}

	public static void TestDoor(Door door, List<Room> rooms, Random random, List<Door> doors)
	{
		if (rooms.Any(room => door.location == room.topLeft || door.location == room.topRight ||
		                      door.location == room.bottomLeft || door.location == room.bottomRight))
		{
			RepositionDoor(door, rooms,random, doors);
		}
		else
		{
			doors.Add(door);
		}
	}

	/// <param name="existingRooms">A list of the existing rooms</param>
	/// <returns>Returns the surface of the smallest room</returns>
	public static int GetSmallestSurface(List<Room> existingRooms)
	{
		int smallestSurface = int.MaxValue;

		foreach (Room room in existingRooms)
		{
			if (room.surface < smallestSurface)
			{
				smallestSurface = room.surface;
			}
		}
		return smallestSurface;
	}
	
	/// <param name="existingRooms">A list of the existing rooms</param>
	/// <returns>Returns the surface of the biggest room</returns>
	public static int GetBiggestSurface(List<Room> existingRooms)
	{
		int biggestSurface = int.MinValue;

		foreach (Room room in existingRooms)
		{
			if (room.surface > biggestSurface)
			{
				biggestSurface = room.surface;
			}
		}
		return biggestSurface;
	}
	
	/// <param name="plannedRooms">A list of the planned rooms</param>
	/// <param name="surface">The specified surface)</param>
	/// <returns>A list of rooms with the specified surface</returns>
	public static List<Room> GetRoomsWithSurface(List<Room> plannedRooms, int surface)
	{
		List<Room> rooms = new List<Room>();

		for (int i = plannedRooms.Count - 1; i >= 0; i--)
		{
			Room room = plannedRooms[i];
			if (room.surface == surface)
			{
				rooms.Add(room);
			}
		}
		return rooms;
	}
	
	public static void DrawDebugLines(List<Room> rooms, Size size, Game game)
	{
		foreach (Room room in rooms)
		{
			float rx = Mathf.Map(room.area.X, 0, size.Width, 0, game.width);
			float ry = Mathf.Map(room.area.Y, 0, size.Height, 0, game.height);
			float rw = Mathf.Map(room.area.Width, 0, size.Width, 0, game.width);
			float rh = Mathf.Map(room.area.Height, 0, size.Height, 0, game.height);
			Gizmos.DrawRectangle(rx + 1, ry, rw, rh, color: (uint) Color.Aqua.ToArgb());
		}
	}

	public static void AssignDoor(Door door, List<Room> rooms)
	{
		foreach (Room room in rooms)
		{
			if (door.location.X < room.area.Right && door.location.X >= room.area.Left && door.location.Y >= room.area.Top &&
			    door.location.Y < room.area.Bottom)
			{
				if (door.roomA == null) door.roomA = room;
				else door.roomB = room;
			}
		}
	}

	public static void RegenerateRooms(Dungeon dungeon)
	{
		//Regenerate dungeon
		if (Input.GetKeyDown(Key.SPACE))
		{
			dungeon.InternalGenerate(AlgorithmsAssignment.MIN_ROOM_SIZE, Utils.Random(int.MinValue, int.MaxValue));
		}
	}

	public static void AddDoorsOfRoomToList(Room room, List<Room> rooms, List<Door> doors, Random random)
	{
		foreach (Room otherRoom in rooms)
		{
			if (room.Equals(otherRoom)) continue;
			Door door = null;

			if (otherRoom.area.Top + 1 == room.area.Bottom && 
			    ((otherRoom.area.X < room.area.Right && otherRoom.area.X >= room.area.X) 
			     || (otherRoom.area.Right <= room.area.Right && otherRoom.area.Right > room.area.X)))
			{
				Point boundaries = new()
				{
					X = room.area.X > otherRoom.area.X ? room.area.X + 1 : otherRoom.area.X + 1,
					Y = room.area.Right < otherRoom.area.Right ? room.area.Right - 1 : otherRoom.area.Right - 1
				};

				if (boundaries.X >= boundaries.Y)
				{
					Console.WriteLine(boundaries + ", horizontal");
				}
				else 
				{
					door = new Door(new Point(random.Next(boundaries.X, boundaries.Y), otherRoom.area.Top), Horizontal, boundaries);
					door.roomA = room;
					door.roomB = otherRoom;
					doors.Add(door);
				}
			}
			
			else if (otherRoom.area.Left + 1 == room.area.Right && 
			         ((otherRoom.area.Y < room.area.Bottom && otherRoom.area.Y >= room.area.Top) 
			          || (otherRoom.area.Bottom <= room.area.Bottom && otherRoom.area.Bottom > room.area.Top)))
			{
				
				Point boundaries = new()
				{
					X = room.area.Y > otherRoom.area.Y ? room.area.Y + 1 : otherRoom.area.Y + 1,
					Y = room.area.Bottom < otherRoom.area.Bottom ? room.area.Bottom - 1 : otherRoom.area.Bottom - 1
				};

				if (boundaries.X >= boundaries.Y) continue;
				
				door = new Door(new Point(otherRoom.area.Left, random.Next(boundaries.X, boundaries.Y)), Vertical, boundaries);
				door.roomA = room;
				door.roomB = otherRoom;
				doors.Add(door);
			}

			if (door != null)
			{
				room.doors.Add(door);
				otherRoom.doors.Add(door);
			}
		}
		
	}

	public static void AddDoorsOfRoomToListExcellent(Room room, List<Room> rooms, List<Door> doors, Random random)
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

			if (boundaries.X >= boundaries.Y)
			{
				return;
			}

			Door door = new(new Point(room.area.Right - 1, random.Next(boundaries.X, boundaries.Y)), Vertical, boundaries);
			doors.Add(door);
			room.doors.Add(door);
			viableRoom.doors.Add(door);
			door.roomA = room;
			door.roomB = viableRoom;
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

			Door door = new(new Point(random.Next(boundaries.X, boundaries.Y), room.area.Bottom - 1), Door.Orientation.Horizontal, boundaries);
			doors.Add(door);
			room.doors.Add(door);
			viableRoom.doors.Add(door);
			door.roomA = room;
			door.roomB = viableRoom;
		}
	}
}