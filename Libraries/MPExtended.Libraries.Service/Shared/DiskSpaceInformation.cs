﻿#region Copyright (C) 2012-2013 MPExtended
// Copyright (C) 2012-2013 MPExtended Developers, http://www.mpextended.com/
// 
// MPExtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MPExtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MPExtended. If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MPExtended.Services.Common.Interfaces;

namespace MPExtended.Libraries.Service.Shared
{
    public static class DiskSpaceInformation
    {
        [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out ulong lpFreeBytesAvailable,
           out ulong lpTotalNumberOfBytes,
           out ulong lpTotalNumberOfFreeBytes);

        public static WebDiskSpaceInformation GetSpaceInformation(string directory)
        {
            ulong freeBytes, totalBytes, freeBytesAvailable;
            directory = Path.GetPathRoot(directory);
            if (!GetDiskFreeSpaceEx(directory, out freeBytesAvailable, out totalBytes, out freeBytes))
                Log.Warn("GetDiskFreeSpaceEx failed (0x{0:x8})", Marshal.GetLastWin32Error());

            return new WebDiskSpaceInformation()
            {
                Disk = directory,
                Available = (float)Math.Round(freeBytes / 1024.0 / 1024 / 1024, 2),
                Size = (float)Math.Round(totalBytes / 1024.0 / 1024 / 1024, 2),
                Used = (float)Math.Round((totalBytes - freeBytes) / 1024.0 / 1024 / 1024, 2),
                PercentageUsed = totalBytes > 0 ? (float)(100 - Math.Round((float)freeBytes / (float)totalBytes * 100, 1)) : (float)0
            };
        }
    }
}
