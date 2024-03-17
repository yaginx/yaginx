﻿namespace Scintillating.ProxyProtocol.Middleware;

internal static class MiddlewareConstants
{
    // HTTP2 Connection preface starts with the string "PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n"
    public static ReadOnlySpan<byte> PrefaceHTTP2 => new byte[] { 0x50, 0x52, 0x49, 0x20, 0x2a, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x32, 0x2e, 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x53, 0x4d, 0x0d, 0x0a, 0x0d, 0x0a };

    public const int PrefaceHTTP2Length = 24;
}