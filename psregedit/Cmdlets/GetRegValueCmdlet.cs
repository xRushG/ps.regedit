namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Get, "RegValue")]
    [OutputType(typeof(string))]
    [SupportedOSPlatform("windows")]
    public sealed class GetRegValueCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path.")]
        public string Path { get; set; } = string.Empty;

        [Parameter(Position = 2, HelpMessage = "Value name. Omit or leave empty to read the default value.")]
        public string Name { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var registry = new WinRegistry();
            WriteObject(registry.GetValue(Hive, Path, Name));
        }
    }
}
