#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Ex0UUQIFEB8VEAMVUQUUAxwCURBEQ0BFQUJHK2Z8QkRBQ0FIQ0BFQR8VURIeHxUYBRgeHwJRHhdRBAIU5O8LfdU2+iqlZ0ZCurV+PL9lGKB8d3hb9zn3hnxwcHR0cXLzcHBxLRgXGBIQBRgeH1EwBAUZHgMYBQhAd3IkbH91Z3VlWqEYNuUHeI+FGvz+AvARt2oqeF7jw4k1OYERSe9khHRxcvNwfnFB83B7c/NwcHGV4Nh4FURSZDpkKGzC5YaH7e++IcuwKSE0D249GiHnMPi1BRN6YfIw9kL78Hlad3B0dHZzcGdvGQUFAQJLXl4GDjDZ6YiguxftVRpgodLKlWpbsm64aAOELH+kDi7qg1RyyyT+PCx8gExXFlH7QhuGfPO+r5rSXogiGyoVz4UC6p+jFX66CD5FqdNPiAmOGrlB83XKQfNy0tFyc3Bzc3BzQXx3eMZqzOIzVWNbtn5sxzztLxK5OvFmAxASBRgSFFECBRAFFBwUHwUCX0Fu9PL0auhMNkaD2Oox/12lwOFjqahHDrD2JKjW6MhDM4qppADvD9AjwEEpnSt1Q/0Zwv5srxQCjhYvFM1eQfCyd3lad3B0dHZzc0Hwx2vwwgtB83AHQX93ciRsfnBwjnV1cnNwR+g9XAnGnP3qrYIG6oMHowZBPrABHRRRMhQDBRgXGBIQBRgeH1EwBAUYFxgSEAUUURMIURAfCFEBEAMF2tIA4zYiJLDeXjDCiYqSAbyX0j0GBl8QAQEdFF8SHhxeEAEBHRQSELESQgaGS3ZdJ5qrflB/q8sCaD7EQkcrQRNAekF4d3IkdXdicyQiQGJBYHdyJHV7YnswAQEdFFE4HxJfQFEeF1EFGRRRBRkUH1EQAQEdGBIQXVESFAMFGBcYEhAFFFEBHh0YEgh1d2JzJCJAYkFgd3IkdXtiezABAQEdFFEjHh4FUTIwQW9mfEFHQUVDV0FVd3IkdXpibDABAR0UUTIUAwUjFB0YEB8SFFEeH1EFGRgCURIUAzipB+5CZRTQBuW4XHNycHFw0vNwCFEQAgIEHBQCURASEhQBBRAfEhQW/nnFUYa63V1RHgHHTnBB/cYyvlWTmqDGAa5+NJBWu4AcCZyWxGZmHRRROB8SX0BXQVV3ciR1emJsMAFb9zn3hnxwcHR0cUETQHpBeHdyJGdBZXdyJHVyYnwwAQEdFFEjHh4FbuCqbzYhmnScLwj1XJpH0yY9JJ3ZrQ9TRLtUpKh+pxql01VSYIbQ3XdBfndyJGxicHCOdXRBcnBwjkFsdp0MSPL6IlGiSbXAzus+exqOWo3zcHF3eFv3OfeGEhV0cEHwg0Fbd8RL3IV+f3HjesBQZ18FpE18qhNnUTIwQfNwU0F8d3hb9zn3hnxwcHDxZVqhGDblB3iPhRr8XzHXhjY8Dn7sTIJaOFlruY+/xMh/qC9tp7pMBRkeAxgFCEBnQWV3ciR1cmJ8MAFREB8VURIUAwUYFxgSEAUYHh9RAV8x14Y2PA55L0Fud3IkbFJ1aUFn+mj4r4g6HYR22lNBc5lpT4kheKJ5L0HzcGB3ciRsUXXzcHlB83B1QSjWdHgNZjEnYG8Fosb6Uko20qQeIdv7pKuVjaF4dkbBBARQ");
        private static int[] order = new int[] { 42,7,43,18,47,11,30,55,14,20,25,35,17,19,56,50,42,59,46,49,58,26,37,28,48,47,39,48,36,57,56,58,41,44,43,56,47,38,48,40,40,53,46,55,59,50,54,57,51,55,57,55,53,55,56,57,58,58,59,59,60 };
        private static int key = 113;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
