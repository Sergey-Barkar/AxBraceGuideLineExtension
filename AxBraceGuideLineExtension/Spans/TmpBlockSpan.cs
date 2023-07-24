﻿/*
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

using System.Collections.Generic;

namespace AxBraceGuideLineExtension
{
    internal class TmpLinkedBlock
    {
        internal const int validHorizontalOffsetCoefficient = 4;
        internal readonly int startBlockBraceIndex;
        internal readonly int startBlockLineIndex;

        public TmpLinkedBlock(int _startBlockLineIndex, int _startBlockBraceIndex)
        {
            startBlockBraceIndex = _startBlockBraceIndex;
            startBlockLineIndex = _startBlockLineIndex;
        }

        public BlockSpan? toBlockSpan(int _endBlockLineIndex, int _endBlockBraceIndex)
        {
            const int validHorizontalOffsetCoefficient = 4;

            BlockSpan? ret = null;

            if (_endBlockLineIndex > startBlockLineIndex)
            {
                int indent = startBlockBraceIndex - startBlockLineIndex - 1;
                bool isValid = indent == _endBlockBraceIndex - _endBlockLineIndex - 1 && ((indent) % validHorizontalOffsetCoefficient).isZero();

                ret = new BlockSpan(isValid, indent, startBlockBraceIndex, _endBlockBraceIndex - startBlockBraceIndex);
            }

            return ret;
        }
    }
}