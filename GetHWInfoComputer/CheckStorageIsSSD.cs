﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GetHWInfoComputer
{
    public static class CheckStorageIsSSD
    {
        // For CreateFile to get handle to drive
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        // CreateFile to get handle to drive
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)]
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        // For control codes
        private const uint FILE_DEVICE_MASS_STORAGE = 0x0000002d;
        private const uint IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;
        private const uint FILE_DEVICE_CONTROLLER = 0x00000004;
        private const uint IOCTL_SCSI_BASE = FILE_DEVICE_CONTROLLER;
        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_ANY_ACCESS = 0;
        private const uint FILE_READ_ACCESS = 0x00000001;
        private const uint FILE_WRITE_ACCESS = 0x00000002;

        private static uint CTL_CODE(uint DeviceType, uint Function,
                                     uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) |
                    (Function << 2) | Method);
        }

        // For DeviceIoControl to check no seek penalty
        private const uint StorageDeviceSeekPenaltyProperty = 7;
        private const uint PropertyStandardQuery = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STORAGE_PROPERTY_QUERY
        {
            public uint PropertyId;
            public uint QueryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] AdditionalParameters;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVICE_SEEK_PENALTY_DESCRIPTOR
        {
            public uint Version;
            public uint Size;
            [MarshalAs(UnmanagedType.U1)]
            public bool IncursSeekPenalty;
        }

        // DeviceIoControl to check no seek penalty
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
                   SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref STORAGE_PROPERTY_QUERY lpInBuffer,
            uint nInBufferSize,
            ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        // For DeviceIoControl to check nominal media rotation rate
        private const uint ATA_FLAGS_DATA_IN = 0x02;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct ATA_PASS_THROUGH_EX
        {
            public ushort Length;
            public ushort AtaFlags;
            public byte PathId;
            public byte TargetId;
            public byte Lun;
            public byte ReservedAsUchar;
            public uint DataTransferLength;
            public uint TimeOutValue;
            public uint ReservedAsUlong;
            public IntPtr DataBufferOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] PreviousTaskFile;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] CurrentTaskFile;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct SCSI_PASS_THROUGH
        {
            public ushort length;
            public byte scsiStatus;
            public byte pathId;
            public byte targetId;
            public byte lun;
            public byte cdbLength;
            public byte senseInfoLength;
            public byte dataIn;
            public uint dataTransferLength;
            public uint timeOutValue;
            public ulong dataBufferOffset;
            public uint senseInfoOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] cdb;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct ATAIdentifyDeviceQuery
        {
            public ATA_PASS_THROUGH_EX header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] data;
        }

        // DeviceIoControl to check nominal media rotation rate
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
                   SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref ATAIdentifyDeviceQuery lpInBuffer,
            uint nInBufferSize,
            ref ATAIdentifyDeviceQuery lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        // For error message
        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint FormatMessage(
            uint dwFlags,
            IntPtr lpSource,
            uint dwMessageId,
            uint dwLanguageId,
            StringBuilder lpBuffer,
            uint nSize,
            IntPtr Arguments);

        public static bool HasNoSeekPenalty(string sDrive)
        {
            SafeFileHandle hDrive = CreateFileW(
                sDrive,
                0, // No access to drive
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero);

            if (hDrive == null || hDrive.IsInvalid)
            {
                string message = GetErrorMessage(Marshal.GetLastWin32Error());
                Console.WriteLine("CreateFile failed 1. " + message);
            }
            else
            {
                Console.WriteLine($"hDrive: {hDrive}");
            }

            uint IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE(
                IOCTL_STORAGE_BASE, 0x500,
                METHOD_BUFFERED, FILE_ANY_ACCESS); // From winioctl.h
            Console.WriteLine($"IOCTL_STORAGE_QUERY_PROPERTY : {IOCTL_STORAGE_QUERY_PROPERTY}");

            STORAGE_PROPERTY_QUERY query_seek_penalty =
                new STORAGE_PROPERTY_QUERY();
            query_seek_penalty.PropertyId = StorageDeviceSeekPenaltyProperty;
            query_seek_penalty.QueryType = PropertyStandardQuery;

            DEVICE_SEEK_PENALTY_DESCRIPTOR query_seek_penalty_desc =
                new DEVICE_SEEK_PENALTY_DESCRIPTOR();

            uint returned_query_seek_penalty_size;

            bool query_seek_penalty_result = DeviceIoControl(
                hDrive,
                IOCTL_STORAGE_QUERY_PROPERTY,
                ref query_seek_penalty,
                (uint)Marshal.SizeOf(query_seek_penalty),
                ref query_seek_penalty_desc,
                (uint)Marshal.SizeOf(query_seek_penalty_desc),
                out returned_query_seek_penalty_size,
                IntPtr.Zero);

            hDrive.Close();

            if (query_seek_penalty_result == false)
            {
                string message = GetErrorMessage(Marshal.GetLastWin32Error());
                Console.WriteLine("DeviceIoControl failed 2. " + message);
            }
            else
            {
                if (query_seek_penalty_desc.IncursSeekPenalty == false)
                {
                    //Console.WriteLine("This drive has NO SEEK penalty.");
                    return true;
                }
                else
                {
                    //Console.WriteLine("This drive has SEEK penalty.");
                   
                }
            }
             return false;
        }

        // Method for nominal media rotation rate
        // (Administrative privilege is required)
        public static bool HasNominalMediaRotationRate(string sDrive)
        {
           Console.WriteLine($"sDrive: {sDrive}");
            SafeFileHandle hDrive = CreateFileW(
                sDrive,
                GENERIC_READ | GENERIC_WRITE, // Administrative privilege is required
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero);            

            if (hDrive == null || hDrive.IsInvalid)
            {
                string message = GetErrorMessage(Marshal.GetLastWin32Error());
                Console.WriteLine("CreateFile failed 3. " + message);
            }
            uint IOCTL_ATA_PASS_THROUGH = CTL_CODE(
                IOCTL_SCSI_BASE, 0x040b, METHOD_BUFFERED,
                FILE_READ_ACCESS | FILE_WRITE_ACCESS); // From ntddscsi.h            
            ATAIdentifyDeviceQuery id_query = new ATAIdentifyDeviceQuery();
            id_query.data = new ushort[256];

            id_query.header.Length = (ushort)Marshal.SizeOf(id_query.header);
            id_query.header.AtaFlags = (ushort)ATA_FLAGS_DATA_IN;
            id_query.header.DataTransferLength =
                (uint)(id_query.data.Length * 2); // Size of "data" in bytes
            id_query.header.TimeOutValue = 3; // Sec
            id_query.header.DataBufferOffset = (IntPtr)Marshal.OffsetOf(
                typeof(ATAIdentifyDeviceQuery), "data");
            id_query.header.PreviousTaskFile = new byte[8];
            id_query.header.CurrentTaskFile = new byte[8];
            id_query.header.CurrentTaskFile[6] = 0xec; // ATA IDENTIFY DEVICE

            uint retval_size;            
            bool result = DeviceIoControl(
                hDrive,
                IOCTL_ATA_PASS_THROUGH,
                ref id_query,
                (uint)Marshal.SizeOf(id_query),
                ref id_query,
                (uint)Marshal.SizeOf(id_query),
                out retval_size,
                IntPtr.Zero);           
            //// DeviceIoControl
            //bool isSuccess = false;
            //isSuccess = DeviceIoControl(handle, 0x4d004 /* IOCTL_SCSI_PASS_THROUGH */, sptwbPtr, 592, sptwbPtr, 592, out returned, IntPtr.Zero);
            //Console.Write("DeviceIoControl=" + isSuccess);


            hDrive.Close();

            if (result == false)
            {
                string message = Marshal.GetLastWin32Error() +"   " + GetErrorMessage(Marshal.GetLastWin32Error());
                Console.WriteLine("DeviceIoControl failed: " + message);
                return false;
            }
            else
            {
                // Word index of nominal media rotation rate
                // (1 means non-rotate device)
                const int kNominalMediaRotRateWordIndex = 217;

                if (id_query.data[kNominalMediaRotRateWordIndex] == 1)
                {
                    //Console.WriteLine("This drive is NON-ROTATE device.");
                    return true;
                }
                else
                {
                    // Console.WriteLine("This drive is ROTATE device.");
                    return false;
                }
            }
        }

        // Method for error message
        private static string GetErrorMessage(int code)
        {
            StringBuilder message = new StringBuilder(255);

            FormatMessage(
              FORMAT_MESSAGE_FROM_SYSTEM,
              IntPtr.Zero,
              (uint)code,
              0,
              message,
              (uint)message.Capacity,
              IntPtr.Zero);

            return message.ToString();
        }
    }
}
