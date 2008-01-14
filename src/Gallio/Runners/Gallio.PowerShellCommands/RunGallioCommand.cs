﻿// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Hosting;
using Gallio.Model;
using System.Management.Automation;
using Gallio.Model.Filters;
using Gallio.PowerShellCommands.Properties;
using Gallio.Runner;

namespace Gallio.PowerShellCommands
{
    /// <summary>
    /// A PowerShell Cmdlet for running Gallio.
    /// </summary>
    /// <remarks>
    /// Only the Assemblies parameter is required.
    /// </remarks>
    /// <example>
    /// <para>There are severals ways to run this cmdlet:</para>
    /// <code>
    /// # Makes the Gallio commands available
    /// Add-PSSnapIn Gallio
    /// # Runs TestAssembly1.dll
    /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll" -f Category:UnitTests -rd C:\build\reports -rf html
    /// </code>
    /// </example>
    [Cmdlet("Run", "Gallio")]
    public class RunGallioCommand : BaseCommand
    {
        #region Private Members

        private string[] assemblies;
        private string[] pluginDirectories;
        private string[] hintDirectories;
        private string[] reportTypes = new string[] { };
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string filter = "*";
        private SwitchParameter showReports;
        private SwitchParameter doNotRun;
        private SwitchParameter noEchoResults;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of relative or absolute paths of test assembly files to execute.
        /// This is required.
        /// </summary>
        /// <example>
        /// <para>There are severals ways to pass the test assemblies names to the
        /// cmdlet:</para>
        /// <code>
        /// # Runs TestAssembly1.dll
        /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll"
        /// 
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll","[Path-to-assembly2]\TestAssembly2.dll"
        /// 
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// $assemblies = "[Path-to-assembly1]\TestAssembly1.dll","[Path-to-assembly2]\TestAssembly2.dll"
        /// Run-Gallio $assemblies
        /// 
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// $assembly1 = "[Path-to-assembly1]\TestAssembly1.dll"
        /// $assembly2 = "[Path-to-assembly2]\TestAssembly2.dll"
        /// $assemblies = $assembly1,$assembly2
        /// Run-Gallio $assemblies
        /// 
        /// # If you don't specify the test assemblies, PowerShell will prompt you for the names:
        /// PS C:\Documents and Settings\jhi> Run-Gallio
        ///
        /// cmdlet Run-Gallio at command pipeline position
        /// Supply values for the following parameters:
        /// Assemblies[0]:
        /// </code>
        /// </example>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true
            )]
        [ValidateNotNullOrEmpty]
        [ValidateCount(1, 99999)]
        public string[] Assemblies
        {
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the hint directories:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -hd C:\SomeFolder
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("hd")]
        public string[] HintDirectories
        {
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the plugin directories:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -pd C:\SomeFolder
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("pd")]
        public string[] PluginDirectories
        {
            set { pluginDirectories = value; }
        }

        /// <summary>
        /// A list of the types of reports to generate, separated by semicolons. 
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: Html, Html-Inline, Text, XHtml,
        /// XHtml-Inline, Xml, and Xml-Inline, but more types could be available as plugins.</item>
        /// <item>The report types are not case sensitives.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <para>In the following example reports will be generated in both HTML and XML format:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -rt "html","text"
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rt", "report-types")]
        public string[] ReportTypes
        {
            set { reportTypes = value; }
        }

        /// <summary>
        /// Sets the format string to use to generate the reports filenames.
        /// </summary>
        /// <remarks>
        /// Any occurence of {0} will be replaced by the date, and any occurrence of {1} by the time.
        /// The default format string is test-report-{0}-{1}.
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rnf", "report-name-format")]
        public string ReportNameFormat
        {
            set { reportNameFormat = value; }
        }

        /// <summary>
        /// Sets the name of the directory where the reports will be put.
        /// </summary>
        /// <remarks>
        /// The directory will be created if it doesn't exist. Existing files will be overwrited.
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rd", "report-directory")]
        public string ReportDirectory
        {
            set { reportDirectory = value; }
        }

        /// <summary>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/summary/*' />
        /// </summary>
        /// <remarks>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/remarks/*' />
        /// </remarks>
        /// <example>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/example/*' />
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("f")]
        public string Filter
        {
            set { filter = value; }
        }

        /// <summary>
        /// Sets whether to open the generated reports once execution has finished.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>This parameter takes the value true if present and false if not. No
        /// value has to be specified.</item>
        /// <item>
        /// The reports are opened in a window using the default system application
        /// registered to the report file type.
        /// </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// # Doesn't show the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll
        /// # Shows the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll -sr
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("sr", "show-reports")]
        public SwitchParameter ShowReports
        {
            set { showReports = value; }
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("dnr", "do-not-run")]
        public SwitchParameter DoNotRun
        {
            set { doNotRun = value; }
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  If this option is specified
        /// only the final summary statistics are displayed.  Otherwise test results are echoed to the
        /// console in varying detail depending on the current verbosity level.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("ne", "no-echo-results")]
        public SwitchParameter NoEchoResults
        {
            set { noEchoResults = value; }
        }

        #endregion

        #region Protected Methods

        /// <exclude />
        protected override void EndProcessing()
        {
            try
            {
                WriteObject(ExecuteWithMessagePump());
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, Resources.UnexpectedErrorDuringExecution, ErrorCategory.NotSpecified, null));
            }
        }

        #endregion

        #region Private Methods

        /// <exclude />
        protected TestLauncherResult ExecuteWithMessagePump()
        {
            TestLauncherResult result = null;

            RunWithMessagePump(delegate
            {
                result = ExecuteWithCurrentDirectory();
            });

            if (result == null)
                throw new InvalidOperationException("The task finished without returning a result.");

            return result;
        }

        /// <exclude />
        protected TestLauncherResult ExecuteWithCurrentDirectory()
        {
            string oldDirectory = Environment.CurrentDirectory;
            try
            {
                if (SessionState != null)
                {
                    // FIXME: Will this throw an exception if the current path is
                    //        within a virtual file system?
                    string resolvedDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
                    Environment.CurrentDirectory = resolvedDirectory;
                }

                return Execute();
            }
            finally
            {
                Environment.CurrentDirectory = oldDirectory;
            }
        }

        /// <exclude />
        protected TestLauncherResult Execute()
        {
            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = Logger;
                launcher.ProgressMonitorProvider = ProgressMonitorProvider;
                launcher.Filter = GetFilter();
                launcher.RuntimeSetup = new RuntimeSetup();
                launcher.ShowReports = showReports.IsPresent;
                launcher.DoNotRun = doNotRun.IsPresent;
                launcher.EchoResults = !noEchoResults.IsPresent;

                AddAllItemSpecs(launcher.TestPackageConfig.AssemblyFiles, assemblies);
                AddAllItemSpecs(launcher.TestPackageConfig.HintDirectories, hintDirectories);
                AddAllItemSpecs(launcher.RuntimeSetup.PluginDirectories, pluginDirectories);

                if (reportDirectory != null)
                    launcher.ReportDirectory = reportDirectory;
                if (!String.IsNullOrEmpty(reportNameFormat))
                    launcher.ReportNameFormat = reportNameFormat;
                if (reportTypes != null)
                    GenericUtils.AddAll(reportTypes, launcher.ReportFormats);

                TestLauncherResult result = RunLauncher(launcher);
                return result;
            }
        }

        /// <exclude />
        /// <summary>
        /// Provided so that the unit tests can override test execution behavior.
        /// </summary>
        protected virtual TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            return launcher.Run();
        }

        private Filter<ITest> GetFilter()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return new AnyFilter<ITest>();
            }

            return FilterUtils.ParseTestFilter(filter);
        }

        private static void AddAllItemSpecs(ICollection<string> collection, IEnumerable<string> items)
        {
            if (items != null)
            {
                foreach (string item in items)
                    collection.Add(item);
            }
        }

        #endregion
    }
}
