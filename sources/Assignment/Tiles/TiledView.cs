using System.Diagnostics;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.Core;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Tiles;

/**
 * A TileView class that allows you to set a 2D array of tiles and render them on screen.
 * If you are used to an MVC setup, this class is both data and view all rolled into one.
 * Also see the 'A note on architecture' document on BlackBoard.
 * 
 * Subclass this class and override the generate method (note the lower case).
 */
abstract class TiledView : GameObject
{
	//the dimensions of the tileView
	public int columns { get; }
	public int rows { get; }
	//stores the tiletype for each cell (and with it whether a tile is walkable)
	private TileType[,] tileData;
	//used to reset all data to the default tiletype when requested
	private TileType _defaultTileType;
	//single sprite, used for rendering all tiles
	private AnimationSprite _tileSet;

	protected bool debugMode;

	public TiledView(int pColumns, int pRows, int pTileSize, TileType pDefaultTileType) {
		Debug.Assert(pColumns > 0, "Invalid amount of columns passed in: " + pColumns);
		Debug.Assert(pRows > 0, "Invalid amount of rows passed in: " + pRows);
		Debug.Assert(pDefaultTileType != null, "Invalid default tile type passed in:" + pDefaultTileType);

		columns = pColumns;
		rows = pRows;

		_defaultTileType = pDefaultTileType;

		//we use a single sprite to render the whole tileview
		_tileSet = new AnimationSprite("assets/tileset.png", 3, 1);
		_tileSet.width = _tileSet.height = pTileSize;

		InitializeTiles();
	}

	private void InitializeTiles ()
	{
		//initialize all tiles to walkable
		tileData = new TileType[columns, rows];
		ResetAllTilesToDefault();
	}

	protected void ResetAllTilesToDefault()
	{
		//a 'trick' to do everything in one for loop instead of a nested loop
		for (int i = 0; i < columns * rows; i++)
		{
			tileData[i % columns, i / columns] = _defaultTileType;
		}
	}

	public void SetTileType(int pColumn, int pRow, TileType pTileType)
	{
		//an example of hardcore defensive coding;)
		Debug.Assert(pColumn >= 0 && pColumn < columns, "Invalid column passed in: " + pColumn);
		Debug.Assert(pRow >= 0 && pRow < rows, "Invalid row passed in:" + pRow);
		Debug.Assert(pTileType != null, "Invalid tile type passed in:" + pTileType);

		tileData[pColumn, pRow] = pTileType;
	}

	public TileType GetTileType(int pColumn, int pRow)
	{
		Debug.Assert(pColumn >= 0 && pColumn < columns, "Invalid column passed in: " + pColumn);
		Debug.Assert(pRow >= 0 && pRow < rows, "Invalid row passed in:" + pRow);

		return tileData[pColumn, pRow];
	}

	protected override void RenderSelf(GLContext glContext)
	{
		//another way of rendering you might not be used to. Instead of adding all 
		//seperate sprites, we override the RenderSelf method, move a sprite around
		//like a stamp and 'stamp' the sprite onto the screen by calling its render method
		for (int column = 0; column < columns; column++)
		{
			for (int row = 0; row < rows; row++)
			{
				_tileSet.currentFrame = GetTileType(column, row).id;
				_tileSet.x = column * _tileSet.width;
				_tileSet.y = row * _tileSet.height;
				_tileSet.Render(glContext);
			}
		}
	}

	/**
	 * Trigger the tile view generation process, do not override this method, 
	 * but override generate (note the lower case) instead.
	 */
	public void InternalGenerate(bool debugging = false)
	{
		debugMode = debugging;
		System.Console.WriteLine(GetType().Name + ".Generate: Generating tile view...");
		Generate();
		System.Console.WriteLine(GetType().Name + ".Generate: tile view generated.");
	}

	protected abstract void Generate();

}