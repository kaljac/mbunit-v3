// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// Typical <see cref="IFormattingRule" /> priority values to use as guidelines.
    /// </summary>
    public static class FormattingRulePriority
    {
        /// <summary>
        /// The default formatting rule priority.  A rule with this priority will
        /// be overridden by all other matching rules.
        /// </summary>
        /// <value><see cref="int.MaxValue" /></value>
        public const int Default = int.MinValue;

        /// <summary>
        /// Typical rule priority.  This is a good starting point for rule priorities.
        /// </summary>
        /// <value>0</value>
        public const int Typical = 0;

        /// <summary>
        /// A better than typical rule.  This value should be used for cases where the
        /// typical rule is not quite precise enough.
        /// </summary>
        /// <value>10</value>
        public const int Better = 10;

        /// <summary>
        /// The best possible formatting rule priority.  A rule with this priority
        /// will override all other matching rules.
        /// </summary>
        /// <value><see cref="int.MaxValue" /></value>
        public const int Best = int.MaxValue;
    }
}