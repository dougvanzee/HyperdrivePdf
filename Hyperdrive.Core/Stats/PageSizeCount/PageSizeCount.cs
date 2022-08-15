using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Stats.PageSizeCount
{
    public class PageSizeCount
    {
        private float sizeX = 0;
        private float sizeY = 0;
        private int numberOfPages = 0;
        private int stagedCounts = 0;
        private bool isDefaultSize;
        private int squareFootage;
        private string displayName;

        public PageSizeCount(float SizeX, float SizeY, string DisplayName = "")
        {
            sizeX = SizeX;
            sizeY = SizeY;
            displayName = DisplayName;
            isDefaultSize = false;
            SetSquareFootage();
        }

        public PageSizeCount(float SizeX, float SizeY, bool IsDefaultSize, string DisplayName = "")
        {
            isDefaultSize = IsDefaultSize;
            sizeX = SizeX;
            sizeY = SizeY;
            displayName = DisplayName;
            SetSquareFootage();
        }

        public float SizeX { get { return sizeX; } }
        public float SizeY { get { return sizeY; } }
        public int NumberOfPages { get { return numberOfPages; } }

        public int StagedCounts { get { return stagedCounts; } }
        public bool IsDefaultSize { get { return isDefaultSize; } }

        public int SquareFootage { get { return squareFootage; } }

        public void AddToCount()
        {
            numberOfPages++;
        }

        public void AddToStagedCount()
        {
            stagedCounts++;
        }

        public void CommitStagedCounts()
        {
            numberOfPages += stagedCounts;
            ClearStagedCounts();
        }

        public void ClearStagedCounts()
        {
            stagedCounts = 0;
        }

        private void SetSquareFootage()
        {
            squareFootage = (int)Math.Ceiling((sizeX * sizeY) / 144f);
        }
    }
}
