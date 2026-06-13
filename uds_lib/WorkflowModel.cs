using System.Collections.Generic;

namespace UdsLib
{
    public enum OnFailureAction { Abort, Continue, Retry }

    public sealed class WorkflowStep
    {
        public string                     Type      { get; set; }
        public string                     Name      { get; set; }
        public int                        TimeoutMs { get; set; }
        public OnFailureAction            OnFailure { get; set; }
        public Dictionary<string, string> Params    { get; set; }

        public WorkflowStep()
        {
            Type      = string.Empty;
            Name      = string.Empty;
            TimeoutMs = 3000;
            OnFailure = OnFailureAction.Abort;
            Params    = new Dictionary<string, string>();
        }
    }

    public sealed class FlashWorkflow
    {
        public string             Name        { get; set; }
        public string             EcuTarget   { get; set; }
        public string             Description { get; set; }
        public string             TxId        { get; set; }
        public string             RxId        { get; set; }
        public List<WorkflowStep> Steps       { get; set; }

        public FlashWorkflow()
        {
            Name        = string.Empty;
            EcuTarget   = string.Empty;
            Description = string.Empty;
            TxId        = "0x000";
            RxId        = "0x000";
            Steps       = new List<WorkflowStep>();
        }

        /// <summary>
        /// Standard PSA/Stellantis IVI head unit programming sequence.
        /// TxId/RxId are Marelli IVI defaults — change per vehicle variant.
        /// </summary>
        public static FlashWorkflow DefaultIviProgramming()
        {
            return new FlashWorkflow
            {
                Name        = "PSA_IVI_Standard_Programming",
                EcuTarget   = "IVI Head Unit",
                Description = "Standard UDS programming sequence for PSA/Stellantis IVI ECU (Marelli supplier).",
                TxId        = "0x6C1",
                RxId        = "0x641",
                Steps = new List<WorkflowStep>
                {
                    new WorkflowStep
                    {
                        Type = "DiagnosticSession", Name = "Enter Extended Session",
                        TimeoutMs = 3000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string> { { "mode", "03" } }
                    },
                    new WorkflowStep
                    {
                        Type = "SecurityAccess", Name = "Unlock – Extended Session",
                        TimeoutMs = 5000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string> { { "level", "01" } }
                    },
                    new WorkflowStep
                    {
                        Type = "RoutineControl", Name = "Check Programming Preconditions",
                        TimeoutMs = 5000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string>
                            { { "subFunc", "01" }, { "routineId", "0203" } }
                    },
                    new WorkflowStep
                    {
                        Type = "DiagnosticSession", Name = "Enter Programming Session",
                        TimeoutMs = 3000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string> { { "mode", "02" } }
                    },
                    new WorkflowStep
                    {
                        Type = "SecurityAccess", Name = "Unlock – Programming Session",
                        TimeoutMs = 5000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string> { { "level", "01" } }
                    },
                    new WorkflowStep
                    {
                        Type = "Flash", Name = "Flash Firmware",
                        TimeoutMs = 300000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string>
                        {
                            { "compression",      "00" },
                            { "blockSizeOverride", ""  }  // empty = use ECU-reported value
                        }
                    },
                    new WorkflowStep
                    {
                        Type = "RoutineControl", Name = "Check Programming Dependencies",
                        TimeoutMs = 10000, OnFailure = OnFailureAction.Abort,
                        Params = new Dictionary<string, string>
                            { { "subFunc", "01" }, { "routineId", "FF01" } }
                    },
                    new WorkflowStep
                    {
                        Type = "EcuReset", Name = "Hard Reset",
                        TimeoutMs = 5000, OnFailure = OnFailureAction.Continue,
                        Params = new Dictionary<string, string> { { "mode", "01" } }
                    },
                }
            };
        }
    }
}
