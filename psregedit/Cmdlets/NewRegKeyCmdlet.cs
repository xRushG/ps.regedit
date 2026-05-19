namespace PSRegedit
{
    [Cmdlet(VerbsCommon.New, "RegKey", SupportsShouldProcess = true)]
    [SupportedOSPlatform("windows")]
    public sealed class NewRegKeyCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path to create.")]
        public string Path { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            if (ShouldProcess($"{Hive}\\{Path}", "Create registry key"))
            {
                var registry = new WinRegistry();
                registry.CreateKey(Hive, Path);
            }
        }
    }
}
