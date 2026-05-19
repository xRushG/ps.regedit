namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Remove, "RegTree", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [SupportedOSPlatform("windows")]
    public sealed class RemoveRegTreeCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path to delete (including all subkeys and values).")]
        public string Path { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            if (ShouldProcess($"{Hive}\\{Path}", "Delete entire registry tree and all subkeys"))
            {
                var registry = new WinRegistry();
                registry.DeleteTree(Hive, Path);
            }
        }
    }
}
