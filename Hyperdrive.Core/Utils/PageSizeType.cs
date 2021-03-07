using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hyperdrive.Core.Utils
{
    public enum PageSizeType
    {
        AUTO,
        CUSTOM
    }

    internal static class PageSizeTypeHelper
    {
        #region Constructor

        static PageSizeTypeHelper()
        {
            // Create enum-to friendly name dictionary
            PageSizeTypeFriendlyNames = new Dictionary<PageSizeType, string>
                {
                {PageSizeType.AUTO, "Auto"},
                {PageSizeType.CUSTOM, "Custom"}
                };

            // Create friendly name-to-enum dictionary
            PageSizeTypeEnumValues = PageSizeTypeFriendlyNames.ToDictionary(x => x.Value, x => x.Key);
        }

        #endregion

        #region Properties

        public static Dictionary<PageSizeType, String> PageSizeTypeFriendlyNames { get; set; }

        public static Dictionary<String, PageSizeType> PageSizeTypeEnumValues { get; set; }

        #endregion
    }

    #region FontStylesListProvider

    public class PageSizeTypeListProvider : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /* Note that this converter does not convert a value passed in. Instead, it generates 
                * a list of user-friendly counterparts for each menber of the target enum and
                * returns that list to the caller. */

            var fontStyleList = PageSizeTypeHelper.PageSizeTypeFriendlyNames.Values.ToList();
            return fontStyleList;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion

    #region FontStylesValueConverter

    public class PageSizeTypeValueConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = (PageSizeType)value;
            var friendlyName = PageSizeTypeHelper.PageSizeTypeFriendlyNames[enumValue];
            return friendlyName;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var friendlyName = (String)value;
            var enumValue = PageSizeTypeHelper.PageSizeTypeEnumValues[friendlyName];
            return enumValue;
        }

        #endregion
    }

    #endregion
}
