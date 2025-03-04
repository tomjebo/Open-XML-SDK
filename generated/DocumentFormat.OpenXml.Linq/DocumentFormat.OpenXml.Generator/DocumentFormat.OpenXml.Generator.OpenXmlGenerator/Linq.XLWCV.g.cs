﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.Linq;

namespace DocumentFormat.OpenXml.Linq
{
    /// <summary>
    /// Declares XNamespace and XName fields for the xmlns:xlwcv="http://schemas.microsoft.com/office/spreadsheetml/2024/workbookCompatibilityVersion" namespace.
    /// </summary>
    public static partial class XLWCV
    {
        /// <summary>
        /// Defines the XML namespace associated with the xlwcv prefix.
        /// </summary>
        public static readonly XNamespace xlwcv = "http://schemas.microsoft.com/office/spreadsheetml/2024/workbookCompatibilityVersion";

        /// <summary>
        /// Represents the xlwcv:version XML element.
        /// </summary>
        /// <remarks>
        /// <para>As an XML element, it:</para>
        /// <list type="bullet">
        /// <item><description>has the following parent XML elements: <see cref="X.ext" />.</description></item>
        /// <item><description>has the following XML attributes: <see cref="NoNamespace.setVersion" />, <see cref="NoNamespace.warnBelowVersion" />.</description></item>
        /// <item><description>corresponds to the following strongly-typed classes: Version.</description></item>
        /// </list>
        /// </remarks>
        public static readonly XName version = xlwcv + "version";
    }
}
