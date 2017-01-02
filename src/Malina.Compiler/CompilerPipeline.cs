using System;
using System.Collections.Generic;

namespace Malina.Compiler
{
    public class CompilerPipeline
    {
        public List<ICompilerStep> Steps { get; private set; } = new List<ICompilerStep>();

        public virtual void Run(CompilerContext context)
        {
            foreach (ICompilerStep step in Steps)
            {
                step.Initialize(context);
                step.Run();
            }
        }

        public CompilerPipeline Add(ICompilerStep step)
        {
            Steps.Add(step);
            return this;
        }
    }
}