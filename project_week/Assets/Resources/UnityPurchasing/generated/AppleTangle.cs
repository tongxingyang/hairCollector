#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("jYiLComHiLgKiYKKComJiGwZIYEjK/kaz9vdSSenyTtwc2v4RW4rxAqJiI6Bog7ADn/r7I2JuAl6uKKOqMvJuAqJqriFjoGiDsAOf4WJiYmPZPWxCwPbqFuwTDk3EseC43ejdDZ8+xNmWuyHQ/HHvFAqtnHwd+NAwVD+F7uc7Sn/HEGliouJiIkrComFjoGiDsAOf4WJiY2NiIsKiYmI1PfJIBBxWULuFKzjmVgrM2yTokuXgNa4ComZjovdlaiMComAuAqJjLhI67v/f7KPpN5jUoephlIy+5HHPQORAVZxw+R9jyOquIpgkLZw2IFbvbq5vLi7vtKfhbu9uLq4sbq5vLjh7uHr6fzh5+aoyf384Of64fzxuZcZU5bP2GONZdbxDKVjvirfxN1kIFT2qr1CrV1Rh17jXCqsq5l/KSSnuAlLjoCjjomNjY+KirgJPpIJO4cVtXujwaCSQHZGPTGGUdaUXkO1QZH6fdWGXffXE3qtizLdB8XVhXn45O2oy+36/OHu4evp/OHn5qjJ/YCjjomNjY+KiZ6W4Pz8+Puyp6f/nricjovdjIubhcn4+OTtqNrn5/zm7Kjr5+bs4fzh5+b7qOfuqP377fGo6fv7/eXt+6jp6+vt+Pzp5uvtpKjr7fr84e7h6+n87aj45+Th6/E9siV8h4aIGoM5qZ6m/F20hVPqnriZjovdjIKbgsn4+OTtqMHm66a5jI6bit3buZu4mY6L3YyCm4LJ+PjvB4A8qH9DJKSo5/g+t4m4BD/LR9rt5OHp5uvtqOfmqPzg4fuo6+36lw0LDZMRtc+/eiETyAakXDkYmlDq5O2o+/zp5uzp+uyo/O365fuo6Tm40GTSjLoE4DsHlVbt+3fv1u00rrisjovdjIOblcn4+OTtqMvt+vyo6ebsqOvt+vzh7uHr6fzh5+ao+I64h46L3ZWbiYl3jI24i4mJd7iVrGpjWT/4V4fNaa9CeeXwZW89n5+4CowzuAqLKyiLiomKiomKuIWOgbu+0rjquYO4gY6L3YyOm4rd27mb5O2owebrprmuuKyOi92Mg5uVyfimyC5/z8X3gNa4l46L3ZWrjJC4nvrp6/zh6+2o+/zp/O3l7eb8+6a4+OTtqNrn5/yoy8m4lp+FuL64vLodFvKELM8D01yev7tDTIfFRpzhWf//pun4+OTtpuvn5afp+Pjk7evpta7vqAK74n+FCkdWYyuncdvi0+zsvaudw53RlTscf34UFkfYMknQ2D+TNRvKrJqiT4eVPsUU1utAwwifB/sJ6E6T04GnGjpwzMB46LAWnX2iDsAOf4WJiY2NiLjquYO4gY6L3c32l8Tj2B7JAUz86oOYC8kPuwIJ/OHu4evp/O2o6vGo6ebxqPjp+vzRL42B9J/I3pmW/Fs/A6uzzytd5wico1jhzxz+gXZ84wWmyC5/z8X3qOfuqPzg7aj84O3mqOn4+OTh6+mOi92VhoyejJyjWOHPHP6BdnzjBfK4Con+uIaOi92Vh4mJd4yMi4qJvhHEpfA/ZQQTVHv/E3r+Wv+4x0n84Of64fzxuZ64nI6L3YyLm4XJ+FG+90kP3VEvETG6ynNQXfkW9ina2CICXVJsdFiBj784/f2p");
        private static int[] order = new int[] { 14,24,34,13,51,50,27,47,17,33,57,16,47,16,15,54,37,22,19,51,35,47,43,40,44,40,33,34,47,38,49,42,33,57,45,38,53,54,43,54,42,46,45,51,53,56,51,59,55,53,56,54,52,57,54,57,59,57,59,59,60 };
        private static int key = 136;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
