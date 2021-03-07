using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperdrive.Core.iTextExtensions;

namespace Hyperdrive.Core.StepAndRepeat
{


    public class StepAndRepeatSettings
    {
        public enum PageSizeMode
        {
            Automatic,
            RollWidth,
            Custom
        }

        public PageSizeMode pageSizeMode;

        /// <summary>
        /// Ignored if <see cref="pageSizeMode"/> is set to <see cref="PageSizeMode.Automatic"/>
        /// Height is ignored if <see cref="pageSizeMode"/> is set to <see cref="PageSizeMode.RollWidth"/>
        /// </summary>
        public PageSizeExt pageSize;

        //
        public Orientation pageOrientation;

        public float pagePaddingX;
        public float pagePaddingY;

        private enum ContentSizeType
        {
            Orginal,
            Scaled,
            Custom
        }

        public PageSizeExt contentSize;

    }
}
