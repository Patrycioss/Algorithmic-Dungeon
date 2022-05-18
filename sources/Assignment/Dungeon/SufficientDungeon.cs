using System.Drawing;
using GXPEngine;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Dungeon
{
	class SufficientDungeon : global::Dungeon
	{
		public SufficientDungeon(Size pSize) : base(pSize)
		{
		}

		protected override void generate(int pMinimumRoomSize)
		{
			rooms.Add(new Room(new Rectangle(0,0,size.Width/4+1,size.Height/4+1)));
			
			//left room from 0 to half of screen + 1 (so that the walls overlap with the right room)
			//(TODO: experiment with removing the +1 below to see what happens with the walls)
			rooms.Add(new Room(new Rectangle(0, 0, size.Width/2+1, size.Height)));
			//right room from half of screen to the end
			rooms.Add(new Room(new Rectangle(size.Width/2, 0, size.Width/2, size.Height)));
			//and a door in the middle wall with a random y position
			//TODO:experiment with changing the location and the Pens.White below
			doors.Add(new Door(new Point(9, size.Height / 2 + Utils.Random(-5, 5))));
		}
	}
}