#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("A89RA3bg7hTOwQbZGERnwUjXaiqBvJavezwQQvq8Enxv3vJLujUE9NNh4sHT7uXqyWWrZRTu4uLi5uPgy30568dGHu2tgZyKzDDTYqIbUaJBIt3Zo2kw0GZA4q2ada+7hwMbxlkYs04uTh3p7tlQKTIzoU2xPGI01eJGVues9aDzrgTYsTD6GqHK1lTaYQtPMpDqyuMboB+eTajg92sWgGHi7OPTYeLp4WHi4uN8vriXwAAx8bGwFe5CH7Ar7noghELCI9FBEXsOYzFRyw0yhLHYXRZo+s1RiINyzrPcIZT8zPIrsiazP4xbYf6NQDtKfKXIEl8IEGhR+g0EqgDDZPCqMXm7VU0GaBKPd4tK9E539w31OhldI3Ap0+ngVAlLEOHg4uPi");
        private static int[] order = new int[] { 13,8,13,7,12,5,9,12,8,10,11,11,12,13,14 };
        private static int key = 227;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
