namespace UdsLib
{
    public enum UdsNrc : byte
    {
        GeneralReject                          = 0x10,
        ServiceNotSupported                    = 0x11,
        SubFunctionNotSupported                = 0x12,
        IncorrectMessageLengthOrFormat         = 0x13,
        ResponseTooLong                        = 0x14,
        BusyRepeatRequest                      = 0x21,
        ConditionsNotCorrect                   = 0x22,
        RequestSequenceError                   = 0x24,
        RequestOutOfRange                      = 0x31,
        SecurityAccessDenied                   = 0x33,
        InvalidKey                             = 0x35,
        ExceededNumberOfAttempts               = 0x36,
        RequiredTimeDelayNotExpired            = 0x37,
        UploadDownloadNotAccepted              = 0x70,
        TransferDataSuspended                  = 0x71,
        GeneralProgrammingFailure              = 0x72,
        WrongBlockSequenceCounter              = 0x73,
        ResponsePending                        = 0x78,
        SubFunctionNotSupportedInActiveSession = 0x7E,
        ServiceNotSupportedInActiveSession     = 0x7F,
    }

    public static class UdsNrcExtensions
    {
        public static string ToDescription(this UdsNrc nrc)
        {
            switch (nrc)
            {
                case UdsNrc.GeneralReject:                          return "General Reject";
                case UdsNrc.ServiceNotSupported:                    return "Service Not Supported";
                case UdsNrc.SubFunctionNotSupported:                return "Sub-Function Not Supported";
                case UdsNrc.IncorrectMessageLengthOrFormat:         return "Incorrect Message Length or Format";
                case UdsNrc.ResponseTooLong:                        return "Response Too Long";
                case UdsNrc.BusyRepeatRequest:                      return "Busy – Repeat Request";
                case UdsNrc.ConditionsNotCorrect:                   return "Conditions Not Correct";
                case UdsNrc.RequestSequenceError:                   return "Request Sequence Error";
                case UdsNrc.RequestOutOfRange:                      return "Request Out Of Range";
                case UdsNrc.SecurityAccessDenied:                   return "Security Access Denied";
                case UdsNrc.InvalidKey:                             return "Invalid Key";
                case UdsNrc.ExceededNumberOfAttempts:               return "Exceeded Number of Attempts";
                case UdsNrc.RequiredTimeDelayNotExpired:            return "Required Time Delay Not Expired";
                case UdsNrc.UploadDownloadNotAccepted:              return "Upload/Download Not Accepted";
                case UdsNrc.TransferDataSuspended:                  return "Transfer Data Suspended";
                case UdsNrc.GeneralProgrammingFailure:              return "General Programming Failure";
                case UdsNrc.WrongBlockSequenceCounter:              return "Wrong Block Sequence Counter";
                case UdsNrc.ResponsePending:                        return "Response Pending";
                case UdsNrc.SubFunctionNotSupportedInActiveSession: return "Sub-Function Not Supported in Active Session";
                case UdsNrc.ServiceNotSupportedInActiveSession:     return "Service Not Supported in Active Session";
                default:                                            return $"Unknown NRC 0x{(byte)nrc:X2}";
            }
        }
    }
}
