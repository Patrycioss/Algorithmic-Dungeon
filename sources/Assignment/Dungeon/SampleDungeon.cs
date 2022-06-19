﻿using System.Drawing;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Dungeon
{
	/**
 * An example of a dungeon implementation.  
 * This implementation places two rooms manually but your implementation has to do it procedurally.
 */
	class SampleDungeon : global::Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Dungeon
	{
		public SampleDungeon(Size pSize) : base(pSize) {}

		/**
	 * This method overrides the super class generate method to implement a two-room dungeon with a single door.
	 * The good news is, it's big enough to house an Ogre and his ugly children, the bad news your implementation
	 * should generate the dungeon procedurally, respecting the pMinimumRoomSize.
	 * 
	 * Hints/tips: 
	 * - start by generating random rooms in your own Dungeon class and placing random doors.
	 * - playing/experiment freely is the key to all success
	 * - this problem can be solved both iteratively or recursively
	 */
		protected override void Generate(int pMinimumRoomSize, int seed)
		{
			//left room from 0 to half of screen + 1 (so that the walls overlap with the right room)
			//(TODO: experiment with removing the +1 below to see what happens with the walls)
			rooms.Add(new Room(new Rectangle(0, 0, size.Width/2+1, size.Height)));
			//right room from half of screen to the end
			rooms.Add(new Room(new Rectangle(size.Width/2, 0, size.Width/2, size.Height)));
			//and a door in the middle wall with a random y position
			//TODO:experiment with changing the location and the Pens.White below
		}
	}
}

