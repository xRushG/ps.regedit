namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Get, "RegSubKeyNames")]
    [OutputType(typeof(string))]
    [SupportedOSPlatform("windows")]
    public sealed class GetRegSubKeyNamesCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path (e.g. SOFTWARE\\Microsoft\\Windows).")]
        public string Path { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var registry = new WinRegistry();
            WriteObject(registry.GetSubKeyNames(Hive, Path), enumerateCollection: true);
        }
    }
}
