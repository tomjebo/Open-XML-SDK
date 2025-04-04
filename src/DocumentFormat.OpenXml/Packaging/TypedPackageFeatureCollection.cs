﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Features;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Packaging;

namespace DocumentFormat.OpenXml.Packaging;

/// <summary>
/// An implementation of <see cref="OpenXmlPackage"/> that provides strongly-typed services.
/// </summary>
internal abstract partial class TypedPackageFeatureCollection<TDocumentType, TMainPart> :
    PackageFeatureCollection,
    IMainPartFeature,
    IDocumentTypeFeature<TDocumentType>,
    IKnownDataPartFeature
    where TDocumentType : struct, Enum
    where TMainPart : OpenXmlPart
{
    private TDocumentType? _documentType;

    protected TypedPackageFeatureCollection(OpenXmlPackage package)
        : base(package)
    {
    }

    string IMainPartFeature.RelationshipType => RelationshipType;

    string IMainPartFeature.ContentType => GetContentType(EnsureDocumentType())!;

    [MemberNotNull(nameof(_documentType))]
    private TDocumentType EnsureDocumentType()
    {
        if (_documentType is { } existing)
        {
            return existing;
        }

        var package = this.GetRequired<IPackageFeature>().Package;

        foreach (var relationship in package.Relationships)
        {
            if (relationship.RelationshipType == RelationshipType)
            {
                var uriTarget = PackUriHelper.ResolvePartUri(OpenXmlPackage.Uri, relationship.TargetUri);
                var metroPart = package.GetPart(uriTarget);

                _documentType = GetDocumentType(metroPart.ContentType);
                break;
            }
        }

        // If document type hasn't been set by content, start with the TDocumentType default value
        if (!_documentType.HasValue)
        {
            _documentType = default(TDocumentType);
        }

        return _documentType.Value;
    }

    TDocumentType IDocumentTypeFeature<TDocumentType>.Current
    {
        get => EnsureDocumentType();
        set => _documentType = value;
    }

    protected TMainPart? MainPart => Package.GetSubPartOfType<TMainPart>();

    OpenXmlPart? IMainPartFeature.Part => MainPart;

    void IDocumentTypeFeature<TDocumentType>.ChangeDocumentType(TDocumentType newType)
    {
        if (newType.Equals(_documentType))
        {
            // same type, just return
            return;
        }

        if (Package.FileOpenAccess == FileAccess.Read)
        {
            throw new IOException(ExceptionMessages.PackageAccessModeIsReadonly);
        }

        var oldType = _documentType;
        _documentType = newType;

        if (MainPart is null)
        {
            return;
        }

        try
        {
            Package.ChangeDocumentTypeInternal(CreateMainPart());
        }
        catch (OpenXmlPackageException e)
        {
            if (e.Message == ExceptionMessages.CannotChangeDocumentType)
            {
                _documentType = oldType;
            }

            throw;
        }
    }

    protected abstract string RelationshipType { get; }

    public abstract string? GetContentType(TDocumentType type);

    public abstract TDocumentType? GetDocumentType(string contentPart);

    protected abstract TMainPart CreateMainPart();

    bool IKnownDataPartFeature.IsKnown(string relationshipId) => false;
}
