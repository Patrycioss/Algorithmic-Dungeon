using System;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.Tiles;

namespace Saxion.CMGT.Algorithms.sources.Solution.TiledViewers;

internal class TiledDungeonView : TiledView
{
	private Dungeon dungeon;
	
	public TiledDungeonView(Dungeon pDungeon) : base(pDungeon.size.Width, pDungeon.size.Height, (int)pDungeon.scale, TileType.Void) => dungeon = pDungeon;

	protected override void Generate()
	{
		if (debugMode)
		{
			Console.WriteLine($"--------------");
			Console.WriteLine($"Making rooms");
			Console.WriteLine($"--------------");
		}
		
		foreach (Room room in dungeon.rooms)
		{
			//Add walls
			for (int i = room.topLeft.X; i <= room.topRight.X; i++)
			{
				for (int j = room.topLeft.Y; j <= room.bottomLeft.Y; j++)
				{
					if (i == room.topLeft.X || i == room.topRight.X || j == room.topLeft.Y || j == room.bottomLeft.Y)
					{
						SetTileType(i,j,TileType.Wall);
						if (debugMode) Console.WriteLine($"Set tile at x: {i} and y {j} to wall");
					}
					else
					{
						SetTileType(i,j,TileType.Ground);
						if (debugMode) Console.WriteLine($"Set tile at x: {i} and y {j} to ground");
					}
				}
			}
		}

		if (debugMode)
		{
			Console.WriteLine($"--------------");
			Console.WriteLine($"Making doors/hallways");
			Console.WriteLine($"--------------");
		}
		
		foreach (Door door in dungeon.doors)
		{
			SetTileType(door.location.X,door.location.Y,TileType.Ground);
			if (debugMode) Console.WriteLine($"Set tile at x: {door.location.X} and y {door.location.Y} to door");

			//Adds the walls to hallways
			if (door.orientation == Door.Orientation.Horizontal)
			{
				SetTileType(door.location.X -1, door.location.Y,TileType.Wall);
				if (debugMode) Console.WriteLine($"Set tile at x: {door.location.X - 1} and y {door.location.Y} to wall");
				
				SetTileType(door.location.X + 1, door.location.Y, TileType.Wall);
				if (debugMode) Console.WriteLine($"Set tile at x: {door.location.X + 1} and y {door.location.Y} to wall");
			}
			else
			{
				SetTileType(door.location.X,door.location.Y - 1, TileType.Wall);
				if (debugMode) Console.WriteLine($"Set tile at x: {door.location.X} and y {door.location.Y - 1} to wall");

				SetTileType(door.location.X,door.location.Y + 1, TileType.Wall);
				if (debugMode) Console.WriteLine($"Set tile at x: {door.location.X} and y {door.location.Y + 1} to wall");
			}
		}
	}
}