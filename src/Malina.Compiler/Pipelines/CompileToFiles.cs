using Malina.Compiler.Steps;

namespace Malina.Compiler.Pipelines
{
    public class CompileToFiles: CompilerPipeline
    {
        public CompileToFiles()
        {
            Steps.Add(new ProcessAliasesAndNamespaces());
            Steps.Add(new ValidateDocuments());
            Steps.Add(new CompileDocumentsToFiles());
        }
    }
}
