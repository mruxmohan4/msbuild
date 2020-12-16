﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Graph;
using Microsoft.Build.Shared;

namespace Microsoft.Build.Experimental.ProjectCache
{
    public class ProjectCacheDescriptor
    {
        /// <summary>
        ///     The path to the assembly containing the project cache plugin.
        /// </summary>
        public string? PluginAssemblyPath { get; }

        /// <summary>
        ///     The entry points with which the plugin will be initialized.
        /// </summary>
        public IReadOnlyCollection<ProjectGraphEntryPoint>? EntryPoints { get; }

        /// <summary>
        ///     The graph with which the plugin will be initialized.
        /// </summary>
        public ProjectGraph? ProjectGraph { get; }

        public IReadOnlyDictionary<string, string> PluginSettings { get; }

        public ProjectCacheBase? PluginInstance { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private ProjectCacheDescriptor()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        private ProjectCacheDescriptor(
            IReadOnlyCollection<ProjectGraphEntryPoint>? entryPoints,
            ProjectGraph? projectGraph,
            IReadOnlyDictionary<string, string>? pluginSettings)
        {
            ErrorUtilities.VerifyThrowArgument(
                (entryPoints == null) ^ (projectGraph == null),
                "EitherEntryPointsOrTheProjectGraphIsSet");

            EntryPoints = entryPoints;
            ProjectGraph = projectGraph;
            PluginSettings = pluginSettings ?? new Dictionary<string, string>();
        }

        private ProjectCacheDescriptor(
            string pluginAssemblyPath,
            IReadOnlyCollection<ProjectGraphEntryPoint>? entryPoints,
            ProjectGraph? projectGraph,
            IReadOnlyDictionary<string, string>? pluginSettings) : this(entryPoints, projectGraph, pluginSettings)
        {
            PluginAssemblyPath = pluginAssemblyPath;
        }

        private ProjectCacheDescriptor(
            ProjectCacheBase pluginInstance,
            IReadOnlyCollection<ProjectGraphEntryPoint>? entryPoints,
            ProjectGraph? projectGraph,
            IReadOnlyDictionary<string, string>? pluginSettings) : this(entryPoints, projectGraph, pluginSettings)
        {
            PluginInstance = pluginInstance;
        }

        public static ProjectCacheDescriptor FromAssemblyPath(
            string pluginAssemblyPath,
            IReadOnlyCollection<ProjectGraphEntryPoint>? entryPoints,
            ProjectGraph? projectGraph,
            IReadOnlyDictionary<string, string>? pluginSettings = null)
        {
            return new ProjectCacheDescriptor(pluginAssemblyPath, entryPoints, projectGraph, pluginSettings);
        }

        public static ProjectCacheDescriptor FromInstance(
            ProjectCacheBase pluginInstance,
            IReadOnlyCollection<ProjectGraphEntryPoint>? entryPoints,
            ProjectGraph? projectGraph,
            IReadOnlyDictionary<string, string>? pluginSettings = null)
        {
            return new ProjectCacheDescriptor(pluginInstance, entryPoints, projectGraph, pluginSettings);
        }

        public override string ToString()
        {
            var loadStyle = PluginInstance != null
                ? $"Instance based: {PluginInstance.GetType().AssemblyQualifiedName}"
                : $"Assembly path based: {PluginAssemblyPath}";

            var entryPointStyle = EntryPoints != null
                ? "Non static graph based"
                : "Static graph based";

            var entryPoints = EntryPoints != null
                ? string.Join(
                    "\n",
                    EntryPoints.Select(e => $"{e.ProjectFile} {{{FormatGlobalProperties(e.GlobalProperties)}}}"))
                : string.Join(
                    "\n",
                    ProjectGraph!.EntryPointNodes.Select(
                        n =>
                            $"{n.ProjectInstance.FullPath} {{{FormatGlobalProperties(n.ProjectInstance.GlobalProperties)}}}"));

            return $"{loadStyle}\nEntry-point style: {entryPointStyle}\nEntry-points:\n{entryPoints}";

            static string FormatGlobalProperties(IDictionary<string, string> globalProperties)
            {
                return string.Join(", ", globalProperties.Select(gp => $"{gp.Key}={gp.Value}"));
            }
        }
    }
}
