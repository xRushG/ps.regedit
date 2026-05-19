namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Get, "RegEntries")]
    [OutputType(typeof(WinRegistryEntry<string>))]
    [SupportedOSPlatform("windows")]
    public sealed class GetRegEntriesCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path.")]
        public string Path { get; set; } = string.Empty;

        [Parameter(HelpMessage = "Recursively retrieve entries from all subkeys.")]
        public SwitchParameter Recursive { get; set; }

        protected override void ProcessRecord()
        {
            var registry = new WinRegistry();

            var entries = Recursive.IsPresent
                ? registry.GetEntriesRecursive(Hive, Path)
                : registry.GetEntries(Hive, Path);

            WriteObject(entries, enumerateCollection: true);
        }
    }
}
