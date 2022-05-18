using System;
using System.Drawing;
using GXPEngine;

namespace Saxion.CMGT.Algorithms.sources.Util
{
	/**
 * Helper class that draws nodelabels for a nodegraph.
 */
	internal sealed class NodeLabelDrawer : Canvas
	{
		private Font labelFont;
		private bool showLabels = false;
		private NodeGraph graph = null;

		public NodeLabelDrawer(NodeGraph pNodeGraph) : base(pNodeGraph.width, pNodeGraph.height)
		{
			Console.WriteLine("\n-----------------------------------------------------------------------------");
			Console.WriteLine("NodeLabelDrawer created.");
			Console.WriteLine("* L key to toggle node label display.");
			Console.WriteLine("-----------------------------------------------------------------------------");

			labelFont = new Font(SystemFonts.DefaultFont.FontFamily, pNodeGraph.nodeSize, FontStyle.Bold);
			graph = pNodeGraph;
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		///							Update loop
		///							

		//this has to be virtual otherwise the subclass won't pick it up
		private void Update()
		{
			//toggle label display when L is pressed
			if (Input.GetKeyDown(Key.L))
			{
				showLabels = !showLabels;
				graphics.Clear(Color.Transparent);
				if (showLabels) drawLabels();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		/// NodeGraph visualization helper methods
		private void drawLabels()
		{
			foreach (Node node in graph.nodes) drawNode(node);
		}

		private void drawNode(Node pNode)
		{
			SizeF size = graphics.MeasureString(pNode.id, labelFont);
			graphics.DrawString(pNode.id, labelFont, Brushes.Black, pNode.location.X - size.Width / 2, pNode.location.Y - size.Height / 2);
		}

	}
}
