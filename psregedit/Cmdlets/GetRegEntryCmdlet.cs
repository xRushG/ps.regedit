namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Get, "RegEntry")]
    [OutputType(typeof(WinRegistryEntry<string>))]
    [SupportedOSPlatform("windows")]
    public sealed class GetRegEntryCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path.")]
        public string Path { get; set; } = string.Empty;

        [Parameter(Mandatory = true, Position = 2, HelpMessage = "Value name.")]
        public string Name { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var registry = new WinRegistry();
            WriteObject(registry.GetEntry(Hive, Path, Name));
        }
    }
}
