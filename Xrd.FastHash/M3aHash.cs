using System;
using System.Text;

namespace Xrd {
	/// <summary>
	/// Modified from: https://github.com/arisoyang/Murmur3Hash/blob/master/MurmurHashPerformance/Murmur3.cs
	/// </summary>
	internal sealed class M3aHash {
		internal static ulong READ_SIZE = 16;
		private static readonly ulong C1 = 0x87c37b91114253d5L;
		private static readonly ulong C2 = 0x4cf5ad432745937fL;
		private ulong length;
		private readonly uint seed;  //If we want to start with a seed, create a constructor.
		ulong h1;
		ulong h2;

		internal M3aHash(uint? mySeed = null) {
			if (mySeed.HasValue)
				seed = mySeed.Value;
		}

		private void MixBody(ulong k1, ulong k2) {
			h1 ^= MixKey1(k1);
			h1 = h1 << 27 | (h1 >> 33);

			h1 += h2;
			h1 = h1 * 5 + 0x52dce729;
			h2 ^= MixKey2(k2);

			h2 = h2 >> 31 | (h2 << 33);

			h2 += h1;
			h2 = h2 * 5 + 0x38495ab5;
		}

		private static ulong MixKey1(ulong k1) {
			k1 *= C1;
			k1 = k1 >> 31 | (k1 << 33);
			k1 *= C2;
			return k1;
		}

		private static ulong MixKey2(ulong k2) {
			k2 *= C2;
			k2 = k2 >> 33 | (k2 << 31);
			k2 *= C1;
			return k2;
		}

		private static ulong MixFixal(ulong k) {
			// avalanche bits
			k ^= k >> 33;
			k *= 0xff51afd7ed558ccdL;
			k ^= k >> 33;
			k *= 0xc4ceb9fe1a85ec53L;
			k ^= k >> 33;
			return k;
		}

		internal byte[] ComputeHash(byte[] input) {
			ProcessBytes(input);
			return Hash;
		}

		internal byte[] ComputeHash(char[] input) {
			ProcessBytes(Encoding.Unicode.GetBytes(input));
			return Hash;
		}

		internal byte[] ComputeHash(string input) {
			ProcessBytes(Encoding.Unicode.GetBytes(input));
			return Hash;
		}

		private void ProcessBytes(byte[] bb) {
			h1 = seed;
			length = 0L;
			int pos = 0;
			ulong remaining = (ulong)bb.Length;
			// read 128 bits (16 bytes or 2 longs) in each cycle.
			while (remaining >= READ_SIZE) {
				ulong k1 = BitConverter.ToUInt64(bb, pos);
				pos += 8;
				ulong k2 = BitConverter.ToUInt64(bb, pos);
				pos += 8;
				length += READ_SIZE;
				remaining -= READ_SIZE;
				MixBody(k1, k2);
			}
			// if the input MOD 16 != 0;
			if (remaining > 0)
				ProcessBytesRemaining(bb, remaining, pos);
		}

		private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos) {
			ulong k1 = 0;
			ulong k2 = 0;
			length += remaining;

			switch (remaining) {
				case 15:
					k2 ^= (ulong)bb[pos + 14] << 48;
					goto case 14;
				case 14:
					k2 ^= (ulong)bb[pos + 13] << 40;
					goto case 13;
				case 13:
					k2 ^= (ulong)bb[pos + 12] << 32;
					goto case 12;
				case 12:
					k2 ^= (ulong)bb[pos + 11] << 24;
					goto case 11;
				case 11:
					k2 ^= (ulong)bb[pos + 10] << 16;
					goto case 10;
				case 10:
					k2 ^= (ulong)bb[pos + 9] << 8;
					goto case 9;
				case 9:
					k2 ^= bb[pos + 8];
					goto case 8;
				case 8:
					k1 ^= BitConverter.ToUInt64(bb, pos);
					break;
				case 7:
					k1 ^= (ulong)bb[pos + 6] << 48;
					goto case 6;
				case 6:
					k1 ^= (ulong)bb[pos + 5] << 40;
					goto case 5;
				case 5:
					k1 ^= (ulong)bb[pos + 4] << 32;
					goto case 4;
				case 4:
					k1 ^= (ulong)bb[pos + 3] << 24;
					goto case 3;
				case 3:
					k1 ^= (ulong)bb[pos + 2] << 16;
					goto case 2;
				case 2:
					k1 ^= (ulong)bb[pos + 1] << 8;
					goto case 1;
				case 1:
					k1 ^= bb[pos];
					break;
				default:
					throw new Exception("Something went wrong with remaining bytes calculation");
			}
			h1 ^= MixKey1(k1);
			h2 ^= MixKey2(k2);
		}

		internal byte[] Hash {
			get {
				h1 ^= length;
				h2 ^= length;
				h1 += h2;
				h2 += h1;
				h1 = MixFixal(h1);
				h2 = MixFixal(h2);
				h1 += h2;
				h2 += h1;
				var hash = new byte[READ_SIZE];
				Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
				Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);
				return hash;
			}
		}
	}
}