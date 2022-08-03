// Copyright (c) 2012-2022 Wojciech Figat. All rights reserved.

using System.Collections.Generic;
using FlaxEngine.Utilities;

namespace FlaxEngine.GUI
{
    partial class RichTextBox
    {
        /// <summary>
        /// Rich Text parsing context.
        /// </summary>
        public struct ParsingContext
        {
            /// <summary>
            /// Text box control.
            /// </summary>
            public RichTextBox Control;

            /// <summary>
            /// HTML tags parser.
            /// </summary>
            public HtmlParser Parser;

            /// <summary>
            /// Current caret location for the new text blocks origin.
            /// </summary>
            public Float2 Caret;
            
            /// <summary>
            /// Index of the current line start character.
            /// </summary>
            public int LineStartCharacterIndex;
            
            /// <summary>
            /// Index of the current line start text block.
            /// </summary>
            public int LineStartTextBlockIndex;

            /// <summary>
            /// Text styles stack (new tags push modified style and pop on tag end).
            /// </summary>
            public Stack<TextBlockStyle> StyleStack;
        }
        
        /// <summary>
        /// The delegate for text blocks post-processing.
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="textBlock">The text block (not yet added to the text box - during parsing).</param>
        public delegate void PostProcessBlockDelegate(ref ParsingContext context, ref TextBlock textBlock);

        /// <summary>
        /// The custom callback for post-processing text blocks.
        /// </summary>
        [HideInEditor]
        public PostProcessBlockDelegate PostProcessBlock;
        
        /// <summary>
        /// The delegate for HTML tag processing.
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="tag">The tag.</param>
        public delegate void ProcessTagDelegate(ref ParsingContext context, ref HtmlTag tag);

        /// <summary>
        /// Collection of HTML tags processors.
        /// </summary>
        public static Dictionary<string, ProcessTagDelegate> TagProcessors = new Dictionary<string, ProcessTagDelegate>
        {
            { "br", ProcessBr },
            { "color", ProcessColor },
            { "alpha", ProcessAlpha },
            { "style", ProcessStyle },
            { "font", ProcessFont },
            { "b", ProcessBold },
            { "i", ProcessItalic },
            { "size", ProcessSize },
        };

        private HtmlParser _parser = new HtmlParser();
        private Stack<TextBlockStyle> _styleStack = new Stack<TextBlockStyle>();

        /// <inheritdoc />
        protected override void OnParseTextBlocks()
        {
            if (ParseTextBlocks != null)
            {
                ParseTextBlocks(_text, _textBlocks);
                return;
            }
            
            // Setup parsing
            _parser.Reset(_text);
            _styleStack.Clear();
            _styleStack.Push(_textStyle);
            var context = new ParsingContext
            {
                Control = this,
                Parser = _parser,
                Caret = Float2.Zero,
                LineStartCharacterIndex = 0,
                LineStartTextBlockIndex = 0,
                StyleStack = _styleStack,
            };

            // Parse text with HTML tags
            int testStartPos = 0;
            while (_parser.ParseNext(out var tag))
            {
                // Insert text found before the tag
                OnAddTextBlock(ref context, testStartPos, tag.StartPosition);

                // Move the text pointer after the tag
                testStartPos = tag.EndPosition;

                // Process the tag
                OnParseTag(ref context, ref tag);
            }

            // Insert remaining text
            OnAddTextBlock(ref context, testStartPos, _text.Length);
        }

        /// <summary>
        /// Parses HTML tag.
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="tag">The tag.</param>
        protected virtual void OnParseTag(ref ParsingContext context, ref HtmlTag tag)
        {
            // Get tag processor matching the tag name
            if (!TagProcessors.TryGetValue(tag.Name, out var process))
                TagProcessors.TryGetValue(tag.Name.ToLower(), out process);

            if (process != null)
            {
                process(ref context, ref tag);
            }
        }

        /// <summary>
        /// Inserts parsed text block using the current style and state (eg. from tags).
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="start">Start position (character index).</param>
        /// <param name="end">End position (character index).</param>
        protected virtual void OnAddTextBlock(ref ParsingContext context, int start, int end)
        {
            if (start >= end)
                return;

            // Setup text block
            var textBlock = new TextBlock
            {
                Style = context.StyleStack.Peek(),
                Range = new TextRange
                {
                    StartIndex = start,
                    EndIndex = end,
                },
            };

            // Process text into text blocks (handle newlines etc.)
            var font = textBlock.Style.Font.GetFont();
            if (!font)
                return;
            var lines = font.ProcessText(_text, ref textBlock.Range);
            if (lines == null || lines.Length == 0)
                return;
            for (int i = 0; i < lines.Length; i++)
            {
                ref var line = ref lines[i];
                textBlock.Range = new TextRange
                {
                    StartIndex = start + line.FirstCharIndex,
                    EndIndex = start + line.LastCharIndex,
                };
                if (i != 0)
                {
                    context.Caret.X = 0;
                    OnLineAdded(ref context, textBlock.Range.StartIndex - 1);
                }
                textBlock.Bounds = new Rectangle(context.Caret, line.Size);
                textBlock.Bounds.X += line.Location.X;

                // Post-processing
                PostProcessBlock?.Invoke(ref context, ref textBlock);
            
                // Add to the text blocks
                _textBlocks.Add(textBlock);
            }
            
            // Update the caret location
            ref var lastLine = ref lines[lines.Length - 1];
            if (lines.Length == 1)
            {
                context.Caret.X += lastLine.Size.X;
            }
            else
            {
                context.Caret.X = lastLine.Size.X;
            }
        }

        private void OnLineAdded(ref ParsingContext context, int lineEnd)
        {
            // Calculate size of the line
            var textBlocks = Utils.ExtractArrayFromList(_textBlocks);
            var lineOrigin = textBlocks[context.LineStartTextBlockIndex].Bounds.Location;
            var lineSize = Float2.Zero;
            var lineAscender = 0.0f;
            for(int i = context.LineStartTextBlockIndex; i < _textBlocks.Count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];
                var textBlockSize = textBlock.Bounds.BottomRight - lineOrigin;
                var textBlockFont = textBlock.Style.Font.GetFont();
                if (textBlockFont)
                    lineAscender = Mathf.Max(lineAscender, textBlockFont.Ascender);
                lineSize = Float2.Max(lineSize, textBlockSize);
            }

            // Organize text blocks to match the baseline of the line (use ascender)
            for(int i = context.LineStartTextBlockIndex; i < _textBlocks.Count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];
                var offset = lineSize.Y - textBlock.Bounds.Height;
                var textBlockFont = textBlock.Style.Font.GetFont();
                if (textBlockFont)
                    offset = lineAscender - textBlockFont.Ascender;
                textBlock.Bounds.Location.Y += offset;
            }

            // Move to the next line
            context.LineStartCharacterIndex = lineEnd + 1;
            context.LineStartTextBlockIndex = _textBlocks.Count;
            context.Caret.Y += lineSize.Y;
        }
    }
}
