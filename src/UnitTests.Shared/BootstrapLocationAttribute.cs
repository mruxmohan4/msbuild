// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using NativeMethodsShared = Microsoft.Build.Framework.NativeMethods;

namespace Microsoft.Build.UnitTests.Shared;

[System.AttributeUsage(System.AttributeTargets.Assembly)]
internal sealed class BootstrapLocationAttribute(string bootstrapMsBuildBinaryLocation, string bootstrapSdkVersion) : System.Attribute
{
    public string BootstrapMsBuildBinaryLocation { get; } = bootstrapMsBuildBinaryLocation;

    public string BootstrapSdkVersion { get; } = bootstrapSdkVersion;
}
