using System;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xrd.Tests {
	[TestClass()]
	public class FastHashExtensionsTests {
		// Generate an array of bytes for testing.
		private byte[] generateTestArray(int length = 100) {
			Random r = new Random();
			byte[] arr = new byte[length];
			r.NextBytes(arr);
			return arr;
		}

		private static bool areEqual(byte[] left, byte[] right) {
			if (left == null && right == null)
				return true;
			if ((left == null) != (right == null))
				return false;
			if (left.Length != right.Length)
				return false;

			for(int i = 0; i < left.Length; i++) {
				if (left[i] != right[i])
					return false;
			}
			return true;
		}

		[TestMethod()]
		public void EmptyArray_IsGuidEmpty() {
			Assert.IsTrue(areEqual(FastHashExtensions.EmptyArray, Guid.Empty.ToByteArray()));
		}

		[TestMethod()]
		public void FastHash_TestEmpty_Bin() {
			// Arrange
			byte[] vs = null;

			// Assert
			Assert.IsTrue(areEqual(FastHashExtensions.EmptyArray, vs.FastHash()));
		}

		[TestMethod()]
		public void FastHash_TestNotEmpty_Bin() {
			// Arrange 
			byte[] vs = generateTestArray();

			// Assert
			Assert.AreNotEqual(FastHashExtensions.EmptyArray, vs.FastHash());
		}

		[TestMethod()]
		public void FastHash_TestDifferent_Bin() {
			// Arrange
			byte[] vs = generateTestArray();
			byte[] vs1 = new byte[vs.Length];

			// Act
			Array.Copy(vs, 0, vs1, 1, vs.Length - 1);
			byte[] h = vs.FastHash();
			byte[] h1 = vs1.FastHash();

			// Assert
			Assert.IsFalse(areEqual(h, h1));
		}

		[TestMethod]
		public void FastHash_TestSame_Bin() {
			// Arrange
			byte[] vs = generateTestArray();
			byte[] vs1 = new byte[vs.Length];

			// Act
			Array.Copy(vs, 0, vs1, 0, vs.Length);
			byte[] h = vs.FastHash();
			byte[] h1 = vs.FastHash();

			// Assert
			Assert.IsTrue(areEqual(h, h1));
		}

		[TestMethod]
		public void FashHash_TestSeed() {
			// Arrange
			byte[] vs = generateTestArray();

			// Act
			byte[] h = vs.FastHash(100);
			byte[] h1 = vs.FastHash(200);
			byte[] h2 = vs.FastHash(100);

			Assert.IsFalse(areEqual(h, h1));
			Assert.IsTrue(areEqual(h, h2));
		}

		// Generate a string for testing.
		private string generateTestString(int length = 100) {
			Random r = new Random();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
				sb.Append((char)r.Next(32, 126));

			return sb.ToString();
		}

		[TestMethod()]
		public void FastHash_TestEmpty_Str() {
			// Arrange
			string vs = null;

			// Assert
			Assert.IsTrue(areEqual(FastHashExtensions.EmptyArray, vs.FastHash()));
		}

		[TestMethod()]
		public void FastHash_TestNotEmpty_Str() {
			// Arrange 
			string vs = generateTestString();

			// Assert
			Assert.AreNotEqual(FastHashExtensions.EmptyArray, vs.FastHash());
		}

		[TestMethod()]
		public void FastHash_TestDifferent_Str() {
			// Arrange
			string vs = generateTestString();
			string vs1 = generateTestString(101);

			// Act
			byte[] h = vs.FastHash();
			byte[] h1 = vs1.FastHash();

			// Assert
			Assert.IsFalse(areEqual(h, h1));
		}

		[TestMethod()]
		public void FastHash_TestSame_Str() {
			// Arrange
			string vs = generateTestString();
			string vs1 = string.Empty;

			// Act
			foreach (var c in vs)
				vs1 += c;
			byte[] h = vs.FastHash();
			byte[] h1 = vs1.FastHash();

			// Assert
			Assert.IsTrue(areEqual(h, h1));
		}

		[TestMethod]
		public void HashGuid_TestNullIsGuidEmpty() {
			// Arrange
			byte[] vs = null;
			byte[] vs1 = new byte[0];
			string s = null;
			string s1 = string.Empty;
			string s2 = "   ";

			Assert.AreEqual(Guid.Empty, vs.HashGuid());
			Assert.AreEqual(Guid.Empty, vs1.HashGuid());
			Assert.AreEqual(Guid.Empty, s.HashGuid());
			Assert.AreEqual(Guid.Empty, s1.HashGuid());
			Assert.AreEqual(Guid.Empty, s2.HashGuid());
		}

		[TestMethod]
		public void HashGuid_Test() {
			// Arrange
			byte[] vs = generateTestArray();
			byte[] vs1 = new byte[vs.Length - 2];

			// Act
			Array.Copy(vs, 2, vs1, 0, vs1.Length);
			Guid h1 = vs.HashGuid();
			Guid h2 = vs1.HashGuid();
			Guid h3 = vs.HashGuid(100);

			// Assert
			Assert.AreNotEqual(h1, h2);
			Assert.AreNotEqual(h1, h3);
		}
	}
}