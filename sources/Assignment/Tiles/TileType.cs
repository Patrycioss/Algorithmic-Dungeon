namespace Saxion.CMGT.Algorithms.sources.Assignment.Tiles;

/**
 * Describes a certain tiletype and whether it's walkable or not.
 * Note how we maintain controls over the possible TileType's in this class (constructor is private)
 */
internal class TileType
{
	//all possible tile types based on this class' properties
	public static readonly TileType Wall = new TileType(false);		//wall, not walkable
	public static readonly TileType Ground = new TileType(true);	//ground, walkable
	public static readonly TileType Void = new TileType(false);		//IT'S DA VOID RUN! -> just kidding, it's not walkable

	//each tileType gets assigned a unique auto incrementing id, used for texture look ups
	private static int lastId = 0;
        
	//tile instance specific properties
	public readonly bool walkable;
	public readonly int id; 
        
	private TileType(bool pWalkable)
	{
		walkable = pWalkable;
		id = lastId++;
	}   
}