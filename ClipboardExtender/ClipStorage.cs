using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardExtender
{
    class ClipboardStorage
    {
        List<string> clips = new List<string>();

        public void AddClip(string clip)
        {
            clips.Add(clip);
        }

        public List<string> GetAllClips()
        {
            return clips;
        }
    }
}
