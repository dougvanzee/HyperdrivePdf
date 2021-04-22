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

        public PageSizeCount(float SizeX, float SizeY)
        {
            sizeX = SizeX;
            sizeY = SizeY;
            isDefaultSize = false;
            SetSquareFootage();
        }

        public PageSizeCount(float SizeX, float SizeY, bool IsDefaultSize)
        {
            isDefaultSize = IsDefaultSize;
            sizeX = SizeX;
            sizeY = SizeY;
            SetSquareFootage();
        }

        public float SizeX { get { return sizeX; } }
        public float SizeY { get { return sizeY; } }
        public int NumberOfPages { get { return numberOfPages; } }

        public int StagedCounts { get { return stagedCounts; } }
        public bool IsDefaultSize { get { return isDefaultSize; } }

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

        public int SquareFootage()
        {
            return squareFootage;
        }

        private void SetSquareFootage()
        {
            squareFootage = (int)Math.Ceiling((sizeX * sizeY) / 144f);
        }
    }
}
