using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution;

public static class Functions
{
	/// <summary>
	/// Divides a room, if the resulting rooms can be divided further, it divides them as well
	/// </summary>
	/// <param name="room">Target room</param>
	/// <param name="random">A seeded random</param>
	/// <param name="doorsToBeAdded">List of doors that need to be added</param>
	/// <param name="minimumRoomSize">The minimum room size</param>
	/// <param name="rooms">List of rooms</param>
	public static void DivideRoom(Room room, Random random, List<Door> doorsToBeAdded, int minimumRoomSize, List<Room> rooms)
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
			Room a = new(room.area with {Width = newPoint.X - room.area.X + 1});
			DivideRoom(a,random,doorsToBeAdded, minimumRoomSize,rooms);

			//Room2 (Right)
			Room b = new(room.area with {X = newPoint.X, Width = room.area.Width - (newPoint.X - room.area.X)});
			DivideRoom(b, random, doorsToBeAdded, minimumRoomSize, rooms);

			Point boundaries = new(room.area.Y + 1, room.area.Bottom - 1);
			doorsToBeAdded.Add(new Door(new Point(newPoint.X, random.Next(boundaries.X, boundaries.Y)), false, boundaries, a, b));
		}

		else if (room.area.Height >= minimumRoomSize * 2)
		{
			minimum = room.area.Y + minimumRoomSize;
			maximum = room.area.Bottom - minimumRoomSize;
			newPoint.Y = random.Next(minimum, maximum);

			//Room1 (Top)
			Room a = new(room.area with {Height = newPoint.Y - room.area.Y + 1});
			DivideRoom(a, random, doorsToBeAdded, minimumRoomSize, rooms);

			//Room2 (Bottom)
			Room b = new(room.area with {Y = newPoint.Y, Height = room.area.Height - (newPoint.Y - room.area.Y)});
			DivideRoom(b, random, doorsToBeAdded, minimumRoomSize, rooms);

			Point boundaries = new(room.area.X + 1, room.area.Right - 1);
			doorsToBeAdded.Add(new Door(newPoint with {X = random.Next(boundaries.X, boundaries.Y)}, true, boundaries, a, b));
		}

		else
		{
			rooms.Add(room);
		}
	}

	/// <summary>
	/// Check if the door is in a valid position, if not, move it and repeat until it is.
	/// </summary>
	/// <param name="door"></param>
	/// <param name="generatedRooms"></param>
	/// <param name="random"></param>
	/// <param name="doors"></param>
	public static void FixDoor(Door door, IEnumerable<Room> generatedRooms, Random random, List<Door> doors)
	{
		List<Room> enumerable = generatedRooms.ToList();
		if (enumerable.Any(room => door.location == room.topLeft || door.location == room.topRight ||
		                           door.location == room.bottomLeft || door.location == room.bottomRight))
		{
			if (door.horizontal)
			{
				door = new Door(door.location with {X = random.Next(door.boundaries.X, door.boundaries.Y)}, true,
					door.boundaries, door.roomA, door.roomB);
			}
			else
			{
				door = new Door(door.location with {Y = random.Next(door.boundaries.X, door.boundaries.Y)},
					false, door.boundaries, door.roomA, door.roomB);
			}

			FixDoor(door, enumerable, random, doors);
		}
		else doors.Add(door);
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
	
	/// <param name="plannedRooms">A list of the planned rooms</param>
	/// <param name="surface">The surface of the smallest room</param>
	/// <returns>The list of planned rooms without the smallest rooms</returns>
	public static List<Room> RemoveSmallestRooms(List<Room> plannedRooms, int surface)
	{
		for (int i = plannedRooms.Count - 1; i >= 0; i--)
		{
			Room room = plannedRooms[i];
			if (room.surface == surface)
			{
				plannedRooms.Remove(room);
			}
		}

		return plannedRooms;
	}
	
	
	
}