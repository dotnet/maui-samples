using System.Reflection;
using System.Text;

namespace XamlSamples
{
    public class NamedColor
    {
        public string Name { private set; get; }

        public string FriendlyName { private set; get; }

        public Color Color { private set; get; }

        // Expose the Color fields as properties
        public float Red => Color.Red;
        public float Green => Color.Green;
        public float Blue => Color.Blue;

        // Static members
        static NamedColor()
        {
            List<NamedColor> all = new List<NamedColor>();
            StringBuilder stringBuilder = new StringBuilder();

            // Loop through the public static fields of the Color structure.
            foreach (FieldInfo fieldInfo in typeof (Colors).GetRuntimeFields ())
            {
                if (fieldInfo.IsPublic && 
                    fieldInfo.IsStatic &&
                    fieldInfo.FieldType == typeof (Color))
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
                    NamedColor namedColor = new NamedColor
                    {
                        Name = name,
                        FriendlyName = stringBuilder.ToString(),
                        Color = (Color)fieldInfo.GetValue(null)
                    };

                    // Add it to the collection.
                    all.Add(namedColor);
                }
            }
            all.TrimExcess();
            All = all;
        }

        public static IEnumerable<NamedColor> All { private set; get; }
    }
}
