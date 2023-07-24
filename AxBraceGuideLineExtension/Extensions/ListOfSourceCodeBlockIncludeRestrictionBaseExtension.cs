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

using System;
using System.Collections.Generic;
using System.Linq;

namespace AxBraceGuideLineExtension
{
    internal static class ListOfSourceCodeBlockIncludeRestrictionBaseExtension
    {
        internal static SourceCodeBlockRestrictionBase getFirst(this List<SourceCodeBlockRestrictionBase> _this, ref string _xppCode, int _index)
        {
            SourceCodeBlockRestrictionBase ret = null;

            foreach (var restruction in _this)
            {
                if (restruction.tryToSetRestriction(ref _xppCode, _index))
                {
                    ret = restruction;
                    break;
                }
            }

            return ret;
        }

        internal static void freeAll(this List<SourceCodeBlockRestrictionBase> _this)
        {
            foreach (var restruction in _this)
            {
                restruction.free();
            }
        }

        internal static char[] getHandlingChars(this List<SourceCodeBlockRestrictionBase> _this)
        {
            char[] ret = new char[0];

            foreach(var restruction in _this)
            {
                ret = ret.Union(restruction.getHandlingChars()).ToArray();
            }

            return ret;
        }
    }
}
