using System;
using System.Collections.Generic;
using OpenGloveApp.Models;

namespace OpenGloveApp.Utils
{
    public static class BooleanStatements
    {
        public static bool NoNullAndEqualCount(List<int> list1, List<int> list2)
        {
            if (list1 != null & list2 != null)
            {
                if (list1.Count == list2.Count)
                    return true;
            }
            return false;
        }

        public static bool NoNullAndEqualCount(List<int> list1, List<string> list2)
        {
            if (list1 != null & list2 != null)
            {
                if (list1.Count == list2.Count)
                    return true;
            }
            return false;
        }

        public static bool NoNullAndCountGreaterThanZero(Dictionary<int, int> dictionary)
        {
            if (dictionary != null)
                if (dictionary.Count > 0)
                    return true;
            return false;
        }

        public static bool NoNullAndCountGreaterThanZero(Dictionary<int, Actuator> dictionary)
        {
            if (dictionary != null)
                if (dictionary.Count > 0)
                    return true;
            return false;
        }
    }
}
