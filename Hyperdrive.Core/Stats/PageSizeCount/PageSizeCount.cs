using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Stats.PageSizeCount
{
    public class PageSizeCount
    {
        private float mSizeX = 0;
        private float mSizeY = 0;
        private int mNumberOfPages = 0;

        public PageSizeCount(float SizeX, float SizeY)
        {
            mSizeX = SizeX;
            mSizeY = SizeY;
        }

        public float SizeX { get { return mSizeX; } }
        public float SizeY { get { return mSizeY; } }
        public int NumberOfPages { get { return mNumberOfPages; } }

        public void AddToCount()
        {
            mNumberOfPages++;
        }
    }
}
