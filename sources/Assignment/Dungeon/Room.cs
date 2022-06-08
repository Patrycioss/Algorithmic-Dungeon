using System.Collections.Generic;
using System.Drawing;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Dungeon
{
	/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
	public class Room
	{
		public Rectangle area;

		public Point topLeft;
		public Point topRight;
		public Point bottomLeft;
		public Point bottomRight;
		
		public List<Door> doors;

		public Room (Rectangle pArea)
		{
			area = pArea;

			topLeft = new Point(area.X, area.Y);
			topRight = new Point(area.Right - 1, area.Y);
			bottomLeft = new Point(area.X, area.Bottom - 1);
			bottomRight = new Point(area.Right - 1, area.Bottom - 1);

			doors = new List<Door>();
		}

		public int surface => area.Width * area.Height;

		public Point center => new Point(area.X + area.Width / 2, area.Y + area.Height / 2);



		//TODO: Implement a toString method for debugging?
		//Return information about the type of object and it's data
		//eg Room: (x, y, width, height)

	}
}
