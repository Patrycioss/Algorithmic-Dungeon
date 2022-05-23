using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata;
using FirstGXPGame;
using GXPEngine;
using Microsoft.Win32.SafeHandles;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution
{
	class SufficientDungeon : global::Dungeon
	{
		private int minimumRoomSize;
		private List<Room> roomsToBeAdded;
		private List<Room> roomsToBeDeleted;
		private List<Door> doorsToBeAdded;
		private bool splittablesLeft;


		private Random random;
		private List<Door> doorsToBeRemoved;

		public SufficientDungeon(Size pSize) : base(pSize) {}

		private void Update()
		{
			//Regenerate dungeon
			if (Input.GetKeyDown(Key.SPACE))
			{
				Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, Utils.Random(0, 90000));
			}
			else if (Input.GetKeyDown(Key.A))
			{
				Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, 50);
			}

			foreach (Room room in rooms)
			{
				float rx = Mathf.Map(room.area.X, 0, size.Width, 0, game.width);
				float ry = Mathf.Map(room.area.Y, 0, size.Height, 0, game.height);
				float rw = Mathf.Map(room.area.Width, 0, size.Width, 0, game.width);
				float rh = Mathf.Map(room.area.Height, 0, size.Height, 0, game.height);
				Gizmos.DrawRectangle(rx + 1, ry, rw, rh, color: (uint) Color.Aqua.ToArgb());
			}
		}
		
		protected override void generate(int pMinimumRoomSize, int seed)
		{
			random = new Random(seed);
			minimumRoomSize = pMinimumRoomSize;
			doorsToBeAdded = new List<Door>();
			
			//Start room (Covers whole dungeon)
			DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)), null);

			//Add doors and fix them if bad
			foreach (Door door in doorsToBeAdded)
			{
				FixDoor(door);
			}
		}

		private void DivideRoom(Room room, Room prevRoom)
		{
			int minimum;
			int maximum;
			
			Point newPoint = new Point(0,0);
			
			if (room.area.Width >= minimumRoomSize * 2)
			{
				minimum = room.area.X + minimumRoomSize;
				maximum = room.area.Right - minimumRoomSize;
				newPoint.X = random.Next(minimum, maximum);

				//Room1 (Left)
				Room a = new Room(new Rectangle(room.area.X, room.area.Y, newPoint.X - room.area.X + 1, room.area.Height));
				DivideRoom(a, room);

				//Room2 (Right)
				Room b = new Room(new Rectangle(newPoint.X, room.area.Y, room.area.Width - (newPoint.X - room.area.X), room.area.Height));
				DivideRoom(b,  room);

				Point boundaries = new Point(room.area.Y + 1, room.area.Bottom - 1);
				doorsToBeAdded.Add(new Door(new Point(newPoint.X, random.Next(boundaries.X,boundaries.Y)),false, boundaries));
			}

			else if (room.area.Height >= minimumRoomSize * 2)
			{
				minimum = room.area.Y + minimumRoomSize;
				maximum = room.area.Bottom - minimumRoomSize;
				newPoint.Y = random.Next(minimum, maximum);

				//Room1 (Top)
				Room a = new Room(new Rectangle(room.area.X, room.area.Y, room.area.Width, newPoint.Y - room.area.Y + 1));
				DivideRoom(a, room);

				//Room2 (Bottom)
				Room b = new Room(new Rectangle(room.area.X, newPoint.Y, room.area.Width, room.area.Height - (newPoint.Y - room.area.Y)));
				DivideRoom(b, room);

				Point boundaries = new Point(room.area.X + 1, room.area.Right - 1);
				doorsToBeAdded.Add(new Door(new Point(random.Next(boundaries.X,boundaries.Y), newPoint.Y), true, boundaries));
			}

			else
			{
				rooms.Add(room);
			}
		}

		private void FixDoor(Door door)
		{
			if (rooms.Any(room => door.location == room.topLeft || door.location == room.topRight ||
			                      door.location == room.bottomLeft || door.location == room.bottomRight))
			{
				if (door.horizontal)
				{
					door = new Door(new Point(random.Next(door.boundaries.X, door.boundaries.Y), door.location.Y), true,
						door.boundaries);
				}
				else
				{
					door = new Door(new Point(door.location.X, random.Next(door.boundaries.X, door.boundaries.Y)),
						false, door.boundaries);
				}
				
				FixDoor(door);
			}
			else doors.Add(door);
		}
	}
}