namespace TempleLang.Diagnostic
{
    public interface IDiagnosticReceiver
    {
        void ReceiveDiagnostic(DiagnosticInfo diagnostic, bool isError);
    }
}