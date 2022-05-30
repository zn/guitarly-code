using System;

namespace Models.EntityModels
{
    public class Chord
    {
        public Chord(string chord)
        {
            parse(chord);
        }

        public string Value { get; set; }
        public char Note { get; set; }
        public bool IsMinor { get; set; }
        public bool IsDies { get; set; }
        public bool IsBemol { get; set; }
        public string RestPart { get; set; }

        private void parse(string chord)
        {
            Value = chord;
            Note = chord[0];
            chord = chord.Remove(0, 1);

            if (!string.IsNullOrEmpty(chord))
            {
                IsDies = chord.Contains("#");
                if (IsDies)
                {
                    chord = chord.Replace("#", "");
                }

                IsBemol = !IsDies && chord.Contains("b");
                if (IsBemol)
                {
                    chord = chord.Replace("b", "");
                }

                IsMinor = !string.IsNullOrEmpty(chord) && chord[0] == 'm' && (chord.Length > 1 && !char.IsLetter(chord[1]) || chord.Length == 1);
                if (IsMinor)
                {
                    chord = chord.Remove(0, 1);
                }
                RestPart = chord;

            }
        }

        public override string ToString()
        {
            return $"{Value} | Note: {Note}; IsMinor: {IsMinor}; IsDies: {IsDies}; IsBemol: {IsBemol}; RestPart: {RestPart}";
        }
    }
}