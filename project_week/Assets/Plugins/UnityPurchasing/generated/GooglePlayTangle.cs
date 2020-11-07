#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("97e2E+hEGbYt6HwmgkTEJddHF33T5EBQ4arzpvWoAt63Nvwcp8zQUs17P+3BQBjrq4eajMo21WSkHVekeqPOFFkOFm5X/AsCrAbFYvasN38IZTdXzQs0grfeWxBu/MtXjoV0yAXJVwVw5ugSyMcA3x5CYcdO0WwsvVNLAG4UiXGNTPJIcfEL8zwfWyXVZ+TH1ejj7M9jrWMS6OTk5ODl5l8etUgoSBvv6N9WLzQ1p0u3OmQyRyTb36VvNtZgRuSrnHOpvYEFHcDcZw1JNJbszOUdphmYS67m8W0QhrXaJ5L6yvQttCC1OYpdZ/iLRj1Mh7qQqX06FkT8uhR6adj0TbwzAvJn5Orl1Wfk7+dn5OTleri+kcYGN3Yv1e/mUg9NFufm5OXk");
        private static int[] order = new int[] { 10,9,7,3,11,13,7,10,13,12,11,11,13,13,14 };
        private static int key = 229;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
