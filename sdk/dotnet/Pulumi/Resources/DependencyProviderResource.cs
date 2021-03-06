// Copyright 2016-2020, Pulumi Corporation

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Pulumi.Serialization;

namespace Pulumi
{
    /// <summary>
    /// <see cref="DependencyProviderResource"/> is a <see cref="Resource"/> that is used by the provider SDK as a
    /// stand-in for a provider that is only used for its reference. Its only valid properties are its URN and ID.
    /// </summary>
    internal sealed class DependencyProviderResource : ProviderResource
    {
        public DependencyProviderResource(string reference)
            : base(package: GetPackage(reference), name: "", args: ResourceArgs.Empty, dependency: true)
        {
            var (urn, id) = ParseReference(reference);

            var resources = ImmutableHashSet.Create<Resource>(this);

            var urnData = OutputData.Create(resources, urn, isKnown: true, isSecret: false);
            this.Urn = new Output<string>(Task.FromResult(urnData));

            var idData = OutputData.Create(resources, id, isKnown: true, isSecret: false);
            this.Id = new Output<string>(Task.FromResult(idData));
        }

        private static string GetPackage(string reference)
        {
            var (urn, _) = ParseReference(reference);
            var urnParts = urn.Split("::");
            var qualifiedType = urnParts[2];
            var qualifiedTypeParts = qualifiedType.Split('$');
            var type = qualifiedTypeParts[^1];
            var typeParts = type.Split(':');
            // type will be "pulumi:providers:<package>" and we want the last part.
            return typeParts.Length > 2 ? typeParts[2] : "";
        }

        private static (string Urn, string Id) ParseReference(string reference)
        {
            var lastSep = reference.LastIndexOf("::", StringComparison.Ordinal);
            if (lastSep == -1)
            {
                throw new ArgumentException($"Expected \"::\" in provider reference ${reference}.");
            }
            var urn = reference[..lastSep];
            var id = reference[(lastSep + 2)..];
            return (urn, id);
        }
    }
}
