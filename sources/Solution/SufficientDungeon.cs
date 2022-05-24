using System;
using System.Collections.Generic;
using System.Drawing;
using FirstGXPGame;
using GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Solution
{
	internal class GoodDungeon : Dungeon
	{
		public GoodDungeon(Size pSize) : base(pSize){}

		protected override void Make(int pMinimumRoomSize, int seed)
		{
			
		}
	}
	
	
	internal class SufficientDungeon : Dungeon
	{
		private List<Room> roomsToBeAdded;
		private List<Door> doorsToBeAdded;
		private Random random;
		public SufficientDungeon(Size pSize) : base(pSize){}
		private void Update()
		{
			//Regenerate dungeon
			if (Input.GetKeyDown(Key.SPACE))
			{
				Generate(AlgorithmsAssignment.MIN_ROOM_SIZE, Utils.Random(int.MinValue, int.MaxValue));
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
		
		protected override void Make(int pMinimumRoomSize, int seed)
		{
			random = new Random(seed);
			doorsToBeAdded = new List<Door>();
			roomsToBeAdded = new List<Room>();
			
			//Start room (Covers whole dungeon)
			Functions.DivideRoom(new Room(new Rectangle(0, 0, size.Width, size.Height)), random, doorsToBeAdded, minimumRoomSize, rooms);
			
			// RemoveSmallestRooms(GetSmallestSurface(roomsToBeAdded));
			rooms.AddRange(roomsToBeAdded);
			roomsToBeAdded.Clear();

			//Add doors and fix them if bad
			foreach (Door door in doorsToBeAdded)
			{
				Functions.FixDoor(door, rooms, random, doors);
			}
			doorsToBeAdded.Clear();
		}
	}
	
	
}