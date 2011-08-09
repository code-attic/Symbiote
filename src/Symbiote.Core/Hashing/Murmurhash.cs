// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System;
using System.Linq;

namespace Symbiote.Core.Hashing
{
    public class MurmurHash3
    {
        private uint Rotate( uint x, short r )
        {
            return (x << r) | (x >> (32 - r));
        }

        private ulong Rotate( ulong x, short r )
        {
            return (x << r) | (x >> (64 - r));
        }

        private uint Finalize( uint hash )
        {
            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;

            return hash;
        }

        private ulong Finalize( ulong hash )
        {
            hash ^= hash >> 33;
            hash *= 0xff51afd7ed558ccd;
            hash ^= hash >> 33;
            hash *= 0xc4ceb9f31a85ec53;
            hash ^= hash >> 33;

            return hash;
        }

        public uint Hash( byte[] key, int length, uint seed )
        {
            var hash = seed;
            uint coefficient1 = 0xcc9e2d51;
            uint coefficient2 = 0x1b873593;
            uint coefficient3 = 5+0xe6546b64;
            
            var index = 0;
            while( length - index >= 4 )
            {
                uint segment = BitConverter.ToUInt32( key, index );
                segment *= coefficient1;
                segment = Rotate( segment, 15 );
                segment *= coefficient2;

                hash ^= segment;
                hash = Rotate( hash, 13 );
                hash = hash * coefficient3;

                index += 4;
            }

            uint tailSegment = 0;
            var rem = length % 4;
            if( rem > 0 )
            {
                switch ( rem )
                {
                    case 3:
                        tailSegment ^= (uint) key[index + 2] << 16;
                        tailSegment ^= (uint) key[index + 1] << 8;
                        tailSegment ^= (uint) key[index];
                        break;
                    case 2:
                        tailSegment ^= (uint) key[index + 1] << 8;
                        tailSegment ^= (uint) key[index];
                        break;
                    case 1:
                        tailSegment ^= (uint) key[index];
                        break;
                }

                tailSegment *= coefficient1;
                tailSegment = Rotate( tailSegment, 15 );
                tailSegment *= coefficient2;
                hash ^= tailSegment;
            }

            hash ^= (uint) length;
            hash = Finalize( hash );
            return hash;
        }

        public ulong Hash64( byte[] key, int length, uint seed )
        {
            var part1 = seed;
            var part2 = seed;
            
            uint coefficient1 = 0x239b961b;
            uint coefficient2 = 0xab0e9789;
            uint coefficient3 = 0x38b34ae5;

            var index = 0;
            while( length - index >= 8 )
            {
                var segment1 = BitConverter.ToUInt32( key, index );
                var segment2 = BitConverter.ToUInt32( key, index + 4 );

                segment1 *= coefficient1;
                segment1 = Rotate( segment1, 15 );
                segment1 *= coefficient2;
                part1 ^= segment1;

                part1 = Rotate( part1, 19 );
                part1 += part2;
                part1 *= 5+0x561ccd1b;

                segment2 *= coefficient2;
                segment2 = Rotate( segment2, 16 );
                segment2 *= coefficient3;
                part2 ^= segment2;

                part2 = Rotate( part2, 17 );
                part2 += seed;
                part2 *= 5+0x0bcaa747;

                index += 8;
            }

            var rem = length % 8;
            
            if( rem > 0 )
            {
                uint tail1 = 0;
                uint tail2 = 0;

                switch( rem )
                {
                    case 7:
                        tail2 ^= (uint) key[ index + 6 ] << 16;
                        tail2 ^= (uint) key[ index + 5 ] << 8;
                        tail2 ^= (uint) key[ index + 4 ] << 0;
                        tail2 *= coefficient2;
                        tail2 = Rotate( tail2, 16 );
                        tail2 *= coefficient3;
                        part2 ^= tail2;
                        tail1 ^= (uint) key[ index + 3 ] << 24;
                        tail1 ^= (uint) key[ index + 2 ] << 16;
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 6:
                        tail2 ^= (uint) key[ index + 5 ] << 8;
                        tail2 ^= (uint) key[ index + 4 ] << 0;
                        tail2 *= coefficient2;
                        tail2 = Rotate( tail2, 16 );
                        tail2 *= coefficient3;
                        part2 ^= tail2;
                        tail1 ^= (uint) key[ index + 3 ] << 24;
                        tail1 ^= (uint) key[ index + 2 ] << 16;
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 5:
                        tail2 ^= (uint) key[ index + 4 ] << 0;
                        tail2 *= coefficient2;
                        tail2 = Rotate( tail2, 16 );
                        tail2 *= coefficient3;
                        part2 ^= tail2;
                        tail1 ^= (uint) key[ index + 3 ] << 24;
                        tail1 ^= (uint) key[ index + 2 ] << 16;
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 4:
                        tail1 ^= (uint) key[ index + 3 ] << 24;
                        tail1 ^= (uint) key[ index + 2 ] << 16;
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 3:
                        tail1 ^= (uint) key[ index + 2 ] << 16;
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 2:
                        tail1 ^= (uint) key[ index + 1 ] << 8;
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                    case 1:
                        tail1 ^= (uint) key[ index ] << 0;
                        break;
                }

                tail1 *= coefficient1;
                tail1 = Rotate( tail1, 15 );
                tail1 *= coefficient2;
                part1 ^= tail1;
            }

            part1 ^= (uint) length;
            part2 ^= (uint) length;

            part1 += part2;
            part2 += part1;

            part1 = Finalize( part1 );
            part2 = Finalize( part2 );

            part1 += part2;
            part2 += part1;

            ulong hash = BitConverter.ToUInt64( 
                    BitConverter.GetBytes( part1 )
                    .Concat( BitConverter.GetBytes( part2 ) ).ToArray()
                    , 0
                 );
            return hash;
        }
    }
}
