using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SliderWithTickLabels
{
	class TickBarLabelMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
                var value = System.Convert.ToDouble(values[0]);
                
				var slider = values[1] as SliderWithTickLabels;

                var isTop = System.Convert.ToBoolean(values[2]);
                var tickBar = slider.Template.FindName(isTop ? "TopTick" : "BottomTick", slider) as TickBar;

                var positionMinimum = tickBar.ReservedSpace / 2;

				double scalingValue = 0.0;
				double left = 0.0;
				double top = 0.0;

				switch (slider.Orientation)
				{
					case Orientation.Horizontal:
						scalingValue = (tickBar.ActualWidth - tickBar.ReservedSpace) / (tickBar.Maximum - tickBar.Minimum);
						left = - (tickBar.ActualWidth / 2 - positionMinimum - scalingValue * (value - tickBar.Minimum)) * 2;
						top = 0;
						break;
					case Orientation.Vertical:
						scalingValue = (tickBar.ActualHeight - tickBar.ReservedSpace) / (tickBar.Maximum - tickBar.Minimum);
						left = 0;
						top = (tickBar.ActualHeight / 2 - positionMinimum - scalingValue * (value - tickBar.Minimum)) * 2;
						break;
					default:
						break;
				}

				if (slider.IsDirectionReversed)
				{
					left *= -1.0;
					top *= -1.0;
				}

				var thickness = new Thickness
				{
					Left = (left <= 0.0) ? left : 0,
					Top = (top <= 0.0) ? top : 0,
					Right = (left > 0.0) ? -left : 0,
					Bottom = (top > 0.0) ? -top : 0
				};

				return thickness;
			}
			catch 
			{
				return null;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
