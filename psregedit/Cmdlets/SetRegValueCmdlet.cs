namespace PSRegedit
{
    [Cmdlet(VerbsCommon.Set, "RegValue", SupportsShouldProcess = true)]
    [SupportedOSPlatform("windows")]
    public sealed class SetRegValueCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Registry hive (e.g. LocalMachine, CurrentUser).")]
        public RegistryHive Hive { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Registry key path.")]
        public string Path { get; set; } = string.Empty;

        [Parameter(Position = 2, HelpMessage = "Value name. Omit or leave empty to set the default value.")]
        public string Name { get; set; } = string.Empty;

        [Parameter(Mandatory = true, Position = 3, HelpMessage = "The value to write.")]
        public object Value { get; set; } = new();

        [Parameter(Mandatory = true, Position = 4, HelpMessage = "Registry value type (e.g. String, DWord, QWord, Binary, MultiString, ExpandString).")]
        public RegistryValueKind ValueKind { get; set; }

        protected override void ProcessRecord()
        {
            string target = $"{Hive}\\{Path}\\{(string.IsNullOrEmpty(Name) ? "(Default)" : Name)}";

            if (ShouldProcess(target, "Set registry value"))
            {
                var registry = new WinRegistry();
                registry.SetValue(Hive, Path, Name, Value, ValueKind);
            }
        }
    }
}
