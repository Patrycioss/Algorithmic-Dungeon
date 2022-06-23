using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class SufficientDungeon : Dungeon
{
	private List<Door> doorsToBeAdded;

	public SufficientDungeon(Size pSize, int pScale) : base(pSize, pScale){}

	protected override void Generate(int pMinimumRoomSize)
	{
		doorsToBeAdded = new List<Door>();
			
		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		if (debugMode) Console.WriteLine("------------------------------------------------------");


		//Add doors and fix them if bad
		foreach (Door door in doorsToBeAdded) TestDoor(door);
		doorsToBeAdded.Clear();
		if (debugMode) Console.WriteLine("------------------------------------------------------");

		
		//Assign the doors to the rooms, if the door doesn't have any rooms to get assigned to it gets deleted
		for (int index = doors.Count - 1; index >= 0; index--)
		{
			Door door = doors[index];
			AssignDoor(door);

			if (door.roomB == null || door.roomA == null)
			{
				doorsToBeAdded.Remove(door);
				if (debugMode) Console.WriteLine($"Removed door {index} in doors");
			}
			else
			{
				door.roomA?.doors.Add(door);
				door.roomB?.doors.Add(door);

				if (debugMode) Console.WriteLine($"Assigned door {index} in doors to roomA at {door.roomA?.topLeft} and roomB at {door.roomB?.topLeft}");
			}
		}
		if (debugMode) Console.WriteLine("------------------------------------------------------");

	}
	
	/// <summary>
	/// Tests whether a door is in an illegal place (on a corner).
	/// </summary>
	/// <param name="door"></param>
	private void TestDoor(Door door)
	{
		if (rooms.Any(room => door.location == room.topLeft || door.location == room.topRight ||
		                      door.location == room.bottomLeft || door.location == room.bottomRight))
		{
			RepositionDoor(door);
		}
		else
		{
			doors.Add(door);
			if (debugMode) Console.WriteLine($"Tested door number {doors.Count-1} in doors at {door.location}");
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
			
			doorsToBeAdded.Add(new Door(newPoint with {Y = randomNumberGenerator.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
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
			
			doorsToBeAdded.Add(new Door(newPoint with {X = randomNumberGenerator.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}
		else
		{
			rooms.Add(room);
			if (debugMode) Console.WriteLine($"Created room at {room.topLeft} corner");
		}

		void Redo(Room roomToBeRedone) => DivideRoom(roomToBeRedone);
	}
	
	/// <summary>
	/// Assign a door to its rooms
	/// </summary>
	/// <param name="door"></param>
	private void AssignDoor(Door door)
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
}