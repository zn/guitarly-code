using System;
using System.Collections.Generic;
using System.Text;

namespace Parsers
{
    class Composition
    {
        public string SongTitle { get; set; }
        public string Url { get; set; }
        public int Views { get; set; }

        public override string ToString()
        {
            return $"{SongTitle}\t{Url}\t{Views}";
        }
    }
}
