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
    internal class BlockBraceParser
    {
        public const int indexNotFound = -1;
        private const char newLineWindowsChar = '\n';
        const char curlyBracketLeft = '{',
                   curlyBracketRight = '}';

        int? parsedVersion;
        public List<BlockSpan> result { get; }

        internal static BlockBraceParser Instance => new BlockBraceParser();

        private BlockBraceParser()
        {
            result = new List<BlockSpan>();
        }

        public List<BlockSpan> Parse(ITextSnapshot _textSnapshot)
        {
            int versionNumber = _textSnapshot.Version.VersionNumber;

            if (!parsedVersion.HasValue || parsedVersion.Value != versionNumber)
            {
                this.Parse(_textSnapshot.GetText());

                parsedVersion = versionNumber;
            }

            return result;
        }

        private void Parse(string _xppCode)
        {
            result.Clear();

            List<int> blockBraceEndIndexes = new List<int>();
            blockBraceEndIndexes.addIndexesСorrespondenceByCharInSourceCodeString(ref _xppCode, curlyBracketRight);
            int? blockEndBraceIndex = default;
            
            foreach (int blockStartBraceIndex in StackExtension.enumerateIndexesСorrespondenceByCharInSourceCodeString(ref _xppCode, curlyBracketLeft))
            {
                blockEndBraceIndex = blockBraceEndIndexes.FirstOrDefault(w => w > blockStartBraceIndex);
                
                if (blockEndBraceIndex.HasValue && !blockEndBraceIndex.Value.isNegative())
                {
                    var offset = blockStartBraceIndex - 1 - _xppCode.LastIndexOf(newLineWindowsChar, blockStartBraceIndex);

                    if (offset == blockStartBraceIndex - 1)
                    {
                        offset = 0;
                    }
                    
                    if (this.isSkipBlock(ref _xppCode, blockStartBraceIndex, blockEndBraceIndex.Value))
                    {
                        result.Add(new BlockSpan(offset, blockStartBraceIndex, blockEndBraceIndex.Value - blockStartBraceIndex));
                    }
                    blockBraceEndIndexes.Remove(blockEndBraceIndex.Value);
                }
            }
        }

        private bool isSkipBlock(ref string _xppCode, int blockStartBraceIndex, int _blockEndBraceIndex)
        {
            return _blockEndBraceIndex > _xppCode.IndexOf(newLineWindowsChar, ++blockStartBraceIndex);
        }
    }
}
