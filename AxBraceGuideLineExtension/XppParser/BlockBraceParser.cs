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
using System.Collections.Generic;
using System.Linq;

namespace AxBraceGuideLineExtension
{
    public class BlockBraceParser : Versionabler
    {
        public const int indexNotFound = -1;
        private const char newLineWindowsChar = '\n';
        const char curlyBracketLeft = '{',
                   curlyBracketRight = '}';

        internal List<BlockSpan> result { get; }
        private List<SourceCodeBlockRestrictionBase> restrictions { get; }
        private char[] analysingChars { get; }

        public static BlockBraceParser Instance => new BlockBraceParser();

        private BlockBraceParser()
        {
            result = new List<BlockSpan>();
            restrictions = SourceCodeBlockRestrictionBase.createInheritors();
            analysingChars = new char[3] { curlyBracketLeft, curlyBracketRight, newLineWindowsChar }.Union(restrictions.getHandlingChars()).ToArray();
        }

        internal List<BlockSpan> Parse(ITextSnapshot _textSnapshot)
        {
            var snapShotVersion = _textSnapshot.Version.VersionNumber;

            if (this.isUnsavedVersion(snapShotVersion))
            {
                this.Parse(_textSnapshot.GetText());

                this.saveVersion(snapShotVersion);
            }

            return result;
        }

        public void Parse(string _xppCode)
        {
            result.Clear();

            Stack<TmpLinkedBlock> tmpCodeBlockSpans = new Stack<TmpLinkedBlock>();
            SourceCodeBlockRestrictionBase activeRestruction = null;

            var index = indexNotFound;
            int lastNewLineIndex = default;

            do
            {
                index = _xppCode.IndexOfAny(activeRestruction is null ? analysingChars : activeRestruction.getHandlingChars(), index + 1);

                if (!index.isNegative())
                {
                    if (activeRestruction is null)
                    {
                        activeRestruction = restrictions.getFirst(ref _xppCode, index);
                    }
                    else if (activeRestruction.tryToRemoveRestriction(ref _xppCode, index))
                    {
                        activeRestruction = null;
                    }

                    if (activeRestruction is null)
                    {
                        BlockSpan? blockSpanNullable = null;

                        switch (_xppCode[index])
                        {
                            case newLineWindowsChar:
                                lastNewLineIndex = index;
                                break;
                            case curlyBracketLeft:
                                tmpCodeBlockSpans.Push(new TmpLinkedBlock(lastNewLineIndex, index));
                                break;
                            case curlyBracketRight:
                                if (tmpCodeBlockSpans.Count.isPositive())
                                {
                                    blockSpanNullable = tmpCodeBlockSpans.Pop().toBlockSpan(lastNewLineIndex, index);

                                    if (blockSpanNullable.HasValue)
                                    {
                                        result.Add(blockSpanNullable.Value);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            while (index != indexNotFound);

            restrictions.freeAll();
            
            result.Sort(BlockSpanSorter.Instance);
            
            Dictionary<int, List<Span>> keyValuePairs = new Dictionary<int, List<Span>>();
            for (int j = 0; j < result.Count; j++)
            {
                BlockSpan blockSpan = result[j];

                List<Span> spans;

                if (!keyValuePairs.TryGetValue(blockSpan.horizontalIndent, out spans))
                {
                    spans = new List<Span>();
                    keyValuePairs.Add(blockSpan.horizontalIndent, spans);
                }

                if (blockSpan.isValid && spans.Exists(w => w.Start < ((Span)blockSpan).Start && w.End > ((Span)blockSpan).End))
                {
                    blockSpan.isValid = false;
                    result[j] = blockSpan;
                }

                spans.Add(blockSpan);
            }

            keyValuePairs = null;
        }
    }
}
