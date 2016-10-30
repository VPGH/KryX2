/*
MBNCSUtil -- Managed Battle.net Authentication Library
Copyright (C) 2005-2008 by Robert Paveza

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met: 

1.) Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. 
2.) Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution. 
3.) The name of the author may not be used to endorse or promote products derived 
from this software without specific prior written permission. 
	
See LICENSE.TXT that should have accompanied this software for full terms and 
conditions.

*/


using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;

namespace BNSharp.BattleNet.Core
{
    /// <summary>
    /// Encompasses any revision check functionality for all Battle.net games.
    /// This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class provides all CheckRevision-related support, 
    /// including file checksumming and EXE version information.
    /// </remarks>
    /// <threadsafety>This type is safe for multithreaded operations.</threadsafety>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static class CheckRevision
    {
        /** These are the hashcodes for the various .mpq files. */
        //private static readonly uint[] hashcodes =
        //    new uint[] 
        //        { 
        //            0xE7F4CB62, 
        //            0xF6A14FFC, 
        //            0xAA5504AF, 
        //            0x871FCDC2, 
        //            0x11BF6A18, 
        //            0xC57292E6, 
        //            0x7927D27E, 
        //            0x2FEC8733 
        //        };

        ///// <summary>
        ///// Extracts the MPQ number from the MPQ specified by the Battle.net server.
        ///// </summary>
        ///// <remarks>
        ///// <para></para>
        ///// <para>For older CheckRevision calls, the MPQ number is a required parameter of the CheckRevision function.  Note that the MPQ number is simply the number represented
        ///// in string format in the 8th position (index 7) of the string -- for example, in "IX86ver<b>1</b>.mpq", 1 is the version number.</para>
        ///// </remarks>
        ///// <param name="mpqName">The name of the MPQ file specified in the SID_AUTH_INFO message.</param>
        ///// <returns>The number from 0 to 7 specifying the number in the MPQ file.</returns>
        ///// <exception cref="ArgumentException">Thrown if the name of the MPQ version file is less than 8 characters long.</exception>
        ///// <exception cref="ArgumentNullException">Thrown if the <i>mpqName</i> parameter is <b>null</b> (<b>Nothing</b> in Visual Basic).
        ///// </exception>
        ///// <exception cref="NotSupportedException">Thrown if the <i>mpqName</i> parameter indicates a Lockdown DLL.</exception>
        //public static int ExtractMPQNumber(string mpqName)
        //{
        //    if (mpqName == null)
        //        throw new ArgumentNullException("mpqName");

        //    if (mpqName.ToUpperInvariant().StartsWith("LOCKDOWN", StringComparison.Ordinal))
        //        throw new NotSupportedException("Lockdown MPQs are not supported for MPQ number extraction.");

        //    if (mpqName.Length < 7)
        //        throw new ArgumentException("The MPQ name was too short.");

        //    string mpqNameUpper = mpqName.ToUpperInvariant();
        //    int num = -1;

        //    // ver-IX86-X.mpq
        //    if (mpqNameUpper.StartsWith("VER", StringComparison.Ordinal))
        //    {
        //        num = int.Parse(mpqName[9].ToString(), CultureInfo.InvariantCulture);
        //    }
        //    else  // IX86VerX.mpq
        //    {
        //        num = int.Parse(mpqName[7].ToString(), CultureInfo.InvariantCulture);
        //    }

        //    return num;
        //}

        ///// <summary>
        ///// Calculates the revision check for the specified files.
        ///// </summary>
        ///// <param name="valueString">The value string for the check revision function specified by Battle.net's SID_AUTH_INFO message.</param>
        ///// <param name="files">The list of files for the given game client.  This parameter must be exactly three files long.</param>
        ///// <param name="mpqNumber">The number of the MPQ file.  To extract this number, see the 
        ///// <see cref="ExtractMPQNumber(String)">ExtractMPQNumber</see> method.</param>
        ///// <returns>The checksum value.</returns>
        ///// <exception cref="ArgumentNullException">Thrown if the <i>valueString</i> or <i>files</i> parameters are <b>null</b>
        ///// (<b>Nothing</b> in Visual Basic).</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="files" /> is not a 3-string array, or if  
        ///// <paramref name="mpqNumber" /> is outside of the range of 0 to 7, inclusive.</exception>
        ///// <exception cref="FileNotFoundException">Thrown if one of the specified game files is not found.</exception>
        ///// <exception cref="IOException">Thrown in the event of a general I/O error.</exception>
        ///// <remarks>
        ///// <para>The file list for this is product-specific and order-specific:</para>
        ///// <list type="table">
        /////		<listheader>
        /////			<term>Product</term>
        /////			<description>File list</description>
        /////		</listheader>
        /////		<item>
        /////			<term>Starcraft; Starcraft: Brood War</term>
        /////			<description>
        /////				<list type="bullet">
        /////					<item>
        /////						<description>Starcraft.exe</description>
        /////					</item>
        /////					<item>
        /////						<description>storm.dll</description>
        /////					</item>
        /////					<item>
        /////						<description>battle.snp</description>
        /////					</item>
        /////				</list>
        /////			</description>
        /////		</item>
        /////		<item>
        /////			<term>Warcraft II: Battle.net Edition</term>
        /////			<description>
        /////				<list type="bullet">
        /////					<item>
        /////						<description>Warcraft II BNE.exe</description>
        /////					</item>
        /////					<item>
        /////						<description>storm.dll</description>
        /////					</item>
        /////					<item>
        /////						<description>battle.snp</description>
        /////					</item>
        /////				</list>
        /////			</description>
        /////		</item>
        /////		<item>
        /////			<term>Diablo II; Diablo II: Lord of Destruction</term>
        /////			<description>
        /////				<list type="bullet">
        /////					<item>
        /////						<description>Game.exe</description>
        /////					</item>
        /////					<item>
        /////						<description>Bnclient.dll</description>
        /////					</item>
        /////					<item>
        /////						<description>D2Client.dll</description>
        /////					</item>
        /////				</list>
        /////			</description>
        /////		</item>
        /////		<item>
        /////			<term>Warcraft III: The Reign of Chaos; Warcraft III: The Frozen Throne</term>
        /////			<description>
        /////				<list type="bullet">
        /////					<item>
        /////						<description>War3.exe</description>
        /////					</item>
        /////					<item>
        /////						<description>storm.dll</description>
        /////					</item>
        /////					<item>
        /////						<description>Game.dll</description>
        /////					</item>
        /////				</list>
        /////			</description>
        /////		</item>
        ///// </list>
        ///// </remarks>
        //public static int DoCheckRevision(
        //    string valueString,
        //    IEnumerable<Stream> files,
        //    int mpqNumber)
        //{
        //    if (valueString == null)
        //        throw new ArgumentNullException("valueString");
        //    if (files == null)
        //        throw new ArgumentNullException("files");
        //    if (files.Count() != 3)
        //        throw new ArgumentOutOfRangeException("files", files, "The file list is invalid.");
        //    if (mpqNumber < 0 || mpqNumber > 7)
        //        throw new ArgumentOutOfRangeException("mpqNumber", mpqNumber, "MPQ number must be between 0 and 7, inclusive.");

        //    using (Stream crStream = new CheckRevisionStream(files))
        //    {
        //        uint A, B, C;
        //        List<string> formulas = new List<string>();

        //        CheckRevisionFormulaTracker.InitializeValues(valueString, formulas, out A, out B, out C);

        //        A ^= hashcodes[mpqNumber];

        //        int result = DoCheckRevisionCompiled(A, B, C, formulas, crStream);
        //        return result;
        //    }
        //}

        // This method assumes all parameters have been sanity checked
        //private static int DoCheckRevisionCompiled(
        //    uint A,
        //    uint B,
        //    uint C,
        //    IEnumerable<string> crevFormulas,
        //    Stream completeDataStream)
        //{
        //    StandardCheckRevisionImplementation impl = CheckRevisionFormulaTracker.GetImplementation(crevFormulas);

        //    using (BinaryReader br = new BinaryReader(completeDataStream))
        //    {
        //        uint S;
        //        while (br.BaseStream.Position < br.BaseStream.Length)
        //        {
        //            S = br.ReadUInt32();
        //            impl(ref A, ref B, ref C, ref S);
        //        }
        //    }
        //    return unchecked((int)C);
        //}

        /// <summary>
        /// Performs the Lockdown revision check.
        /// </summary>
        /// <param name="valueString">The value string parameter, not including the null terminator.</param>
        /// <param name="gameFiles">The three game files.  This parameter must be exactly three files long.</param>
        /// <param name="lockdownFile">The path to the lockdown file requested.</param>
        /// <param name="imageFile">The path to the screen dump.</param>
        /// <param name="version">[return value] The EXE version.</param>
        /// <param name="version">[return value] The EXE version.</param>
        /// <param name="checksum">[return value] The EXE hash.</param>
        /// <returns>The "EXE Information" data.  This value should be null-terminated when being inserted into the authorization packet.</returns>
        /// <remarks>
        /// <para>The file list for this is product-specific and order-specific:</para>
        /// <list type="table">
        ///		<listheader>
        ///			<term>Product</term>
        ///			<description>File list</description>
        ///		</listheader>
        ///		<item>
        ///			<term>Starcraft; Starcraft: Brood War</term>
        ///			<description>
        ///				<list type="bullet">
        ///					<item>
        ///						<description>Starcraft.exe</description>
        ///					</item>
        ///					<item>
        ///						<description>storm.dll</description>
        ///					</item>
        ///					<item>
        ///						<description>battle.snp</description>
        ///					</item>
        ///				</list>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>Warcraft II: Battle.net Edition</term>
        ///			<description>
        ///				<list type="bullet">
        ///					<item>
        ///						<description>Warcraft II BNE.exe</description>
        ///					</item>
        ///					<item>
        ///						<description>storm.dll</description>
        ///					</item>
        ///					<item>
        ///						<description>battle.snp</description>
        ///					</item>
        ///				</list>
        ///			</description>
        ///		</item>
        /// </list>
        /// </remarks>
        public static byte[] DoLockdownCheckRevision(
            byte[] valueString,
            string[] gameFiles,
            string lockdownPath,
            string lockdownFile,
            string imageFile,
            ref int version,
            ref int checksum)
        {
            byte[] digest;
            LockdownCrev.CheckRevision(gameFiles[0], gameFiles[1], gameFiles[2], valueString, ref version, ref checksum, out digest, lockdownPath, lockdownFile, imageFile);

            return digest;
        }

        //private static int getNum(char c)
        //{
        //    c = char.ToUpper(c, CultureInfo.InvariantCulture);
        //    if (c == 'S')
        //        return 3;
        //    else
        //        return c - 'A';
        //}

        ///// <summary>
        ///// Gets EXE information for the specified file.
        ///// </summary>
        ///// <param name="fileName">The name of the file.</param>
        ///// <param name="exeInfoString">Returns the file's timestamp and other information.</param>
        ///// <returns>The file's version.</returns>
        ///// <exception cref="ArgumentNullException">Thrown if the <i>fileName</i> parameter is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        ///// <exception cref="FileNotFoundException">Thrown if the file specified by <i>fileName</i> does not exist in the specified path.</exception>
        public static int GetExeInfo(
            string fileName,
            out string exeInfoString)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            string file = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            uint fileSize = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileSize = unchecked((uint)fs.Length);
            }

            DateTime ft = File.GetLastWriteTimeUtc(fileName);

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(fileName);
            /* // updated version
            int version = ((fvi.FileMajorPart & 0x00ff0000) << 8) |
                ((fvi.FileMajorPart & 0x000000ff) << 16) |
                ((fvi.FileMinorPart & 0x00ff0000) >> 8) |
                (fvi.FileMinorPart & 0x000000ff);
            */
            int version = ((fvi.FileMajorPart << 24) |
                (fvi.FileMinorPart << 16) |
                (fvi.FileBuildPart << 8) | fvi.FilePrivatePart);

            exeInfoString = String.Format(CultureInfo.InvariantCulture,
                "{0} {1:MM/dd/yy hh:mm:ss} {2}",
                file, ft.Month, ft.Day, ft.Year % 100, ft.Hour, ft.Minute, ft.Second, fileSize
                );

            return version;
        }
    }
}

