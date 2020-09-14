#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1DY/FHMEF0sgzRBNZ/oKuTye9r7fhxIqsVCXdOQS2J1zIrzuRJIf3DHCllH7QYV3TxUSVgSLFMMXYCtyBBbg2tRarS0Y8LT2ZRj36HWqN4bLglzP7B6Uzx0v7uu+oY08zzO/0fTW9ZaAiSXWze/tgfMqiwns2UQFivQmYHop3LSi9eBLt3XWmkHzTxtew4E2NdNjzBARtF0DGrhyfO3srpXdm8guC2IRuofLiCA7vOAGwDFDIfXzStcHfAhRClr8bACFBqDu/1r8Ts3u/MHKxeZKhEo7wc3NzcnMz6J2qepyu3hbsdnGyERJjWstqofdTs3DzPxOzcbOTs3NzElNKENW5C0gKXZ9abAoldH3nKdDEVlJuxn2B756RagOhNttn87PzczN");
        private static int[] order = new int[] { 5,4,3,8,11,7,9,13,8,9,13,12,12,13,14 };
        private static int key = 204;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
