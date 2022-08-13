using System;
using OpenCCNET;

namespace chii.Services
{
    internal interface INormalizer
    {
        public string Normalize(string str);
    }

    public class Normalizer: INormalizer
    {
        public Normalizer()
        {
            
        }
    }
}
