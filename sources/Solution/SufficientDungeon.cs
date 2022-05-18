using System;
using System.Collections.Generic;
using System.Drawing;
using FirstGXPGame;
using GXPEngine;

namespace Saxion.CMGT.Algorithms.sources.Solution
{
	class SufficientDungeon : global::Dungeon
	{
		private int minimumRoomSize;
		private List<Room> roomsToBeAdded;
		private List<Room> roomsToBeDeleted;

		public SufficientDungeon(Size pSize) : base(pSize)
		{
			
		}

		private void Update()
		{
			//Regenerate dungeon
			if (Input.GetKeyDown(Key.SPACE))
			{
				Generate(AlgorithmsAssignment.MIN_ROOM_SIZE);
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




		protected override void generate(int pMinimumRoomSize)
		{
			minimumRoomSize = pMinimumRoomSize;
			
			roomsToBeAdded = new List<Room>();
			roomsToBeDeleted = new List<Room>();
			
			
			
			
			for (int i = 0; i < 900; i++)
			{
				SplitRooms();

				foreach (Room room in roomsToBeDeleted)
				{
					rooms.Remove(room);
				}
				roomsToBeDeleted.Clear();

				foreach (Room room in roomsToBeAdded)
				{
					rooms.Add(room); 
				}
				roomsToBeAdded.Clear();
			}

			Console.WriteLine(rooms.Count);
		}

		private void SplitRooms()
		{		
			Room room1;
			Room room2;
			
			if (rooms.Count == 0)
			{
				room1 = new Room(new Rectangle(0,0, size.Width, size.Height));
				roomsToBeAdded.Add(room1);
				Console.WriteLine(size.Width);
				Console.WriteLine(minimumRoomSize);
			}
			else
			{
				foreach (Room room in rooms)
				{
					int minimum;
					int maximum;

					if (room.area.Width >= minimumRoomSize * 2)
					{
						minimum = room.area.X + minimumRoomSize;
						maximum = room.area.X + room.area.Width - minimumRoomSize;
						int newX = Utils.Random(minimum, maximum);
						
						room1 = new Room(new Rectangle(room.area.X, room.area.Y, newX - room.area.X + 1, room.area.Height));
						
						
						
						
						room2 = new Room(new Rectangle(newX, room.area.Y, room.area.Width - (newX - room.area.X), room.area.Height));
						
						// if (newX + room2.area.Width < room.area.X + room.area.Width && )
						
						roomsToBeAdded.Add(room1);
						roomsToBeAdded.Add(room2);
						roomsToBeDeleted.Add(room);
					}
					else if (room.area.Height >= minimumRoomSize * 2)
					{
						minimum = room.area.Y + minimumRoomSize;
						maximum = room.area.Y + room.area.Height - minimumRoomSize;
						int newY = Utils.Random(minimum, maximum);
						
						room1 = new Room(new Rectangle(room.area.X, room.area.Y, room.area.Width, newY - room.area.Y + 1));

						room2 = new Room(new Rectangle(room.area.X, newY, room.area.Width, room.area.Height - (newY - room.area.Y)));				
						
						roomsToBeAdded.Add(room1);
						roomsToBeAdded.Add(room2);
						roomsToBeDeleted.Add(room);					
					}
				}
			}
		}
	}
}