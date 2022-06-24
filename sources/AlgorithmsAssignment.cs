using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.AddOns;
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
		
		
		//Debug
		private static readonly List<(Vec2, int)> Crosses = new();


		//common settings
		public const int SCALE = 15;
		public const int MIN_ROOM_SIZE = 10;
		public const bool CHECK_IF_COMPLETELY_CONNECTED = false;
		public const bool DO_WONKY_STEP_BY_STEP = false;

		public AlgorithmsAssignment() : base(1080, 700, false, true, -1, -1, false) => Create();

		private void Create(int seed = 1)
		{
			//set our default background color and title
			GL.ClearColor(1, 1, 1, 1);
			GL.glfwSetWindowTitle("Algorithms Game");

			Grid grid = new(width, height, SCALE);
			Size size = new(width / SCALE, height / SCALE);
			
			//Dungeon
			dungeon = new BetterDungeon(size, SCALE);
			dungeon?.InternalGenerate(MIN_ROOM_SIZE,seed, false);

			//NodeGraph
			graph = new HighLevelNodeGraph(dungeon);
			graph?.InternalGenerate(false);

			//TiledView
			tiledView = new TiledDungeonView(dungeon);
			tiledView?.InternalGenerate(false);

			//PathFinder
			pathFinder = new RecursivePathFinder(graph, dungeon, true);


			//Agent
			// agent = new BetterNodeGraphAgent(graph);
			agent = new PathFindingAgent(graph, pathFinder, false);
			
			//Adding stuff
			// if (grid != null) AddChild(grid);
			if (dungeon != null) AddChild(dungeon);
			if (tiledView != null) AddChild(tiledView);
			if (graph != null) AddChild(graph);
			if (pathFinder != null) AddChild(pathFinder); //pathfinder on top of that
			if (graph != null) AddChild(new NodeLabelDrawer(graph)); //node label display on top of that
			if (agent != null) AddChild(agent); //and last but not least the agent itself
		}


		private void Update()
		{
			//Credit to Jelle :D
			for (int i = 0; i < Crosses.Count; i++)
			{
				(Vec2 cross, int count) = Crosses[i];
				Gizmos.DrawCross(cross.x, cross.y, 10, color: (uint) Color.Red.ToArgb());
				if (count > currentFps*3)
				{
					Crosses.RemoveAt(i);
					continue;
				}
				Crosses[i] = (cross, count + 1);
			}
		
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
		
		public static void DrawCross(float x, float y)
		{
			Crosses.Add((new Vec2(x, y), 0));
		}
	}
}


