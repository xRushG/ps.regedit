namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Remove, "RegValue", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [SupportedOSPlatform("windows")]
    public sealed class RemoveRegValueCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path.")]
        public string Path { get; set; } = string.Empty;

        [Parameter(Mandatory = true, Position = 2, HelpMessage = "Value name to delete.")]
        public string Name { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            if (ShouldProcess($"{Hive}\\{Path}\\{Name}", "Delete registry value"))
            {
                var registry = new WinRegistry();
                registry.DeleteRegistryValue(Hive, Path, Name);
            }
        }
    }
}
