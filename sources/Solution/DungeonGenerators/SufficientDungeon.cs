﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Door.Orientation;

namespace Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;

internal class SufficientDungeon : Dungeon
{
	private List<Room> roomsToBeAdded;
	private List<Door> doorsToBeAdded;
	private Random random;
	public SufficientDungeon(Size pSize) : base(pSize){}
	private void Update()
	{
		if (Input.GetKeyDown(Key.A))
		{
			InternalGenerate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
		}

		//Obsolete
		// //Generate a random room when pressing space
		// if (Input.GetKeyDown(Key.SPACE))
		// {
		// 	InternalGenerate(AlgorithmsAssignment.MIN_ROOM_SIZE, Utils.Random(int.MinValue, int.MaxValue));
		// }
	}
		
	protected override void Generate(int pMinimumRoomSize, int seed)
	{
		random = new Random(seed);
		doorsToBeAdded = new List<Door>();
		roomsToBeAdded = new List<Room>();
			
		//Start room (Covers whole dungeon)
		DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)));
		
		rooms.AddRange(roomsToBeAdded);
		roomsToBeAdded.Clear();

		//Add doors and fix them if bad
		foreach (Door door in doorsToBeAdded)
		{
			TestDoor(door);
		}
		doorsToBeAdded.Clear();
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
			
			doorsToBeAdded.Add(new Door(newPoint with {Y = random.Next(boundaries.X, boundaries.Y)}, Vertical, boundaries));
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
			
			doorsToBeAdded.Add(new Door(newPoint with {X = random.Next(boundaries.X, boundaries.Y)}, Horizontal, boundaries));
		}

		else rooms.Add(room);

		void Redo(Room roomToBeRedone)
		{
			DivideRoom(roomToBeRedone);
		}
	}
}