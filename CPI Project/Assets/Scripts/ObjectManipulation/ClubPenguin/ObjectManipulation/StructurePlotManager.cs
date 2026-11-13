using System.Collections.Generic;

namespace ClubPenguin.ObjectManipulation
{
	public class StructurePlotManager
	{
		private List<MultiPointLineAttractor> registeredPlots;

		private int largestPlotSize;

		public int LargestPlotSize
		{
			get
			{
				return largestPlotSize;
			}
		}

		public StructurePlotManager()
		{
			registeredPlots = new List<MultiPointLineAttractor>();
		}

		public void RegisterStructurePlot(MultiPointLineAttractor plot)
		{
			if (plot.Segments > largestPlotSize)
			{
				largestPlotSize = plot.Segments;
			}
			registeredPlots.Add(plot);
		}

		public void RemoveStructurePlot(MultiPointLineAttractor plot)
		{
			registeredPlots.Remove(plot);
			CalculateLargestPlot();
		}

		private void CalculateLargestPlot()
		{
			largestPlotSize = 0;
			for (int i = 0; i < registeredPlots.Count; i++)
			{
				if (registeredPlots[i].Segments > largestPlotSize)
				{
					largestPlotSize = registeredPlots[i].Segments;
				}
			}
		}
	}
}
