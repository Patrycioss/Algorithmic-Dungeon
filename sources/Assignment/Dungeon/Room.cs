using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Dungeon
{
	/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
	public class Room
	{
		public Rectangle area;
		public List<Door> doors;

		public Room (Rectangle pArea)
		{
			area = pArea;
			doors = new List<Door>();
		}

		public int surface => area.Width * area.Height;

		public Point center => new Point(area.X + area.Width / 2, area.Y + area.Height / 2);

		public Point topLeft => new Point(area.X, area.Y);
		
		public Point topRight => new Point(area.Right - 1, area.Y);
		
		public Point bottomLeft => new Point(area.X, area.Bottom - 1);
		
		public Point bottomRight => new Point(area.Right - 1, area.Bottom - 1);

		public Node node; 





		//TODO: Implement a toString method for debugging?
		//Return information about the type of object and it's data
		//eg Room: (x, y, width, height)

	}
}
