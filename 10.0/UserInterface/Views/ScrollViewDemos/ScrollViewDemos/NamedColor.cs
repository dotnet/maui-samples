﻿using System.Reflection;
using System.Text;

namespace ScrollViewDemos
{
    public class NamedColor : IEquatable<NamedColor>, IComparable<NamedColor>
    {
        public static IList<NamedColor> All { get; set; }

        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public Color Color { get; set; }

        public bool Equals(NamedColor other)
        {
            return Name.Equals(other.Name);
        }

        public int CompareTo(NamedColor other)
        {
            return Name.CompareTo(other.Name);
        }

        // Static members
        static NamedColor()
        {
            List<NamedColor> all = new List<NamedColor>();
            StringBuilder stringBuilder = new StringBuilder();

            // Loop through the public static fields of the Color structure.
            foreach (FieldInfo fieldInfo in typeof(Colors).GetRuntimeFields())
            {
                if (fieldInfo.IsPublic &&
                    fieldInfo.IsStatic &&
                    fieldInfo.FieldType == typeof(Color))
                {
                    // Convert the name to a friendly name.
                    string name = fieldInfo.Name;
                    stringBuilder.Clear();
                    int index = 0;

                    foreach (char ch in name)
                    {
                        if (index != 0 && Char.IsUpper(ch))
                        {
                            stringBuilder.Append(' ');
                        }
                        stringBuilder.Append(ch);
                        index++;
                    }

                    // Instantiate a NamedColor object.
                    Color color = (Color)fieldInfo.GetValue(null);

                    NamedColor namedColor = new NamedColor
                    {
                        Name = name,
                        FriendlyName = stringBuilder.ToString(),
                        Color = color
                    };

                    // Add it to the collection.
                    all.Add(namedColor);
                }
            }
            all.TrimExcess();
            all.Sort();
            All = all;
        }
    }
}
