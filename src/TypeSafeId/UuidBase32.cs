using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace TypeSafeId;

// UUID Base32 encoding as defined in https://github.com/jetify-com/typeid/tree/main/spec#base32-encoding
// Copied from https://github.com/firenero/TypeId/blob/main/src/FastIDs.TypeId/TypeId.Core/Base32.cs
internal class UuidBase32
{
    public static int Encode(Guid uuid, Span<char> output)
    {
        if (output.Length < Constants.EncodedLength)
        {
            throw new ArgumentException(
                $"Output buffer must be at least {Constants.EncodedLength} chars long."
            );
        }

        Span<byte> bytes = stackalloc byte[Constants.DecodedLength];
        var success = uuid.TryWriteBytes(bytes, bigEndian: true, out _);
        Debug.Assert(success);

        return EncodeImpl(bytes, output, Constants.Alphabet);
    }

    private static int EncodeImpl<TChar>(
        ReadOnlySpan<byte> bytes,
        Span<TChar> output,
        ReadOnlySpan<TChar> alpha
    )
        where TChar : struct, IBinaryInteger<TChar>
    {
        // 10 byte timestamp
        output[0] = alpha[(bytes[0] & 224) >> 5];
        output[1] = alpha[bytes[0] & 31];
        output[2] = alpha[(bytes[1] & 248) >> 3];
        output[3] = alpha[((bytes[1] & 7) << 2) | ((bytes[2] & 192) >> 6)];
        output[4] = alpha[(bytes[2] & 62) >> 1];
        output[5] = alpha[((bytes[2] & 1) << 4) | ((bytes[3] & 240) >> 4)];
        output[6] = alpha[((bytes[3] & 15) << 1) | ((bytes[4] & 128) >> 7)];
        output[7] = alpha[(bytes[4] & 124) >> 2];
        output[8] = alpha[((bytes[4] & 3) << 3) | ((bytes[5] & 224) >> 5)];
        output[9] = alpha[bytes[5] & 31];

        // 16 bytes of entropy
        output[10] = alpha[(bytes[6] & 248) >> 3];
        output[11] = alpha[((bytes[6] & 7) << 2) | ((bytes[7] & 192) >> 6)];
        output[12] = alpha[(bytes[7] & 62) >> 1];
        output[13] = alpha[((bytes[7] & 1) << 4) | ((bytes[8] & 240) >> 4)];
        output[14] = alpha[((bytes[8] & 15) << 1) | ((bytes[9] & 128) >> 7)];
        output[15] = alpha[(bytes[9] & 124) >> 2];
        output[16] = alpha[((bytes[9] & 3) << 3) | ((bytes[10] & 224) >> 5)];
        output[17] = alpha[bytes[10] & 31];
        output[18] = alpha[(bytes[11] & 248) >> 3];
        output[19] = alpha[((bytes[11] & 7) << 2) | ((bytes[12] & 192) >> 6)];
        output[20] = alpha[(bytes[12] & 62) >> 1];
        output[21] = alpha[((bytes[12] & 1) << 4) | ((bytes[13] & 240) >> 4)];
        output[22] = alpha[((bytes[13] & 15) << 1) | ((bytes[14] & 128) >> 7)];
        output[23] = alpha[(bytes[14] & 124) >> 2];
        output[24] = alpha[((bytes[14] & 3) << 3) | ((bytes[15] & 224) >> 5)];
        output[25] = alpha[bytes[15] & 31];

        return Constants.EncodedLength;
    }

    public static bool TryDecode(ReadOnlySpan<char> input, out Guid uuid)
    {
        if (input.Length != Constants.EncodedLength)
        {
            uuid = default;
            return false;
        }

        Span<byte> inputBytes = stackalloc byte[Constants.EncodedLength];
        var writtenBytesCount = Encoding.UTF8.GetBytes(input, inputBytes);
        if (writtenBytesCount != Constants.EncodedLength)
        {
            uuid = default;
            return false;
        }

        if (!IsValidAlphabet(input))
        {
            uuid = default;
            return false;
        }

        Span<byte> buf = stackalloc byte[Constants.DecodedLength];
        var dec = Constants.DecodingTable;
        // 6 bytes timestamp (48 bits)
        buf[0] = (byte)((dec[inputBytes[0]] << 5) | dec[inputBytes[1]]);
        buf[1] = (byte)((dec[inputBytes[2]] << 3) | (dec[inputBytes[3]] >> 2));
        buf[2] = (byte)(
            (dec[inputBytes[3]] << 6) | (dec[inputBytes[4]] << 1) | (dec[inputBytes[5]] >> 4)
        );
        buf[3] = (byte)((dec[inputBytes[5]] << 4) | (dec[inputBytes[6]] >> 1));
        buf[4] = (byte)(
            (dec[inputBytes[6]] << 7) | (dec[inputBytes[7]] << 2) | (dec[inputBytes[8]] >> 3)
        );
        buf[5] = (byte)((dec[inputBytes[8]] << 5) | dec[inputBytes[9]]);

        // 10 bytes of entropy (80 bits)
        buf[6] = (byte)((dec[inputBytes[10]] << 3) | (dec[inputBytes[11]] >> 2)); // First 4 bits are the version
        buf[7] = (byte)(
            (dec[inputBytes[11]] << 6) | (dec[inputBytes[12]] << 1) | (dec[inputBytes[13]] >> 4)
        );
        buf[8] = (byte)((dec[inputBytes[13]] << 4) | (dec[inputBytes[14]] >> 1)); // First 2 bits are the variant
        buf[9] = (byte)(
            (dec[inputBytes[14]] << 7) | (dec[inputBytes[15]] << 2) | (dec[inputBytes[16]] >> 3)
        );
        buf[10] = (byte)((dec[inputBytes[16]] << 5) | dec[inputBytes[17]]);
        buf[11] = (byte)((dec[inputBytes[18]] << 3) | dec[inputBytes[19]] >> 2);
        buf[12] = (byte)(
            (dec[inputBytes[19]] << 6) | (dec[inputBytes[20]] << 1) | (dec[inputBytes[21]] >> 4)
        );
        buf[13] = (byte)((dec[inputBytes[21]] << 4) | (dec[inputBytes[22]] >> 1));
        buf[14] = (byte)(
            (dec[inputBytes[22]] << 7) | (dec[inputBytes[23]] << 2) | (dec[inputBytes[24]] >> 3)
        );
        buf[15] = (byte)((dec[inputBytes[24]] << 5) | dec[inputBytes[25]]);

        uuid = new(buf, bigEndian: true);
        return true;
    }

    public static bool IsValid(ReadOnlySpan<char> input)
    {
        if (input.Length != Constants.EncodedLength)
        {
            return false;
        }

        return IsValidAlphabet(input);
    }

    private static bool IsValidAlphabet(ReadOnlySpan<char> chars)
    {
        foreach (var c in chars)
        {
            if (!Constants.AlphabetValues.Contains(c))
            {
                return false;
            }
        }

        return true;
    }

    static class Constants
    {
        public const string Alphabet = "0123456789abcdefghjkmnpqrstvwxyz";
        public static ReadOnlySpan<byte> Utf8Alphabet => "0123456789abcdefghjkmnpqrstvwxyz"u8;
        public static readonly SearchValues<char> AlphabetValues = SearchValues.Create(Alphabet);
        public const int DecodedLength = 16;
        public const int EncodedLength = 26;

        // csharpier-ignore
        public static readonly byte[] DecodingTable = {
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x01,
        0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x0A, 0x0B, 0x0C,
        0x0D, 0x0E, 0x0F, 0x10, 0x11, 0xFF, 0x12, 0x13, 0xFF, 0x14,
        0x15, 0xFF, 0x16, 0x17, 0x18, 0x19, 0x1A, 0xFF, 0x1B, 0x1C,
        0x1D, 0x1E, 0x1F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
    };
    }
}
