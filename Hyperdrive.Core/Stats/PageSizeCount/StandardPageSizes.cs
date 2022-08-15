using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Stats.PageSizeCount
{
    public static class StandardPageSizes
    {
        public static List<PageSizeCount> Pages()
        {
            List<PageSizeCount> DefaultPageSizes = new List<PageSizeCount>
            {
                new PageSizeCount(8.27f, 11.69f, true, "A4"),
                new PageSizeCount(8.5f, 11f, true, "8.5x11"),
                new PageSizeCount(8.5f, 14f, true, "8.5x14"),
                new PageSizeCount(9f, 11f, true, "9x11"),
                new PageSizeCount(9f, 12f, true, "ARCH A"),
                new PageSizeCount(11f, 17f, true, "11x17"),
                new PageSizeCount(11.69f, 16.54f, true, "A3"),
                new PageSizeCount(12f, 18f, true, "12x18"),
                new PageSizeCount(15f, 21f, true, "15x21"),
                new PageSizeCount(15f, 22f, true, "15x22"),
                new PageSizeCount(17f, 21f, true, "17x21"),
                new PageSizeCount(17f, 22f, true, "17x22"),
                new PageSizeCount(18f, 24f, true, "18x24"),
                new PageSizeCount(18f, 36f, true, "18x36"),
                new PageSizeCount(22f, 34f, true, "22x34"),
                new PageSizeCount(22f, 36f, true, "22x36"),
                new PageSizeCount(24f, 36f, true, "24x36"),
                new PageSizeCount(24f, 44f, true, "24x44"),
                new PageSizeCount(26f, 36f, true, "26x36"),
                new PageSizeCount(30f, 36f, true, "30x36"),
                new PageSizeCount(30f, 40f, true, "30x40"),
                new PageSizeCount(30f, 42f, true, "30x42"),
                new PageSizeCount(34f, 44f, true, "34x44"),
                new PageSizeCount(36f, 42f, true, "30x42"),
                new PageSizeCount(36f, 48f, true, "36x48")
            };

            return DefaultPageSizes;
        }

            
    }
}
