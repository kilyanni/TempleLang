namespace TempleLang.Diagnostic
{
    public static class DiagnosticReceiver
    {
        public static void ReceiveDiagnostic(this IDiagnosticReceiver receiver, DiagnosticCode code, FileLocation? location, bool isError) =>
            receiver.ReceiveDiagnostic(new DiagnosticInfo(code, location), isError);
    }
}