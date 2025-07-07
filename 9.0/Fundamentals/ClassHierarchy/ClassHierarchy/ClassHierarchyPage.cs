using System.Reflection;

namespace ClassHierarchy;

class ClassHierarchyPage : ContentPage
{
    private Assembly mauiAssembly;
    private List<TypeInformation> classList = new List<TypeInformation>();
    private HashSet<string> addedTypeNames = new HashSet<string>();
    private StackLayout stackLayout;

    public ClassHierarchyPage()
    {
        void AddWithoutDuplicates(Type type)
        {
            string displayName = GetDisplayName(type);
            
            if (!addedTypeNames.Contains(displayName))
            {
                classList.Add(new TypeInformation(type));
                addedTypeNames.Add(displayName);
            }
        }

        // Get MAUI assembly.
        mauiAssembly = typeof(View).GetTypeInfo().Assembly;

        // Loop through all the types.
        foreach (Type type in mauiAssembly.ExportedTypes)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            // Public types only but exclude interfaces
            if (typeInfo.IsPublic && !typeInfo.IsInterface)
            {
                // Use the local function to add without duplicates
                AddWithoutDuplicates(type);
            }
        }

        // Ensure that all classes have a base type in the list.
        //  (i.e., add Attribute, ValueType, Enum, EventArgs, etc.)
        int index = 0;

        // Watch out! Loops through expanding classList!
        do
        {
            // Get a child type from the list.
            TypeInformation childType = classList[index];

            if (childType.Type != typeof(Object))
            {
                bool hasBaseType = false;

                // Loop through the list looking for a base type.
                foreach (TypeInformation parentType in classList)
                {
                    if (childType.IsDerivedDirectlyFrom(parentType.Type))
                        hasBaseType = true;
                }

                // If there's no base type, add it (but check if it already exists first).
                if (!hasBaseType &&
                    childType.BaseType != typeof(Object))
                {
                    // Use the local function to add without duplicates
                    AddWithoutDuplicates(childType.BaseType);
                }
            }
            index++;
        }
        while (index < classList.Count);

        // Now sort the list.
        classList.Sort((t1, t2) =>
        {
            return string.Compare(t1.Type.Name, t2.Type.Name);
        });

        // Start the display with System.Object.
        ClassAndSubclasses rootClass =
            new ClassAndSubclasses(typeof(Object));

        // Recursive method to build the hierarchy tree.
        AddChildrenToParent(rootClass, classList);

        // Create the StackLayout for displaying the list.
        stackLayout = new StackLayout
        {
            Spacing = 0
        };

        // Recursive method for adding items to StackLayout.
        AddItemToStackLayout(rootClass, 0);

        // Put the StackLayout in a ScrollView.
        this.Content = new ScrollView
        {
            Content = stackLayout,
            Padding = new Thickness(16,
                DeviceInfo.Platform == DevicePlatform.iOS ? 20 : 6, 0, 0)
        };
    }

    void AddChildrenToParent(ClassAndSubclasses parentClass,
                             List<TypeInformation> classList)
    {
        foreach (TypeInformation typeInformation in classList)
        {
            if (typeInformation.IsDerivedDirectlyFrom(parentClass.Type))
            {
                ClassAndSubclasses subClass =
                    new ClassAndSubclasses(typeInformation.Type);
                parentClass.Subclasses.Add(subClass);
                AddChildrenToParent(subClass, classList);
            }
        }
    }

    void AddItemToStackLayout(ClassAndSubclasses parentClass,
                              int level)
    {
        // Use GetDisplayName method for consistent naming
        string name = GetDisplayName(parentClass.Type);

        // Create Label and add to StackLayout
        Label label = new Label
        {
            Text = string.Format("{0}{1}", new string(' ', 4 * level),
                                            name),
            TextColor = parentClass.Type.GetTypeInfo().IsAbstract ?
                (Color)Application.Current.Resources["Primary"] : null
        };

        stackLayout.Children.Add(label);

        // Now display nested types.
        foreach (ClassAndSubclasses subclass in parentClass.Subclasses)
            AddItemToStackLayout(subclass, level + 1);
    }

    private string GetDisplayName(Type type)
    {
        string name = type.Name;
        TypeInfo typeInfo = type.GetTypeInfo();

        if (!string.IsNullOrEmpty(type.FullName)
            && typeInfo.Assembly != mauiAssembly)
        {
            name = type.FullName;
        }

        // If generic, display angle brackets and parameters.
        if (typeInfo.IsGenericType)
        {
            Type[] parameters = typeInfo.GenericTypeParameters;
            name = name.Substring(0, name.Length - 2);
            name += "<";

            for (int i = 0; i < parameters.Length; i++)
            {
                name += parameters[i].Name;
                if (i < parameters.Length - 1)
                    name += ", ";
            }
            name += ">";
        }

        return name;
    }
}
