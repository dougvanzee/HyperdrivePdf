using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Hyperdrive.Core.iTextExtensions
{
    public enum Orientation
    {
        Portrait,
        Landscape
    }

    public class PageSizeExt : PageSize
    {
        public static iText.Kernel.Geom.PageSize _4x6 = new iText.Kernel.Geom.PageSize(432, 288);
        public static iText.Kernel.Geom.PageSize _5x7 = new iText.Kernel.Geom.PageSize(504, 360);
        public static iText.Kernel.Geom.PageSize LETTER_QUARTER = new iText.Kernel.Geom.PageSize(306, 396);
        public static iText.Kernel.Geom.PageSize LETTER_HALF = new iText.Kernel.Geom.PageSize(396, 612);
        public static iText.Kernel.Geom.PageSize LETTER_TAB = new iText.Kernel.Geom.PageSize(648, 792);
        public static iText.Kernel.Geom.PageSize TABLOID_EXTRA = new iText.Kernel.Geom.PageSize(1296, 864);

        public static iText.Kernel.Geom.PageSize _15x21 = new iText.Kernel.Geom.PageSize(1512, 1080);
        public static iText.Kernel.Geom.PageSize ARCH_A = new iText.Kernel.Geom.PageSize(864, 648);
        public static iText.Kernel.Geom.PageSize ARCH_B = new iText.Kernel.Geom.PageSize(1296, 864);
        public static iText.Kernel.Geom.PageSize ARCH_C = new iText.Kernel.Geom.PageSize(1728, 1296);
        public static iText.Kernel.Geom.PageSize ARCH_D = new iText.Kernel.Geom.PageSize(2592, 1728);
        public static iText.Kernel.Geom.PageSize ARCH_E1 = new iText.Kernel.Geom.PageSize(3024, 2160);
        public static iText.Kernel.Geom.PageSize ARCH_E2 = new iText.Kernel.Geom.PageSize(2736, 1872);
        public static iText.Kernel.Geom.PageSize ARCH_E3 = new iText.Kernel.Geom.PageSize(2808, 1944);
        public static iText.Kernel.Geom.PageSize ARCH_E = new iText.Kernel.Geom.PageSize(3456, 2592);

        public static iText.Kernel.Geom.PageSize ANSI_A = new iText.Kernel.Geom.PageSize(792, 612);
        public static iText.Kernel.Geom.PageSize ANSI_B = new iText.Kernel.Geom.PageSize(1224, 792);
        public static iText.Kernel.Geom.PageSize ANSI_C = new iText.Kernel.Geom.PageSize(1584, 1224);
        public static iText.Kernel.Geom.PageSize ANSI_D = new iText.Kernel.Geom.PageSize(2448, 1584);
        public static iText.Kernel.Geom.PageSize ANSI_E = new iText.Kernel.Geom.PageSize(3168, 2448);

        public PageSizeExt(float width, float height) : base(width, height)
        {

        }

        public PageSizeExt(float width, float height, Orientation orientation) : base(PageSizeExt.OrientationWidth(width, height, orientation), PageSizeExt.OrientationHeight(width, height, orientation))
        {

        }

        public PageSizeExt(Rectangle box) : base(box)
        {

        }

        private static float OrientationWidth(float width, float height, Orientation orientation)
        {
            if (orientation == Orientation.Portrait)
            {
                return width <= height ? width : height;
            }
            else
            {
                return width > height ? width : height;
            }
        }

        private static float OrientationHeight(float width, float height, Orientation orientation)
        {
            if (orientation == Orientation.Portrait)
            {
                return height >= width ? height : width;
            }
            else
            {
                return height < width ? height : width;
            }
        }
    }
}
