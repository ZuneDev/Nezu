using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Nezu.Core.Memory
{
    public unsafe class RAM : IDisposable
    {
        private byte* _ram;
        private readonly nuint _size;

        public RAM(nuint size)
        {
            _size = size;
            _ram = (byte*)NativeMemory.AllocZeroed(size);
        }

        public void Dispose()
        {
            if (_ram != null)
            {
                NativeMemory.Free(_ram);
                _ram = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte(uint address) => *(_ram + address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadHalfWord(uint address) => *(ushort*)(_ram + address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadWord(uint address) => *(uint*)(_ram + address);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(uint address, byte value) => *(_ram + address) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteHalfWord(uint address, ushort value) => *(ushort*)(_ram + address) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWord(uint address, uint value) => *(uint*)(_ram + address) = value;
    }
}