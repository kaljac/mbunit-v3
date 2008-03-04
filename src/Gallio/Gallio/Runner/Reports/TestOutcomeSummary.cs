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

using System;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes the number of test cases with a particular <see cref="TestOutcome"/>.
    /// </summary>
    /// <seealso cref="TestOutcome"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestOutcomeSummary
    {
        private TestOutcome outcome;
        private int count;

        /// <summary>
        /// Gets or sets the outcome.
        /// </summary>
        [XmlElement("outcome", IsNullable=false)]
        public TestOutcome Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases with the specified outcome.
        /// </summary>
        [XmlAttribute("count")]
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}