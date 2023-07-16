/*
    Copyright (C) 2023 Sergey Barkar / imperialSergius@Live.Ru

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), 
    to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Windows.Media;
using System.Windows.Shapes;

namespace AxBraceGuideLineExtension
{
    internal static class LineExtension
    {
        private const double standardOpacity = 0.5;
        private const double invalidThickness = 1.5;
        private readonly static DoubleCollection invalidDashArray = new DoubleCollection(new double[] { 1.0, 0 });
        private readonly static SolidColorBrush invalidBlockBrush = new SolidColorBrush(Colors.Red) { Opacity = standardOpacity };
        
        internal readonly static Line Instance = new Line()
        {
            DataContext = 1,
            Stroke = new SolidColorBrush(Colors.Gray) { Opacity = standardOpacity },
            StrokeThickness = 1.0,
            StrokeDashArray = new DoubleCollection(new double[] { 3.0, 5.0 })
        };

        internal static Line copy(this Line _line)
        {
            Line ret = new Line()
            {
                DataContext = _line.DataContext,
                Stroke = _line.Stroke,
                StrokeThickness = _line.StrokeThickness,
                StrokeDashArray = _line.StrokeDashArray
            };

            return ret;
        }

        internal static Line newFromBlockSpan(this Line _line, BlockSpan _span)
        {
            Line ret = _line.copy();

            if (!_span.isValid)
            {
                ret.Stroke = invalidBlockBrush;
                ret.StrokeThickness = invalidThickness;
                ret.StrokeDashArray = invalidDashArray;
            }

            return ret;
        }

        internal static Line updatePosition(this Line _line, BlockSpanViewIntersecting _viewIntersecting)
        {
            return _line.updatePosition(_viewIntersecting.horizontalOffsetLeft, _viewIntersecting.verticalOffsetTop, _viewIntersecting.verticalOffsetBottom);
        }

        internal static Line updatePosition(this Line _line, double _left, double _top, double _bottom)
        {
            _line.X2 = _left;
            _line.X1 = _left;
            _line.Y1 = _top;
            _line.Y2 = _bottom;

            return _line;
        }
    }
}
