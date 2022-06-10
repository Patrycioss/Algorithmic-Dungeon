using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.Tiles;

namespace Saxion.CMGT.Algorithms.sources.Solution.TiledViewers;

internal class TiledDungeonView : TiledView
{
	private Dungeon dungeon;
	
	public TiledDungeonView(Dungeon pDungeon) : base(pDungeon.size.Width, pDungeon.size.Height, (int)pDungeon.scale, TileType.Void)
	{
		dungeon = pDungeon;
	}

	
	protected override void Generate()
	{
		foreach (Room room in dungeon.rooms)
		{
			for (int i = room.topLeft.X; i <= room.topRight.X; i++)
			{
				for (int j = room.topLeft.Y; j <= room.bottomLeft.Y; j++)
				{
					if (i == room.topLeft.X || i == room.topRight.X || j == room.topLeft.Y || j == room.bottomLeft.Y)
					{
						SetTileType(i,j,TileType.Wall);
					}
					else SetTileType(i,j,TileType.Ground);
				}
			}

			for (int i = room.topRight.X + 1; i < room.topRight.X - 1; i++)
			{
				for (int j = room.topRight.Y + 1; i < room.bottomLeft.Y - 1; i++)
				{
					SetTileType(i,j,TileType.Ground);
				}
			}

		}

		foreach (Door door in dungeon.doors)
		{
			SetTileType(door.location.X,door.location.Y,TileType.Ground);
		}
	}
}