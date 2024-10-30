using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flowtrail
{
    internal class Button
    {
        public Button() { }

        public Vector2 position;
        private String text;

        public Color currentColor;
        public Color idleColor;
        public Color highlightColor;

        public string Text { get => text; set => text = value; }
    }
}
