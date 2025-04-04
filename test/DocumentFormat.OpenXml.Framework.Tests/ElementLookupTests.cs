﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Features;
using DocumentFormat.OpenXml.Framework.Metadata;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

using static DocumentFormat.OpenXml.Framework.Tests.TestUtility;

namespace DocumentFormat.OpenXml.Framework.Tests
{
    public class ElementLookupTests
    {
        private readonly ITestOutputHelper _output;

        public ElementLookupTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuiltInOpenXmlElements()
        {
            var elements = GetBuiltIn();
#if DEBUG
            _output.WriteObjectToTempFile("built in elements", elements);
#endif
            var expected = Deserialize<LookupData[]>("ElementChildren.json");
            var actual = GetBuiltIn().ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void VerifyTypedRootsCanBeCreatedWord()
        {
            // Arrange
            using var word = WordprocessingDocument.Create(new MemoryStream(), WordprocessingDocumentType.Document);
            using var excel = SpreadsheetDocument.Create(new MemoryStream(), SpreadsheetDocumentType.Workbook);
            using var ppt = PresentationDocument.Create(new MemoryStream(), PresentationDocumentType.Presentation);

            var feature = new CompositeRootElementFeature(word, excel, ppt);
            var allTypedParts = typeof(WordprocessingDocument).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(OpenXmlPartRootElement).IsAssignableFrom(t))
                .ToList();

            Assert.Equal(103, allTypedParts.Count);

            // Act/Assert
            foreach (var rootType in allTypedParts)
            {
                var root = ((OpenXmlElement)Activator.CreateInstance(rootType))!;
                Assert.True(feature.TryCreate(root.QName, out var created));
                Assert.IsType(rootType, created);
            }
        }

        [Fact]
        public void DumpBuiltInOpenXmlElements()
        {
            // This should align with the text in ElementChildren.json
            var expected = Serialize(GetBuiltIn());

            Assert.NotNull(expected);
        }

        private static readonly Type[] TypesFromFramework = new[]
        {
            typeof(OpenXmlPartRootElement),
            typeof(AlternateContent),
            typeof(AlternateContentChoice),
            typeof(AlternateContentFallback),
            typeof(OpenXmlMiscNode),
            typeof(OpenXmlUnknownElement),
        };

        private static IEnumerable<LookupData> GetBuiltIn()
        {
            return typeof(SpreadsheetDocument).GetTypeInfo().Assembly.GetTypes()
                .Where(t => !t.GetTypeInfo().IsAbstract && typeof(OpenXmlElement).GetTypeInfo().IsAssignableFrom(t))
                .Concat(TypesFromFramework)
                .Distinct()
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .Select(type => new LookupData(type));
        }

        private sealed class CompositeRootElementFeature : IRootElementFeature
        {
            private readonly IRootElementFeature[] _features;

            public CompositeRootElementFeature(params OpenXmlPackage[] package)
            {
                _features = package.Select(p => p.Features.GetRequired<IRootElementFeature>()).ToArray();
            }

            public bool TryCreate(in OpenXmlQualifiedName qname, [NotNullWhen(true)] out OpenXmlElement element)
            {
                foreach (var feature in _features)
                {
                    if (feature.TryCreate(qname, out element))
                    {
                        return true;
                    }
                }

                element = null;
                return false;
            }
        }

        private class LookupData : IEquatable<LookupData>
        {
            public LookupData()
            {
            }

            public LookupData(Type type)
            {
                Element = type.FullName;

                ElementFactoryCollection GetLookup()
                {
                    if (type.GetConstructor(Cached.Array<Type>()) is not null)
                    {
                        var instance = (OpenXmlElement)Activator.CreateInstance(type);
                        return instance.Metadata.Children;
                    }
                    else
                    {
                        return ElementFactoryCollection.Empty;
                    }
                }

                Children = GetLookup().Elements.Select(t => new ChildData
                {
                    Name = t.QName.Name,
                    Namespace = t.QName.Namespace.Uri,
                });
            }

            public string Element { get; set; }

            public IEnumerable<ChildData> Children { get; set; }

            public override bool Equals(object obj) => Equals(obj as LookupData);

            public bool Equals(LookupData other)
            {
                if (other is null)
                {
                    return false;
                }

                if (!string.Equals(Element, other.Element, StringComparison.Ordinal) || !Children.SequenceEqual(other.Children))
                {
                    System.Diagnostics.Debugger.Break();
                }

                return string.Equals(Element, other.Element, StringComparison.Ordinal)
                    && Children.SequenceEqual(other.Children);
            }

            public override int GetHashCode() => throw new NotImplementedException();

            public class ChildData : IEquatable<ChildData>
            {
                public string Name { get; set; }

                public string Namespace { get; set; }

                public bool Equals(ChildData other)
                {
                    return string.Equals(Name, other.Name, StringComparison.Ordinal)
                        && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal);
                }

                public override int GetHashCode() => throw new NotImplementedException();

                public override bool Equals(object obj) => Equals(obj as ChildData);
            }
        }
    }
}
