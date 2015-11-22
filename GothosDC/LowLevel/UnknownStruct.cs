using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GothosDC.LowLevel
{
    // perhaps this is some kind of lookup table, to go from strings to segment addresses?
    public struct UnknownStruct
    {
        public uint Key; // goes from 0 to UInt32 max value over the course of a segment
        public uint Column4;
        public uint Column8;
        public SegmentAddress Address;// into strings/names
    }
}
