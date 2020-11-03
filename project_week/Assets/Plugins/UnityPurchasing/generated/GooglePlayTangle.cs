#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("woMo1bXVhnJ1Qsuyqag61iqn+a9qKiuOddmEK7B14bsf2Vm4StqK4FDmonBc3YV2NhoHEVerSPk5gMo5IM7WnfOJFOwQ0W/V7GyWbqGCxrj6eXd4SPp5cnr6eXl45yUjDFubqkj6eVpIdX5xUv4w/o91eXl5fXh7mFTKmO17dY9VWp1Cg9/8WtNM8bFOed3NfDduO2g1n0Mqq2GBOlFNz0H6kNSpC3FReIA7hAXWM3ts8I0b2rlGQjjyq0v923k2Ae40IByYgF0oR7oPZ1dpsCm9KKQXwPplFtug0RonDTTgp4vZYSeJ5/RFadAhrp9v5z5TicSTi/PKYZafMZtY/2sxquKV+KrKUJapHypDxo3zYVbKExjpVeuySHJ7z5LQi3p7eXh5");
        private static int[] order = new int[] { 5,10,7,7,10,5,13,9,10,12,13,13,12,13,14 };
        private static int key = 120;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
