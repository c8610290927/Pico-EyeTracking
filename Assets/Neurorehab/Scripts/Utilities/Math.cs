namespace Neurorehab.Scripts.Utilities
{
    /// <summary>
    /// Responsible for any math calculations that are usefull for the project
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Parses a string to a float. If the string is empty, returns 0.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static float GetValue(string text)
        {
            return text == "" || text.Length == 1 && (text == "-" || text == ".") ? 0f : float.Parse(text);
        }
    }
}