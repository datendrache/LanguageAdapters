using System;
using System.Collections;

namespace PhlozLanguages
{
    public class PhlozLanguages
    {
        public ArrayList Languages = new ArrayList();

        public void registerLanguage(IntLanguage language)
        {
            Languages.Add(language);
        }

        public void deregisterLanguage(IntLanguage language)
        {
            Languages.Remove(language);
        }
    }
}
