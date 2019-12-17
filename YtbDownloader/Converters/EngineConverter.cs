using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using YtbDownloader.Core.Common;

namespace YtbDownloader.Converters
{
    public class EngineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DownloadEngine)
            {
                if (targetType == typeof(string))
                {
                    return ((DownloadEngine)value).ToString();
                }
                else if (targetType == typeof(int))
                {
                    return (int)((DownloadEngine)value);
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(DownloadEngine))
            {
                if (value is int)
                {
                    return Enum.ToObject(typeof(DownloadEngine), (int)value);
                }
                else if (value is string)
                {
                    try
                    {
                        return Enum.Parse(typeof(DownloadEngine), (string)value);
                    }
                    catch (OverflowException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}