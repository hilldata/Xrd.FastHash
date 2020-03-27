using System;
using System.Text;

namespace Xrd {
	/// <summary>
	/// Class used to generate hash values using <see cref="M3aHash"/>
	/// </summary>
	/// <remarks>Used primarily for HMAC hashing and change-tracking</remarks>
	public static class FastHashExtensions {
		/// <summary>
		/// An array that represents Guid.Empty. 
		/// Used as return value for inputs that are null or empty.
		/// </summary>
		public static byte[] EmptyArray => new byte[16];

		/// <summary>
		/// Calculate a "fast-hash" value using the <see cref="M3aHash"/>
		/// </summary>
		/// <param name="vs">The binary data to hash.</param>
		/// <param name="seed">An optional seed for the <see cref="M3aHash"/></param>
		/// <returns>A "fast hash" of the input.</returns>
		public static byte[] FastHash(this byte[] vs, uint? seed = null) {
			if (vs == null || vs.Length < 1)
				return EmptyArray;
			M3aHash m3AHash = new M3aHash(seed);
			return m3AHash.ComputeHash(vs);
		}

		/// <summary>
		/// Calculate a "fast-hash" value using the <see cref="M3aHash"/>
		/// </summary>
		/// <param name="vs">The test to hash.</param>
		/// <param name="seed">An optional seed for the <see cref="M3aHash"/></param>
		/// <returns>A "fast-hash" of the input.</returns>
		public static byte[] FastHash(this string vs, uint? seed = null) =>
			string.IsNullOrWhiteSpace(vs)
			? EmptyArray
			: Encoding.UTF8.GetBytes(vs).FastHash(seed);

		/// <summary>
		/// Calculate the "fast-hash" value using the <see cref="M3aHash"/> and convert the result to a Guid for easy storage/manipulation.
		/// </summary>
		/// <param name="vs">The binary data to hash.</param>
		/// <param name="seed">An optional seed for the <see cref="M3aHash"/></param>
		/// <returns>A Guid representation of the "fast-hash" of the input.</returns>
		public static Guid HashGuid(this byte[] vs, uint? seed = null) =>
			new Guid(vs.FastHash(seed));

		/// <summary>
		/// Calculate the "fast-hash" value using the <see cref="M3aHash"/> and convert the result to a Guid for easy storage/manipulation.
		/// </summary>
		/// <param name="vs">The text to hash.</param>
		/// <param name="seed">An optional seed for the <see cref="M3aHash"/></param>
		/// <returns>A Guid representation of the "fast-hash" of the input.</returns>
		public static Guid HashGuid(this string vs, uint? seed = null) =>
			new Guid(vs.FastHash(seed));
	}
}