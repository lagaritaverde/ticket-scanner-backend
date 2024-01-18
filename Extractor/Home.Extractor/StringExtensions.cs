namespace Home.Extractor {
    public static class StringExtensions {
        public static int NextCharIndex(this string value,int position,char charecter) {
            for (int i = position; i < value.Length; i++) {
                if (value[i]== charecter) {
                    return i;
                }
            }

            return -1;
        }

        public static int PreviousCharIndex(this string value, int position, char charecter) {
            for (int i = position; i > 0; i--) {
                if (value[i] == charecter) {
                    return i;
                }
            }

            return -1;
        }
    }
}
