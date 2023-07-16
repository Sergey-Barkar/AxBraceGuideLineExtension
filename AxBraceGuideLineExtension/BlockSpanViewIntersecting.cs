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

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;

namespace AxBraceGuideLineExtension
{
    internal class BlockSpanViewIntersecting
    {
        private const int firstDrawableLineIndex = 1;
        private const int skipLines = 2;

        public double horizontalOffsetLeft { get; private set; }
        public double verticalOffsetTop { get; private set; }
        public double verticalOffsetBottom { get; private set; }
        public bool isDrawable { get; private set; }

        internal BlockSpanViewIntersecting(IWpfTextView _IView, BlockSpan _blockSpan)
        {
            SnapshotSpan snapshotSpanToDraw = _blockSpan.toSnapshotSpan(_IView.TextSnapshot);
            var viewLines = _IView.TextViewLines.GetTextViewLinesIntersectingSpan(snapshotSpanToDraw);
            var endIndex = viewLines.Count - skipLines;
            
            isDrawable = endIndex.isPositive();

            if (isDrawable)
            {
                var line = snapshotSpanToDraw.Start.GetContainingLine().Start + _blockSpan.horizontalOffsetLeft;
                var bounds = _IView.GetTextViewLineContainingBufferPosition(line).GetExtendedCharacterBounds(line);
                horizontalOffsetLeft = Math.Round(bounds.Left + bounds.Width / 2);
                verticalOffsetTop = viewLines[firstDrawableLineIndex].Top;
                verticalOffsetBottom = viewLines[endIndex].Bottom;
            }
        }
    }
}
