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
using System.Reflection;
using System.Text;

namespace AxBraceGuideLineExtension
{
    internal abstract class SourceCodeBlockRestrictionBase : ISourceCodeBlockIncludeRestriction
    {
        public bool isRestructed { get; private set; }
        protected int analysingIndex = default;
        public abstract char[] getHandlingChars();
        public abstract bool isSetRestriction(char _analysingChar);
        public abstract bool isRemoveRestriction(char _analysingChar);

        public bool tryToSetRestriction(ref string _analysingText, int _analysingIndex)
        {
            analysingIndex = _analysingIndex;

            if (this.isSetRestriction(_analysingText[analysingIndex]))
            {
                isRestructed = true;
            }
            
            return isRestructed;
        }

        public bool tryToRemoveRestriction(ref string _analysingText, int _analysingIndex)
        {
            analysingIndex = _analysingIndex;

            if (this.isRemoveRestriction(_analysingText[analysingIndex]))
            {
                isRestructed = false;
            }

            return !isRestructed;
        }

        internal static List<SourceCodeBlockRestrictionBase> createInheritors()
        {
            List<SourceCodeBlockRestrictionBase> ret = new List<SourceCodeBlockRestrictionBase>();

            foreach (var type in Assembly.GetAssembly(typeof(SourceCodeBlockRestrictionBase)).GetTypes().Where(w => w.IsClass && !w.IsAbstract && w.IsSubclassOf(typeof(SourceCodeBlockRestrictionBase))))
            {
                ret.Add((SourceCodeBlockRestrictionBase)Activator.CreateInstance(type));
            }

            return ret;
        }

        internal void free()
        {
            isRestructed = false;
        }
    }
}
