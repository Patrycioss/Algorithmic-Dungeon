#nullable enable

using System.Drawing;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;


public class Door
{
	public readonly Point location;

	public enum Orientation
	{
		Horizontal,
		Vertical
	}

	public Room? roomA = null;
	public Room? roomB = null;
	public Orientation orientation;
	public readonly Point boundaries;

	public Door(Point pLocation, Orientation orientation_, Point pBoundaries = new Point())
	{
		location = pLocation;
		orientation = orientation_;
		boundaries = pBoundaries;
	}
}