using System;

namespace UdsLib
{
    public static class UdsServiceCodec
    {
        // 0x10 DiagnosticSessionControl
        public static byte[] SessionControl(byte subFunction)
            => new byte[] { 0x10, subFunction };

        // 0x11 ECUReset  (01=hard, 02=keyOffOn, 03=soft)
        public static byte[] EcuReset(byte resetType = 0x01)
            => new byte[] { 0x11, resetType };

        // 0x27 SecurityAccess – request seed (odd level byte)
        public static byte[] SecurityAccessRequestSeed(byte levelOdd)
            => new byte[] { 0x27, levelOdd };

        // 0x27 SecurityAccess – send key (levelOdd+1 subfunction)
        public static byte[] SecurityAccessSendKey(byte levelOdd, byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            byte[] req = new byte[2 + key.Length];
            req[0] = 0x27;
            req[1] = (byte)(levelOdd + 1);
            Array.Copy(key, 0, req, 2, key.Length);
            return req;
        }

        // 0x31 RoutineControl
        public static byte[] RoutineControl(byte subFunc, ushort routineId, byte[]? options = null)
        {
            int optLen = options != null ? options.Length : 0;
            byte[] req = new byte[4 + optLen];
            req[0] = 0x31;
            req[1] = subFunc;
            req[2] = (byte)(routineId >> 8);
            req[3] = (byte)(routineId & 0xFF);
            if (options != null && options.Length > 0)
                Array.Copy(options, 0, req, 4, options.Length);
            return req;
        }

        // 0x34 RequestDownload  (4-byte address + 4-byte size, addrLenFmt = 0x44)
        public static byte[] RequestDownload(uint memoryAddress, uint memorySize, byte compression = 0x00)
        {
            return new byte[]
            {
                0x34,
                compression,                       // dataFormatIdentifier
                0x44,                              // addrLenFmt: 4B addr + 4B size
                (byte)(memoryAddress >> 24),
                (byte)(memoryAddress >> 16),
                (byte)(memoryAddress >>  8),
                (byte)(memoryAddress),
                (byte)(memorySize    >> 24),
                (byte)(memorySize    >> 16),
                (byte)(memorySize    >>  8),
                (byte)(memorySize),
            };
        }

        // 0x36 TransferData
        public static byte[] TransferData(byte blockSeqNum, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            byte[] req = new byte[2 + data.Length];
            req[0] = 0x36;
            req[1] = blockSeqNum;
            Array.Copy(data, 0, req, 2, data.Length);
            return req;
        }

        // 0x37 RequestTransferExit
        public static byte[] RequestTransferExit()
            => new byte[] { 0x37 };

        // 0x3E TesterPresent  (sub=0x00 → respond; sub=0x80 → suppress response)
        public static byte[] TesterPresent(bool suppressResponse = false)
            => new byte[] { 0x3E, suppressResponse ? (byte)0x80 : (byte)0x00 };

        // ── Response parsers ──────────────────────────────────────────────────

        // Extract seed bytes from 0x67 response
        public static byte[] ParseSeedFromResponse(byte[] response)
        {
            if (response == null || response.Length < 3)
                throw new ArgumentException("Response too short to contain seed.", nameof(response));
            if (response[0] != 0x67)
                throw new ArgumentException($"Expected 0x67 (SecurityAccess pos. response), got 0x{response[0]:X2}.");

            byte[] seed = new byte[response.Length - 2];
            Array.Copy(response, 2, seed, 0, seed.Length);
            return seed;
        }

        // Extract maxBlockLength from 0x74 response
        public static uint ParseMaxBlockLength(byte[] response)
        {
            if (response == null || response.Length < 3)
                throw new ArgumentException("Response too short.", nameof(response));
            if (response[0] != 0x74)
                throw new ArgumentException($"Expected 0x74 (RequestDownload pos. response), got 0x{response[0]:X2}.");

            int numBytes = (response[1] >> 4) & 0x0F;
            uint maxBlock = 0;
            for (int i = 0; i < numBytes && (2 + i) < response.Length; i++)
                maxBlock = (maxBlock << 8) | response[2 + i];
            return maxBlock;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        public static bool IsPositiveResponse(byte[] response, byte serviceId)
            => response != null && response.Length >= 1 && response[0] == (byte)(serviceId + 0x40);

        public static bool IsNegativeResponse(byte[] response)
            => response != null && response.Length >= 3 && response[0] == 0x7F;

        // NRC 0x78 = ResponsePending – caller must re-issue the same request
        public static bool IsResponsePending(byte[] response)
            => response != null && response.Length >= 3 && response[0] == 0x7F && response[2] == 0x78;

        public static string FormatHex(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;
            return BitConverter.ToString(data).Replace("-", " ");
        }
    }
}
