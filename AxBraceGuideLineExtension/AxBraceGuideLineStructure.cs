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
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace AxBraceGuideLineExtension
{
    public class AxBraceGuideLineExtension : ITagger<TextMarkerTag>
    {
        readonly IWpfTextView IView;
        readonly IAdornmentLayer layer;

        [Export(typeof(AdornmentLayerDefinition))]
        [Name(blockStructureLayerName)]
        static AdornmentLayerDefinition adornmentLayer;

        int drawnViewHash = 0;

        const string blockStructureLayerName = "BlockStructure";

        public AxBraceGuideLineExtension(IWpfTextView _IView)
        {
            IView = _IView;
            layer = IView.GetAdornmentLayer(blockStructureLayerName);

            IView.LayoutChanged += IView_LayoutChanged;
        }

        private bool isDrawRequired(int _currentViewHash)
        {
            return drawnViewHash != _currentViewHash && IView.TextViewLines != null;
        }

        void IView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            int currentHash = IView.TextSnapshot.GetHashCode() ^ IView.FormattedLineSource.GetHashCode();

            if (this.isDrawRequired(currentHash))
            {
                this.drawNormalizedSnapshotSpans(new NormalizedSnapshotSpanCollection(IView.TextViewLines.FormattedSpan));
                drawnViewHash = currentHash;
            }
        }

        private void drawNormalizedSnapshotSpans(NormalizedSnapshotSpanCollection _normalizedSnapshotSpans)
        {
            layer.RemoveAllAdornments();

            if (_normalizedSnapshotSpans.Count.isPositive())
            {
                HashSet<Span> updated = new HashSet<Span>();

                foreach (var snapshotSpan in _normalizedSnapshotSpans)
                {
                    var textSnapshot = snapshotSpan.Snapshot;

                    foreach (var spanToDraw in BlockBraceParser.Instance.Parse(textSnapshot))
                    {
                        if (!updated.Contains(spanToDraw))
                        {
                            if (layer.drawSpan(IView, spanToDraw))
                            {
                                updated.Add(spanToDraw);
                            }
                        }
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }
    }
}


