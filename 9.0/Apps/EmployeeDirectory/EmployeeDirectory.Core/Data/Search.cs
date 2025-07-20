//
//  Copyright 2012, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
using EmployeeDirectory.Core.Services;
using System.Xml.Serialization;

namespace EmployeeDirectory.Core.Data;

/// <summary>
/// Represents a saved search: a filter and the latest results.
/// </summary>
public class Search
{
    public string Name { get; set; }

    public string Text { get; set; }

    public SearchProperty Property { get; set; }

    public Collection<Person> Results { get; set; }

    public Search()
        : this("")
    {
    }

    public Search(string name)
    {
        Name = name;
        Text = "";
        Property = SearchProperty.Name;
        Results = new Collection<Person>();
    }

    public Filter Filter
    {
        get
        {
            var trimmed = Text.Trim();
            if (Property == SearchProperty.All)
            {
                return new OrFilter(
                    new ContainsFilter("Name", trimmed),
                    new ContainsFilter("Title", trimmed),
                    new ContainsFilter("Department", trimmed));
            }
            else
            {
                var propName = Property.ToString();
                return new ContainsFilter(propName, trimmed);
            }
        }
    }

    public static Search Open(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be given.", "name");

        var filePath = Path.Combine(FileSystem.AppDataDirectory, name);
        var serializer = new XmlSerializer(typeof(Search));

        using (var stream = File.OpenRead(filePath))
        {
            var s = serializer.Deserialize(stream) as Search ?? throw new InvalidOperationException("Deserialization resulted in a null Search object.");
            s.Name = name;
            return s;
        }
    }

    public async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Name must be set.");

        var filePath = Path.Combine(FileSystem.AppDataDirectory, Name);
        var serializer = new XmlSerializer(typeof(Search));

        using (var stream = File.Create(filePath))
        {
            serializer.Serialize(stream, this);
        }
    }
}
