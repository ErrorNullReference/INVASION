using System;
/// <summary>
/// Flag that determines several types of data format
/// </summary>
[Flags]
public enum AudioDataTypeFlag : byte
{
    /// <summary>
    /// No flag set
    /// </summary>
    None = 0,
    /// <summary>
    /// Uses data stored in Int16 format
    /// </summary>
    Int16 = 1,
    /// <summary>
    /// Uses data stored in single precision point format
    /// </summary>
    Single = 2,
    /// <summary>
    /// Uses both Int16 and Single formats
    /// </summary>
    Both = 3,
}