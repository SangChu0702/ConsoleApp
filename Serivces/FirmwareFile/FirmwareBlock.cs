/**
 * @file
 * @copyright  Copyright (c) 2020 Jesús González del Río
 * @license    See LICENSE.txt
 */

using System;
using System.Collections.Generic;

namespace Core.FirmwareFile
{
    /**
     * Represents a block of firmware corresponding to the data for a continuous block
     * of memory starting at a given address.
     */
    public class FirmwareBlock
    {
        /*===========================================================================
         *                           PUBLIC PROPERTIES
         *===========================================================================*/

        /**
         * Starting address of the firmware block.
         */
        public uint StartAddress { get; private set; }

        /**
         * Data of the firmware block.
         */
        public byte[] Data { get => m_data.ToArray(); }

        /**
         * Size of the firmware block.
         */
        public uint Size { get => (uint)m_data.Count; }

        /*===========================================================================
         *                         INTERNAL CONSTRUCTORS
         *===========================================================================*/

        internal FirmwareBlock(uint startAddress, byte[] data)
        {
            StartAddress = startAddress;
            m_data = new List<byte>(data);
        }

        /*===========================================================================
         *                            INTERNAL METHODS
         *===========================================================================*/

        internal void SetDataAtAddress(uint address, byte[] data)
        {
            int offset = (int)address - (int)StartAddress;

            SetDataAtOffset(offset, data);
        }

        internal void SetDataAtOffset(int offset, byte[] data)
        {
            int endOffset = offset + data.Length;

            if (offset >= 0 && offset <= Size)
            {
                if (endOffset < Size)
                {
                    m_data.RemoveRange(offset, data.Length);
                }
                else
                {
                    m_data.RemoveRange(offset, (int)(Size - offset));
                }
                m_data.InsertRange(offset, data);
            }
            else if (offset < 0 && endOffset >= 0)
            {
                StartAddress -= (uint)-offset;

                if (endOffset >= Size)
                {
                    m_data.Clear();
                }
                else if (endOffset > 0)
                {
                    m_data.RemoveRange(0, endOffset);
                }
                m_data.InsertRange(0, data);
            }
            else
            {
                throw new ArgumentException("Inserted data region does not overlap the block data region");
            }
        }

        internal void AppendData(byte[]? data)
        {
            if (data != null)
            {
                m_data.AddRange(data);
            }
        }

        internal void EraseDataRangeAfterAddress(uint address)
        {
            uint offset = 0;
            if (address > StartAddress)
            {
                offset = address - StartAddress;
            }

            EraseDataRangeAfterOffset(offset);
        }

        internal void EraseDataRangeBeforeAddress(uint address)
        {
            uint offset = 0;
            if (address > StartAddress)
            {
                offset = address - StartAddress;
            }

            EraseDataRangeBeforeOffset(offset);
        }

        internal void EraseDataRangeAfterOffset(uint offset)
        {
            if (offset < Size)
            {
                m_data.RemoveRange((int)offset, (int)(Size - offset));
            }
        }

        internal void EraseDataRangeBeforeOffset(uint offset)
        {
            if (offset < Size)
            {
                m_data.RemoveRange(0, (int)offset);
                StartAddress += offset;
            }
            else
            {
                m_data.Clear();
            }
        }

        /*===========================================================================
         *                           PRIVATE ATTRIBUTES
         *===========================================================================*/

        private List<byte> m_data;
    }
}
