using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.OpenGL;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;
using Saxion.CMGT.Algorithms.sources.Assignment.Tiles;
using Saxion.CMGT.Algorithms.sources.Solution.Agents;
using Saxion.CMGT.Algorithms.sources.Solution.DungeonGenerators;
using Saxion.CMGT.Algorithms.sources.Solution.NodeGraphGenerators;
using Saxion.CMGT.Algorithms.sources.Solution.PathFinders;
using Saxion.CMGT.Algorithms.sources.Solution.TiledViewers;
using Saxion.CMGT.Algorithms.sources.Util;

namespace Saxion.CMGT.Algorithms.sources
{
	public class AlgorithmsAssignment : Game
	{
		//Required for assignment 1
		private Dungeon dungeon;

		//Required for assignment 2
		private NodeGraph graph;
		private TiledView tiledView;
		private NodeGraphAgent agent;

		//Required for assignment 3
		private PathFinder pathFinder;

		//common settings
		public static int SCALE = 20;				
		public static int MIN_ROOM_SIZE = 7;
		public static bool checkIfCompletelyConnected = false;

		public AlgorithmsAssignment() : base(1080, 700, false, true, -1, -1, false)
		{
			Create();
		}

		private void Create(int seed = 1)
		{
			//set our default background color and title
			GL.ClearColor(1, 1, 1, 1);
			GL.glfwSetWindowTitle("Algorithms Game");

			Grid grid = new(width, height, SCALE);
			Size size = new(width / SCALE, height / SCALE);

			//Dungeon
			dungeon = new BetterDungeon(size);

			if (dungeon != null)
			{
				//assign the SCALE we talked about above, so that it no longer looks like a tinietiny stamp:
				dungeon.scale = SCALE;
				//Tell the dungeon to generate rooms and doors with the given MIN_ROOM_SIZE
				dungeon.InternalGenerate(MIN_ROOM_SIZE, seed);
			}

			//NodeGraph
			graph = new ExcellentDungeonNodeGraph(dungeon);

			graph?.InternalGenerate();

			//TiledView
			
			// tiledView = new TiledDungeonView(dungeon);
			tiledView?.InternalGenerate();

			//PathFinder
			pathFinder = new AStarPathFinder(graph, dungeon);

			//Agent
			agent = new PathFindingAgent(graph, pathFinder);
			
			//Adding stuff
			if (grid != null) AddChild(grid);
			if (dungeon != null) AddChild(dungeon);
			if (tiledView != null) AddChild(tiledView);
			if (graph != null) AddChild(graph);
			if (pathFinder != null) AddChild(pathFinder); //pathfinder on top of that
			if (graph != null) AddChild(new NodeLabelDrawer(graph)); //node label display on top of that
			if (agent != null) AddChild(agent); //and last but not least the agent itself
		}

		private void Update()
		{
			if (Input.GetKeyDown(Key.SPACE))
			{
				ClearScreen();
				Create(Utils.Random(int.MinValue, int.MaxValue));
			}
			
			for (int i = 48; i <= 57; i++)
			{
				if (Input.GetKeyDown(i))
				{
					ClearScreen();
					Create(i);
				}
			}

			void ClearScreen()
			{
				List<GameObject> list = GetChildren();
				for (int index = list.Count-1; index >= 0; index--)
				{
					GameObject gameObject = list[index];
					gameObject.Destroy();
				}	
			}
		}
	}
}


