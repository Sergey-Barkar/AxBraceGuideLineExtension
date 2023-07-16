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
using System.Windows.Shapes;

namespace AxBraceGuideLineExtension
{
    internal struct BlockSpan 
    {
        internal const int validHorizontalOffsetCoefficient = 4;
        internal readonly bool isValid;
        internal readonly int horizontalOffsetLeft;
        private readonly Span verticalSpan;

        internal BlockSpan(int _horizontalOffsetLeft, int _start, int _length)
        {
            horizontalOffsetLeft = _horizontalOffsetLeft;

            isValid = ((_horizontalOffsetLeft) % validHorizontalOffsetCoefficient).isZero();

            verticalSpan = new Span(_start, _length);
        }

        public static implicit operator Span(BlockSpan _typesSpan)
        {
            return _typesSpan.verticalSpan;
        }

        internal SnapshotSpan toSnapshotSpan(ITextSnapshot _textSnapshot)
        {
            return new SnapshotSpan(_textSnapshot, this);
        }

        internal BlockSpanViewIntersecting toBlockSpanViewIntersecting(IWpfTextView _IView)
        {
            return new BlockSpanViewIntersecting(_IView, this);
        }

        internal Line newLineFromBlockSpanViewIntersecting(BlockSpanViewIntersecting _viewIntersecting)
        {
            return LineExtension.Instance.newFromBlockSpan(this).updatePosition(_viewIntersecting);
        }
    }
}
