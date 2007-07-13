// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using System.Threading;
using MbUnit.Framework.Model;
using MbUnit.Framework;
using MbUnit.Framework.Services.Contexts;

namespace MbUnit.Framework.Services.Contexts
{
    /// <summary>
    /// The context manager is responsible for creating and managing context
    /// objects during test execution.  It also encapsulates the algorithm
    /// for associating a context with an execution thread.  A context may
    /// be implicitly associated with a thread by virtue of control flow
    /// (a new created thread may inherit the context of its parent) or
    /// it may be explicitly associated by direct assignment.
    /// </summary>
    /// <remarks>
    /// All context manager operations are thread safe.
    /// </remarks>
    public interface IContextManager
    {
        /// <summary>
        /// Gets the context of the current thread.
        /// </summary>
        IContext CurrentContext { get; }

        /// <summary>
        /// Gets the root context for the system.
        /// </summary>
        IContext RootContext { get; }

        /// <summary>
        /// Creates a context for a test.
        /// </summary>
        /// <param name="parent">The parent context</param>
        /// <param name="test">The test that will run within the context</param>
        /// <returns>The context</returns>
        IContext CreateContext(IContext parent, ITest test);

        /// <summary>
        /// Sets the context associated with the specified thread overriding
        /// the context that it might normally inherit from its environment.
        /// Context information becomes accessible to code running on that thread
        /// so that assertion failures and report messages generated by the
        /// thread are captured by the context and included in the output.
        /// If the context is null, resets the context of the thread to just that it
        /// would otherwise have acquired.
        /// </summary>
        /// <param name="thread">The thread whose context is to be set</param>
        /// <param name="context">The context to associate with the thread, or null to reset to the default</param>
        void SetThreadContext(Thread thread, IContext context);

        /// <summary>
        /// Runs a block of code within the specified context.
        /// The thread's current context is switched to that which is specified
        /// for the duration of the block then restored to what it was before.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="block">The block to run</param>
        void RunWithContext(IContext context, Block block);
    }
}