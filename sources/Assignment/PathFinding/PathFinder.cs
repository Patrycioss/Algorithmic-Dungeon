using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

/**
 * This class is the base class for your pathfinder, you 'only' have to override generate so that it returns
 * the requested path and then it will handle the visualization part for you. This class can be used in two ways:
 * 1. By setting the start and end node by left/right shift-clicking and then pressing G (for Generate)
 * 2. By calling Generate directly with the given start and end node
 * 
 * TODO:
 * - create a subclass for this class and override the generate method (See SamplePathFinder for an example)
 */
abstract class PathFinder : Canvas
{
	protected Node startNode;							
	protected Node endNode;
	protected List<Node> lastCalculatedPath = null;

	protected NodeGraph.NodeGraph nodeGraph;
	protected Dungeon.Dungeon dungeon;

	//some values for drawing the path
	private Pen _outlinePen = new Pen(Color.Black, 4);
	private Pen _connectionPen1 = new Pen(Color.Black, 10);
	private Pen _connectionPen2 = new Pen(Color.Yellow, 3);

	private Brush _startNodeColor = Brushes.Green;
	private Brush _endNodeColor = Brushes.Red;
	private Brush _pathNodeColor = Brushes.Yellow;
	
	//Custom
	protected List<Node> excludedNodes = new();
	protected bool connected;

	public PathFinder (NodeGraph.NodeGraph pGraph, Dungeon.Dungeon pDungeon) : base (pGraph.width, pGraph.height)
	{
		dungeon = pDungeon;
		nodeGraph = pGraph;
		nodeGraph.onNodeShiftLeftClicked += (node) => { startNode = node; Draw(); };
		nodeGraph.onNodeShiftRightClicked += (node) => { endNode = node; Draw(); };
		nodeGraph.onNodeControlLeftClicked += (node) => { excludedNodes.Add(node); Draw(); ConnectedCheck(); };
		nodeGraph.onNodeControlRightClicked += (node) => { excludedNodes.Remove(node); Draw(); ConnectedCheck(); };

		Console.WriteLine("\n-----------------------------------------------------------------------------");
		Console.WriteLine(this.GetType().Name + " created.");
		Console.WriteLine("* Shift-LeftClick to set the starting node.");
		Console.WriteLine("* Shift-RightClick to set the target node.");
		Console.WriteLine("* CTRL-LeftClick to exclude node");
		Console.WriteLine("* G to generate the Path.");
		Console.WriteLine("* C to clear the Path.");
		Console.WriteLine("-----------------------------------------------------------------------------");


		ConnectedCheck();
		Draw();

		
		void ConnectedCheck()
		{
			if (AlgorithmsAssignment.CHECK_IF_COMPLETELY_CONNECTED) CheckIfDungeonIsConnected();
		}
	}

	protected void CheckIfDungeonIsConnected()
	{
		connected = true;
		

		for (int i = 1; i < dungeon.rooms.Count; i++)
		{
			if (Generate(dungeon.rooms[0].node, dungeon.rooms[i].node).Count == 0)
			{
				connected = false;
				break;
			}
		}
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////
	/// Core PathFinding methods

	public List<Node> InternalGenerate(Node pFrom, Node pTo)
	{
		System.Console.WriteLine(this.GetType().Name + ".Generate: Generating path...");

		lastCalculatedPath = null;
		startNode = pFrom;
		endNode = pTo;

		if (startNode == null || endNode == null)
		{
			Console.WriteLine("Please specify start and end node before trying to generate a path.");
		}
		else
		{
			lastCalculatedPath = Generate(pFrom, pTo);
		}

		Draw();

		System.Console.WriteLine(this.GetType().Name + ".Generate: Path generated.");
		return lastCalculatedPath;
	}

	/**
	 * @return the last found path. 
	 *	-> 'null'		means	'Not completed.'
	 *	-> Count == 0	means	'Completed but empty (no path found).'
	 *	-> Count > 0	means	'Yolo let's go!'
	 */
	protected abstract List<Node> Generate(Node pFrom, Node pTo);

	/////////////////////////////////////////////////////////////////////////////////////////
	/// PathFinder visualization helpers method
	///	As you can see this looks a lot like the code in NodeGraph, but that is just coincidence
	///	By not reusing any of that code you are free to tweak the visualization anyway you want

	protected virtual void Draw()
	{
		//to keep things simple we redraw all debug info every frame
		graphics.Clear(Color.Transparent);

		//draw path if we have one
		if (lastCalculatedPath != null) DrawPath();

		//draw start and end if we have one
		if (startNode != null) DrawNode(startNode, _startNodeColor);
		if (endNode != null) DrawNode(endNode, _endNodeColor);

		foreach (Node node in excludedNodes)
		{
			DrawNode(node, Brushes.Orange);
		}

		if (connected)
		{
			graphics.DrawString("Connected!", SystemFonts.DefaultFont, Brushes.Green, new Point(10,10));
		}
		else
		{
			graphics.DrawString("Not connected!", SystemFonts.DefaultFont,Brushes.Red, new Point(10,10));
		}
		
		//TODO: you could override this method and draw your own additional stuff for debugging
	}

	protected virtual void DrawPath()
	{
		//draw all lines
		for (int i = 0; i < lastCalculatedPath.Count - 1; i++)
		{
			DrawConnection(lastCalculatedPath[i], lastCalculatedPath[i + 1]);
		}

		//draw all nodes between start and end
		for (int i = 1; i < lastCalculatedPath.Count - 1; i++)
		{
			DrawNode(lastCalculatedPath[i], _pathNodeColor);
		}
	}

	protected virtual void DrawNodes (IEnumerable<Node> pNodes, Brush pColor)
	{
		foreach (Node node in pNodes) DrawNode(node, pColor);
	}

	protected virtual void DrawNode(Node pNode, Brush pColor)
	{
		int nodeSize = nodeGraph.nodeSize+2;

		//colored fill
		graphics.FillEllipse(
			pColor,
			pNode.location.X - nodeSize,
			pNode.location.Y - nodeSize,
			2 * nodeSize,
			2 * nodeSize
		);

		//black outline
		graphics.DrawEllipse(
			_outlinePen,
			pNode.location.X - nodeSize - 1,
			pNode.location.Y - nodeSize - 1,
			2 * nodeSize + 1,
			2 * nodeSize + 1
		);
	}

	protected virtual void DrawConnection(Node pStartNode, Node pEndNode)
	{
		//draw a thick black line with yellow core
		graphics.DrawLine(_connectionPen1,	pStartNode.location,pEndNode.location);
		graphics.DrawLine(_connectionPen2,	pStartNode.location,pEndNode.location);
	}

	/////////////////////////////////////////////////////////////////////////////////////////
	///							Keypress handling
	///							

	public void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (Input.GetKeyDown(Key.C))
		{
			//clear everything
			graphics.Clear(Color.Transparent);
			startNode = endNode = null;
			lastCalculatedPath = null;
		}

		if (Input.GetKeyDown(Key.G))
		{
			if (startNode != null && endNode != null)
			{
				InternalGenerate(startNode, endNode);
			}
		}
	}


}