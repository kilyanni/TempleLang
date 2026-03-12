namespace TempleLang.Diagnostic
{
    using Properties;
    using System;

    public class DiagnosticFacts
    {
        public DiagnosticCode Code { get; }
        public string Description { get; }
        public Severity Severity { get; }

        public DiagnosticFacts(DiagnosticCode code)
        {
            Description = Resources.ResourceManager.GetString("DESCRIPTION_" + code.ToString());
            Enum.TryParse<Severity>(Resources.ResourceManager.GetString("SEVERITY_" + code.ToString()), out var severity);
            Severity = severity;
        }
    }
}