using System;

namespace UdsLib
{
    public sealed class UdsResponse
    {
        public bool   Success    { get; }
        public byte[] Raw        { get; }
        public string Error      { get; }

        public bool IsPositive => Success && Raw.Length >= 1 && Raw[0] != 0x7F && (Raw[0] & 0x40) != 0;
        public bool IsNegative => Success && Raw.Length >= 3 && Raw[0] == 0x7F;

        public byte    ServiceId  => IsPositive ? (byte)(Raw[0] & ~0x40) : (byte)0x00;
        public UdsNrc  Nrc        => IsNegative ? (UdsNrc)Raw[2] : (UdsNrc)0x00;
        public string  NrcText    => IsNegative ? Nrc.ToDescription() : string.Empty;

        // PSA TRPR byte in 0x36 response (index 2): 0x02 = OK, 0x0A = CRC Fault
        public bool HasTrprOk    => IsPositive && ServiceId == 0x36 && Raw.Length >= 3 && Raw[2] == 0x02;
        public bool HasTrprFault => IsPositive && ServiceId == 0x36 && Raw.Length >= 3 && Raw[2] == 0x0A;

        public static UdsResponse Parse(byte[] raw)
        {
            if (raw == null || raw.Length == 0)
                return new UdsResponse(false, new byte[0], "Empty or null response");
            return new UdsResponse(true, raw, string.Empty);
        }

        public static UdsResponse FromError(string error)
            => new UdsResponse(false, new byte[0], error ?? string.Empty);

        private UdsResponse(bool success, byte[] raw, string error)
        {
            Success = success;
            Raw     = raw;
            Error   = error;
        }

        public override string ToString()
        {
            if (!Success) return $"Error: {Error}";
            if (IsNegative) return $"NRC 0x{(byte)Nrc:X2} ({NrcText})";
            if (Raw.Length == 0) return "Empty";
            return BitConverter.ToString(Raw).Replace("-", " ");
        }
    }
}
